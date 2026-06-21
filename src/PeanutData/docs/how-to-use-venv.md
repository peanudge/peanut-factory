# Venv

venv는 파이썬 표준 라이브러리에 내장된 가상환경 도구입니다. 프로젝트마다 독립된 패키지 공간을 만들어서, 한 프로젝트의 라이브러리가 다른 프로젝트나 시스템 전체에 영향을 주지 않도록 격리합니다. 앞서 다룬 pyenv가 "파이썬 버전"을 관리한다면, venv는 "그 버전 위에서 쓰는 패키지"를 격리한다고 보면 됩니다.

- 1. Create virtual python environment

```
cd ~/projects/my-app
python -m venv .venv
```

- 2. Activate virtual environment

```bash
source .venv/bin/activate
```

- 3. Inactivate

```bash
deactivate
```

- 4. Sharing pip denpendecies

가상환경 자체는 보통 git에 올리지 않습니다. 대신 설치된 패키지 목록을 파일로 뽑아서 공유합니다.

```bash
pip freeze > requirements.txt
```

- 5. Recover pip dependeces on other environment

```bash
python -m venv .venv
source .venv/bin/activate
pip install -r requirements.txt
```
