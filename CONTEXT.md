# Workspace Launcher v2.0 - 프로젝트 기술 문서

다중 프로그램 일괄 실행 관리 유틸리티. 자주 사용하는 프로그램, 바로가기, 웹사이트를 작업 공간별로 그룹화하고 원클릭으로 동시 실행합니다.

---

## 1. 프로젝트 개요

| 항목 | 내용 |
| :--- | :--- |
| **프레임워크** | C# WPF (.NET 10) |
| **빌드 시스템** | dotnet CLI |
| **데이터 저장** | JSON (`%AppData%\WorkspaceLauncher\workspaces.json`) |
| **아이콘 추출** | Win32 Shell32 API (P/Invoke, 추가 패키지 불필요) |
| **UI 테마** | 커스텀 다크 테마 (WindowChrome 커스텀 타이틀바) |

---

## 2. v2.0 주요 변경 이력 (v1.0 → v2.0)

### 신규 기능
1. **드래그 앤 드롭**: `.exe`, `.lnk`, `.bat`, `.cmd`, `.msc` 파일을 드래그하여 즉시 등록
2. **프로그램 / 웹사이트 분리 등록**: `🖥 프로그램 추가` 버튼(OpenFileDialog)과 `🌐 웹사이트 추가` 버튼(URL 입력)으로 별도 추가 흐름 제공
3. **아이콘 자동 추출**: Shell32 `SHGetFileInfo` API로 프로그램/바로가기의 실제 아이콘을 추출하여 목록에 표시. 웹사이트는 🌐 이모지로 표시
4. **작업 공간 이름 변경**: 좌측 작업 공간 더블클릭 시 이름 변경 팝업
5. **항목 개별 실행**: 우측 항목 더블클릭 시 해당 항목만 단독 실행
6. **실행 결과 카운터**: 일괄 실행 후 성공/실패 건수를 상태바에 표시
7. **상태바**: 하단에 현재 상태 메시지 및 항목 통계(프로그램/웹사이트 개수) 표시
8. **다중 파일 선택**: 프로그램 추가 시 OpenFileDialog에서 여러 파일 동시 선택 가능
9. **URL 자동 보정**: `https://` 스키마가 없는 URL 입력 시 자동 추가

### UI 개편
- **다크 테마 전면 적용**: `#1A1B2E` 계열 배경 + `#7C5CFC`~`#5B8DEF` 그라디언트 강조색
- **커스텀 타이틀바**: `WindowChrome` 기반, 최소화/최대화/닫기 버튼 직접 구현
- **모던 버튼 스타일**: `CornerRadius`, 호버 효과, 그라디언트 Accent 버튼, Danger 버튼
- **카드형 항목 리스트**: 각 항목이 둥근 모서리 카드로 표시, 호버 시 보라색 테두리 강조
- **드래그 앤 드롭 영역**: 시각적으로 구분된 드롭존, 드래그 진입 시 색상 변화 피드백

---

## 3. 소스코드 디렉토리 구조

```text
D:\WorkspaceLauncher\
│  App.xaml                   # 글로벌 다크 테마 리소스 딕셔너리
│  App.xaml.cs                # 애플리케이션 진입점
│  AssemblyInfo.cs            # 어셈블리 메타데이터
│  InputWindow.xaml           # 입력 대화 상자 UI (다크 테마)
│  InputWindow.xaml.cs        # 입력 대화 상자 코드 비하인드
│  MainWindow.xaml            # 메인 대시보드 UI (다크 테마)
│  MainWindow.xaml.cs         # 핵심 로직 (드래그&드롭, 실행, 데이터 관리)
│  WorkspaceLauncher.csproj   # 프로젝트 설정 파일
│  CONTEXT.md                 # 본 문서
│  README.md                  # GitHub용 소개 문서
│
├─Models/
│      AppItem.cs             # 실행 항목 모델 (Program/Website 타입, 아이콘)
│      DataManager.cs         # JSON 영속화 매니저
│      IconHelper.cs          # Shell32 P/Invoke 아이콘 추출기
│      Workspace.cs           # 작업 공간 그룹 모델
```

