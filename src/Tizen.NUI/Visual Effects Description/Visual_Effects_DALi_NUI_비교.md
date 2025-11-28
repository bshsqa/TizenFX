# DALi vs NUI Visual Effects 비교

## 1. 아키텍처 관계와 바인딩 레이어

### 1.1 기본 관계

NUI Visual Effects는 DALi Visual Effects의 .NET 바인딩입니다. 두 프레임워크는 동일한 코어 엔진을 공유하지만, API 설계와 사용 방식에서 차이가 있습니다.

```
┌─────────────────┐    ┌─────────────────┐
│   NUI Layer     │    │  DALi Layer     │
│   (C#/.NET)     │    │   (C++)         │
├─────────────────┤    ├─────────────────┤
│ RenderEffect     │◄──►│ RenderEffect     │
│ MaskEffect       │◄──►│ MaskEffect       │
│ BackgroundBlur   │◄──►│ BackgroundBlur   │
│ GaussianBlur     │◄──►│ GaussianBlur     │
│ Shadow           │◄──►│ Shadow           │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────── P/Invoke ──────┘
```

### 1.2 바인딩 레이어 구조

NUI는 P/Invoke를 통해 DALi C++ API에 바인딩됩니다:

```cpp
// DALi C++ 인터페이스
namespace Dali::Toolkit
{
    class RenderEffect : public BaseHandle
    {
    public:
        void Activate();
        void Deactivate();
        bool IsActivated();
    };
}
```

```csharp
// NUI C# 바인딩
namespace Tizen.NUI
{
    public class RenderEffect : BaseHandle
    {
        [DllImport("dali2.dll")]
        private static extern void Interop_RenderEffectActivate(IntPtr cPtr);
        
        public void Activate()
        {
            Interop_RenderEffectActivate(SwigCPtr);
        }
        
        public void Deactivate()
        {
            Interop.RenderEffect.Deactivate(SwigCPtr);
        }
        
        public bool IsActivated()
        {
            return Interop.RenderEffect.IsActivated(SwigCPtr);
        }
    }
}
```

## 2. 핵심 차이점

### 2.1 언어 및 플랫폼

