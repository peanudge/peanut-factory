'use strict';

/**
 * electron/main.js — Electron 메인 프로세스 진입점
 *
 * ===== Electron 아키텍처 개요 (처음 접하는 개발자를 위한 설명) =====
 *
 * Electron 앱은 두 종류의 프로세스로 구성됩니다:
 *
 * 1. 메인 프로세스 (Main Process) — 이 파일
 *    - Node.js 환경에서 실행됩니다 (파일 시스템, 자식 프로세스 등 OS 접근 가능)
 *    - 앱의 생명주기(시작/종료)를 관리합니다
 *    - BrowserWindow를 생성하고 관리합니다
 *    - 백엔드(ASP.NET Core) 프로세스를 시작하고 종료합니다
 *    - 전체 앱에서 단 하나만 존재합니다
 *
 * 2. 렌더러 프로세스 (Renderer Process) — BrowserWindow 내부
 *    - Chromium(브라우저 엔진)에서 실행됩니다
 *    - React UI가 표시되는 곳입니다
 *    - 보안상의 이유로 Node.js API에 직접 접근할 수 없습니다
 *    - preload.js를 통해 메인 프로세스와 제한적으로 통신합니다
 *
 * PeanutVision의 전체 구조:
 *   [Electron 메인] → 백엔드 exe 시작 → 포트 확인 → BrowserWindow에 React UI 로드
 *   [React UI] ↔ [ASP.NET Core API] : HTTP 통신
 */

const { app, BrowserWindow, dialog } = require('electron');
const { spawn } = require('child_process');
const fs = require('fs');
const net = require('net');
const http = require('http');
const path = require('path');

// ─── 전역 변수 ───────────────────────────────────────────────────────────────
// 메인 창과 백엔드 프로세스에 대한 참조를 전역으로 유지합니다.
// 앱 종료 시 정리(cleanup)하기 위해 전역에서 접근할 수 있어야 합니다.
let mainWindow = null;  // BrowserWindow 인스턴스
let backendProcess = null;  // ASP.NET Core 자식 프로세스
let isQuitting = false;  // 앱이 종료 중인지 추적 (중복 종료 방지)
let backendPort = null;  // 백엔드가 리스닝하는 포트 (graceful shutdown에서 사용)

// ─── 유틸리티: 사용 가능한 TCP 포트 찾기 ──────────────────────────────────────
/**
 * 운영체제에게 사용 가능한 TCP 포트를 자동으로 할당받습니다.
 *
 * 방법: port 0으로 서버를 바인드하면 OS가 빈 포트를 자동으로 골라줍니다.
 * 그 포트 번호를 읽고 서버를 즉시 닫은 뒤 반환합니다.
 *
 * 왜 포트를 동적으로 찾나요?
 * - 고정 포트(예: 5000)는 이미 다른 프로세스가 사용 중일 수 있습니다
 * - 여러 인스턴스가 동시에 실행될 경우 충돌이 발생합니다
 * - OS가 빈 포트를 직접 골라주므로 가장 신뢰할 수 있는 방법입니다
 *
 * @returns {Promise<number>} 사용 가능한 포트 번호
 */
function findFreePort() {
  return new Promise((resolve, reject) => {
    const server = net.createServer();

    // port 0으로 바인드: OS가 사용 가능한 포트를 자동 배정
    server.listen(0, '127.0.0.1', () => {
      const { port } = server.address();
      // 포트 번호를 읽은 직후 서버를 닫습니다.
      // 서버를 열어둔 채로 두면 해당 포트가 점유되어 백엔드가 사용할 수 없습니다.
      server.close(() => resolve(port));
    });

    server.on('error', reject);
  });
}

