# Vision System Software Project (Grablink Full & TC-A160K)

이 프로젝트는 Euresys Grablink Full 프레임 그래버와 TC-A160K 카메라를 사용하여 고속 이미지를 획득하고 처리하는 C# 기반 애플리케이션입니다.
📋 사전 요구 사항 (Prerequisites)

1. 하드웨어 요구 사항
   • 프레임 그래버: Euresys Grablink Full (PC1622).
   • 카메라: TC-A160K (Area-Scan, Camera Link 인터페이스).
   • 케이블: 표준 Camera Link 케이블.
   • 전원: PoCL(Power over Camera Link) 지원 여부 확인.
2. 소프트웨어 요구 사항
   • 드라이버: Euresys MultiCam 6.19.4 버전 이상의 공식 드라이버.
   • 운영체제: Windows 7/8.1/10 (32/64-bit) 또는 Linux.
   • 개발 환경: .NET Framework 기반 C# 개발 환경 (Visual Studio 등).
3. 라이브러리 설정
   • 프로젝트 참조에 Euresys.MultiCam.dll 어셈블리를 추가해야 합니다.
   • 코드 상단에 using Euresys.MultiCam; 네임스페이스 선언이 필요합니다.

## ⚙️ 주요 카메라 설정 (CamFile)

이 프로젝트는 TC-A160K-SEM_freerun_RGB8.cam 파일을 사용합니다. 주요 파라미터는 다음과 같습니다:
• Imaging: AREA (Area-Scan).
• Resolution: 1456(H) x 1088(V).
• Tap Configuration: BASE_1T24 (24-bit RGB).
• Color Method: RGB.
• Acquisition Mode: SNAPSHOT 또는 VIDEO.

## 💻 하드웨어 상태 조회 코드 예제 (C#)

```cs
using System;
using Euresys.MultiCam;

namespace VisionSystem
{
class Program
{
static void Main(string[] args)
{
try
{
// 드라이버 통신 시작 [21, 22]
Mc.OpenDriver();

                // 설치된 보드 개수 조회 [23, 24]
                int boardCount;
                Mc.GetParamInt(Mc.Configuration, MC.BoardCount, out boardCount);
                Console.WriteLine($"Detected Boards: {boardCount}");

                for (int i = 0; i < boardCount; i++)
                {
                    uint boardHandle = Mc.Board + (uint)i; // 보드 핸들 [25, 26]

                    string boardName;
                    Mc.GetParamStr(boardHandle, MC.BoardName, out boardName);

                    // PoCL 상태 확인 [7, 8]
                    int poclStatus;
                    Mc.GetParamInt(boardHandle, MC.PoCL_Status, out poclStatus);

                    Console.WriteLine($"Board [{i}]: {boardName}");
                    Console.WriteLine($"PoCL Status: {poclStatus}");
                }
            }
            catch (MultiCamException e)
            {
                Console.WriteLine($"Error: {e.Message}"); [27, 28]
            }
            finally
            {
                // 자원 해제 [21, 29]
                Mc.CloseDriver();
            }
        }
    }

}
```

# 📚 관련 문서 및 링크 (Documentation)

모든 최신 문서는 Euresys 지원 페이지에서 확인 가능합니다.

- https://documentation.euresys.com/Products/MULTICAM/MULTICAM/Content/00_Home/PDF_Guides.htm