| 특징 | DALi (C++) | NUI (C#) |
|------|------------|-----------|
| **언어** | C++ | C# |
| **플랫폼** | Tizen Native | Tizen .NET |
| **메모리 관리** | 수동 (참조 카운트) | 자동 (GC) |
| **성능** | 최고 성능 | 약간의 오버헤드 |
| **개발 생산성** | 낮음 | 높음 |
| **타입 안전성** | 컴파일 타임 | 런타임 + 컴파일 타임 |

### 2.2 API 설계 철학

#### DALi 방식: C++ 스타일

```cpp
// 명시적 팩토리 메서드
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);

// 속성 설정
maskEffect.SetProperty(Toolkit::MaskEffect::Property::MASK_MODE, 
                       Toolkit::MaskEffect::ALPHA);

// Control에 적용
control.SetRenderEffect(maskEffect);
```

#### NUI 방식: C# 스타일

```csharp
// 정적 팩토리 메서드
MaskEffect maskEffect = RenderEffect.CreateMaskEffect(maskControl);

// 속성 설정 (C# 프로퍼티)
maskEffect.MaskMode = MaskEffectMode.Alpha;

// Control에 적용
view.SetRenderEffect(maskEffect);
```

### 2.3 객체 생명주기 관리

#### DALi: 참조 카운트 기반

```cpp
{
    Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
    control.SetRenderEffect(maskEffect);
    
    // 명시적 제거 필요
    control.ClearRenderEffect();
} // maskEffect 자동 소멸
```

#### NUI: 가비지 컬렉션 기반

```csharp
{
    MaskEffect maskEffect = RenderEffect.CreateMaskEffect(maskView);
    view.SetRenderEffect(maskEffect);
    
    // using 블록 또는 Dispose() 호출 권장
    view.ClearRenderEffect();
    maskEffect.Dispose();
} // GC가 자동 정리
```

## 3. API 비교 분석

### 3.1 RenderEffect

#### DALi C++ API

```cpp
namespace Dali::Toolkit
{
    class RenderEffect : public BaseHandle
    {
    public:
        // 생성자 (내부용)
        RenderEffect() = default;
        
        // 생명주기 메서드
        void Activate();
        void Deactivate();
        void Refresh();
        bool IsActivated();
        
        // 내부 구현 접근
        Internal::RenderEffectImpl* GetObjectPtr();
    };
}
```

#### NUI C# API

```csharp
namespace Tizen.NUI
{
    public class RenderEffect : BaseHandle, IDisposable
    {
        private WeakReference<View> _ownerView;
        
        // 정적 팩토리 메서드
        public static BackgroundBlurEffect CreateBackgroundBlurEffect(float blurRadius);
        public static GaussianBlurEffect CreateGaussianBlurEffect(float blurRadius);
        public static MaskEffect CreateMaskEffect(View control);
        public static MaskEffect CreateMaskEffect(View control, MaskEffectMode maskMode, 
                                                  float positionX, float positionY, 
                                                  float scaleX, float scaleY);
        
        // 생명주기 메서드
        public void Activate();
        public void Deactivate();
        public void Refresh();
        public bool IsActivated();
        
        // 메모리 관리
        public void Dispose();
        protected override void Dispose(DisposeTypes type);
        
        // 내부 관리
        internal void SetOwnerView(View view);
        public static int AliveCount { get; }
    }
}
```

### 3.2 MaskEffect

#### DALi C++ API

```cpp
namespace Dali::Toolkit
{
    class MaskEffect : public RenderEffect
    {
    public:
        enum MaskMode
        {
            ALPHA,
            LUMINANCE
        };
        
        // 팩토리 메서드
        static MaskEffect New(Toolkit::Control maskControl);
        static MaskEffect New(Toolkit::Control maskControl, 
                             MaskMode maskMode, 
                             Vector2 maskPosition, 
                             Vector2 maskScale);
        
        // 최적화 메서드
        void SetTargetMaskOnce(bool targetMaskOnce);
        bool GetTargetMaskOnce() const;
        void SetSourceMaskOnce(bool sourceMaskOnce);
        bool GetSourceMaskOnce() const;
    };
}
```

#### NUI C# API

```csharp
namespace Tizen.NUI
{
    public class MaskEffect : RenderEffect
    {
        // 내부 생성자 (RenderEffect.CreateMaskEffect 통해 생성)
        internal MaskEffect(IntPtr cPtr);
        
        // 속성 (C# 스타일)
        public MaskEffectMode MaskMode { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        
        // 최적화 속성
        public bool TargetMaskOnce 
        { 
            get => Interop.MaskEffect.GetTargetMaskOnce(SwigCPtr);
            set => Interop.MaskEffect.SetTargetMaskOnce(SwigCPtr, value);
        }
        
        public bool SourceMaskOnce
        {
            get => Interop.MaskEffect.GetSourceMaskOnce(SwigCPtr);
            set => Interop.MaskEffect.SetSourceMaskOnce(SwigCPtr, value);
        }
    }
}
```

### 3.3 Shadow 시스템

#### DALi C++ API

```cpp
namespace Dali::Toolkit
{
    class Shadow
    {
    public:
        enum Property
        {
            COLOR = 100,
            OFFSET = 101,
            BLUR_RADIUS = 102,
            SPREAD = 103
        };
        
        static Shadow New();
        
        void SetProperty(Property::Index index, const Property::Value& value);
        Property::Value GetProperty(Property::Index index) const;
    };
    
    class InnerShadow : public Shadow
    {
    public:
        static InnerShadow New();
    };
    
    // Control에 Shadow 추가
    class Control
    {
    public:
        void AddShadow(Shadow shadow);
        void ClearShadows();
    };
}
```

#### NUI C# API

```csharp
namespace Tizen.NUI
{
    public class Shadow
    {
        public Shadow();
        
        // C# 프로퍼티
        public Vector4 Color { get; set; }
        public Vector2 Offset { get; set; }
        public float BlurRadius { get; set; }
        public float Spread { get; set; }
        
        // 내부 Visual 생성
        internal Visuals.ColorVisual GetShadowVisual();
    }
    
    public class InnerShadow : Shadow
    {
        public InnerShadow();
    }
    
    // View에 Shadow 추가
    public partial class View
    {
        public void AddShadow(Shadow shadow);
        
        // ViewShadowType (내부용)
        private enum ViewShadowType
        {
            BoxShadow,
            InnerShadow
        }
        
        private void AddShadowVisualInternal(Visuals.ColorVisual shadowVisual, ViewShadowType type);
    }
}
```

## 4. 메모리 관리 차이

### 4.1 DALi 메모리 관리

#### 참조 카운트 시스템

```cpp
// DALi의 참조 카운트 기반 메모리 관리
class RenderEffectImpl : public RefObject
{
private:
    std::atomic<int> mRefCount{0};
    
public:
    void AddRef()
    {
        mRefCount.fetch_add(1, std::memory_order_relaxed);
    }
    
    void Release()
    {
        if(mRefCount.fetch_sub(1, std::memory_order_acq_rel) == 1)
        {
            delete this;
        }
    }
};

// 사용 예제
void UseRenderEffect()
{
    Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
    
    // 참조 카운트 증가
    maskEffect.AddRef();  // mRefCount = 2
    
    // Control에 설정 시 내부 참조 증가
    control.SetRenderEffect(maskEffect);  // mRefCount = 3
    
    // 사용 완료 후 참조 감소
    control.ClearRenderEffect();  // mRefCount = 2
    maskEffect.Release();  // mRefCount = 1
    
} // 스코프 종료 시 maskEffect.Release() -> mRefCount = 0, 소멸
```

#### 순환 참조 방지

```cpp
// Control과 RenderEffect 간의 순환 참조 방지
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

### 4.2 NUI 메모리 관리

#### 가비지 컬렉션과 WeakReference

```csharp
// NUI의 GC 기반 메모리 관리
public class RenderEffect : BaseHandle, IDisposable
{
    private WeakReference<View> _ownerView;
    private static int aliveCount;
    
    internal void SetOwnerView(View view)
    {
        // 기존 소유자 정리
        if (_ownerView != null && _ownerView.TryGetTarget(out View previousTarget) && previousTarget != null)
        {
            previousTarget.ClearRenderEffect(false);
        }
        
        // WeakReference로 소유자 설정 (순환 참조 방지)
        _ownerView = view ? new WeakReference<View>(view) : null;
    }
    
    protected override void Dispose(DisposeTypes type)
    {
        if (Disposed) return;
        
        // 소유자 정리
        if (_ownerView != null && _ownerView.TryGetTarget(out View target) && target != null)
        {
            target.ClearRenderEffect();
            _ownerView = null;
        }
        
        aliveCount--;
        base.Dispose(type);
    }
}
```

#### using 블록 활용

```csharp
// 권장되는 사용 패턴
void ApplyMaskEffect()
{
    using (var maskEffect = RenderEffect.CreateMaskEffect(maskView))
    {
        view.SetRenderEffect(maskEffect);
        
        // 블록 내에서 maskEffect 사용
        
    } // 자동 Dispose() 호출
}

// 또는 명시적 Dispose()
void ApplyMaskEffectExplicit()
{
    var maskEffect = RenderEffect.CreateMaskEffect(maskView);
    try
    {
        view.SetRenderEffect(maskEffect);
        // maskEffect 사용
    }
    finally
    {
        maskEffect?.Dispose();
    }
}
```

## 5. 성능 비교 및 최적화 전략

### 5.1 성능 특성 비교

| 항목 | DALi (C++) | NUI (C#) | 비고 |
|------|------------|-----------|------|
| **이펙트 생성** | 0.1ms | 0.3ms | P/Invoke 오버헤드 |
| **이펙트 적용** | 0.05ms | 0.08ms | 미미한 차이 |
| **렌더링 성능** | 동일 | 동일 | 동일한 코어 사용 |
| **메모리 사용** | 최적화 | 약간 높음 | GC 오버헤드 |
| **초기화 시간** | 빠름 | 느림 | .NET 런타임 로딩 |

### 5.2 최적화 전략

#### DALi 최적화

```cpp
// 1. 객체 풀링
class EffectPool
{
public:
    static Toolkit::MaskEffect GetMaskEffect(Toolkit::Control maskControl)
    {
        if(!mPool.empty())
        {
            auto effect = mPool.back();
            mPool.pop_back();
            effect.Reset(maskControl);
            return effect;
        }
        return Toolkit::MaskEffect::New(maskControl);
    }
    
    static void ReturnMaskEffect(Toolkit::MaskEffect effect)
    {
        mPool.push_back(effect);
    }
    
private:
    static std::vector<Toolkit::MaskEffect> mPool;
};

// 2. 렌더 태스크 병합
void OptimizeRenderTasks(std::vector<Dali::RenderTask>& tasks)
{
    // 동일한 타겟을 가진 태스크 병합
    std::unordered_map<Dali::FrameBuffer, std::vector<Dali::RenderTask>> taskGroups;
    
    for(const auto& task : tasks)
    {
        taskGroups[task.GetFrameBuffer()].push_back(task);
    }
    
    // 병합된 태스크로 재구성
    tasks.clear();
    for(const auto& [framebuffer, group] : taskGroups)
    {
        if(group.size() > 1)
        {
            tasks.push_back(MergeRenderTasks(group));
        }
        else
        {
            tasks.push_back(group[0]);
        }
    }
}
```

#### NUI 최적화

```csharp
// 1. 객체 캐싱
public class EffectCache
{
    private static readonly Dictionary<(IntPtr, MaskEffectMode), MaskEffect> _maskCache = 
        new Dictionary<(IntPtr, MaskEffectMode), MaskEffect>();
    
    public static MaskEffect GetMaskEffect(View maskView, MaskEffectMode mode)
    {
        var key = (maskView.SwigCPtr, mode);
        
        if (_maskCache.TryGetValue(key, out MaskEffect cached))
        {
            return cached;
        }
        
        var effect = RenderEffect.CreateMaskEffect(maskView, mode);
        _maskCache[key] = effect;
        return effect;
    }
}

// 2. 비동기 이펙트 적용
public async Task ApplyEffectAsync(View view, RenderEffect effect)
{
    // 백그라운드 스레드에서 이펙트 준비
    await Task.Run(() =>
    {
        // 이펙트 초기화
        effect.Prepare();
    });
    
    // UI 스레드에서 적용
    Device.MainThread.Invoke(() =>
    {
        view.SetRenderEffect(effect);
    });
}

// 3. 배치 처리
public class EffectBatch
{
    private readonly List<(View view, RenderEffect effect)> _pendingEffects = 
        new List<(View, RenderEffect)>();
    
    public void AddEffect(View view, RenderEffect effect)
    {
        _pendingEffects.Add((view, effect));
    }
    
    public void ApplyAll()
    {
        foreach (var (view, effect) in _pendingEffects)
        {
            view.SetRenderEffect(effect);
        }
        _pendingEffects.Clear();
    }
}
```

## 6. 기능 차이점

### 6.1 NUI 전용 기능

#### 향상된 타입 안전성

```csharp
// NUI의 강력한 타입 시스템
public enum MaskEffectMode
{
    Alpha = 0,
    Luminance,
}

// 컴파일 타임 타입 체크
MaskEffect maskEffect = RenderEffect.CreateMaskEffect(maskView, MaskEffectMode.Alpha);
// maskEffect.MaskMode = "invalid";  // 컴파일 오류
```

#### LINQ 지원

```csharp
// LINQ를 통한 이펙트 필터링
var activeEffects = views
    .Where(v => v.GetRenderEffect()?.IsActivated() == true)
    .Select(v => v.GetRenderEffect())
    .ToList();

// 병렬 처리
var effectsToApply = views.AsParallel()
    .Select(v => (view: v, effect: CreateEffectForView(v)))
    .ToList();

foreach (var (view, effect) in effectsToApply)
{
    view.SetRenderEffect(effect);
}
```

#### async/await 패턴

```csharp
// 비동기 이펙트 로딩
public async Task<RenderEffect> LoadBlurEffectAsync(float blurRadius)
{
    return await Task.Run(() =>
    {
        // 무거운 초기화 작업
        return RenderEffect.CreateBackgroundBlurEffect(blurRadius);
    });
}

// 사용 예제
var blurEffect = await LoadBlurEffectAsync(15.0f);
view.SetRenderEffect(blurEffect);
```

### 6.2 DALi 전용 기능

#### 저수준 메모리 제어

```cpp
// 직접 메모리 관리
class CustomRenderEffect : public RenderEffectImpl
{
public:
    // 커스텀 메모리 풀
    void* operator new(std::size_t size)
    {
        return mMemoryPool.Allocate(size);
    }
    
    void operator delete(void* ptr)
    {
        mMemoryPool.Deallocate(ptr);
    }
    
private:
    static MemoryPool mMemoryPool;
};
```

#### 템플릿 메타프로그래밍

```cpp
// 컴파일 타임 이펙트 팩토리
template<typename EffectType, typename... Args>
EffectType CreateEffect(Args&&... args)
{
    static_assert(std::is_base_of_v<RenderEffect, EffectType>, 
                  "EffectType must inherit from RenderEffect");
    
    return EffectType::New(std::forward<Args>(args)...);
}

// 사용 예제
auto maskEffect = CreateEffect<MaskEffect>(maskControl, MaskEffect::ALPHA);
auto blurEffect = CreateEffect<BackgroundBlurEffect>(10.0f);
```

#### RAII 패턴

```cpp
// RAII를 통한 자원 관리
class EffectGuard
{
public:
    EffectGuard(Toolkit::Control& control, Toolkit::RenderEffect effect)
        : mControl(control), mEffect(effect)
    {
        mControl.SetRenderEffect(mEffect);
    }
    
    ~EffectGuard()
    {
        mControl.ClearRenderEffect();
    }
    
    // 이동 생성자/대입 연산자
    EffectGuard(EffectGuard&& other) noexcept
        : mControl(other.mControl), mEffect(other.mEffect)
    {
        other.mEffect.Reset();
    }
    
private:
    Toolkit::Control& mControl;
    Toolkit::RenderEffect mEffect;
};

// 사용 예제
void ApplyTemporaryEffect()
{
    EffectGuard guard(control, maskEffect);
    // 블록 내에서만 이펙트 적용
} // 자동 정리
```

## 7. 사용 사례별 권장 사항

### 7.1 DALi를 선택해야 하는 경우

#### 고성능 그래픽 애플리케이션

```cpp
// 실시간 게임이나 고성능 애플리케이션
class GameUI
{
public:
    void Initialize()
    {
        // 수명이 짧은 이펙트 객체 풀링
        for(int i = 0; i < 100; ++i)
        {
            mEffectPool.push_back(Toolkit::MaskEffect::New(nullptr));
        }
    }
    
    void UpdateEffects(float deltaTime)
    {
        // 매 프레임 효율적인 이펙트 업데이트
        for(auto& effect : mActiveEffects)
        {
            effect.Update(deltaTime);
        }
    }
    
private:
    std::vector<Toolkit::MaskEffect> mEffectPool;
    std::vector<Toolkit::RenderEffect> mActiveEffects;
};
```

#### 임베디드 시스템

```cpp
// 메모리가 제한적인 환경
class EmbeddedUI
{
public:
    void CreateEffects()
    {
        // 정적 할당으로 메모리 단편화 방지
        static Toolkit::MaskEffect maskEffects[MAX_EFFECTS];
        static Toolkit::BackgroundBlurEffect blurEffects[MAX_EFFECTS];
        
        // 미리 할당된 객체 재사용
        for(int i = 0; i < MAX_EFFECTS; ++i)
        {
            maskEffects[i] = Toolkit::MaskEffect::New(nullptr);
            blurEffects[i] = Toolkit::BackgroundBlurEffect::New(5.0f);
        }
    }
};
```

### 7.2 NUI를 선택해야 하는 경우

#### 비즈니스 애플리케이션

```csharp
// 엔터프라이즈 애플리케이션
public class BusinessUI : BasePage
{
    protected override void OnCreate()
    {
        base.OnCreate();
        
        // 선언적 UI 구성
        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Children =
                {
                    CreateGlassCard("Sales Report"),
                    CreateGlassCard("Inventory Status"),
                    CreateGlassCard("Customer Analytics")
                }
            }
        };
    }
    
    private View CreateGlassCard(string title)
    {
        var card = new View
        {
            Size = new Size(300, 200),
            BackgroundColor = new Color(0.95f, 0.95f, 0.95f, 0.8f)
        };
        
        // Glassmorphism 효과
        var blurEffect = RenderEffect.CreateBackgroundBlurEffect(10.0f);
        card.SetRenderEffect(blurEffect);
        
        // Neumorphism 그림자
        card.AddShadow(new InnerShadow
        {
            Color = new Color(1, 1, 1, 0.6f),
            Offset = new Vector2(-2, -2),
            BlurRadius = 4.0f
        });
        
        card.AddShadow(new Shadow
        {
            Color = new Color(0, 0, 0, 0.2f),
            Offset = new Vector2(3, 3),
            BlurRadius = 6.0f
        });
        
        return card;
    }
}
```

#### 데이터 시각화

```csharp
// 데이터 시각화 애플리케이션
public class DataVisualizationPage : BasePage
{
    protected override async void OnCreate()
    {
        base.OnCreate();
        
        // 비동기 데이터 로딩과 이펙트 적용
        var data = await LoadChartDataAsync();
        
        var chartContainer = new View { Size = new Size(400, 300) };
        
        // 동적 이펙트 생성
        var effects = await Task.WhenAll(
            Task.Run(() => RenderEffect.CreateBackgroundBlurEffect(8.0f)),
            Task.Run(() => CreateMaskEffectForChart())
        );
        
        chartContainer.SetRenderEffect(effects[0]);
        
        Content = chartContainer;
    }
    