// ─── 유틸리티: 백엔드 준비 대기 ───────────────────────────────────────────────
/**
 * ASP.NET Core 백엔드가 HTTP 요청을 받을 준비가 될 때까지 폴링합니다.
 *
 * 왜 폴링이 필요한가요?
 * - 백엔드 프로세스를 spawn()하는 순간은 exe가 실행된 것이지
 *   API 서버가 준비된 것이 아닙니다
 * - ASP.NET Core는 시작 시 DI 컨테이너 구성, DB 마이그레이션 등을 수행합니다
 * - 준비되기 전에 UI를 로드하면 "연결 실패" 오류가 발생합니다
 *
 * /health 엔드포인트: ASP.NET Core의 HealthCheck 미들웨어가 제공하는
 * 표준 엔드포인트로, 서버가 살아있으면 200 OK를 반환합니다.
 *
 * @param {number} port - 백엔드가 리스닝하는 포트
 * @param {number} [timeoutMs=15000] - 최대 대기 시간 (밀리초)
 * @param {number} [intervalMs=300] - 폴링 간격 (밀리초)
 * @returns {Promise<void>} 준비되면 resolve, 타임아웃이면 reject
 */
function waitForBackend(port, timeoutMs = 15000, intervalMs = 300) {
  return new Promise((resolve, reject) => {
    const started = Date.now();

    function poll() {
      // 앱이 종료 중이면 더 이상 폴링하지 않습니다
      if (isQuitting) {
        reject(new Error('앱이 종료되어 백엔드 대기를 중단합니다.'));
        return;
      }

      // 타임아웃 확인
      if (Date.now() - started > timeoutMs) {
        reject(
          new Error(
            `백엔드가 ${timeoutMs / 1000}초 내에 응답하지 않았습니다. ` +
            `포트 ${port}에서 PeanutVision.Api.exe가 정상적으로 시작되었는지 확인하세요.`
          )
        );
        return;
      }

      // /health 엔드포인트에 HTTP GET 요청
      const req = http.get(`http://localhost:${port}/health`, (res) => {
        if (res.statusCode === 200) {
          // 200 OK: 백엔드 준비 완료
          resolve();
        } else {
          // 다른 상태 코드: 아직 준비 중이므로 재시도
          setTimeout(poll, intervalMs);
        }
        // 응답 바디를 소비해야 소켓이 해제됩니다
        res.resume();
      });

      req.on('error', () => {
        // ECONNREFUSED 등: 아직 서버가 포트를 열지 않은 상태 → 재시도
        setTimeout(poll, intervalMs);
      });

      // 요청 자체의 타임아웃 설정 (폴링 간격보다 짧게)
      req.setTimeout(intervalMs - 50, () => {
        req.destroy();
        setTimeout(poll, intervalMs);
      });
    }

    poll();
  });
}

// ─── 유틸리티: 백엔드 프로세스 시작 ───────────────────────────────────────────
/**
 * ASP.NET Core 백엔드 exe를 자식 프로세스로 시작합니다.
 *
 * 경로 결정 로직:
 * - 패키징된 앱(배포 시): Electron이 exe와 함께 패키징됩니다.
 *   electron-builder는 추가 파일을 resources/ 폴더에 넣으며,
 *   Node.js에서 process.resourcesPath로 접근합니다.
 * - 개발 모드: 로컬에서 빌드한 경로를 직접 지정합니다.
 *   (개발자가 실제 경로로 수정해야 합니다)
 *
 * 환경 변수:
 * - PEANUT_PORT: 백엔드가 리스닝할 포트 번호
 * - ASPNETCORE_ENVIRONMENT: ASP.NET Core 실행 환경 지정
 *
 * @param {number} port - 백엔드가 리스닝할 포트
 * @returns {ChildProcess} Node.js child_process 인스턴스
 */
