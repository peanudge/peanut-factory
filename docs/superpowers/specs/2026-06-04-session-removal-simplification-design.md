# Session 제거 및 기능 단순화 설계

**날짜:** 2026-06-04  
**접근 방식:** A — BE + FE 동시 제거  
**목표:** Session 개념과 불필요한 ImageSaveSettings 필드를 제거해 코드 복잡도를 낮춘다.

---

## 변경 후 ImageSaveSettings

```json
{
  "outputDirectory": "C:\\PeanutData\\{date}",
  "format": "png",
  "autoSave": true
}
```

### 지원 경로 토큰 (`outputDirectory` 내 사용 가능)

| 토큰 | 확장 결과 | 예시 |
|------|----------|------|
| `{date}` | 촬영 날짜 (`yyyy-MM-dd`) | `2026-06-04` |
| `{profile}` | cam 파일명 (확장자 제외, 특수문자 `_` 치환) | `crevis-tc-a160k-freerun-rgb8` |
| 없음 | 지정 경로 그대로 사용 | — |

### 고정 파일명 포맷

```
capture_20260604_143000_123_00001.png
         ^타임스탬프(yyyyMMdd_HHmmss_fff)  ^5자리 시퀀스번호
```

확장자는 `format` 설정에 따라 결정 (`.png` / `.bmp` / `.raw`).

---

## BE 제거 목록

### 파일 전체 삭제
- `Services/Session.cs`
- `Services/ISessionRepository.cs`
- `Services/SessionRepository.cs`
- `Controllers/SessionController.cs`

### 필드/프로퍼티 제거
- `CapturedImage.SessionId` (nullable FK)
- `CapturedImage.Session` (navigation property)
- `ImageSaveSettings.FilenamePrefix`
- `ImageSaveSettings.TimestampFormat`
- `ImageSaveSettings.IncludeSequenceNumber`
- `ImageSaveSettings.SubfolderStrategy`
- `SubfolderStrategy` enum 전체

### 서비스 수정
- `AutoSaveService` — `ISessionRepository` 조회 제거. `IServiceScopeFactory`는 유지 (`ICapturedImageRepository`가 scoped 서비스이므로 singleton에서 직접 주입 불가). `SaveAsync` 내부에서 scope 생성 후 `ICapturedImageRepository`만 resolve.
- `FilenameGenerator` — `BySession` 케이스 제거, `{date}`/`{profile}` 토큰 치환 로직 추가, 기존 `filenamePrefix`/`timestampFormat`/`includeSequenceNumber` 파라미터 제거
- `AppDbContext` — `Sessions` DbSet 제거

### API 수정
- `GET /api/images` — `sessionId` 쿼리 파라미터 제거 (`dateFrom`, `dateTo` 유지)
- `ImagesController` — `sessionId` 필터 로직 제거
- `CapturedImageRepository.GetPageAsync` — `sessionId` 파라미터 제거

### DB 처리
EF Core `EnsureCreated` 방식이므로 마이그레이션 파일 없음.  
`Program.cs`의 수동 SQL에서 아래 처리:
- `Sessions` 테이블 생성 SQL 제거
- `CapturedImages.SessionId` 컬럼 및 FK 인덱스 생성 SQL 제거
- 기존 DB 파일이 있는 경우 런타임에 컬럼이 남아 있어도 EF가 무시하므로 하위 호환 가능.

---

## FE 제거 목록

### 파일 전체 삭제
- `components/shared/SessionSelector/` (디렉토리 전체)

### `client.ts`에서 제거할 함수
- `getSessions`
- `getActiveSession`
- `createSession`
- `endSession`
- `deleteSession`

### `types.ts`에서 제거할 타입/필드
- `Session` 인터페이스
- `CapturedImageRecord.sessionId`
- `ImageSaveSettings.filenamePrefix`
- `ImageSaveSettings.timestampFormat`
- `ImageSaveSettings.includeSequenceNumber`
- `ImageSaveSettings.subfolderStrategy`
- `SubfolderStrategy` 타입

### `queryKeys.ts`에서 제거
- `sessions` 관련 키

### `useImageGallery.ts` 수정
- `filterSessionId` 상태 → `dateFrom`/`dateTo` (string | null) 상태로 교체
- `listImages` 호출 파라미터 업데이트

### `ImageGallery` 컴포넌트 수정
- 세션 필터 드롭다운 → 날짜 범위 from/to 입력으로 교체

### `ImageSaveSettingsPanel` 수정
- 제거: `filenamePrefix`, `timestampFormat`, `includeSequenceNumber`, `subfolderStrategy` 필드
- 유지: `outputDirectory` (토큰 힌트 표시), `format`, `autoSave`
- 파일명 예시 → 고정 포맷으로 업데이트

### `Acquisition/index.tsx` 수정
- `<SessionSelector />` 제거 (Settings 탭에서)

---

## 테스트 업데이트

### 삭제
- `AcquisitionManagerTests`의 `NullLatencyService` 주변 session 참조 확인 후 정리
- session 관련 spec 파일 있으면 삭제

### 수정
- `AutoSaveServiceTests` — `FakeScopeFactory`에서 `FakeSessionRepository` 제거 (`IServiceScopeFactory`와 scope 자체는 유지)
- `AutoSaveServiceTests` — `FakeAcquisitionService`의 session 관련 코드 정리
- `ImageSaveSettingsServiceTests` — 제거된 필드 assertion 정리
- `AcquisitionAutoSaveSpec` — session 관련 검증 없으므로 그대로 유지

---

## 검증 체크리스트

구현 완료 후 아래를 확인:

- [ ] `grep -r "Session\|session" src/ --include="*.cs"` — 드라이버 레벨 `AcquisitionStatisticsSnapshot`·`GetSnapshot()` 외 결과 없음
- [ ] `grep -r "session\|Session" src/peanut-vision-app/src` — 결과 없음
- [ ] `dotnet build` 경고 0개
- [ ] `dotnet test` 전체 통과
- [ ] `npx tsc --noEmit` 에러 없음
- [ ] `GET /api/images?dateFrom=2026-06-01&dateTo=2026-06-04` 정상 동작
