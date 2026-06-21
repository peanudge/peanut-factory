# Docker + Local Dataset → Label Studio 세팅 가이드

반자동으로 라벨링된 **로컬 YOLO 데이터셋**을, Docker로 띄운 Label Studio에
**사전 라벨(bounding box)이 그려진 상태로** 연결하는 방법.

이 프로젝트의 Label Studio는 `src/PeanutData/docker-compose.yml` 로 Docker 컨테이너로 실행한다.

---

## 동작 원리 (먼저 이해하기)

**1) 변환 JSON은 이미지를 품지 않는다.**
`output.json`에는 이미지 경로(URL 참조)만 들어간다. 따라서 Label Studio가 그 경로의
실제 파일을 서빙할 수 있게 **Local Storage 연결**까지 해야 이미지가 보인다.

**2) 경로가 두 세계로 나뉜다 (Docker 특유).**

| 세계 | 무엇 | 경로 |
|------|------|------|
| **호스트** | 변환 도구가 파일을 읽는 곳 | `…/yolo-data-to-label-studio/dataset` |
| **컨테이너** | Label Studio가 이미지를 서빙하는 곳 | `/label-studio/files/dataset` |

`DOCUMENT_ROOT`, Local Storage 경로, `--image-root-url` 은 **모두 컨테이너 경로 기준**으로 맞춘다.

**3) 이미지가 서빙되려면 아래 3가지가 모두 충족돼야 한다.** (하나라도 빠지면 이미지 404)

| 조건 | 충족 방법 |
|------|-----------|
| ① 로컬 파일 서빙 활성화 | `LABEL_STUDIO_LOCAL_FILES_SERVING_ENABLED=true` (2번) |
| ② 파일이 `DOCUMENT_ROOT` 아래 실제 존재 | 데이터셋 볼륨 마운트 (2번) |
| ③ **요청 경로를 포함하는 Local Storage가 프로젝트에 등록** | UI에서 Add Storage **저장** (5번) |

> ③이 가장 자주 누락된다. 환경변수·파일만 맞추고 Local Storage 등록(저장)을 안 하면
> 서버가 `/data/local-files/?d=…` 요청에 **404**를 반환한다.

---

## 1. 데이터셋 구조

```
dataset/
├── classes.txt        # 클래스 이름 (한 줄에 하나, 0번 인덱스부터 순서대로)
├── images/            # 이미지 파일 (.png, .jpg 등)
│   ├── img_0001.png
│   └── img_0002.png
└── labels/            # YOLO 라벨 (이미지와 같은 파일명, 확장자만 .txt)
    ├── img_0001.txt
    └── img_0002.txt
```

### 준비 체크리스트

| 항목 | 요구사항 |
|------|----------|
| `images/` ↔ `labels/` 파일명 | **확장자만 다르고 이름은 1:1 동일** (`img_0001.png` ↔ `img_0001.txt`) |
| `classes.txt` | 클래스명을 **0번 인덱스 순서대로** 한 줄에 하나씩 |
| 라벨 포맷 | `<class_id> <x_center> <y_center> <width> <height>` — 모두 **0~1 정규화**, 공백 구분 |
| 박스 없는 이미지 | 라벨 `.txt`를 **빈 파일로라도** 둔다 (없으면 누락 가능) |
| 클래스 인덱스 | 라벨의 `class_id` = `classes.txt` 줄 번호(0-base) |

이 폴더의 샘플 `dataset/`: `classes.txt` = `good`(0), `bad`(1) / 이미지·라벨 각 2쌍(테스트용).

---

## 2. docker-compose.yml — 마운트 + DOCUMENT_ROOT (1회성)

`src/PeanutData/docker-compose.yml` 의 `label-studio` 서비스에 아래가 설정돼 있어야 한다.

```yaml
services:
  label-studio:
    environment:
      - LABEL_STUDIO_LOCAL_FILES_SERVING_ENABLED=true
      - LABEL_STUDIO_LOCAL_FILES_DOCUMENT_ROOT=/label-studio/files   # 서빙 루트(컨테이너)
    volumes:
      - ./mydata:/label-studio/data:rw                      # DB·미디어 영속
      - ./yolo-data-to-label-studio:/label-studio/files:ro  # 데이터셋 마운트(읽기전용)
```

