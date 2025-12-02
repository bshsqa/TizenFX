# DALi Control과 Visual 통합 아키텍처 가이드

## 개요

DALi는 고성능 UI 프레임워크로, Control과 Visual이라는 두 개의 핵심 아키텍처를 통해 유연하고 확장 가능한 UI 시스템을 제공합니다. Control은 UI 컴포넌트의 논리적 기능과 사용자 상호작용을 담당하고, Visual은 시각적 렌더링을 담당하여 명확한 관심사 분리를 이룹니다.

## 아키텍처 개요

### 설계 원칙

DALi의 Control-Visual 아키텍처는 다음과 같은 설계 원칙을 따릅니다:

1. **관심사 분리 (Separation of Concerns)**
   - Control: 사용자 입력, 상태 관리, 비즈니스 로직
   - Visual: 렌더링, 리소스 관리, 시각적 효과

2. **단일 책임 원칙 (Single Responsibility Principle)**
   - 각 클래스는 명확하게 정의된 단일 책임을 가짐

3. **개방-폐쇄 원칙 (Open-Closed Principle)**
   - 확장에는 열려 있고, 수정에는 닫혀 있는 구조

4. **의존성 역전 원칙 (Dependency Inversion Principle)**
   - 상위 모듈은 하위 모듈에 의존하지 않음

### 계층 구조

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
├─────────────────────────────────────────────────────────────┤
│                  DALi Toolkit Layer                        │
│  ┌─────────────────┐  ┌─────────────────────────────────────┐ │
│  │   Control Layer │  │        Visual Management Layer      │ │
│  │                 │  │                                     │ │
│  │ • Control       │  │ • VisualFactory                     │ │
│  │ • ImageView     │  │ • VisualFactoryCache                │ │
│  │ • TextLabel     │  │ • VisualData                        │ │
│  │ • ...           │  │                                     │ │
│  └─────────────────┘  └─────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │                  Visual Layer                            │
│  │                                                         │ │
│  │ • VisualBase                                            │ │
│  │ • ImageVisual                                           │ │
│  │ • TextVisual                                            │ │
│  │ • ColorVisual                                           │ │
│  │ • ...                                                   │ │
│  └─────────────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                    DALi Core Layer                         │
│  ┌─────────────────┐  ┌─────────────────────────────────────┐ │
│  │   Actor Layer   │  │        Rendering Layer              │ │
│  │                 │  │                                     │ │
│  │ • Actor         │  │ • Renderer                          │ │
│  │ • CustomActor   │  │ • Geometry                          │ │
│  │ • ...           │  │ • Material                          │ │
│  │                 │  │ • Texture                           │ │
│  │                 │  │ • Shader                            │ │
│  └─────────────────┘  └─────────────────────────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                Resource Management Layer                    │
│                                                             │
│  • TextureManager  • ImageLoader  • FontManager             │
└─────────────────────────────────────────────────────────────┘
```

## Control과 Visual의 관계

### 1. 컨테이너-컴포넌트 관계

Control은 Visual을 컨테이너로 관리합니다:

- **일대다 관계**: 하나의 Control은 여러 Visual을 가질 수 있음
- **수명 주기 관리**: Control이 Visual의 생성, 소멸을 관리
- **상태 동기화**: Control의 상태 변경이 Visual에 전파

### 2. 데이터 흐름

```
Application
     ↓ (Control 생성)
Control::New()
     ↓ (초기화)
ControlImpl::OnInitialize()
     ↓ (데이터 설정)
ControlDataImpl::SetProperties()
     ↓ (Visual 생성 요청)
VisualFactory::CreateVisual()
     ↓ (Visual 생성)
VisualBase::New()
     ↓ (속성 설정)
VisualBase::SetProperties()
     ↓ (씬 연결)
VisualBase::SetOnScene()
     ↓ (렌더링)