function startBackend(port) {
  let exePath;

  if (app.isPackaged) {
    // 패키징된 앱(배포 환경):
    // electron-builder가 백엔드를 resources/PeanutVision.Api/ 에 복사합니다.
    // process.resourcesPath는 Electron이 제공하는 resources 폴더 절대경로입니다.
    exePath = path.join(
      process.resourcesPath,
      'PeanutVision.Api',
      'PeanutVision.Api.exe'
    );
  } else {
    // 개발 환경:
    // 아래 경로를 실제 로컬 빌드 출력 경로로 수정하세요.
    // 예: dotnet publish 후의 publish 폴더 경로
    // TODO: 개발자가 실제 경로로 변경해야 합니다
    exePath = path.join(
      __dirname,           // electron/ 폴더
      '..',                // 프로젝트 루트
      'src',
      'PeanutVision.Api',
      'bin',
      'Debug',
      'net8.0',
      'win-x64',
      'publish',
      'PeanutVision.Api.exe'
    );
  }

  // 실행 파일이 존재하는지 먼저 확인합니다.
  // 개발 환경에서 dotnet publish 없이 실행하면 파일이 없어 spawn이 모호한 오류를 냅니다.
  if (!fs.existsSync(exePath)) {
    throw new Error(
      `백엔드 실행 파일을 찾을 수 없습니다: ${exePath}\n` +
      `먼저 dotnet publish를 실행하세요.`
    );
  }

  console.log(`[백엔드] 시작 경로: ${exePath}`);
  console.log(`[백엔드] 포트: ${port}`);

  // child_process.spawn()으로 백엔드를 별도 프로세스로 실행합니다.
  // - 첫 번째 인수: 실행할 파일 경로
  // - 두 번째 인수: 명령줄 인수 배열 (이 경우 없음)
  // - 세 번째 인수: 옵션 객체
  const child = spawn(exePath, [], {
    env: {
      ...process.env,                          // 현재 환경 변수 상속
      PEANUT_PORT: String(port),               // 백엔드가 사용할 포트
      ASPNETCORE_ENVIRONMENT: 'Production',    // ASP.NET Core 환경 설정
    },
    // stdio: 자식 프로세스의 표준 입출력 처리 방식
    // - 'ignore': stdin (입력 불필요)
    // - 'pipe': stdout/stderr를 Node.js 스트림으로 받아 로깅에 사용
    stdio: ['ignore', 'pipe', 'pipe'],
  });

  // 백엔드 표준 출력을 Electron 콘솔에 중계합니다 (디버깅용)
  if (child.stdout) {
    child.stdout.on('data', (data) => {
      process.stdout.write(`[백엔드 stdout] ${data}`);
    });
  }

  // 백엔드 표준 에러를 Electron 콘솔 에러에 중계합니다 (디버깅 필수)
  if (child.stderr) {
    child.stderr.on('data', (data) => {
      process.stderr.write(`[백엔드 stderr] ${data}`);
    });
  }

  // 백엔드 프로세스가 예기치 않게 종료된 경우를 감지합니다.
  // isQuitting이 true인 경우는 앱이 정상 종료하면서 직접 kill()한 것이므로 무시합니다.
  child.on('exit', (code, signal) => {
    if (!isQuitting) {
      // 예기치 않은 종료: 오류를 로깅하지만 Electron 창을 강제로 닫지는 않습니다.
      // 창에는 이미 연결 실패 메시지가 표시될 것입니다.
      console.error(
        `[백엔드] 예기치 않게 종료되었습니다. 종료 코드: ${code}, 시그널: ${signal}`
      );
    }
  });

  return child;
}

// ─── 로딩 화면 HTML ────────────────────────────────────────────────────────────
/**
 * 백엔드가 준비되기 전에 표시할 인라인 HTML입니다.
 *
 * 왜 인라인 HTML인가요?
 * - 별도의 loading.html 파일을 만들지 않아도 됩니다
 * - 파일 경로 문제(패키징 시 경로 변경)를 피할 수 있습니다
 * - data: URI 형식으로 BrowserWindow에 직접 로드합니다
 */
const LOADING_HTML = `data:text/html;charset=utf-8,<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <title>PeanutVision</title>
  <style>
    * { margin: 0; padding: 0; box-sizing: border-box; }
    body {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
      background: #1a1a2e;
      color: #e0e0e0;
      font-family: 'Segoe UI', sans-serif;
    }
    .container { text-align: center; }
    h1 { font-size: 2rem; margin-bottom: 1rem; color: #f0c040; }
    p { font-size: 1rem; color: #aaa; }
    .dot { display: inline-block; animation: bounce 1s infinite; }
    .dot:nth-child(2) { animation-delay: 0.2s; }
    .dot:nth-child(3) { animation-delay: 0.4s; }
    @keyframes bounce {
      0%, 80%, 100% { transform: translateY(0); }
      40% { transform: translateY(-8px); }
    }
  </style>
</head>
<body>
  <div class="container">
    <h1>PeanutVision</h1>
    <p>시작 중<span class="dot">.</span><span class="dot">.</span><span class="dot">.</span></p>
  </div>
</body>
</html>`;

