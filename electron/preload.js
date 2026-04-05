'use strict';

/**
 * electron/preload.js — 렌더러 프로세스 preload 스크립트
 *
 * ===== preload 스크립트란? =====
 *
 * preload 스크립트는 렌더러 프로세스(Chromium/웹 페이지)가 시작되기 직전에
 * 실행되는 특별한 Node.js 스크립트입니다.
 *
 * 실행 순서:
 *   [메인 프로세스] → BrowserWindow 생성 → [preload.js 실행] → [웹 페이지 로드]
 *
 * preload 스크립트의 특수한 위치:
 * - Node.js API(require, process 등)에 접근할 수 있습니다
 * - 웹 페이지의 DOM에도 접근할 수 있습니다
 * - 그러나 contextIsolation 덕분에 웹 페이지 JS와는 별도의 컨텍스트에서 실행됩니다
 *
 * ===== contextIsolation이란? =====
 *
 * contextIsolation: true 설정(main.js에서 설정됨)은 보안의 핵심입니다.
 *
 * 비활성화(false)일 때의 위험:
 * - preload에서 정의한 변수/함수가 웹 페이지 JS에 그대로 노출됩니다
 * - 로드된 웹 페이지(또는 XSS로 삽입된 악성 JS)가 Node.js API를 직접 호출 가능
 * - 예: window.require('child_process').exec('rm -rf /') 같은 공격이 가능
 *
 * 활성화(true)일 때의 보호:
 * - preload와 웹 페이지는 완전히 분리된 JavaScript 컨텍스트에서 실행됩니다
 * - preload에서 명시적으로 contextBridge.exposeInMainWorld()로 노출한 것만
 *   웹 페이지에서 접근할 수 있습니다
 * - 노출된 함수는 Electron이 직렬화/검증하여 안전하게 중계합니다
 *
 * ===== nodeIntegration: false인 이유 =====
 *
 * main.js에서 nodeIntegration: false로 설정했습니다.
 * 이는 렌더러 프로세스(React UI)에서 require()를 사용할 수 없게 합니다.
 *
 * 왜 비활성화하나요?
 * - React UI는 외부 npm 패키지를 사용합니다 (번들러로 이미 처리됨)
 * - 웹 앱이 Node.js API에 직접 접근하면 보안 경계가 무너집니다
 * - 악의적인 CDN 스크립트나 XSS 공격이 파일 시스템에 접근할 수 있게 됩니다
 *
 * 대신, 메인 프로세스의 기능이 필요하면:
 * - preload.js에서 contextBridge로 안전하게 API를 노출하고
 * - ipcRenderer/ipcMain을 통해 메인 프로세스와 통신합니다
 *
 * ===== 이 파일의 현재 역할 =====
 *
 * 현재는 웹 페이지에 노출하는 API가 없습니다.
 * React UI는 ASP.NET Core 백엔드와 직접 HTTP 통신하므로
 * 메인 프로세스를 거칠 필요가 없습니다.
 *
 * 향후 IPC가 필요한 경우의 예시 (현재 주석 처리):
 * - 앱 버전 정보 제공
 * - 파일 저장 다이얼로그 열기
 * - 시스템 트레이 알림 등
 *
 * 이 파일은 미래 확장을 위한 구조적 토대입니다.
 */

const { contextBridge } = require('electron');

// ─── 향후 IPC 확장을 위한 구조 ─────────────────────────────────────────────────
//
// 현재는 아무것도 노출하지 않습니다.
//
// 나중에 메인 프로세스 기능이 필요해지면 아래와 같이 추가하세요:
//
// const { contextBridge, ipcRenderer } = require('electron');
//
// contextBridge.exposeInMainWorld('electronAPI', {
//   // 예시: 앱 버전 정보 요청
//   getAppVersion: () => ipcRenderer.invoke('get-app-version'),
//
//   // 예시: 파일 저장 다이얼로그
//   showSaveDialog: (options) => ipcRenderer.invoke('show-save-dialog', options),
// });
//
// exposeInMainWorld('electronAPI', {...})를 호출하면
// 웹 페이지에서 window.electronAPI.getAppVersion() 으로 접근 가능합니다.
// 단, 여기서 명시적으로 노출한 함수만 접근 가능하며,
// Node.js 전체가 노출되는 것이 아닙니다.

// 현재는 노출할 API가 없으므로 contextBridge를 import만 하고 사용하지 않습니다.
// 린터 경고를 피하기 위해 아래와 같이 명시적으로 참조합니다.
void contextBridge; // 향후 exposeInMainWorld() 호출 시 사용