적용 (환경변수·볼륨 변경은 컨테이너 재생성 필요):

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/PeanutData
docker compose up -d
```

### 경로 매핑

| 구분 | 경로 |
|------|------|
| 호스트 데이터셋 루트 | `…/yolo-data-to-label-studio/dataset` |
| 컨테이너 마운트 위치 | `/label-studio/files/dataset` |
| `DOCUMENT_ROOT` (컨테이너) | `/label-studio/files` |
| 컨테이너 이미지 폴더 | `/label-studio/files/dataset/images` |
| `?d=` 값 = 이미지폴더 − DOCUMENT_ROOT | **`dataset/images`** |

설정·마운트 검증:

```bash
docker exec label-studio printenv | grep LABEL_STUDIO_LOCAL_FILES
docker exec label-studio ls /label-studio/files/dataset/images
```

---

## 3. YOLO → Label Studio JSON 변환 (호스트 venv에서)

```bash
cd /Users/sonjiho/Workspace/peanut-factory/src/PeanutData
source .venv/bin/activate
pip install label-studio-converter   # 최초 1회
```

```bash
cd yolo-data-to-label-studio
label-studio-converter import yolo \
  -i dataset \
  -o output.json \
  --image-ext .png \
  --image-root-url "/data/local-files/?d=dataset/images"
```

| 플래그 | 의미 |
|--------|------|
| `-i dataset` | **호스트 경로** YOLO 데이터셋 루트 (변환기가 실제 파일을 읽음) |
| `-o output.json` | 출력 JSON (5번에서 브라우저로 업로드) |
| `--image-ext .png` | 검색할 확장자. **기본값 `.jpg`** — png 데이터셋은 이 옵션 없으면 `No labels converted` 실패 |
| `--image-root-url` | **컨테이너 기준** 이미지 URL. `?d=` 뒤는 2번 표의 `dataset/images` 와 일치 |

성공 시 `output.json` 과 `output.label_config.xml`(Labeling Config) 이 함께 생성된다.

> ❌ `yolo2json` 서브커맨드는 존재하지 않음. ✅ `import yolo` 사용.

---

## 4. 프로젝트 + Labeling Config

1. 브라우저에서 http://localhost:8080 접속 → 프로젝트 생성
2. **Settings → Labeling Interface** → `output.label_config.xml` 내용 붙여넣기
   (`<RectangleLabels>` 의 라벨명 `good`/`bad` 이 `classes.txt`와 일치해야 박스가 렌더링됨)

---

## 5. Local Storage 연결 ⭐ (이미지 서빙의 핵심)

1. **Settings → Cloud Storage → Add Source Storage**
2. Storage Type: **Local files**
3. **Absolute local path**: `/label-studio/files/dataset/images`
   → ⚠️ **컨테이너 경로**다. 호스트 경로(`/Users/...`)를 넣으면 안 됨.
4. **Add Storage** 클릭 → **반드시 저장 확인**
   (저장 안 하면 storage 미등록 → 이미지 404. 동작 원리 ③ 참고)

> Sync는 누르지 않아도 됨 — task는 6번 import가 생성한다. Local Storage는 **서빙 권한용**.

---

## 6. JSON Import

**Data Manager → Import → `output.json` 선택 → 업로드**

→ 각 이미지 위에 `good`/`bad` 바운딩 박스가 미리 그려진 상태로 보이면 성공.

---

## 7. Troubleshooting

| 증상 | 원인 / 해결 |
|------|-------------|
| **이미지 404 / "issue loading URL from $image"** | Local Storage 미등록(5번 저장 안 됨)이 1순위. 그다음 ②볼륨 마운트, ③`?d=` 경로 불일치 순으로 확인 |
| 이미지가 호스트 경로로 안 됨 | Local Storage Absolute path는 **컨테이너 경로**(`/label-studio/files/...`) |
| `Local files serving is not enabled` (403) | `LABEL_STUDIO_LOCAL_FILES_SERVING_ENABLED=true` 후 `docker compose up -d` |
| `No labels converted` | png인데 `--image-ext .png` 누락 (기본 `.jpg`) |
| `yolo2json` 명령 없음 | 옛 문법. `import yolo` 사용 |
| 박스가 안 그려짐 | Labeling Config 라벨명과 `classes.txt` 불일치 (4번) |
| 일부 이미지 누락 | 해당 이미지의 `labels/*.txt` 없음 → 빈 파일이라도 생성 |
| 박스 위치 어긋남 | 라벨이 0~1 정규화가 아니거나 중심점 기준이 아님 (YOLO는 중심점 기준 정규화) |
| 마운트/환경변수 변경 미반영 | `docker compose down && docker compose up -d` 로 재생성 |

### 404 빠른 진단 (서버 로그 확인)

```bash
docker compose logs --tail=50 label-studio | grep local-files
# 404 → 위 표 1행(Local Storage 미등록)부터 확인
```

---

## 참고 자료

- [Import YOLO Annotations into Label Studio (공식 튜토리얼)](https://labelstud.io/blog/tutorial-importing-local-yolo-pre-annotated-images-to-label-studio/)
- [Label Studio — Local Storage 가이드](https://labelstud.io/guide/storage.html#Local-storage)
- [label-studio-converter (GitHub)](https://github.com/HumanSignal/label-studio-converter)
