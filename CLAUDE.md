이 가이드는 레거시 `.NET Framework` 라이브러리를 사용하지 않고, **P/Invoke(`LibraryImport`)**를 통해 네이티브 드라이버에 직접 접근하여 **Grablink Full(PC1622)** 보드와 **Crevis TC-A160K** 카메라를 제어하는 데 최적화되어 있습니다.

---

# CLAUDE.md: Vision System Project (Grablink & Crevis)

## 📌 Project Overview

- **Frame Grabber:** Euresys Grablink Full (PC1622) [User Spec].
- **Camera:** Crevis TC-A160K (Area-Scan, Camera Link) [User Spec].
- **Runtime:** .NET 8 (C# 12) [User Query].
- **SDK Strategy:** Native C API Interop via `LibraryImport` (P/Invoke) [User Query].

## 🛠 Tech Stack & Dependencies

- **Core Library:** `MultiCam.dll` (System level driver).
- **Header References:** `MultiCam.h`, `McParams.h` (Standard C Identifiers).
- **Interoperability:** `System.Runtime.InteropServices` for source-generated P/Invoke.

## 📐 Implementation Architecture

- **Driver Connection:** `McOpenDriver(null)`을 호출하여 드라이버 통신 채널 확보.
- **Channel Object:** 카메라와 메모리 사이의 획득 경로인 `MC_CHANNEL` 생성 및 관리.
- **Signaling:** `McRegisterCallback`과 `MC_SIG_SURFACE_PROCESSING`을 사용한 이벤트 기반 이미지 획득.
- **Memory Management:** 서피스(Surface)와 클러스터(Cluster)를 통한 다중 버퍼 관리.

## 💻 Key Native API Snippets

모든 네이티브 메서드는 `partial`로 선언하며 `LibraryImport`를 사용합니다.

```csharp
public static partial class MultiCamNative
{
    private const string LibraryName = "MultiCam.dll";

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McOpenDriver(string? multiCamName); // NULL 전달 필수

    [LibraryImport(LibraryName)]
    public static partial int McCloseDriver(); // 종료 시 리소스 해제

    [LibraryImport(LibraryName)]
    public static partial int McCreate(uint model, out uint instance); // 모델: 0x20000000(CHANNEL)

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial int McSetParamStr(uint instance, uint paramId, string value); // 파라미터 설정

    [LibraryImport(LibraryName)]
    public static partial int McSetParamInt(uint instance, uint paramId, int value); // 정수형/상수 설정
}
```

## 💻 HAL(Hardware Abstraction Layer)

## 📸 Hardware Control (Crevis TC-A160K)

카메라 전용 기능은 `MC_CamConfig` 또는 `MC_CamFile` 파라미터를 통해 제어합니다.

- **CamFile 로드:** `TC-A160K-SEM_freerun_RGB8.cam` [User Spec].
- **White Balance:** `Balance White Auto = ONCE` 실행 시 흰색 타겟(200DN 수준) 촬영 필수 [Crevis 6, 7].
- **FFC Calibration:**
  - **Black:** 렌즈 차폐 후 `BlackCalibration = Execute` [Crevis 5].
  - **White:** 평평한 조명 아래서 `WhiteCalibration = Execute` [Crevis 5].
- **Trigger:** 소프트웨어 트리거 발생 시 `MC_ForceTrig = MC_ForceTrig_TRIG` 설정.

## ⚠️ Development Rules

1. **Error Handling:** 모든 API 호출 결과(`MCSTATUS`)가 `0(MC_OK)`이 아니면 예외를 발생시키거나 로그를 남길 것.
2. **Resource Cleanup:** `IDisposable` 패턴을 사용하여 `McDelete` 및 `McCloseDriver`를 반드시 호출할 것.
3. **Thread Safety:** 콜백 함수는 별도의 드라이버 전용 스레드에서 실행되므로 UI 업데이트 시 스레드 동기화(Invoke) 주의.
4. **Driver Polling:** `McOpenDriver` 호출 시 `MC_SERVICE_ERROR(-25)` 발생 시 성공할 때까지 루프 폴링 권장.

## 📂 Project Structure Suggestion

- `/Native`: `MultiCamNative.cs` (LibraryImport 정의 및 상수).
- `/Services`: `VisionService.cs` (채널 제어, 이미지 획득 로직).
- `/Calibration`: `CrevisController.cs` (FFC, 화이트 밸런스 전용).
- `/Models`: 서피스 데이터 및 신호 정보 구조체.

---

이 `CLAUDE.md` 파일을 프로젝트 루트에 저장하고 AI와 대화할 때 참고하게 하면, **.NET 8 기반의 고성능 비전 시스템 코드**를 일관성 있게 생성할 수 있습니다. [User Spec, User Query].