Renderer::Render()
```

### 3. 속성 매핑

Control은 Property::Index를 통해 Visual을 관리합니다:

```cpp
// Control에서 Visual 등록
control.RegisterVisual(Control::Property::BACKGROUND, backgroundVisual);
control.RegisterVisual(ImageView::Property::IMAGE, imageVisual);

// Visual 활성화/비활성화
control.EnableVisual(Control::Property::BACKGROUND, true);
control.EnableVisual(ImageView::Property::IMAGE, false);
```

## 상세 아키텍처 분석

### 1. Control 아키텍처

#### 3계층 구조

Control은 명확한 책임 분리를 위해 3계층 구조를 가집니다:

1. **Control (Public API)**
   - 퍼블릭 인터페이스 제공
   - 사용자와의 상호작용 처리
   - 속성 시스템 노출

2. **ControlImpl (Internal Implementation)**
   - 내부 구현 로직
   - 상태 관리
   - 이벤트 처리

3. **ControlDataImpl (Data Management)**
   - 데이터 저장 및 관리
   - Visual 관리
   - 리소스 상태 추적

#### 주요 책임

- **사용자 입력 처리**: 키보드, 마우스, 터치 이벤트
- **상태 관리**: NORMAL, FOCUSED, DISABLED 등의 상태
- **스타일링**: 스타일 매니저와 연동
- **비주얼 관리**: Visual 등록, 활성화, 상태 관리
- **접근성**: 접근성 API 지원

### 2. Visual 아키텍처

#### 3계층 구조

Visual 역시 3계층 구조를 가집니다:

1. **VisualBase (Public API)**
   - 퍼블릭 인터페이스 제공
   - 속성 설정 및 조회
   - 애니메이션 지원

2. **VisualBaseImpl (Internal Implementation)**
   - 내부 구현 로직
   - 렌더링 파이프라인 관리
   - 리소스 로딩 조정

3. **VisualBaseDataImpl (Data Management)**
   - 렌더링 데이터 저장
   - 트랜스폼 정보
   - 커스텀 셰이더 관리

#### 주요 책임

- **렌더링**: GPU를 통한 시각적 요소 렌더링
- **리소스 관리**: 텍스처, 폰트 등 리소스 로딩 및 캐싱
- **애니메이션**: 속성 애니메이션 및 전환 효과
- **최적화**: 배치 렌더링, 아틀라싱 등 성능 최적화

## 구체적 구현 예시

### 1. ImageView 구현

ImageView는 Control과 Visual의 협력을 보여주는 좋은 예시입니다:

```cpp
// ImageView 생성
ImageView imageView = ImageView::New("image.jpg");

// 내부 동작:
// 1. Control::New() 호출
// 2. ControlImpl 초기화
// 3. ControlDataImpl에 ImageVisual 등록
// 4. VisualFactory를 통해 ImageVisual 생성
// 5. ImageVisual이 텍스처 로딩 시작
// 6. 로딩 완료 시 렌더러 설정
// 7. 씬에 추가되어 렌더링 시작
```

### 2. TextLabel 구현

TextLabel은 텍스트 렌더링을 위한 복잡한 협력을 보여줍니다:

```cpp
// TextLabel 생성
TextLabel textLabel = TextLabel::New("Hello DALi");

// 내부 동작:
// 1. Control::New() 호출
// 2. ControlImpl 초기화
// 3. ControlDataImpl에 TextVisual 등록
// 4. VisualFactory를 통해 TextVisual 생성
// 5. TextVisual이 FontManager를 통해 폰트 로딩
// 6. 텍스트 렌더링을 위한 지오메트리 생성
// 7. 셰이더 설정 및 렌더러 생성
// 8. 씬에 추가되어 렌더링 시작
```

## 확장 및 커스터마이징

### 1. 새로운 Control 생성

새로운 Control을 생성하는 과정:

```cpp
// 1. Control 클래스 정의
class MyControl : public Control
{
public:
    static MyControl New();
    void SetCustomProperty(const std::string& value);
    std::string GetCustomProperty() const;
};

