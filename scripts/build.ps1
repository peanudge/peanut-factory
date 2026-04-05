<#
.SYNOPSIS
    PeanutVision 전체 빌드 파이프라인 자동화 스크립트

.DESCRIPTION
    이 스크립트는 PeanutVision 데스크톱 앱의 전체 빌드 및 패키징 과정을 자동으로 수행합니다:
    1. React 프론트엔드 빌드 (TypeScript + Vite)
    2. ASP.NET Core 백엔드 게시 (self-contained Windows x64 binary)
    3. Electron npm 패키지 설치
    4. Electron 인스톨러 생성 (NSIS .exe)

    최종 결과: electron/dist/PeanutVision Setup {version}.exe

.PREREQUISITES
    - Node.js 및 npm (최신 버전)
    - .NET 10 SDK 이상
    - PowerShell 5.0 이상

.EXAMPLE
    PS> .\scripts\build.ps1

.NOTES
    Author: PeanutVision Build System
    Version: 1.0.0
#>

# 스크립트 내에서 에러가 발생하면 즉시 중단 (fail-fast)
$ErrorActionPreference = "Stop"

# 변수 정의
$repoRoot = Split-Path -Parent -Path $PSScriptRoot
$reactDir = Join-Path $repoRoot "src" "peanut-vision-ui"
$apiDir = Join-Path $repoRoot "src" "PeanutVision.Api"
$electronDir = Join-Path $repoRoot "electron"
$electronResourcesDir = Join-Path $electronDir "resources" "PeanutVision.Api"
$electronDistDir = Join-Path $electronDir "dist"

# 색상 정의 (진행 상황 시각화)
$colors = @{
    Info = 'Cyan'
    Success = 'Green'
    Warning = 'Yellow'
    Error = 'Red'
}

function Write-Section {
    <#
    .SYNOPSIS
    빌드 섹션 헤더를 출력합니다.
    #>
    param([string]$Message)
    Write-Host ""
    Write-Host ("=" * 70) -ForegroundColor $colors.Info
    Write-Host $Message -ForegroundColor $colors.Info
    Write-Host ("=" * 70) -ForegroundColor $colors.Info
}

function Write-Status {
    <#
    .SYNOPSIS
    진행 상황 메시지를 출력합니다.
    #>
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor $colors.Info
}

function Write-Success {
    <#
    .SYNOPSIS
    성공 메시지를 출력합니다.
    #>
    param([string]$Message)
    Write-Host "[OK] $Message" -ForegroundColor $colors.Success
}

function Write-Error-Custom {
    <#
    .SYNOPSIS
    에러 메시지를 출력합니다.
    #>
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor $colors.Error
}

