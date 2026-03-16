# Euresys MultiCam SDK Documentation Index

> Converted from Euresys MultiCam 6.19.4 official PDF documentation

---

## Core SDK Documentation

| Doc ID | Title | File | Size |
|--------|-------|------|------|
| D402EN | MultiCam User Guide | [multicam-user-guide.md](multicam-user-guide.md) | 90KB |
| D403EN | MultiCam API Reference | [multicam-api-reference.md](multicam-api-reference.md) | 51KB |
| D405EN | Acquisition Principles | [multicam-acquisition-principles.md](multicam-acquisition-principles.md) | 33KB |
| D406EN | Storage Formats | [multicam-storage-formats.md](multicam-storage-formats.md) | 26KB |
| D412EN | MultiCam Parameters | [multicam-parameters.md](multicam-parameters.md) | 391KB |
| D404EN | Sample Programs | [multicam-sample-programs.md](multicam-sample-programs.md) | 11KB |
| — | Error Codes | [multicam-error-codes.md](multicam-error-codes.md) | 16KB |
| D401EN | Release Notes | [multicam-release-notes.md](multicam-release-notes.md) | 41KB |

## Grablink Documentation

| Doc ID | Title | File | Size |
|--------|-------|------|------|
| D411EN | Grablink Functional Guide | [grablink/README.md](grablink/README.md) (9 chapters) | 63KB |
| D413EN | Grablink Hardware Manual | [grablink-hardware-manual.md](grablink-hardware-manual.md) | 59KB |
| — | Grablink GPIO User Guide | [grablink-gpio-guide.md](grablink-gpio-guide.md) | 22KB |
| D421EN | Grablink Installation Guide | [grablink-installation-guide.md](grablink-installation-guide.md) | 10KB |
| — | Grablink Migration Guide | [grablink-migration-guide.md](grablink-migration-guide.md) | 24KB |

## Project-Specific Documentation

| Title | File | Size |
|-------|------|------|
| Grablink Full (PC1622) Spec | [grablink_full_spec.md](grablink_full_spec.md) | 7KB |
| Crevis TC-A160K Reference | [crevis-tc-a160k-reference.md](crevis-tc-a160k-reference.md) | 7KB |
| Hardware Layout (3-Camera) | [hw-layout-description.md](hw-layout-description.md) | 3KB |
| Capture Mode Guide | [capture-mode-guide.md](capture-mode-guide.md) | 14KB |

---

## Cross-Reference: Common Tasks

| Task | Primary Doc | Supporting Docs |
|------|-------------|-----------------|
| Channel 설정 & 이미지 획득 | [acquisition-principles](multicam-acquisition-principles.md) | [user-guide](multicam-user-guide.md), [parameters](multicam-parameters.md) |
| 파라미터 이름/값 조회 | [parameters](multicam-parameters.md) | [api-reference](multicam-api-reference.md) |
| 픽셀 포맷 & 메모리 레이아웃 | [storage-formats](multicam-storage-formats.md) | [parameters](multicam-parameters.md) |
| 트리거 설정 (SW/HW) | [acquisition-principles](multicam-acquisition-principles.md) | [user-guide](multicam-user-guide.md), [capture-mode](capture-mode-guide.md) |
| GPIO / 외부 I/O 배선 | [gpio-guide](grablink-gpio-guide.md) | [hardware-manual](grablink-hardware-manual.md), [grablink/07-io-toolbox](grablink/07-io-toolbox.md) |
| 카메라 연결 & 커넥터 핀배치 | [hardware-manual](grablink-hardware-manual.md) | [grablink/02-camera-link](grablink/02-camera-link-interface.md) |
| 캘리브레이션 (FFC, WB) | [crevis-reference](crevis-tc-a160k-reference.md) | [grablink/06-white-balance](grablink/06-white-balance.md) |
| 에러 코드 디버깅 | [error-codes](multicam-error-codes.md) | [release-notes](multicam-release-notes.md) |
| C API 함수 호출 | [api-reference](multicam-api-reference.md) | [sample-programs](multicam-sample-programs.md) |
| 보드 설치 & 드라이버 설정 | [installation-guide](grablink-installation-guide.md) | [release-notes](multicam-release-notes.md) |

---

*Generated: 2026-03-16 | Source: Euresys MultiCam SDK 6.19.4*
