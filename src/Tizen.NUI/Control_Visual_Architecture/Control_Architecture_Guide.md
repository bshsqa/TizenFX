# DALi Control 아키텍처 가이드

## 개요

DALi의 Control은 모든 UI 컴포넌트의 기반이 되는 핵심 아키텍처입니다. Control은 사용자 입력 처리, 스타일링, 상태 관리, 비주얼 관리 등 UI 컴포넌트의 기본 기능을 제공하는 추상 기본 클래스입니다.

## Control 클래스 계층 구조

### 상속 계층

```
BaseObject
├── CustomActorImpl
├── CustomActor
└── Control
    ├── ImageView
    ├── TextLabel
    ├── Button
    ├── Slider
    └── ... (기타 컨트롤들)
```

### 주요 클래스 상세

#### 1. Control (Public API)

Control은 모든 컨트롤의 퍼블릭 인터페이스를 제공하는 핸들 클래스입니다.

**주요 속성:**
- `StyleName`: 컨트롤에 적용될 스타일 이름
- `KeyInputFocus`: 키보드 입력 포커스 여부
- `Background`: 배경 속성 (색상 또는 이미지)
- `Margin`: 컨트롤 외부 여백
- `Padding`: 컨트롤 내부 여백

**주요 메서드:**
- `SetStyleName()`: 스타일 이름 설정
- `SetKeyInputFocus()`: 키보드 입력 포커스 설정
- `SetBackgroundColor()`: 배경 색상 설정
- `ClearBackground()`: 배경 제거
- `KeyEventSignal()`: 키 이벤트 시그널
- `ResourceReadySignal()`: 리소스 준비 완료 시그널

**주요 시그널:**
| 시그널 이름 | 설명 |
|------------|------|
| keyEvent | 키 이벤트 발생 시 |
| keyInputFocusGained | 키보드 포커스 획득 시 |
| keyInputFocusLost | 키보드 포커스 상실 시 |
| resourceReady | 리소스 준비 완료 시 |
| tapped | 탭 제스처 감지 시 |
| panned | 팬 제스처 감지 시 |
| pinched | 핀치 제스처 감지 시 |
| longPressed | 롱프레스 제스처 감지 시 |

#### 2. ControlImpl (Internal Implementation)

Control의 내부 구현을 담당하는 클래스입니다.

**주요 멤버 변수:**
- `mControlImpl`: Control 참조
- `mState`: 컨트롤 상태
- `mSubStateName`: 서브 상태 이름
- `mStyleName`: 스타일 이름
- `mBackgroundColor`: 배경 색상
- `mKeyEventSignal`: 키 이벤트 시그널
- `mKeyInputFocusGainedSignal`: 포커스 획득 시그널
- `mKeyInputFocusLostSignal`: 포커스 상실 시그널
- `mResourceReadySignal`: 리소스 준비 시그널

**주요 메서드:**
- `OnInitialize()`: 초기화 처리
- `OnStyleChange()`: 스타일 변경 처리
- `OnKeyInputFocusGained()`: 포커스 획득 처리
- `OnKeyInputFocusLost()`: 포커스 상실 처리
- `OnKeyEvent()`: 키 이벤트 처리

#### 3. ControlDataImpl (Data Implementation)

Control의 실제 데이터 관리를 담당하는 클래스입니다.

**주요 멤버 변수:**
- `mControlImpl`: Control 참조
- `mState`: 컨트롤 상태
- `mSubStateName`: 서브 상태 이름
- `mAccessibilityData`: 접근성 데이터
- `mVisualData`: 비주얼 데이터
- `mStyleName`: 스타일 이름
- `mBackgroundColor`: 배경 색상
- `mMargin`: 외부 여백
- `mPadding`: 내부 여백
- `mKeyEventSignal`: 키 이벤트 시그널
- `mKeyInputFocusGainedSignal`: 포커스 획득 시그널
- `mKeyInputFocusLostSignal`: 포커스 상실 시그널
- `mResourceReadySignal`: 리소스 준비 시그널