// 2. ControlImpl 클래스 정의
class MyControlImpl : public ControlImpl
{
public:
    void OnInitialize() override;
    void OnSizeSet(const Vector3& size) override;
    void OnCustomPropertySet(const std::string& value);
};

// 3. ControlDataImpl에서 Visual 관리
void MyControlImpl::OnInitialize()
{
    // 커스텀 Visual 등록
    auto visual = VisualFactory::Get().CreateColorVisual(colorMap);
    Self().RegisterVisual(Property::CUSTOM_VISUAL, visual);
}
```

### 2. 새로운 Visual 생성

새로운 Visual을 생성하는 과정:

```cpp
// 1. VisualBase 상속
class MyVisual : public Visual::Base
{
public:
    static MyVisualPtr New(VisualFactoryCache& factoryCache);
    
protected:
    void OnInitialize() override;
    void DoSetProperties(const Property::Map& propertyMap) override;
    void DoSetOnScene(Actor& actor) override;
    void DoSetOffScene(Actor& actor) override;
    void OnSetTransform() override;
    void DoCreatePropertyMap(Property::Map& map) const override;
};

// 2. VisualFactory에 등록
Visual::BasePtr VisualFactory::CreateMyVisual(const Property::Map& properties)
{
    auto visual = MyVisual::New(mFactoryCache);
    visual->SetProperties(properties);
    return visual;
}
```

## 성능 최적화 전략

### 1. 렌더링 최적화

#### 드로우 콜 최소화
- **배치 렌더링**: 동일한 타입의 Visual들을 그룹화
- **인스턴싱**: 동일한 지오메트리를 여러 번 렌더링
- **오클루전 컬링**: 보이지 않는 객체 렌더링 건너뛰기

#### 리소스 최적화
- **아틀라싱**: 작은 텍스처들을 하나로 묶기
- **LOD (Level of Detail)**: 거리에 따른 상세 레벨 조정
- **리소스 풀링**: 자주 사용하는 리소스 재사용

### 2. 메모리 관리

#### 스마트 포인터 사용
```cpp
// IntrusivePtr을 통한 자동 메모리 관리
typedef IntrusivePtr<ImageVisual> ImageVisualPtr;
ImageVisualPtr visual = ImageVisual::New(factoryCache);
```

#### 지연 로딩
```cpp
// 필요한 시점에 리소스 로딩
void ImageVisual::DoSetOnScene(Actor& actor)
{
    if(!mTextures)
    {
        LoadTexture(); // 지연 로딩
    }
    InitializeRenderer();
}
```

### 3. CPU 최적화

#### 멀티스레딩
- **리소스 로딩**: 백그라운드 스레드에서 처리
- **이미지 디코딩**: 별도 스레드에서 병렬 처리
- **텍스트 렌더링**: 폰트 렌더링을 백그라운드에서 처리

#### 캐싱 전략
- **VisualFactoryCache**: 렌더러, 셰이더, 지오메트리 캐싱
- **TextureManager**: 텍스처 캐싱 및 아틀라싱
- **FontManager**: 폰트 및 글리프 캐싱

## 애니메이션 시스템

### 1. Control 레벨 애니메이션

Control은 상태 기반 애니메이션을 지원합니다:

```cpp
// 상태 전환 애니메이션
control.SetState(ControlState::FOCUSED, true); // with transition

// 속성 애니메이션
Animation animation = Animation::New(1.0f);
animation.AnimateTo(Property(control, Actor::Property::POSITION), Vector3(100, 0, 0));
animation.Play();
```

### 2. Visual 레벨 애니메이션

Visual은 세밀한 속성 애니메이션을 지원합니다:

```cpp
// Visual 속성 애니메이션
Animation animation = Animation::New(0.5f);
visual.AnimateProperty(animation, animator);
animation.Play();

