# Visual Effects 디자인 패턴 및 결정 사항 분석

## 1. 개요

본 문서는 DALi Visual Effects 시스템에 적용된 다양한 디자인 패턴과 설계 결정들을 심층적으로 분석합니다. 특히 NUI와 DALi 간의 RenderEffect 생성 방식 차이와 그 이유, 그리고 현재 접근 방식의 장점을 상세히 설명합니다.

## 2. 핵심 디자인 패턴 분석

### 2.1 Strategy Pattern (전략 패턴)

#### 적용 사례

RenderEffect는 Strategy Pattern의 전형적인 예입니다:

```cpp
// 전략 인터페이스
class RenderEffect : public BaseHandle
{
public:
    virtual void Activate() = 0;
    virtual void Deactivate() = 0;
    virtual void Refresh() = 0;
    virtual bool IsActivated() const = 0;
    virtual OffScreenRenderableType GetOffScreenRenderableType() const = 0;
};

// 구체적인 전략들
class MaskEffect : public RenderEffect { /* ... */ };
class BackgroundBlurEffect : public RenderEffect { /* ... */ };
class GaussianBlurEffect : public RenderEffect { /* ... */ };
```

#### 설계 결정 분석

**결정 이유:**
- **확장성**: 새로운 비주얼 이펙트를 쉽게 추가할 수 있음
- **런타임 전환**: 실행 중에 이펙트를 동적으로 교체 가능
- **캡슐화**: 각 이펙트의 복잡한 내부 구현을 숨김

**장점:**
- 개방-폐쇄 원칙(OCP) 준수
- 단일 책임 원칙(SRP) 준수
- 클라이언트 코드 변경 없이 새로운 이펙트 추가

**단점:**
- 전략 클래스 수가 증가하면 복잡성 증가
- 각 전략을 이해해야 하는 학습 곡선 존재

### 2.2 Factory Pattern (팩토리 패턴)

#### DALi 방식: 정적 팩토리 메서드

```cpp
class MaskEffect : public RenderEffect
{
public:
    static MaskEffect New(Toolkit::Control maskControl)
    {
        Internal::MaskEffectImpl* impl = new Internal::MaskEffectImpl(maskControl);
        return MaskEffect(impl);
    }
    
    static MaskEffect New(Toolkit::Control maskControl, 
                             MaskMode maskMode, 
                             Vector2 maskPosition, 
                             Vector2 maskScale)
    {
        Internal::MaskEffectImpl* impl = new Internal::MaskEffectImpl(maskControl, maskMode, maskPosition, maskScale);
        return MaskEffect(impl);
    }
};
```

#### NUI 방식: 통합 팩토리 메서드

```csharp
public class RenderEffect : BaseHandle
{
    // 통합 팩토리 - 모든 이펙트 생성을 한 곳에서 관리
    public static BackgroundBlurEffect CreateBackgroundBlurEffect(float blurRadius)
    {
        return new BackgroundBlurEffect(Interop.BackgroundBlurEffect.New((uint)Math.Round(blurRadius, 0)));
    }
    
    public static GaussianBlurEffect CreateGaussianBlurEffect(float blurRadius)
    {
        return new GaussianBlurEffect(Interop.GaussianBlurEffect.New((uint)Math.Round(blurRadius, 0)));
    }
    
    public static MaskEffect CreateMaskEffect(View control)
    {
        return new MaskEffect(Interop.MaskEffect.New(control.SwigCPtr));
    }
    
    public static MaskEffect CreateMaskEffect(View control, MaskEffectMode maskMode, 
                                                  float positionX, float positionY, 
                                                  float scaleX, float scaleY)
    {
        return new MaskEffect(Interop.MaskEffect.New(control.SwigCPtr, maskMode, positionX, positionY, scaleX, scaleY));
    }
}
```

#### 설계 결정 분석

**DALi 방식의 장점:**
- **타입 안전성**: 각 이펙트 타입별로 명확한 팩토리 메서드
- **컴파일 타임 검증**: 잘못된 파라미터 조합을 컴파일 시에 발견
- **직관성**: 클래스 이름과 팩토리 메서드 이름이 일치