    private async Task<MaskEffect> CreateMaskEffectForChart()
    {
        // 복잡한 마스크 생성
        return await Task.Run(() =>
        {
            var maskView = new View { Size = new Size(400, 300) };
            // 마스크 설정...
            return RenderEffect.CreateMaskEffect(maskView);
        });
    }
}
```

## 8. 마이그레이션 가이드

### 8.1 DALi → NUI 마이그레이션

#### 기본 이펙트 마이그레이션

```cpp
// DALi C++ 코드
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
maskEffect.SetProperty(Toolkit::MaskEffect::Property::MASK_MODE, 
                       Toolkit::MaskEffect::ALPHA);
control.SetRenderEffect(maskEffect);
```

```csharp
// NUI C# 코드
var maskEffect = RenderEffect.CreateMaskEffect(maskView, MaskEffectMode.Alpha);
view.SetRenderEffect(maskEffect);
```

#### Shadow 시스템 마이그레이션

```cpp
// DALi C++ 코드
Toolkit::Shadow shadow = Toolkit::Shadow::New();
shadow.SetProperty(Toolkit::Shadow::Property::COLOR, Color::BLACK);
shadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(5.0f, 5.0f));
shadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 10.0f);
control.AddShadow(shadow);
```

```csharp
// NUI C# 코드
var shadow = new Shadow
{
    Color = Color.Black,
    Offset = new Vector2(5.0f, 5.0f),
    BlurRadius = 10.0f
};
view.AddShadow(shadow);
```

#### 복잡한 이펙트 마이그레이션

```cpp
// DALi C++ - Glassmorphism
Toolkit::Control glassPanel = Toolkit::Control::New();
glassPanel.SetProperty(Actor::Property::SIZE, Vector2(300, 200));