try {
    Write-Section "PeanutVision 빌드 시작 ($(Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))"

    # ============================================================================
    # Step 1: React 프론트엔드 빌드
    # ============================================================================
    # React는 TypeScript로 작성되어 있으며, npm run build는 다음을 수행합니다:
    # - TypeScript 타입체크 (tsc -b)
    # - Vite를 사용하여 최적화된 프로덕션 번들 생성
    # - 결과: src/peanut-vision-ui/dist/ 디렉토리에 정적 파일 생성
    # ============================================================================
    Write-Section "Step 1: React 프론트엔드 빌드"

    Write-Status "React 프로젝트 디렉토리로 이동 ($reactDir)"
    Push-Location $reactDir
    try {
        Write-Status "npm dependencies 설치 중..."
        npm install
        Write-Status "React 프로젝트 빌드 중 (tsc -b && vite build)..."
        npm run build
        Write-Success "React 프론트엔드 빌드 완료"
    }
    finally {
        Pop-Location
    }

    # ============================================================================
    # Step 2: ASP.NET Core 백엔드 게시
    # ============================================================================
    # dotnet publish는 다음을 수행합니다:
    # - -r win-x64: Windows 64비트용 런타임 지정
    # - --self-contained true: 독립적인 .NET 런타임 포함 (별도 설치 불필요)
    # - -c Release: 최적화된 Release 빌드
    # - -o electron/resources/PeanutVision.Api: 출력 경로 지정
    #
    # 중요: 빌드 시 MSBuild 타겟 "CopyReactBuild"가 자동으로 실행되어
    # src/peanut-vision-ui/dist/의 파일들을 wwwroot/로 복사합니다.
    # (이렇게 하면 ASP.NET Core가 React 정적 파일을 서빙할 수 있음)
    # ============================================================================
    Write-Section "Step 2: ASP.NET Core 백엔드 게시 (self-contained win-x64)"

    Write-Status "ASP.NET Core 프로젝트 디렉토리로 이동 ($apiDir)"
    Push-Location $apiDir
    try {
        Write-Status "게시 중 (dotnet publish -r win-x64 --self-contained true -c Release)..."
        Write-Status "이 과정에서 MSBuild 타겟이 React 빌드 파일을 wwwroot/로 자동 복사합니다..."
        dotnet publish `
            -r win-x64 `
            --self-contained true `
            -c Release `
            -o $electronResourcesDir
        Write-Success ".NET API 게시 완료 ($electronResourcesDir)"
    }
    finally {
        Pop-Location
    }

    # ============================================================================
    # Step 3: Electron npm 패키지 설치
    # ============================================================================
    # Electron 프로젝트의 dependencies와 devDependencies를 설치합니다:
    # - electron: 런타임
    # - electron-builder: NSIS 인스톨러 생성 도구
    # ============================================================================
    Write-Section "Step 3: Electron npm 패키지 설치"

    Write-Status "Electron 프로젝트 디렉토리로 이동 ($electronDir)"
    Push-Location $electronDir
    try {
        Write-Status "npm install 실행 중..."
        npm install
        Write-Success "Electron npm 패키지 설치 완료"
    }
    finally {
        Pop-Location
    }

    # ============================================================================
    # Step 4: Electron 인스톨러 빌드
    # ============================================================================
    # npm run dist는 electron-builder를 실행하여 NSIS 인스톨러를 생성합니다:
    # - --win --x64: Windows 64비트 타겟
    # - 빌드 설정: electron/package.json의 "build" 섹션 참고
    # - extraResources로 Step 2에서 게시한 .NET API 바이너리를 포함
    # - 결과: electron/dist/PeanutVision Setup {version}.exe
    # ============================================================================
    Write-Section "Step 4: Electron 인스톨러 빌드 (NSIS)"

    Write-Status "Electron 프로젝트 디렉토리로 이동 ($electronDir)"
    Push-Location $electronDir
    try {
        Write-Status "Electron 인스톨러 빌드 중 (npm run dist --win --x64)..."
        Write-Status "이 과정에서 electron/dist/ 디렉토리에 최종 .exe 파일이 생성됩니다..."
        npm run dist
        Write-Success "Electron 인스톨러 빌드 완료"
    }
    finally {
        Pop-Location
    }

    # ============================================================================
    # 빌드 완료
    # ============================================================================
    Write-Section "빌드 성공!"
    Write-Success "PeanutVision 빌드 파이프라인이 성공적으로 완료되었습니다"
    Write-Host ""
    Write-Host "출력 위치:" -ForegroundColor $colors.Info
    Write-Host "  $electronDistDir" -ForegroundColor $colors.Success
    Write-Host ""
    Write-Host "최종 인스톨러:" -ForegroundColor $colors.Info
    Write-Host "  PeanutVision Setup {version}.exe" -ForegroundColor $colors.Success
    Write-Host ""
    Write-Host "생성된 시간: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor $colors.Info
    Write-Host ""
}
catch {
    Write-Section "빌드 실패"
    Write-Error-Custom "PeanutVision 빌드 중 오류가 발생했습니다:"
    Write-Host ""
    Write-Host $_.Exception.Message -ForegroundColor $colors.Error
    Write-Host ""
    Write-Host "스택 트레이스:" -ForegroundColor $colors.Warning
    Write-Host $_.ScriptStackTrace -ForegroundColor $colors.Warning
    Write-Host ""
    Write-Host "해결 방법:" -ForegroundColor $colors.Warning
    Write-Host "1. Node.js와 npm이 설치되어 있는지 확인하세요 (npm --version)"
    Write-Host "2. .NET 10 SDK가 설치되어 있는지 확인하세요 (dotnet --version)"
    Write-Host "3. git 저장소가 올바르게 초기화되었는지 확인하세요"
    Write-Host "4. 위 오류 메시지를 검토하고 필요한 조치를 취하세요"
    Write-Host ""
    exit 1
}