**NUI 방식의 장점:**
- **중앙화**: 모든 이펙트 생성이 한 곳에서 관리
- **일관성**: 통합된 생성 패턴 제공
- **확장성**: 새로운 이펙트 타입 추가 시 기존 코드 영향 최소화

**NUI 방식 선택 이유:**
1. **API 단순화**: 개발자가 RenderEffect 클래스 하나만 알면 됨
2. **검색 용이성**: IDE의 자동 완성 기능에서 모든 이펙트 생성 메서드 쉽게 노출
3. **네이밍 규칙**: C# 표준 라이브러리 디자인 패턴 준수
4. **유지보수**: 새로운 이펙트 추가 시 기존 코드 수정 최소화

### 2.3 Observer Pattern (옵저버 패턴)

#### 적용 사례

```cpp
class Control
{
private:
    RenderEffectImplPtr mRenderEffect;
    
public:
    void SetRenderEffect(Toolkit::RenderEffect effect)
    {
        ClearRenderEffect();  // 기존 옵저버에게 알림
        
        if(effect)
        {
            Internal::RenderEffectImpl* impl = dynamic_cast<Internal::RenderEffectImpl*>(effect.GetObjectPtr());
            impl->SetOwnerControl(Toolkit::Control(GetOwner()));  // 새로운 옵저버 등록
            mImpl->mRenderEffect = impl;
            impl->Activate();  // 옵저버에게 변경 알림
        }
    }
};
```

#### 설계 결정 분석

**결정 이유:**
- **생명주기 동기화**: Control과 RenderEffect의 생명주기를 동기화
- **느슨한 결합**: Control이 RenderEffect의 내부 구현에 직접 의존하지 않음
- **이벤트 기반 통신**: 상태 변경을 자동으로 전파

**장점:**
- 재사용성 향상
- 유지보수성 개선
- 컴포넌트 간 결합도 감소

### 2.4 Composite Pattern (컴포지트 패턴)

#### 적용 사례: 다중 Shadow 시스템

```cpp
class Control
{
public:
    void AddShadow(Shadow shadow)
    {
        // 여러 Shadow 객체를 컴포지트처럼 관리
        Visuals::ColorVisual shadowVisual = shadow.GetShadowVisual();
        AddShadowVisualInternal(shadowVisual, (shadow is InnerShadow) ? ViewShadowType.InnerShadow : ViewShadowType.BoxShadow);
    }
    
    // 개별 Shadow를 컴포지트의 일부로 처리
    void ClearShadows()
    {
        // 모든 자식 컴포넌트 정리
        for(auto& shadowVisual : mShadowVisuals)
        {
            RemoveRenderer(shadowVisual);
        }
        mShadowVisuals.clear();
    }
};
```

#### 설계 결정 분석

**결정 이유:**
- **통합 인터페이스**: 개별 Shadow와 다중 Shadow를 동일한 방식으로 처리
- **재귀적 구조**: Shadow 내부에 또 다른 Shadow를 포함할 수 있는 구조
- **단일 책임**: 클라이언트가 개별 객체나 컴포지트를 동일하게 다룰 수 있음

## 3. NUI vs DALi RenderEffect 생성 방식 차이 분석

### 3.1 근본적인 차이점

#### DALi: 분산된 팩토리

```cpp
// 각 클래스가 자신의 팩토리 메서드를 가짐
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
Toolkit::BackgroundBlurEffect blurEffect = Toolkit::BackgroundBlurEffect::New(10.0f);
Toolkit::GaussianBlurEffect gaussianEffect = Toolkit::GaussianBlurEffect::New(5.0f);
```

#### NUI: 중앙화된 팩토리

```csharp
// RenderEffect 클래스가 모든 이펙트 생성을 중앙 관리
MaskEffect maskEffect = RenderEffect.CreateMaskEffect(maskView);
BackgroundBlurEffect blurEffect = RenderEffect.CreateBackgroundBlurEffect(10.0f);
GaussianBlurEffect gaussianEffect = RenderEffect.CreateGaussianBlurEffect(5.0f);
```

### 3.2 NUI 방식의 기술적 이유

#### 3.2.1 P/Invoke 오버헤드 최소화