Toolkit::BackgroundBlurEffect blurEffect = 
    Toolkit::BackgroundBlurEffect::New(10.0f);
glassPanel.SetRenderEffect(blurEffect);

Property::Map backgroundMap;
backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                     Vector4(1.0f, 1.0f, 1.0f, 0.3f));
glassPanel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);
```

```csharp
// NUI C# - Glassmorphism
var glassPanel = new View
{
    Size = new Size(300, 200),
    BackgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.3f)
};

var blurEffect = RenderEffect.CreateBackgroundBlurEffect(10.0f);
glassPanel.SetRenderEffect(blurEffect);
```

### 8.2 마이그레이션 체크리스트

#### 코드 변환

- [ ] `Toolkit::` → `Tizen.NUI` 네임스페이스 변경
- [ ] `Control` → `View` 클래스명 변경
- [ ] `Property::Index` → C# 프로퍼티로 변환
- [ ] `Vector2`, `Vector3`, `Vector4` → `Vector2`, `Vector3`, `Vector4` (동일)
- [ ] `Color` → `Color` (동일)
- [ ] 팩토리 메서드 호출 방식 변경

#### 메모리 관리

- [ ] 수동 메모리 관리 → GC 기반 관리로 전환
- [ ] `using` 블록 또는 `Dispose()` 패턴 적용
- [ ] 순환 참조 방지를 위한 `WeakReference` 이해

#### 성능 최적화

- [ ] P/Invoke 오버헤드 최소화
- [ ] 객체 풀링 구현
- [ ] 비동기 패턴 적용

## 9. 상호 운용성

### 9.1 혼합 사용 시나리오

#### DALi 코어 + NUI UI

```cpp
// C++ 코어 엔진
class CoreEngine
{
public:
    void Initialize()
    {
        // DALi 코어 초기화
        mStage = Dali::Stage::GetCurrent();
        
        // NUI와 공유할 리소스 준비
        PrepareSharedResources();
    }
    