// ─── 메인 창 생성 ─────────────────────────────────────────────────────────────
/**
 * BrowserWindow를 생성합니다.
 *
 * BrowserWindow란?
 * - Electron에서 UI 창을 나타내는 객체입니다
 * - 내부적으로 Chromium을 사용하여 HTML/CSS/JavaScript를 렌더링합니다
 * - 일반 브라우저 창과 달리 주소표시줄, 뒤로/앞으로 버튼 등을 숨길 수 있습니다
 * - 네이티브 앱처럼 보이게 만들 수 있습니다
 *
 * @returns {BrowserWindow}
 */
function createWindow() {
  const win = new BrowserWindow({
    width: 1280,
    height: 800,
    minWidth: 800,
    minHeight: 600,

    // autoHideMenuBar: true
    // - 상단의 파일/편집/도움말 메뉴바를 숨깁니다
    // - 브라우저처럼 보이는 느낌을 없애고 네이티브 앱처럼 보이게 합니다
    // - Alt 키를 누르면 일시적으로 표시됩니다
    autoHideMenuBar: true,

    // show: false
    // - 창을 처음에 숨긴 상태로 생성합니다
    // - 백엔드가 준비된 후에 win.show()를 호출해야 창이 나타납니다
    // - 이렇게 하면 빈 창이 깜빡이는 현상(flash of unstyled content)을 방지합니다
    show: false,

    webPreferences: {
      // contextIsolation: true (보안 필수 설정)
      // - 렌더러 프로세스(웹 페이지)와 preload 스크립트를 서로 다른
      //   JavaScript 컨텍스트에서 실행합니다
      // - 웹 페이지의 JS가 preload에서 노출한 API 외에는 Node.js에 접근 불가
      // - XSS 공격이 발생해도 Node.js API를 악용할 수 없게 합니다
      contextIsolation: true,

      // nodeIntegration: false (보안 필수 설정)
      // - 렌더러 프로세스(React UI)에서 Node.js require()를 사용할 수 없게 합니다
      // - true로 설정하면 웹 페이지에서 파일 시스템, 자식 프로세스 등에
      //   직접 접근할 수 있어 심각한 보안 취약점이 됩니다
      // - 대신 preload.js의 contextBridge를 통해 안전하게 기능을 노출합니다
      nodeIntegration: false,

      // preload: 렌더러 프로세스가 시작되기 직전에 실행할 스크립트 경로
      // - Node.js 환경에서 실행되므로 require() 사용 가능
      // - contextBridge로 렌더러에 API를 안전하게 제공합니다
      // - __dirname: 현재 파일(main.js)이 있는 디렉터리 (electron/)
      preload: path.join(__dirname, 'preload.js'),
    },
  });

  return win;
}

// ─── 앱 시작 시퀀스 ────────────────────────────────────────────────────────────
/**
 * app.whenReady(): Electron 앱이 초기화를 완료했을 때 resolve되는 Promise입니다.
 * - macOS에서는 dock 아이콘 클릭으로 앱이 다시 활성화될 때도 호출될 수 있습니다
 * - 이 콜백 안에서 BrowserWindow를 생성해야 합니다
 *
 * async IIFE (즉시 실행 함수 표현식) 패턴을 사용하여 await를 활용합니다.
 */