```csharp
// NUI 내부 구현
public class RenderEffect : BaseHandle
{
    public static BackgroundBlurEffect CreateBackgroundBlurEffect(float blurRadius)
    {
        // 단일 P/Invoke 호출로 C++ 객체 생성
        return new BackgroundBlurEffect(Interop.BackgroundBlurEffect.New((uint)Math.Round(blurRadius, 0)));
    }
    
    public static GaussianBlurEffect CreateGaussianBlurEffect(float blurRadius)
    {
        // 단일 P/Invoke 호출로 C++ 객체 생성
        return new GaussianBlurEffect(Interop.GaussianBlurEffect.New((uint)Math.Round(blurRadius, 0)));
    }
}
```

**기술적 장점:**
- **P/Invoke 호출 최적화**: 각 이펙트 생성에 필요한 P/Invoke 호출을 최소화
- **메모리 관리 통합**: C# 객체 생성과 C++ 객체 생성을 한 곳에서 관리
- **에러 핸들링 중앙화**: 생성 과정에서 발생하는 예외를 일관되게 처리

#### 3.2.2 타입 안전성 강화

```csharp
// 컴파일 타임 타입 안전성 보장
public static MaskEffect CreateMaskEffect(View control, MaskEffectMode maskMode, 
                                          float positionX = 0.0f, float positionY = 0.0f, 
                                          float scaleX = 1.0f, float scaleY = 1.0f)
{
    // 파라미터 유효성 검증
    if (control == null)
        throw new ArgumentNullException(nameof(control));
    
    // 타입 변환 및 검증
    if (!Enum.IsDefined(typeof(MaskEffectMode), maskMode))
        throw new ArgumentException($"Invalid maskMode: {maskMode}");
    
    // C++ 객체 생성 및 C# 래퍼 연결
    return new MaskEffect(Interop.MaskEffect.New(control.SwigCPtr, maskMode, positionX, positionY, scaleX, scaleY));
}
```

#### 3.2.3 API 일관성 및 사용성

**개발자 경험 향상:**
```csharp
// 일관된 API 패턴
var effect1 = RenderEffect.CreateMaskEffect(maskView);
var effect2 = RenderEffect.CreateBackgroundBlurEffect(10.0f);
var effect3 = RenderEffect.CreateGaussianBlurEffect(5.0f);

// 체이닝된 호출
view.SetRenderEffect(effect1);
effect1.Deactivate();
view.SetRenderEffect(effect2);
```

**IDE 지원:**
- IntelliSense에서 모든 이펙트 생성 메서드 쉽게 노출
- 파라미터 힌트와 문서화 제공
- 리팩토링 지원으로 코드 일관성 유지

### 3.3 현재 접근 방식의 장점

#### 3.3.1 확장성

```csharp
// 새로운 이펙트 추가 시 기존 코드 영향 최소
public class RenderEffect : BaseHandle
{
    // 기존 메서드들은 그대로 유지
    public static BackgroundBlurEffect CreateBackgroundBlurEffect(float blurRadius) { /* ... */ }
    public static GaussianBlurEffect CreateGaussianBlurEffect(float blurRadius) { /* ... */ }
    
    // 새로운 이펙트 추가
    public static CustomEffect CreateCustomEffect(CustomEffectParameters parameters)
    {
        return new CustomEffect(Interop.CustomEffect.New(parameters.ToNative()));
    }
}
```

#### 3.3.2 유지보수성

```csharp
// 내부 구현 변경이 API에 미치는 영향 최소화
public class RenderEffect : BaseHandle
{
    public static BackgroundBlurEffect CreateBackgroundBlurEffect(float blurRadius)
    {
        // 내부 구현이 변경되어도 API는 그대로 유지
        // 예: 최적화된 알고리즘으로 교체
        uint optimizedRadius = OptimizeBlurRadius(blurRadius);
        return new BackgroundBlurEffect(Interop.BackgroundBlurEffect.NewOptimized(optimizedRadius));
    }
}
```

#### 3.3.3 테스트 용이성