---

## 4. 핵심 모듈 상세

| 파일 | 핵심 역할 |
| :--- | :--- |
| **`AppItem.cs`** | `AppItemType` 열거형(`Program`/`Website`) 도입. `IconSource`(런타임 전용)에 추출된 아이콘 캐싱. `IconEmoji`, `ItemTypeDisplay` 계산 속성 제공 |
| **`IconHelper.cs`** | `SHGetFileInfo` P/Invoke로 `.exe`/`.lnk` 아이콘 추출 → `Imaging.CreateBitmapSourceFromHIcon`으로 WPF `ImageSource` 변환. `Freeze()` 처리로 스레드 안전성 확보 |
| **`DataManager.cs`** | `%AppData%\WorkspaceLauncher\workspaces.json`에 `JsonSerializer`로 직렬화/역직렬화. `JsonStringEnumConverter`로 열거형을 문자열로 저장 |
| **`App.xaml`** | 16개 색상 브러쉬, 5종 버튼 스타일(`TitleBarButton`, `CloseButton`, `ModernButton`, `AccentButton`, `DangerButton`), 2종 `ListBoxItem` 스타일 정의 |
| **`MainWindow.xaml`** | `WindowChrome` 커스텀 타이틀바, 좌측 사이드바(작업 공간), 우측 메인(드롭존+항목리스트+액션버튼+실행버튼), 하단 상태바 구성 |
| **`MainWindow.xaml.cs`** | 드래그 앤 드롭 핸들러, `OpenFileDialog` 기반 프로그램 추가, URL 입력 기반 웹사이트 추가, `Process.Start` 일괄 실행, 더블클릭 이벤트 핸들러 |

---

## 5. 사용법

### 빌드 및 실행 (개발 환경)
```powershell
cd D:\WorkspaceLauncher
dotnet run
```

### 다른 컴퓨터 배포용 독립형 빌드 (Self-contained Publish)
다른 컴퓨터에 .NET SDK나 런타임이 설치되어 있지 않아도 실행할 수 있게, 모든 런타임 요소를 하나의 파일에 패키징하여 릴리즈를 수행합니다.

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```

- **출력 경로**: `D:\WorkspaceLauncher\bin\Release\net10.0-windows\win-x64\publish\`
- **배포 방식**: 위 `publish` 폴더 전체를 ZIP 압축하여 다른 컴퓨터로 전달 후, 압축을 풀고 `WorkspaceLauncher.exe`를 더블클릭하면 아무런 환경 세팅 없이 즉각 동작합니다.


### 프로그램 사용 가이드

#### 작업 공간 관리
- **추가**: 좌측 하단 `+ 추가` 버튼 클릭 → 이름 입력
- **삭제**: 작업 공간 선택 후 `삭제` 버튼 클릭
- **이름 변경**: 작업 공간 항목을 **더블클릭**하여 이름 수정

#### 프로그램 등록 (2가지 방법)
1. **드래그 앤 드롭**: 바탕화면이나 탐색기에서 `.exe`, `.lnk` 파일을 우측 드롭존 영역으로 끌어다 놓기
2. **파일 선택 다이얼로그**: `🖥 프로그램 추가` 버튼 → 파일 탐색기에서 프로그램 선택 (다중 선택 가능)

#### 웹사이트 등록
- `🌐 웹사이트 추가` 버튼 → 이름 입력 → URL 입력 (`https://` 없이 입력해도 자동 추가)

#### 실행
- **일괄 실행**: 하단 보라색 `🚀 일괄 실행` 버튼 클릭 → 현재 작업 공간의 모든 항목 동시 실행
- **개별 실행**: 항목을 **더블클릭**하면 해당 항목만 단독 실행

---

## 6. 데이터 저장 위치

설정 데이터는 아래 경로에 JSON 형식으로 자동 저장됩니다:
```
%AppData%\WorkspaceLauncher\workspaces.json
```

프로그램 재시작 시 자동으로 불러옵니다.