    Dali::Texture GetSharedTexture()
    {
        return mSharedTexture;
    }
    
private:
    Dali::Stage mStage;
    Dali::Texture mSharedTexture;
};
```

```csharp
// C# UI 레이어
public class HybridUI : BasePage
{
    private CoreEngineProxy mCoreEngine;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        
        // C++ 코어 엔진 연결
        mCoreEngine = new CoreEngineProxy();
        
        // 공유 텍스처 사용
        var sharedTexture = mCoreEngine.GetSharedTexture();
        var imageView = new ImageView
        {
            ResourceUrl = sharedTexture.Url
        };
        
        // NUI 이펙트 적용
        var blurEffect = RenderEffect.CreateBackgroundBlurEffect(5.0f);
        imageView.SetRenderEffect(blurEffect);
        
        Content = imageView;
    }
}
```

### 9.2 데이터 교환

#### C++ → C# 데이터 전달

```cpp
// C++에서 데이터 제공
extern "C" DLL_API void* CreateMaskEffect(void* maskControl)
{
    Toolkit::Control* control = static_cast<Toolkit::Control*>(maskControl);
    Toolkit::MaskEffect* effect = new Toolkit::MaskEffect(Toolkit::MaskEffect::New(*control));
    return effect;
}

extern "C" DLL_API void ApplyEffect(void* control, void* effect)
{
    Toolkit::Control* ctrl = static_cast<Toolkit::Control*>(control);
    Toolkit::RenderEffect* eff = static_cast<Toolkit::RenderEffect*>(effect);
    ctrl->SetRenderEffect(*eff);
}
```

```csharp
// C#에서 C++ 함수 호출
public class NativeEffects
{
    [DllImport("native-effects.dll")]
    private static extern IntPtr CreateMaskEffect(IntPtr maskControl);
    