app.whenReady().then(async () => {
  let port;

  try {
    // 1단계: 사용 가능한 포트 확보
    port = await findFreePort();
    backendPort = port;  // 모듈 스코프에 저장해 graceful shutdown에서 참조할 수 있게 합니다
    console.log(`[앱] 할당된 포트: ${port}`);

    // 2단계: 창 생성 (아직 표시하지 않음)
    mainWindow = createWindow();

    // 3단계: 로딩 화면 표시
    // 백엔드가 준비되는 동안 사용자에게 진행 상황을 보여줍니다
    await mainWindow.loadURL(LOADING_HTML);
    mainWindow.show();

    // 4단계: 백엔드 프로세스 시작
    backendProcess = startBackend(port);

    // 5단계: 백엔드가 HTTP 요청을 받을 준비가 될 때까지 대기
    console.log('[앱] 백엔드 준비 대기 중...');
    await waitForBackend(port);
    console.log('[앱] 백엔드 준비 완료');

    // 6단계: 실제 앱 URL 로드
    // 로딩 화면을 React UI로 교체합니다
    await mainWindow.loadURL(`http://localhost:${port}`);

  } catch (err) {
    // 시작 실패 시 오류를 로깅합니다.
    // isQuitting이 true면 사용자가 로딩 중에 창을 닫은 것이므로 정상입니다.
    if (!isQuitting) {
      console.error('[앱] 시작 실패:', err.message);
      // 사용자가 오류를 인지할 수 있도록 네이티브 에러 다이얼로그를 표시합니다.
      dialog.showErrorBox(
        'PeanutVision 시작 실패',
        `PeanutVision을 시작하는 중 오류가 발생했습니다.\n\n${err.message}\n\n앱을 닫고 다시 시도하세요.`
      );
      app.quit();
    }
  }
});

// ─── 앱 생명주기 이벤트 ────────────────────────────────────────────────────────

// window-all-closed: 모든 BrowserWindow가 닫혔을 때 발생합니다.
// 기본 동작: macOS에서는 창을 닫아도 앱이 종료되지 않습니다 (Dock에 남음).
// PeanutVision은 Windows 전용 앱이므로, 모든 플랫폼에서 창 닫힘 = 앱 종료로 처리합니다.
app.on('window-all-closed', () => {
  app.quit();
});

// before-quit: app.quit()이 호출된 후, 창들이 닫히기 전에 발생합니다.
// 백엔드 프로세스를 정상적으로 종료할 마지막 기회입니다.
//
// 왜 graceful HTTP shutdown이 중요한가요?
// MultiCam 드라이버(McDelete, McCloseDriver)는 정상적인 종료 절차 없이 프로세스가
// 강제 종료되면 드라이버 리소스가 해제되지 않아 다음 실행 시 초기화 실패가 발생할 수 있습니다.
// /shutdown 엔드포인트를 통해 ASP.NET Core가 DI 컨테이너를 정상적으로 정리하도록 합니다.
app.on('before-quit', async (event) => {
  if (isQuitting) return;
  isQuitting = true;
  event.preventDefault(); // 정리 작업이 끝날 때까지 종료를 잠시 중단합니다

  // 1단계: HTTP /shutdown 엔드포인트로 graceful shutdown 요청
  try {
    await fetch(`http://localhost:${backendPort}/shutdown`, { method: 'POST' });
    // 최대 3초간 백엔드가 스스로 종료되길 기다립니다
    await new Promise((resolve) => {
      const timeout = setTimeout(resolve, 3000);
      backendProcess.once('exit', () => { clearTimeout(timeout); resolve(); });
    });
  } catch (_) {
    // 백엔드가 이미 종료됐거나 연결할 수 없는 경우 — 무시하고 강제 종료
  }

  // 2단계: 아직 살아있으면 강제 종료
  if (backendProcess && !backendProcess.killed) {
    console.log('[앱] 백엔드 프로세스 강제 종료 중...');
    backendProcess.kill();
  }

  app.quit(); // 이제 실제로 종료합니다
});

// 앱이 예기치 않게 종료될 때 백엔드 프로세스가 고아가 되지 않도록 함.
// before-quit 이벤트는 정상 종료 시에만 발생하므로, 크래시 경우를 대비해 process.on('exit')도 사용.
// 단, process.on('exit')는 동기 콜백만 실행할 수 있어 async 정리는 불가능.
process.on('exit', () => {
  if (backendProcess && !backendProcess.killed) {
    backendProcess.kill();
  }
});