```csharp
// 단위 테스트 작성 용이
[Test]
public void CreateMaskEffect_ValidParameters_ReturnsEffect()
{
    // Arrange
    var maskView = new View();
    
    // Act
    var effect = RenderEffect.CreateMaskEffect(maskView);
    
    // Assert
    Assert.IsNotNull(effect);
    Assert.IsTrue(effect.IsActivated);
}

// 모의 객체를 사용한 통합 테스트
[Test]
public void CreateMaskEffect_WithMock_CallsInteropCorrectly()
{
    // Arrange
    var mockInterop = new Mock<InteropMaskEffect>();
    var expectedPtr = new IntPtr(0x12345678);
    mockInterop.Setup(x => x.New(It.IsAny<IntPtr>())).Returns(expectedPtr);
    
    // Act & Assert
    // 통합된 팩토리 메서드로 쉽운 테스트 가능
}
```

## 4. 기타 중요한 설계 결정

### 4.1 Handle-Impl 패턴 (핸들-구현 패턴)

#### 설계 구조

```cpp
// 퍼블릭 핸들
class RenderEffect : public BaseHandle
{
public:
    // 내부 구현에 대한 접근자
    Internal::RenderEffectImpl* GetObjectPtr();
};

// 내부 구현
class RenderEffectImpl : public RefObject
{
public:
    // 실제 기능 구현
    virtual void Activate();
    virtual void Deactivate();
};
```

#### 설계 결정 분석

**결정 이유:**
- **이중 인터페이스**: 퍼블릭 API와 내부 구현 분리
- **메모리 안전성**: 참조 카운트 기반의 안전한 메모리 관리
- **ABI 안정성**: 내부 구현 변경이 퍼블릭 API에 영향 최소화

**장점:**
- **캡슐화**: 복잡한 내부 구현 숨김
- **이식성**: 퍼블릭 API와 내부 구현의 독립적 발전
- **테스트 용이성**: 모의 객체를 통한 단위 테스트 용이

### 4.2 오프스크린 렌더링 타입 시스템

#### 설계 구조

```cpp
enum class OffScreenRenderableType
{
    NONE = 0,      // 오프스크린 렌더링 사용 안 함
    BACKWARD = 1,  // 역방향: Control → 이펙트
    FORWARD = 2    // 순방향: 이펙트 → Control
};
```

#### 설계 결정 분석

**결정 이유:**
- **렌더링 순서 제어**: 복잡한 이펙트 체이닝을 위한 순서 제어
- **성능 최적화**: 불필요한 렌더링 패스를 피하기 위한 최적화
- **확장성**: 새로운 렌더링 타입을 쉽게 추가 가능

**각 타입의 특징:**

**BACKWARD 타입 (MaskEffect):**
```cpp
// 1. 소스 Control 렌더링
// 2. 마스크 Control 렌더링  
// 3. 두 결과 합성
void MaskEffectImpl::GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward)
{
    if(!isForward)  // BACKWARD 렌더링
    {
        tasks.push_back(CreateSourceRenderTask());
        tasks.push_back(CreateMaskRenderTask());
        tasks.push_back(CreateCompositeRenderTask());
    }
}
```

**FORWARD 타입 (BackgroundBlurEffect):**
```cpp
// 1. 배경 캡처
// 2. 배경 블러 적용
// 3. 최종 Control 렌더링
void BackgroundBlurEffectImpl::GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward)
{
    if(isForward)  // FORWARD 렌더링
    {
        tasks.push_back(CreateBackgroundCaptureTask());
        tasks.push_back(CreateBlurRenderTask());
        tasks.push_back(CreateFinalRenderTask());
    }
}
```

### 4.3 RenderTask 재정렬 시스템

#### 설계 결정

```cpp
void RenderTaskList::ReorderTasks(Dali::Internal::LayerList& layerList)
{
    // 1. BACKWARD 타입 먼저 처리 (의존성이 낮은 순서)
    for(auto& subtree : renderableData)
    {
        for(auto& actor : subtree.second)
        {
            if(actor->GetOffScreenRenderableType() & OffScreenRenderable::Type::BACKWARD)
            {
                // BACKWARD 태스크에 낮은 OrderIndex 할당
                SetOrderIndexForBackwardTasks(actor, orderIndex++);
            }
        }
    }
    
    // 2. FORWARD 타입 나중에 처리 (의존성이 높은 순서)
    for(auto& subtree : renderableData)
    {
        if(subtree.first && subtree.first->GetOffScreenRenderableType() & OffScreenRenderable::Type::FORWARD)
        {
            // FORWARD 태스크에 높은 OrderIndex 할당
            SetOrderIndexForForwardTasks(subtree.first, orderIndex++);
        }
    }
}
```