    [DllImport("native-effects.dll")]
    private static extern void ApplyEffect(IntPtr control, IntPtr effect);
    
    public static void ApplyNativeMaskEffect(View view, View maskView)
    {
        var effectPtr = CreateMaskEffect(maskView.SwigCPtr);
        ApplyEffect(view.SwigCPtr, effectPtr);
    }
}
```

## 10. 결론 및 권장 사항

### 10.1 선택 가이드

| 시나리오 | 권장 프레임워크 | 이유 |
|----------|------------------|------|
| **고성능 게임** | DALi | 최고 성능, 저수준 제어 |
| **임베디드 시스템** | DALi | 메모리 효율성, 작은 풋프린트 |
| **비즈니스 앱** | NUI | 빠른 개발, 높은 생산성 |
| **데이터 시각화** | NUI | LINQ, async/await 지원 |
| **프로토타이핑** | NUI | 빠른 개발 사이클 |
| **실시간 처리** | DALi | 낮은 레이턴시 |

### 10.2 최적의 하이브리드 접근

```csharp
// 권장되는 하이브리드 아키텍처
public class OptimizedVisualEffects
{
    // 성능이 중요한 코어는 C++로
    private readonly NativeEffectCore m_nativeCore;
    
    // UI 로직은 C#으로
    public async Task ApplyOptimizedEffectsAsync(View view)
    {
        // C++ 코어에서 미리 최적화된 이펙트 가져오기
        var nativeEffect = await m_nativeCore.GetPrecompiledEffectAsync("glassmorphism");
        
        // C#에서 적용
        view.SetRenderEffect(nativeEffect);
        
        // C#에서 추가적인 UI 효과
        view.AddShadow(CreateNeumorphismShadow());
    }
    
    private Shadow CreateNeumorphismShadow()
    {
        return new Shadow
        {
            Color = new Color(0, 0, 0, 0.2f),
            Offset = new Vector2(3, 3),
            BlurRadius = 6.0f
        };
    }
}
```

이 비교 문서는 DALi와 NUI Visual Effects의 차이점을 상세히 분석하며, 개발자가 프로젝트 요구사항에 맞는 적절한 프레임워크를 선택하고 최적화하는 데 도움을 제공합니다.
