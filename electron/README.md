# PeanutVision Electron 앱

PeanutVision 데스크톱 앱의 Electron 쉘(shell) 모듈입니다. ASP.NET Core 백엔드 실행 파일을 자식 프로세스로 실행하고, React UI를 네이티브 창에 표시합니다.

---

## 목차

1. [Electron이란?](#1-electron이란)
2. [이 앱의 구조](#2-이-앱의-구조)
3. [개발 환경 실행](#3-개발-환경-실행)
4. [배포용 인스톨러 빌드](#4-배포용-인스톨러-빌드)
5. [설치 / 업그레이드 / 삭제 방법](#5-설치--업그레이드--삭제-방법)
6. [트러블슈팅](#6-트러블슈팅)

---

## 1. Electron이란?

**Electron**은 Node.js + Chromium을 조합하여 데스크톱 앱을 만드는 프레임워크입니다.

| 구성 요소 | 역할 |
|-----------|------|
| **메인 프로세스** (Node.js) | OS 접근, 파일 시스템, 자식 프로세스 생성, 창 관리 |
| **렌더러 프로세스** (Chromium) | HTML/CSS/JavaScript UI 렌더링 |

일반 웹 앱과 달리, Electron 앱은 파일 시스템 접근, 외부 프로세스 실행, 시스템 트레이 통합 등 OS 수준의 작업이 가능합니다.

**왜 Electron을 선택했나요?**

- 성숙한 패키징 도구 (electron-builder)로 NSIS 인스톨러를 쉽게 생성할 수 있습니다.
- React 프론트엔드와 자연스럽게 통합됩니다.
- VS Code, Slack, Discord 등이 실제로 사용하는 검증된 기술 스택입니다.

---

## 2. 이 앱의 구조

### 전체 아키텍처

```
┌─────────────────────────────────────────────────┐
│                  Electron 앱                    │
│                                                 │
│  ┌─────────────────────────────────────────┐   │
│  │         메인 프로세스 (main.js)          │   │
│  │         [Node.js 환경]                   │   │
│  │                                         │   │
│  │  1. 빈 TCP 포트 탐색                    │   │
│  │  2. PeanutVision.Api.exe 시작           │───┼──→  [ASP.NET Core 백엔드]
│  │  3. /health 폴링으로 준비 확인          │   │      포트: 동적 할당
│  │  4. BrowserWindow에 React UI 로드       │   │      PEANUT_PORT 환경변수로 전달
│  │  5. 앱 종료 시 POST /shutdown 호출      │   │
│  └───────────────┬─────────────────────────┘   │
│                  │ preload.js (보안 브릿지)      │
│  ┌───────────────▼─────────────────────────┐   │
│  │       렌더러 프로세스 (Chromium)         │   │
│  │       React UI                          │   │
│  │       http://localhost:{port}           │   │
│  └─────────────────────────────────────────┘   │
└─────────────────────────────────────────────────┘
```

### 각 파일의 역할

#### `main.js` — 메인 프로세스 진입점

앱의 핵심 로직이 모두 여기에 있습니다.

| 단계 | 동작 |
|------|------|
| **포트 탐색** | OS에게 빈 TCP 포트를 자동 배정받음 (port 0 바인드 트릭) |
| **백엔드 시작** | `child_process.spawn()`으로 `PeanutVision.Api.exe` 실행, `PEANUT_PORT` 환경변수 전달 |
| **준비 대기** | `/health` 엔드포인트를 300ms 간격으로 폴링, 최대 15초 대기 |
| **창 생성** | `BrowserWindow`로 네이티브 창 생성 (주소창 없음) |
| **UI 로드** | `http://localhost:{port}` 로드 (백엔드가 React 정적 파일 서빙) |
| **앱 종료** | `POST /shutdown` 호출 → 3초 대기 → 프로세스 강제 종료 |

고정 포트를 쓰지 않는 이유: 다른 프로세스와 충돌을 방지하기 위해 OS가 빈 포트를 자동 배정합니다.

graceful shutdown이 중요한 이유: MultiCam 드라이버(`McDelete`, `McCloseDriver`)는 강제 종료 시 드라이버 리소스가 해제되지 않아 다음 실행 시 초기화 실패가 발생할 수 있습니다. `/shutdown` 호출로 ASP.NET Core가 DI 컨테이너를 정상 정리하도록 합니다.

#### `preload.js` — 렌더러 보안 브릿지

렌더러 프로세스(웹 페이지)가 시작되기 직전에 실행되는 특별한 스크립트입니다.

- **왜 필요한가?** `contextIsolation: true` 설정으로 웹 페이지 JS는 Node.js API에 직접 접근할 수 없습니다. `contextBridge.exposeInMainWorld()`로 명시적으로 노출한 것만 접근 가능합니다.
- **현재 상태:** PeanutVision의 React UI는 백엔드와 직접 HTTP 통신하므로 현재는 아무것도 노출하지 않습니다. 향후 IPC(파일 다이얼로그, 시스템 트레이 알림 등)가 필요해지면 이 파일에 추가합니다.

#### `package.json` — npm 메타데이터 및 빌드 설정

- **`build` 섹션**: electron-builder 설정 (NSIS 인스톨러, 아이콘, extraResources 등)
- **`extraResources`**: `resources/PeanutVision.Api/` 폴더 전체를 인스톨러에 포함
- **`files`**: `main.js`, `preload.js`만 번들에 포함

#### 왜 백엔드를 별도 프로세스로 실행하나요?

WPF 앱처럼 C# 코드를 직접 호스팅하는 방식과 달리, Electron(Node.js)은 C# DLL을 인프로세스로 실행할 수 없습니다. 대신 별도 exe로 실행하고 HTTP로 통신하는 아키텍처를 택했습니다. 이 덕분에 프론트엔드(React)와 백엔드(ASP.NET Core)가 완전히 독립적으로 개발/테스트 가능합니다.

---

## 3. 개발 환경 실행

### 사전 요구사항

- Node.js 20 이상 및 npm
- .NET 10 SDK 이상

### 실행 순서

**1단계: React 프론트엔드 빌드**

```powershell
cd src/peanut-vision-ui
npm install
npm run build
```

빌드 결과는 `src/peanut-vision-ui/dist/`에 생성됩니다.

**2단계: ASP.NET Core 백엔드 게시**

```powershell
dotnet publish src/PeanutVision.Api `
    -r win-x64 `
    --self-contained true `
    -c Release `
    -o electron/resources/PeanutVision.Api
```

이 명령은 다음을 수행합니다:
- Windows x64용 self-contained 실행 파일 생성 (별도 .NET 설치 불필요)
- MSBuild 타겟 `CopyReactBuild`가 자동 실행되어 React 빌드 파일을 `wwwroot/`로 복사
- 결과: `electron/resources/PeanutVision.Api/PeanutVision.Api.exe`

**3단계: Electron npm 패키지 설치**

```powershell
cd electron
npm install
```

**4단계: 앱 실행**

```powershell
npm start
```

내부적으로 `electron .`을 실행합니다.

### 개발 모드 경로 설정 주의사항

`main.js`의 개발 경로 (`app.isPackaged === false` 분기)는 아래 기본값으로 설정되어 있습니다:

```
src/PeanutVision.Api/bin/Debug/net8.0/win-x64/publish/PeanutVision.Api.exe
```

위 2단계처럼 `electron/resources/PeanutVision.Api/`로 publish한 경우 경로가 다릅니다. 개발 시에는 `electron/resources/PeanutVision.Api/PeanutVision.Api.exe`를 직접 가리키도록 `main.js`의 개발 경로를 수정하거나, 2단계의 출력 경로를 그대로 사용하세요.

---

## 4. 배포용 인스톨러 빌드

### 원커맨드 빌드

저장소 루트에서 실행합니다:

```powershell
.\scripts\build.ps1
```

이 스크립트는 순서대로 다음 4단계를 자동으로 수행합니다:

1. React 프론트엔드 빌드 (`npm install && npm run build`)
2. ASP.NET Core 백엔드 게시 (`dotnet publish -r win-x64 --self-contained`)
3. Electron npm 패키지 설치
4. NSIS 인스톨러 생성 (`npm run dist`)

### 빌드 결과물

```
electron/dist/PeanutVision Setup {version}.exe
```

### 버전 변경 방법

`electron/package.json`의 `"version"` 필드를 수정합니다:

```json
{
  "version": "1.0.1"
}
```

인스톨러 파일명이 `PeanutVision Setup 1.0.1.exe`로 자동 변경됩니다.

### 아이콘 파일 준비

빌드 전에 다음 파일을 `electron/build-resources/`에 추가해야 합니다:

| 파일 | 용도 |
|------|------|
| `icon.ico` | 앱 창 아이콘 및 실행 파일 아이콘 |
| `installer-icon.ico` | NSIS 인스톨러/언인스톨러 아이콘 |

이 파일들은 git에 체크인되어 있지 않습니다 (`build-resources/.gitkeep`만 존재). 빌드 담당자가 직접 준비해야 합니다.

---

## 5. 설치 / 업그레이드 / 삭제 방법

### 설치

`PeanutVision Setup {version}.exe`를 실행합니다. 설치 경로를 선택할 수 있습니다 (`allowToChangeInstallationDirectory: true`).

설치 후 자동으로 바탕화면 바로가기와 시작 메뉴 항목이 생성됩니다.

### 업그레이드

새 버전의 인스톨러 `.exe`를 실행하면 자동으로 이전 버전을 교체합니다. 기존 버전을 먼저 삭제할 필요 없습니다.

### 삭제

**제어판** → **프로그램 및 기능** → **PeanutVision** → **제거**

또는 시작 메뉴 → PeanutVision → 우클릭 → 제거

사용자 데이터(DB, 설정 파일 등)는 제거 후에도 유지됩니다 (`deleteAppDataOnUninstall: false`).

---

## 6. 트러블슈팅

| 증상 | 원인 | 해결 방법 |
|------|------|-----------|
| 앱 실행 시 "백엔드 실행 파일을 찾을 수 없습니다" 오류 | `dotnet publish` 미실행으로 `PeanutVision.Api.exe` 없음 | 먼저 `dotnet publish`를 실행하여 `electron/resources/PeanutVision.Api/`를 생성하세요 |
| 앱 시작 중 로딩 화면에서 멈춤 (15초 후 오류 다이얼로그) | 백엔드 포트 충돌 또는 MultiCam DLL 누락 | 작업 관리자에서 기존 `PeanutVision.Api.exe` 프로세스를 종료하고, MultiCam 드라이버 설치 여부를 확인하세요 |
| 빌드 중 "icon.ico를 찾을 수 없음" 오류 | 아이콘 파일이 없음 | `electron/build-resources/icon.ico` 및 `installer-icon.ico`를 추가하세요 |
| 창은 뜨는데 화면이 빈 채로 나옴 | React 빌드 파일이 `wwwroot/`에 없음 | `npm run build` 후 `dotnet publish`를 다시 실행하세요 (MSBuild가 React 파일을 복사함) |
| 업그레이드 후 구버전이 실행됨 | 바탕화면의 구버전 바로가기가 남아있음 | 새로 생성된 바로가기를 사용하거나 재설치하세요 |
| `npm start` 시 Electron이 즉시 종료됨 | `electron/resources/PeanutVision.Api/` 경로가 잘못됨 | `main.js`의 개발 경로(`exePath`)를 실제 `.exe` 경로로 수정하세요 |