// 커스텀 셰이더 애니메이션
Property::Map shaderMap;
shaderMap.Insert("vertexShader", vertexShaderSource);
shaderMap.Insert("fragmentShader", fragmentShaderSource);
visual.SetCustomShader(shaderMap);
```

## 디버깅 및 프로파일링

### 1. 디버깅 도구

#### Visual 디버깅
- **렌더러 정보**: 현재 설정된 렌더러 상태 확인
- **리소스 상태**: 텍스처, 셰이더 로딩 상태 모니터링
- **성능 카운터**: 렌더링 시간, 드로우 콜 수 측정

#### Control 디버깅
- **상태 추적**: Control의 현재 상태 및 전환 이력
- **이벤트 로그**: 사용자 입력 이벤트 추적
- **속성 모니터링**: 실시간 속성 값 확인

### 2. 프로파일링

#### 렌더링 프로파일링
```cpp
// 렌더링 성능 측정
auto start = std::chrono::high_resolution_clock::now();
renderer.Render();
auto end = std::chrono::high_resolution_clock::now();
auto duration = std::chrono::duration_cast<std::chrono::microseconds>(end - start);
```

#### 메모리 프로파일링
```cpp
// 메모리 사용량 추적
size_t textureMemory = TextureManager::Get().GetMemoryUsage();
size_t visualCount = VisualFactory::Get().GetActiveVisualCount();
```

## 베스트 프랙티스

### 1. Control 설계

#### 단일 책임 준수
- 하나의 Control은 명확한 단일 책임을 가짐
- 복잡한 기능은 여러 Control으로 분리

#### 상태 관리
- 상태 변경은 명확한 인터페이스를 통해
- 상태 전환 시 부작용 최소화

#### 이벤트 처리
- 이벤트 핸들러는 가볍게 유지
- 비동기 처리는 별도 스레드에서

### 2. Visual 설계

#### 리소스 관리
- 리소스는 필요한 시점에 로딩
- 사용하지 않는 리소스는 즉시 해제

#### 렌더링 최적화
- 불필요한 상태 변경 최소화
- 배치 렌더링 적극 활용

#### 셰이더 관리
- 커스텀 셰이더는 캐싱
- 동적 셰이더 생성은 최소화

### 3. 통합 설계

#### 인터페이스 설계
- Control과 Visual 간의 인터페이스는 명확하게
- 의존성은 최소화

#### 성능 고려
- Control과 Visual 간의 통신은 최소화
- 상태 동기화는 효율적으로

## 결론

DALi의 Control과 Visual 아키텍처는 유연하고 확장 가능한 UI 시스템을 제공하기 위해 설계되었습니다. 명확한 관심사 분리를 통해 개발자는 비즈니스 로직과 렌더링 로직을 독립적으로 개발할 수 있으며, 풍부한 확장 포인트를 통해 커스터마이징이 용이합니다.

성능 최적화, 메모리 관리, 애니메이션 시스템 등 고급 기능을 통해 고품질의 UI를 구현할 수 있으며, 잘 정의된 아키텍처를 통해 유지보수성과 확장성을 보장합니다.

이 아키텍처를 이해하고 활용함으로써 개발자는 효율적이고 성능 뛰어난 UI 애플리케이션을 개발할 수 있습니다.

---

## 관련 문서

- [Control 아키텍처 가이드](Control_Architecture_Guide.md)
- [Visual 아키텍처 가이드](Visual_Architecture_Guide.md)

## 관련 다이어그램

- [Control 클래스 계층 다이어그램](Control_Class_Hierarchy.puml)
- [Visual 클래스 계층 다이어그램](Visual_Class_Hierarchy.puml)
- [Control과 Visual 관계 다이어그램](Control_Visual_Relationship.puml)
- [DALi 전체 아키텍처 다이어그램](DALi_Overall_Architecture.puml)

## 참고 자료

- DALi 공식 문서
- DALi 샘플 코드
- DALi API 레퍼런스
- 성능 최적화 가이드
