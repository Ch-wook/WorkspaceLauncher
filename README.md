# 🚀 Workspace Launcher

**여러 프로그램과 웹사이트를 한 번에 실행하는 Windows 데스크탑 유틸리티**

자주 사용하는 프로그램, 바로가기(.lnk), 웹사이트 URL을 **작업 공간(Workspace)** 단위로 묶어서 관리하고, 원클릭으로 일괄 실행할 수 있습니다.

---

## ✨ 주요 기능

| 기능 | 설명 |
| :--- | :--- |
| **작업 공간 관리** | 여러 개의 프로필을 만들어 용도별로 프로그램을 분류 (개발, 업무, 게임 등) |
| **드래그 앤 드롭** | `.exe`, `.lnk` 파일을 마우스로 끌어다 놓으면 자동 등록 |
| **프로그램 / 웹사이트 분리** | 프로그램은 파일 선택, 웹사이트는 URL 입력으로 각각 별도 추가 |
| **아이콘 자동 표시** | 등록된 프로그램의 실제 아이콘을 자동 추출하여 표시 |
| **일괄 실행** | 작업 공간 내 모든 항목을 한 번에 실행 |
| **개별 실행** | 항목 더블클릭으로 개별 실행 가능 |
| **이름 변경** | 작업 공간 더블클릭으로 이름 즉시 변경 |
| **다크 테마 UI** | 모던한 다크 테마 디자인 + 커스텀 타이틀바 |

---

## 🛠️ 기술 스택

- **언어**: C#
- **프레임워크**: WPF (.NET 10)
- **데이터 저장**: JSON (로컬 AppData)
- **아이콘 추출**: Win32 Shell32 API (P/Invoke)

---

## 🚀 실행 및 배포 방법

### 1. 개발 환경에서 실행
#### 사전 요구사항
- [.NET 10 SDK](https://dotnet.microsoft.com/download) 설치 필요

#### 빌드 및 실행
```powershell
git clone https://github.com/Ch-wook/WorkspaceLauncher.git
cd WorkspaceLauncher
dotnet run
```

### 2. 다른 컴퓨터에서 실행하기 (독립형 배포판 빌드)
다른 컴퓨터에 .NET 10이 설치되어 있지 않아도 실행할 수 있도록, .NET 런타임이 포함된 단일 실행 파일로 배포할 수 있습니다.

#### 독립형 단일 실행 파일 빌드 명령어
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```

#### 배포 및 사용 방법
1. 빌드가 완료되면 `D:\WorkspaceLauncher\bin\Release\net10.0-windows\win-x64\publish` 폴더가 생성됩니다.
2. 해당 `publish` 폴더 전체를 압축(ZIP)하여 USB나 공유 드라이브를 통해 다른 컴퓨터로 전달합니다.
3. 다른 컴퓨터에서 압축을 푼 뒤, **`WorkspaceLauncher.exe`**를 더블클릭하면 아무런 프레임워크나 런타임 설치 없이 즉시 작동합니다.
4. 설정값과 작업 공간 정보는 동일 폴더 내에 `workspaces.json`으로 자동 관리됩니다.


---

## 📖 사용법

1. **작업 공간 추가**: 좌측 `+ 추가` 버튼으로 새 작업 공간 생성
2. **프로그램 등록**: 
   - 바탕화면 바로가기를 드롭존에 **드래그 앤 드롭**
   - 또는 `🖥 프로그램 추가` 버튼으로 파일 선택
3. **웹사이트 등록**: `🌐 웹사이트 추가` 버튼으로 URL 입력
4. **일괄 실행**: 하단 `🚀 일괄 실행` 버튼 클릭

> **💡 팁**: 항목을 **더블클릭**하면 개별 실행, 작업 공간을 **더블클릭**하면 이름 변경이 가능합니다.

---

## 📁 프로젝트 구조

```
WorkspaceLauncher/
├── Models/
│   ├── AppItem.cs        # 실행 항목 데이터 모델
│   ├── Workspace.cs      # 작업 공간 그룹 모델
│   ├── DataManager.cs    # JSON 데이터 영속화
│   └── IconHelper.cs     # Shell32 아이콘 추출
├── App.xaml              # 다크 테마 리소스
├── MainWindow.xaml/.cs   # 메인 UI 및 로직
├── InputWindow.xaml/.cs  # 입력 대화 상자
└── CONTEXT.md            # 상세 기술 문서
```

---

## 📝 라이선스

이 프로젝트는 MIT 라이선스를 따릅니다.