**주요 메서드:**
- `RegisterVisual()`: 비주얼 등록
- `UnregisterVisual()`: 비주얼 해제
- `GetVisual()`: 비주얼 조회
- `EnableVisual()`: 비주얼 활성화
- `SetState()`: 상태 설정
- `SetSubState()`: 서브 상태 설정
- `IsResourceReady()`: 리소스 준비 상태 확인

## Control 동작 원리

### 1. 생명주기

Control의 생명주기는 다음과 같은 단계를 거칩니다:

1. **생성**: `Control::New()` 또는 파생 클래스의 정적 생성 메서드 호출
2. **초기화**: `OnInitialize()` 가상 함수 호출
3. **스타일 적용**: 스타일 매니저를 통해 스타일 적용
4. **씬 연결**: `OnSceneConnection()` 호출
5. **비주얼 설정**: 비주얼 등록 및 활성화
6. **리소스 로딩**: 비동기 리소스 로딩
7. **렌더링**: 렌더링 루프에 참여
8. **씬 해제**: `OnSceneDisconnection()` 호출
9. **소멸**: 리소스 정리 및 소멸

### 2. 상태 관리

Control은 상태 기반 디자인을 지원합니다:

- **State**: NORMAL, FOCUSED, DISABLED 등의 기본 상태
- **SubState**: 사용자 정의 서브 상태
- **상태 전환**: `SetState()`와 `SetSubState()`를 통한 상태 변경
- **전환 애니메이션**: 상태 변경 시 애니메이션 효과 적용

### 3. 비주얼 관리

Control은 다중 비주얼을 지원합니다:

- **비주얼 등록**: `RegisterVisual()`로 Property::Index와 비주얼 매핑
- **깊이 관리**: 각 비주얼의 깊이 인덱스로 렌더링 순서 제어
- **활성화 제어**: `EnableVisual()`로 비주얼 표시/숨김 제어
- **리소스 상태**: 각 비주얼의 리소스 로딩 상태 모니터링

## 구체적 Control 구현

### 1. ImageView

이미지 표시를 전문으로 하는 컨트롤입니다.

**주요 속성:**
- `Image`: 이미지 URL 또는 속성 맵
- `PreMultipliedAlpha`: 프리멀티플라이드 알파 사용 여부
- `PlaceholderImage`: 플레이스홀더 이미지
- `EnableTransitionEffect`: 전환 효과 활성화
- `TransitionEffectOption`: 전환 효과 옵션
- `PixelArea`: 이미지 픽셀 영역

**주요 기능:**
- URL 기반 이미지 로딩
- 비동기 리소스 로딩
- 픽셀 영역 지정
- 전환 효과 지원
- 플레이스홀더 이미지 지원

### 2. TextLabel

텍스트 표시를 전문으로 하는 컨트롤입니다.

**주요 속성:**
- `Text`: 표시할 텍스트
- `FontFamily`: 폰트 패밀리
- `FontStyle`: 폰트 스타일
- `PointSize`: 폰트 크기 (포인트)
- `MultiLine`: 다중 라인 여부
- `HorizontalAlignment`: 수평 정렬
- `VerticalAlignment`: 수직 정렬
- `TextColor`: 텍스트 색상
- `EnableMarkup`: 마크업 처리 활성화
- `EnableAutoScroll`: 자동 스크롤 활성화
- `AutoScrollSpeed`: 자동 스크롤 속도

**주요 기능:**
- 다중 라인 텍스트 지원
- 마크업 처리
- 자동 스크롤
- 텍스트 스타일링
- 폰트 렌더링

## Control과 다른 모듈과의 관계

### 1. Actor와의 관계

Control은 CustomActor를 상속받아 Actor의 모든 기능을 상속받습니다:

- **위치 및 크기**: Actor의 변환 기능 상속
- **계층 구조**: 부모-자식 관계 지원
- **렌더링**: 렌더링 트리에 참여
- **이벤트**: Actor의 이벤트 시스템 활용

### 2. Visual과의 관계

Control은 Visual을 관리하는 컨테이너 역할을 합니다:

- **Visual 생성**: VisualFactory를 통해 Visual 생성
- **Visual 등록**: ControlDataImpl에 Visual 등록
- **Visual 제어**: 활성화/비활성화, 상태 변경
- **리소스 관리**: Visual의 리소스 상태 모니터링

### 3. StyleManager와의 관계

Control은 StyleManager와 연동하여 스타일링을 지원합니다:

- **스타일 적용**: 스타일 이름으로 스타일 검색 및 적용
- **스타일 변경**: 스타일 변경 이벤트 수신 및 처리
- **테마 지원**: 테마 변경 시 자동 스타일 업데이트

## Control의 확장

### 1. 새로운 Control 생성

새로운 Control을 생성하려면 다음 단계를 따릅니다:

1. **Control 클래스 상속**: `class MyControl : public Control`
2. **Impl 클래스 생성**: `class MyControlImpl : public ControlImpl`
3. **정적 생성 메서드**: `static MyControl New()`
4. **가상 함수 구현**: `OnInitialize()`, `OnSizeSet()` 등
5. **속성 등록**: 필요한 속성 등록
6. **비주얼 관리**: 필요한 Visual 등록 및 관리

### 2. ControlBehavior 설정

Control 생성 시 동작을 제어할 수 있습니다:

```cpp
enum ControlBehaviour {
    CONTROL_BEHAVIOUR_DEFAULT = 0,
    DISABLE_SIZE_NEGOTIATION = 1 << 0,
    REQUIRES_KEYBOARD_NAVIGATION_SUPPORT = 1 << 1,
    DISABLE_STYLE_CHANGE_SIGNALS = 1 << 2,
    DISABLE_VISUALS = 1 << 3
};
```

## 성능 최적화

### 1. 비주얼 최적화

- **비주얼 재사용**: 비슷한 비주얼은 캐시에서 재사용
- **깊이 최적화**: 불필요한 깊이 변경 최소화
- **리소스 관리**: 사용하지 않는 비주얼은 즉시 해제

### 2. 상태 관리 최적화

- **상태 캐싱**: 현재 상태를 캐싱하여 불필요한 상태 변경 방지
- **전환 최적화**: 상태 전환 시 최소한의 속성만 변경
- **애니메이션 최적화**: 불필요한 애니메이션은 건너뛰기

### 3. 메모리 관리

- **스마트 포인터**: 메모리 누수 방지
- **지연 로딩**: 필요한 리소스만 지연 로딩
- **풀링**: 자주 사용하는 객체는 풀에서 재사용

## 접근성 지원

Control은 접근성을 위한 다양한 기능을 제공합니다:

- **Accessible 객체**: 접근성을 위한 객체 생성
- **속성 설정**: 접근성 관련 속성 설정
- **이벤트 처리**: 접근성 관련 이벤트 처리
- **포커스 관리**: 키보드 포커스 관리

## 결론

DALi의 Control 아키텍처는 유연하고 확장 가능한 UI 컴포넌트 시스템을 제공합니다. Control-Impl-DataImpl의 3계층 구조를 통해 명확한 책임 분리를 이루고 있으며, Visual 시스템과의 긴밀한 연동을 통해 효율적인 렌더링을 지원합니다. 상태 기반 디자인과 스타일링 시스템을 통해 일관된 UI를 구현할 수 있으며, 접근성 지원을 통해 모든 사용자를 위한 UI를 제공합니다.

---

## 관련 다이어그램

- [Control 클래스 계층 다이어그램](Control_Class_Hierarchy.puml)
- [Control과 Visual 관계 다이어그램](Control_Visual_Relationship.puml)
- [DALi 전체 아키텍처 다이어그램](DALi_Overall_Architecture.puml)