#### 설계 결정 분석

**결정 이유:**
- **의존성 관리**: BackgroundBlurEffect가 배경에 의존하므로 먼저 처리
- **렌더링 정확성**: 올바른 순서로 렌더링하여 시각적 아티팩트 방지
- **자동 최적화**: 개발자가 수동으로 순서를 관리할 필요 없음

**장점:**
- **자동화**: 복잡한 렌더링 순서를 자동으로 계산
- **안정성**: 렌더링 순서로 인한 버그 방지
- **확장성**: 새로운 이펙트 타입에 쉽게 적응

### 4.4 메모리 관리 전략

#### DALi: 참조 카운트 + WeakHandle

```cpp
class Control
{
private:
    RenderEffectImplPtr mRenderEffect;  // 강한 참조
};

class RenderEffectImpl
{
private:
    WeakHandle<Toolkit::Control> mOwnerControl;  // 약한 참조
    
public:
    void SetOwnerControl(Toolkit::Control control)
    {
        mOwnerControl = WeakHandle<Toolkit::Control>(control);
    }
};
```

#### NUI: GC + WeakReference

```csharp
public class RenderEffect : BaseHandle
{
    private WeakReference<View> _ownerView;  // 약한 참조
    
    internal void SetOwnerView(View view)
    {
        // 기존 소유자 정리
        if (_ownerView != null && _ownerView.TryGetTarget(out View previousTarget) && previousTarget != null)
        {
            previousTarget.ClearRenderEffect(false);
        }
        
        // 새로운 소유자 설정
        _ownerView = view ? new WeakReference<View>(view) : null;
    }
}
```

#### 설계 결정 분석

**순환 참조 방지:**
- **Control → RenderEffect**: 강한 참조 (소유 관계)
- **RenderEffect → Control**: 약한 참조 (생명주기 관리)

**장점:**
- **메모리 누수 방지**: 순환 참조로 인한 메모리 누수 방지
- **안전한 정리**: 객체가 적절한 시점에 정리됨을 보장
- **성능 최적화**: 불필요한 객체 유지 방지

## 5. 성능 최적화를 위한 설계 결정

### 5.1 객체 풀링 시스템

#### 설계 구조

```cpp
class FramebufferManager
{
public:
    static FramebufferManager& Get()
    {
        static FramebufferManager instance;  // 싱글톤
        return instance;
    }
    
    Dali::FrameBuffer AcquireFramebuffer(Vector2 size)
    {
        // 재사용 가능한 Framebuffer 찾기
        for(auto& fb : mFramebufferPool)
        {
            if(!fb.inUse && fb.size == size)
            {
                fb.inUse = true;
                return fb.framebuffer;
            }
        }
        
        // 새로운 Framebuffer 생성 (최후 수단)
        Dali::FrameBuffer newFb = Dali::FrameBuffer::New(size.width, size.height);
        mFramebufferPool.push_back({newFb, size, true});
        return newFb;
    }
    
private:
    std::vector<FramebufferEntry> mFramebufferPool;
};
```

#### 설계 결정 분석

**결정 이유:**
- **생성 비용 절감**: Framebuffer 생성은 비용이 높으므로 재사용
- **메모리 단편화 방지**: 빈번은 생성/소멸로 인한 메모리 단편화 방지
- **성능 향상**: 객체 생성 시간 단춱으로 렌더링 성능 향상

**장점:**
- **메모리 효율성**: 제한된 GPU 메모리를 효율적으로 사용
- **렌더링 성능**: 객체 생성 오버헤드 감소
- **확장성**: 다른 리소스 타입에도 적용 가능한 패턴

### 5.2 렌더 태스크 최적화

#### 설계 구조

```cpp
class RenderTaskOptimizer
{
public:
    void OptimizeRenderTasks(std::vector<Dali::RenderTask>& tasks)
    {
        // 1. 동일한 타겟을 가진 태스크 병합
        MergeTasksWithSameTarget(tasks);
        
        // 2. 불필요한 태스크 제거
        RemoveRedundantTasks(tasks);
        
        // 3. 태스크 순서 최적화
        OptimizeTaskOrder(tasks);
    }
    
private:
    void MergeTasksWithSameTarget(std::vector<Dali::RenderTask>& tasks)
    {
        std::map<Dali::FrameBuffer, std::vector<Dali::RenderTask>> tasksByFramebuffer;
        
        // Framebuffer별로 태스크 그룹화
        for(const auto& task : tasks)
        {
            Dali::FrameBuffer framebuffer = task.GetFrameBuffer();
            tasksByFramebuffer[framebuffer].push_back(task);
        }
        
        // 병합 가능한 태스크 병합
        tasks.clear();
        for(const auto& [framebuffer, taskList] : tasksByFramebuffer)
        {
            if(taskList.size() > 1)
            {
                tasks.push_back(MergeRenderTasks(taskList));
            }
            else
            {
                tasks.push_back(taskList[0]);
            }
        }
    }
};
```

#### 설계 결정 분석

**결정 이유:**
- **렌더링 호출 최소화**: 불필요한 GPU 상태 변경 방지
- **배치 처리**: 동일한 작업을 그룹화하여 한 번에 처리
- **GPU 활용 최적화**: GPU 파이프라인 효율성 극대화

**장점:**
- **렌더링 성능**: GPU 호출 횟수 감소로 성능 향상
- **전력 소비 최적화**: 불필요한 GPU 작업으로 전력 소비 감소
- **안정성**: 렌더링 순서로 인한 시각적 아티팩트 감소

## 6. 결론 및 향후 개선 방향

### 6.1 현재 설계의 성공 요인

1. **확장성**: Strategy Pattern을 통한 쉬운 이펙트 확장
2. **성능**: 객체 풀링과 렌더 태스크 최적화로 높은 성능 달성
3. **안정성**: 순환 참조 방지와 메모리 관리로 안정성 확보
4. **사용성**: NUI의 중앙화된 팩토리로 개발자 경험 향상

### 6.2 잠재적 개선점

1. **복잡성 증가**: 다양한 패턴 적용으로 시스템 복잡성 증가
2. **학습 곡선**: 새로운 개발자가 모든 패턴을 이해하는 데 시간 소요
3. **디버깅 어려움**: 복잡한 내부 동작으로 디버깅의 어려움 존재

### 6.3 향후 개선 방향

#### 6.3.1 단순화된 API 제공

```csharp
// 고수준 API 제안
public class VisualEffects
{
    public static View ApplyGlassmorphism(View view, float blurRadius = 10.0f, float opacity = 0.3f)
    {
        var blurEffect = RenderEffect.CreateBackgroundBlurEffect(blurRadius);
        view.SetRenderEffect(blurEffect);
        view.BackgroundColor = new Color(1.0f, 1.0f, 1.0f, opacity);
        return view;
    }
    
    public static View ApplyNeumorphism(View view, Vector3 cornerRadius)
    {
        view.CornerRadius = cornerRadius;
        view.AddShadow(new InnerShadow { /* ... */ });
        view.AddShadow(new Shadow { /* ... */ });
        return view;
    }
}
```

#### 6.3.2 선언적 구성 지원

```csharp
// XAML 스타일 지원 제안
<View>
  <View.RenderEffect>
    <BackgroundBlurEffect BlurRadius="10" />
  </View.RenderEffect>
  <View.Shadows>
    <Shadow Color="Black" Offset="5,5" BlurRadius="10" />
    <InnerShadow Color="White" Offset="-2,-2" BlurRadius="5" />
  </View.Shadows>
</View>
```

#### 6.3.3 성능 모니터링 강화

```csharp
// 내장된 성능 모니터링
public class PerformanceMonitor
{
    public static void EnableEffectProfiling()
    {
        // 이펙트 생성, 적용, 렌더링 시간 측정
        // 자동 성능 병목점 생성
        // 성능 저감 시 자동 최적화 제안
    }
}
```

Visual Effects 시스템의 현재 설계는 복잡한 요구사항을 효과적으로 해결하는 잘 설계된 아키텍처입니다. 다양한 디자인 패턴의 조합을 통해 확장성, 성능, 안정성을 모두 확보했으며, 특히 NUI와 DALi 간의 차이점을 고려한 설계 결정들이 개발자 경험을 크게 향상시켰습니다.
