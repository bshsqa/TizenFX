# Visual Effects 아키텍처

## 1. 아키텍처 설계 원칙

### 1.1 핵심 설계 목표

Visual Effects 아키텍처는 다음과 같은 핵심 목표를 기반으로 설계되었습니다:

1. **확장성**: 새로운 비주얼 이펙트를 쉽게 추가할 수 있는 모듈러 구조
2. **성능**: 오프스크린 렌더링과 RenderTask 최적화를 통한 효율적인 처리
3. **통합성**: 기존 DALi Control 시스템과의 완벽한 통합
4. **유연성**: 다양한 이펙트의 조합과 체이닝 지원
5. **호환성**: 기존 DALi 시스템과의 하위 호환성 유지

### 1.2 아키텍처 패턴

Visual Effects 시스템은 다음과 같은 디자인 패턴을 사용합니다:

- **Strategy Pattern**: RenderEffect를 통한 이펙트 전략의 캡슐화
- **Factory Pattern**: 각 이펙트의 정적 팩토리 메서드
- **Observer Pattern**: Control과 이펙트 간의 생명주기 동기화
- **Composite Pattern**: 다중 Shadow와 같은 복합 이펙트 지원

## 2. RenderEffect 상세 분석

### 2.1 클래스 계층 구조

```cpp
// 기본 인터페이스
class RenderEffect : public BaseHandle
{
public:
    virtual void Activate() = 0;
    virtual void Deactivate() = 0;
    virtual void Refresh() = 0;
    virtual bool IsActivated() const = 0;
    virtual OffScreenRenderableType GetOffScreenRenderableType() const = 0;
    
protected:
    virtual void SetOwnerControl(Toolkit::Control control) = 0;
    virtual void ClearOwnerControl() = 0;
};

// 구체적인 구현
class MaskEffect : public RenderEffect
{
    // MaskEffect 특정 구현
};

class BackgroundBlurEffect : public RenderEffect
{
    // BackgroundBlurEffect 특정 구현
};

class GaussianBlurEffect : public RenderEffect
{
    // GaussianBlurEffect 특정 구현
};
```

### 2.2 내부 구현 아키텍처

#### RenderEffectImpl 기반 클래스

```cpp
namespace Dali::Internal::Toolkit
{
class RenderEffectImpl : public RefObject
{
public:
    // 생명주기 관리
    virtual void Initialize() {}
    virtual void Activate() {}
    virtual void Deactivate() {}
    virtual void Refresh() {}
    
    // 오프스크린 렌더링
    virtual OffScreenRenderableType GetOffScreenRenderableType() const { return OffScreenRenderableType::NONE; }
    virtual void GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward) {}
    
    // 소유자 관리
    void SetOwnerControl(Toolkit::Control control);
    void ClearOwnerControl();
    
protected:
    Toolkit::Control mOwnerControl;
    bool mIsActivated = false;
};
}
```

### 2.3 생명주기 관리

#### 이펙트 생성 단계

```cpp
// 1. 팩토리 메서드 호출
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);

// 내부적으로:
// - MaskEffectImpl 생성
// - 초기화 파라미터 설정
// - 핸들-구현 객체 연결
```

#### Control 적용 단계

```cpp
// 2. Control에 적용
control.SetRenderEffect(maskEffect);

// Control::SetRenderEffect() 내부:
void Control::SetRenderEffect(Toolkit::RenderEffect effect)
{
    // 기존 이펙트 정리
    ClearRenderEffect();
    
    if(effect)
    {
        // 이펙트 구현 객체 가져오기
        Internal::RenderEffectImpl* impl = dynamic_cast<Internal::RenderEffectImpl*>(effect.GetObjectPtr());
        
        // 오프스크린 렌더링 타입 등록
        RegisterOffScreenRenderableType(impl->GetOffScreenRenderableType());
        
        // 소유자 설정
        impl->SetOwnerControl(Toolkit::Control(GetOwner()));
        
        // 내부 저장
        mImpl->mRenderEffect = impl;
        
        // 이펙트 활성화
        impl->Activate();
    }
}
```

#### 이펙트 제거 단계

```cpp
// 3. Control에서 제거
control.ClearRenderEffect();

// Control::ClearRenderEffect() 내부:
void Control::ClearRenderEffect()
{
    if(mImpl->mRenderEffect)
    {
        // 이펙트 구현 객체 이동
        RenderEffectImplPtr effectImpl = std::move(mImpl->mRenderEffect);
        
        // 핸들 초기화 (순환 참조 방지)
        mImpl->mRenderEffect.Reset();
        
        // 오프스크린 렌더링 타입 해제
        UnregisterOffScreenRenderableType(effectImpl->GetOffScreenRenderableType());
        
        // 소유자 정리
        effectImpl->ClearOwnerControl();
        
        // 이펙트 비활성화
        effectImpl->Deactivate();
    }
}
```

## 3. 오프스크린 렌더링 시스템

### 3.1 오프스크린 렌더링 타입

```cpp
enum class OffScreenRenderableType
{
    NONE = 0,      // 오프스크린 렌더링 사용 안 함
    BACKWARD = 1,  // 역방향: Control을 먼저 렌더링 후 이펙트 적용
    FORWARD = 2    // 순방향: 이펙트를 먼저 적용 후 Control 렌더링
};
```

### 3.2 오프스크린 렌더링 흐름

#### BACKWARD 타입 렌더링

```cpp
// MaskEffect의 BACKWARD 렌더링 흐름
void MaskEffectImpl::GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward)
{
    if(!isForward)  // BACKWARD 렌더링
    {
        // 1. 소스 Control을 오프스크린에 렌더링
        Dali::RenderTask sourceTask = CreateSourceRenderTask();
        tasks.push_back(sourceTask);
        
        // 2. 마스크 Control을 오프스크린에 렌더링
        Dali::RenderTask maskTask = CreateMaskRenderTask();
        tasks.push_back(maskTask);
        
        // 3. 최종 합성 렌더링
        Dali::RenderTask compositeTask = CreateCompositeRenderTask();
        tasks.push_back(compositeTask);
    }
}
```

#### FORWARD 타입 렌더링

```cpp
// BackgroundBlurEffect의 FORWARD 렌더링 흐름
void BackgroundBlurEffectImpl::GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward)
{
    if(isForward)  // FORWARD 렌더링
    {
        // 1. 배경을 오프스크린에 캡처
        Dali::RenderTask captureTask = CreateBackgroundCaptureTask();
        tasks.push_back(captureTask);
        
        // 2. 캡처된 배경에 블러 적용
        Dali::RenderTask blurTask = CreateBlurRenderTask();
        tasks.push_back(blurTask);
        
        // 3. 블러된 배경 위에 Control 렌더링
        Dali::RenderTask finalTask = CreateFinalRenderTask();
        tasks.push_back(finalTask);
    }
}
```

### 3.3 Framebuffer 관리

```cpp
class FramebufferManager
{
public:
    static FramebufferManager& Get()
    {
        static FramebufferManager instance;
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
        
        // 새로운 Framebuffer 생성
        Dali::FrameBuffer newFb = Dali::FrameBuffer::New(size.width, size.height);
        mFramebufferPool.push_back({newFb, size, true});
        return newFb;
    }
    
    void ReleaseFramebuffer(Dali::FrameBuffer framebuffer)
    {
        // Framebuffer를 풀로 반환
        for(auto& fb : mFramebufferPool)
        {
            if(fb.framebuffer == framebuffer)
            {
                fb.inUse = false;
                break;
            }
        }
    }
    
private:
    struct FramebufferEntry
    {
        Dali::FrameBuffer framebuffer;
        Vector2 size;
        bool inUse;
    };
    
    std::vector<FramebufferEntry> mFramebufferPool;
};
```

## 4. RenderTask 재정렬 메커니즘

### 4.1 재정렬의 필요성

BackgroundBlurEffect와 같은 FORWARD 타입 이펙트는 올바른 렌더링을 위해 RenderTask의 순서가 중요합니다. 배경이 먼저 렌더링되고, 그 위에 이펙트가 적용된 Control이 렌더링되어야 합니다.

### 4.2 재정렬 알고리즘

```cpp
void RenderTaskList::ReorderTasks(Dali::Internal::LayerList& layerList)
{
    if(mIsRequestedToReorderTask)
    {
        // 1. 오프스크린 렌더러블 데이터 수집
        OffScreenRenderableData renderableData;
        CollectOffScreenRenderables(layerList, renderableData);
        
        // 2. 렌더링 순서 계산
        int32_t orderIndex = GetBaseOrderIndex();
        CalculateRenderingOrder(renderableData, orderIndex);
        
        // 3. RenderTask 정렬
        SortTasks();
    }
    mIsRequestedToReorderTask = false;
}

void CollectOffScreenRenderables(Dali::Internal::LayerList& layerList, 
                                OffScreenRenderableData& renderableData)
{
    // 레이어 순회하며 OffScreenRenderable 찾기
    for(uint32_t i = 0; i < layerList.GetLayerCount(); ++i)
    {
        auto* layer = layerList.GetLayer(i);
        FindOffScreenRenderablesInLayer(layer, renderableData);
    }
}

void CalculateRenderingOrder(OffScreenRenderableData& renderableData, int32_t& orderIndex)
{
    // BACKWARD 타입 먼저 처리 (소스 -> 마스크 -> 합성)
    for(auto& subtree : renderableData)
    {
        for(auto& actor : subtree.second)
        {
            if(actor->GetOffScreenRenderableType() & OffScreenRenderable::Type::BACKWARD)
            {
                std::vector<Dali::RenderTask> tasks;
                actor->GetOffScreenRenderTasks(tasks, false);
                
                for(auto& task : tasks)
                {
                    GetImplementation(task).SetOrderIndex(orderIndex++);
                }
            }
        }
    }
    
    // FORWARD 타입 나중에 처리 (배경 -> 이펙트 -> 최종)
    for(auto& subtree : renderableData)
    {
        if(subtree.first && subtree.first->GetOffScreenRenderableType() & OffScreenRenderable::Type::FORWARD)
        {
            std::vector<Dali::RenderTask> tasks;
            subtree.first->GetOffScreenRenderTasks(tasks, true);
            
            for(auto& task : tasks)
            {
                GetImplementation(task).SetOrderIndex(orderIndex++);
            }
        }
    }
}
```

### 4.3 재정렬 트리거

```cpp
// Control에서 이펙트 설정 시 재정렬 요청
void Control::SetRenderEffect(Toolkit::RenderEffect effect)
{
    // ... 이펙트 설정 로직 ...
    
    // RenderTask 재정렬 요청
    RequestRenderTaskReordering();
}

void Control::RequestRenderTaskReordering()
{
    // Stage를 통해 RenderTaskList에 재정렬 요청
    Dali::Stage stage = Dali::Stage::GetCurrent();
    if(stage)
    {
        Dali::Internal::Stage& stageImpl = Dali::Internal::GetImplementation(stage);
        stageImpl.GetRenderTaskList().RequestReordering();
    }
}
```

## 5. 이펙트 체이닝 및 조합

### 5.1 이펙트 체이닝

여러 이펙트를 체인처럼 연결하여 복합적인 효과를 만들 수 있습니다.

```cpp
class EffectChain
{
public:
    void AddEffect(Toolkit::RenderEffect effect)
    {
        mEffects.push_back(effect);
        UpdateChain();
    }
    
    void RemoveEffect(Toolkit::RenderEffect effect)
    {
        mEffects.erase(std::remove(mEffects.begin(), mEffects.end(), effect), mEffects.end());
        UpdateChain();
    }
    
private:
    void UpdateChain()
    {
        // 체인의 각 이펙트를 순서대로 적용
        for(size_t i = 0; i < mEffects.size(); ++i)
        {
            if(i == 0)
            {
                // 첫 번째 이펙트를 Control에 직접 적용
                mControl.SetRenderEffect(mEffects[i]);
            }
            else
            {
                // 이후 이펙트는 이전 이펙트의 결과에 적용
                ApplyEffectToEffect(mEffects[i-1], mEffects[i]);
            }
        }
    }
    
    std::vector<Toolkit::RenderEffect> mEffects;
    Toolkit::Control mControl;
};
```

### 5.2 이펙트 조합 패턴

```cpp
// Glassmorphism + Neumorphism 조합
Toolkit::Control CreateAdvancedPanel(Vector2 size)
{
    Toolkit::Control panel = Toolkit::Control::New();
    panel.SetProperty(Actor::Property::SIZE, size);
    
    // 1. 배경 블러 (Glassmorphism)
    Toolkit::BackgroundBlurEffect blurEffect = 
        Toolkit::BackgroundBlurEffect::New(10.0f);
    panel.SetRenderEffect(blurEffect);
    
    // 2. 반투명 배경
    Property::Map backgroundMap;
    backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
    backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                         Vector4(1.0f, 1.0f, 1.0f, 0.3f));
    panel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);
    
    // 3. Neumorphism 그림자
    Toolkit::InnerShadow lightShadow = Toolkit::InnerShadow::New();
    lightShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(1, 1, 1, 0.6f));
    lightShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-2, -2));
    lightShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 4.0f);
    panel.AddShadow(lightShadow);
    
    Toolkit::Shadow darkShadow = Toolkit::Shadow::New();
    darkShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.2f));
    darkShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(3, 3));
    darkShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 6.0f);
    panel.AddShadow(darkShadow);
    
    return panel;
}
```

## 6. 메모리 관리 시스템

### 6.1 참조 카운트 관리

```cpp
// RenderEffect의 참조 카운트 기반 메모리 관리
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
    
    int GetRefCount() const
    {
        return mRefCount.load(std::memory_order_relaxed);
    }
};
```

### 6.2 순환 참조 방지

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
    
    Toolkit::Control GetOwnerControl() const
    {
        return mOwnerControl.GetHandle();
    }
};
```

### 6.3 리소스 풀링

```cpp
// 텍스처 및 Framebuffer 풀링
class ResourcePool
{
public:
    static ResourcePool& Get()
    {
        static ResourcePool instance;
        return instance;
    }
    
    Dali::Texture GetTexture(Vector2 size, Pixel::Format format)
    {
        auto key = std::make_pair(size, format);
        
        auto it = mTexturePool.find(key);
        if(it != mTexturePool.end() && !it->second.inUse)
        {
            it->second.inUse = true;
            return it->second.texture;
        }
        
        // 새로운 텍스처 생성
        Dali::Texture newTexture = Dali::Texture::New(Dali::TextureType::TEXTURE_2D, format, size.width, size.height);
        mTexturePool[key] = {newTexture, true};
        return newTexture;
    }
    
    void ReleaseTexture(Dali::Texture texture)
    {
        for(auto& [key, entry] : mTexturePool)
        {
            if(entry.texture == texture)
            {
                entry.inUse = false;
                break;
            }
        }
    }
    
private:
    struct TextureEntry
    {
        Dali::Texture texture;
        bool inUse;
    };
    
    std::map<std::pair<Vector2, Pixel::Format>, TextureEntry> mTexturePool;
};
```

## 7. 커스터마이징 및 확장

### 7.1 새로운 RenderEffect 구현

```cpp
// 커스텀 이펙트 구현 예제
class CustomEffect : public Toolkit::RenderEffect
{
public:
    static CustomEffect New(float parameter)
    {
        Internal::CustomEffectImpl* impl = new Internal::CustomEffectImpl(parameter);
        return CustomEffect(impl);
    }
    
private:
    CustomEffect(Internal::RenderEffectImpl* impl) : RenderEffect(impl) {}
};

namespace Dali::Internal::Toolkit
{
class CustomEffectImpl : public RenderEffectImpl
{
public:
    CustomEffectImpl(float parameter) : mParameter(parameter) {}
    
    void Initialize() override
    {
        // 셰이더 프로그램 컴파일
        SetupShaders();
        
        // 렌더링 타겟 설정
        SetupRenderTargets();
    }
    
    OffScreenRenderableType GetOffScreenRenderableType() const override
    {
        return OffScreenRenderableType::BACKWARD;
    }
    
    void GetOffScreenRenderTasks(std::vector<Dali::RenderTask>& tasks, bool isForward) override
    {
        if(!isForward)
        {
            // 커스텀 렌더링 태스크 생성
            Dali::RenderTask customTask = CreateCustomRenderTask();
            tasks.push_back(customTask);
        }
    }
    
private:
    void SetupShaders()
    {
        // 정점 셰이더
        std::string vertexShader = R"(
            attribute vec2 aPosition;
            attribute vec2 aTexCoord;
            varying vec2 vTexCoord;
            
            void main()
            {
                gl_Position = vec4(aPosition, 0.0, 1.0);
                vTexCoord = aTexCoord;
            }
        )";
        
        // 프래그먼트 셰이더
        std::string fragmentShader = R"(
            precision mediump float;
            varying vec2 vTexCoord;
            uniform sampler2D uTexture;
            uniform float uParameter;
            
            void main()
            {
                vec4 color = texture2D(uTexture, vTexCoord);
                // 커스텀 이펙트 적용
                gl_FragColor = ApplyCustomEffect(color, vTexCoord, uParameter);
            }
        )";
        
        mShader = Dali::Shader::New(vertexShader, fragmentShader);
    }
    
    Dali::RenderTask CreateCustomRenderTask()
    {
        // 커스텀 렌더 태스크 생성 로직
        Dali::RenderTask task = Dali::RenderTask::New();
        
        // 소스 Actor 설정
        task.SetSourceActor(mOwnerControl);
        
        // 셰이더 설정
        Dali::Renderer renderer = CreateCustomRenderer();
        task.SetRenderer(renderer);
        
        // Framebuffer 설정
        Dali::FrameBuffer framebuffer = ResourcePool::Get().GetFramebuffer(GetTargetSize());
        task.SetFrameBuffer(framebuffer);
        
        return task;
    }
    
    float mParameter;
    Dali::Shader mShader;
};
}
```

### 7.2 이펙트 팩토리 확장

```cpp
// 이펙트 팩토리 패턴
class EffectFactory
{
public:
    static EffectFactory& Get()
    {
        static EffectFactory instance;
        return instance;
    }
    
    template<typename EffectType, typename... Args>
    EffectType CreateEffect(Args&&... args)
    {
        return EffectType::New(std::forward<Args>(args)...);
    }
    
    // 등록된 이펙트 생성
    Toolkit::RenderEffect CreateEffect(const std::string& effectName, const Property::Map& parameters)
    {
        auto it = mEffectCreators.find(effectName);
        if(it != mEffectCreators.end())
        {
            return it->second(parameters);
        }
        
        return Toolkit::RenderEffect();
    }
    
    // 이펙트 생성기 등록
    void RegisterEffectCreator(const std::string& name, std::function<Toolkit::RenderEffect(const Property::Map&)> creator)
    {
        mEffectCreators[name] = creator;
    }
    
private:
    std::map<std::string, std::function<Toolkit::RenderEffect(const Property::Map&)>> mEffectCreators;
};

// 팩토리 사용 예제
void InitializeEffectFactory()
{
    auto& factory = EffectFactory::Get();
    
    // 기본 이펙트 등록
    factory.RegisterEffectCreator("MaskEffect", [](const Property::Map& params) {
        auto* maskControl = params["maskControl"].Get<Toolkit::Control*>();
        return Toolkit::MaskEffect::New(*maskControl);
    });
    
    factory.RegisterEffectCreator("BackgroundBlurEffect", [](const Property::Map& params) {
        float blurRadius = params["blurRadius"].Get<float>();
        return Toolkit::BackgroundBlurEffect::New(blurRadius);
    });
    
    // 커스텀 이펙트 등록
    factory.RegisterEffectCreator("CustomEffect", [](const Property::Map& params) {
        float parameter = params["parameter"].Get<float>();
        return CustomEffect::New(parameter);
    });
}
```

## 8. 성능 최적화

### 8.1 렌더링 최적화 전략

#### 렌더 태스크 병합

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
        // 동일한 Framebuffer를 사용하는 태스크 병합
        std::map<Dali::FrameBuffer, std::vector<Dali::RenderTask>> tasksByFramebuffer;
        
        for(const auto& task : tasks)
        {
            Dali::FrameBuffer framebuffer = task.GetFrameBuffer();
            tasksByFramebuffer[framebuffer].push_back(task);
        }
        
        // 병합된 태스크로 교체
        tasks.clear();
        for(const auto& [framebuffer, taskList] : tasksByFramebuffer)
        {
            if(taskList.size() > 1)
            {
                Dali::RenderTask mergedTask = MergeTasks(taskList);
                tasks.push_back(mergedTask);
            }
            else
            {
                tasks.push_back(taskList[0]);
            }
        }
    }
};
```

#### 셰이더 캐싱

```cpp
class ShaderCache
{
public:
    static ShaderCache& Get()
    {
        static ShaderCache instance;
        return instance;
    }
    
    Dali::Shader GetShader(const std::string& vertexSource, const std::string& fragmentSource)
    {
        std::string key = vertexSource + "|" + fragmentSource;
        
        auto it = mShaderCache.find(key);
        if(it != mShaderCache.end())
        {
            return it->second;
        }
        
        // 새로운 셰이더 컴파일
        Dali::Shader shader = Dali::Shader::New(vertexSource, fragmentSource);
        mShaderCache[key] = shader;
        return shader;
    }
    
private:
    std::unordered_map<std::string, Dali::Shader> mShaderCache;
};
```

### 8.2 메모리 최적화

#### LRU 캐시

```cpp
template<typename Key, typename Value>
class LRUCache
{
public:
    LRUCache(size_t capacity) : mCapacity(capacity) {}
    
    Value Get(const Key& key)
    {
        auto it = mCache.find(key);
        if(it != mCache.end())
        {
            // 사용 순서 업데이트
            mUsageOrder.erase(it->second.second);
            mUsageOrder.push_front(key);
            it->second.second = mUsageOrder.begin();
            
            return it->second.first;
        }
        return Value();
    }
    
    void Put(const Key& key, const Value& value)
    {
        auto it = mCache.find(key);
        if(it != mCache.end())
        {
            // 기존 항목 업데이트
            mUsageOrder.erase(it->second.second);
            mCache.erase(it);
        }
        else if(mCache.size() >= mCapacity)
        {
            // 가장 오래된 항목 제거
            Key oldestKey = mUsageOrder.back();
            mUsageOrder.pop_back();
            mCache.erase(oldestKey);
        }
        
        // 새 항목 추가
        mUsageOrder.push_front(key);
        mCache[key] = {value, mUsageOrder.begin()};
    }
    
private:
    size_t mCapacity;
    std::unordered_map<Key, std::pair<Value, std::list<Key>::iterator>> mCache;
    std::list<Key> mUsageOrder;
};
```

## 9. 에러 핸들링 및 디버깅

### 9.1 에러 핸들링

```cpp
class EffectErrorHandler
{
public:
    enum class ErrorType
    {
        SHADER_COMPILATION_FAILED,
        FRAMEBUFFER_CREATION_FAILED,
        TEXTURE_CREATION_FAILED,
        INVALID_PARAMETERS
    };
    
    using ErrorCallback = std::function<void(ErrorType, const std::string&)>;
    
    static EffectErrorHandler& Get()
    {
        static EffectErrorHandler instance;
        return instance;
    }
    
    void SetErrorCallback(ErrorCallback callback)
    {
        mErrorCallback = callback;
    }
    
    void ReportError(ErrorType type, const std::string& message)
    {
        if(mErrorCallback)
        {
            mErrorCallback(type, message);
        }
        
        // 로그 기록
        LogError(type, message);
    }
    
private:
    void LogError(ErrorType type, const std::string& message)
    {
        const char* typeStr = GetErrorTypeString(type);
        DALI_LOG_ERROR("Visual Effects Error [%s]: %s\n", typeStr, message.c_str());
    }
    
    const char* GetErrorTypeString(ErrorType type)
    {
        switch(type)
        {
            case ErrorType::SHADER_COMPILATION_FAILED: return "ShaderCompilation";
            case ErrorType::FRAMEBUFFER_CREATION_FAILED: return "FramebufferCreation";
            case ErrorType::TEXTURE_CREATION_FAILED: return "TextureCreation";
            case ErrorType::INVALID_PARAMETERS: return "InvalidParameters";
            default: return "Unknown";
        }
    }
    
    ErrorCallback mErrorCallback;
};
```

### 9.2 디버깅 도구

```cpp
class EffectDebugger
{
public:
    static EffectDebugger& Get()
    {
        static EffectDebugger instance;
        return instance;
    }
    
    void EnableDebugMode(bool enable)
    {
        mDebugMode = enable;
        if(enable)
        {
            InitializeDebugTools();
        }
    }
    
    void DumpRenderTaskInfo(const std::vector<Dali::RenderTask>& tasks)
    {
        if(!mDebugMode) return;
        
        DALI_LOG_INFO("=== Render Task Dump ===\n");
        for(size_t i = 0; i < tasks.size(); ++i)
        {
            const auto& task = tasks[i];
            DALI_LOG_INFO("Task %zu:\n", i);
            DALI_LOG_info("  Source Actor: %p\n", task.GetSourceActor().GetObjectPtr());
            DALI_LOG_INFO("  Framebuffer: %p\n", task.GetFrameBuffer().GetObjectPtr());
            DALI_LOG_INFO("  Order Index: %d\n", task.GetOrderIndex());
        }
    }
    
    void DumpEffectInfo(Toolkit::RenderEffect effect)
    {
        if(!mDebugMode) return;
        
        DALI_LOG_INFO("=== Render Effect Info ===\n");
        DALI_LOG_INFO("Effect Type: %s\n", GetEffectTypeName(effect));
        DALI_LOG_INFO("Is Activated: %s\n", effect.IsActivated() ? "true" : "false");
        
        auto impl = dynamic_cast<Internal::RenderEffectImpl*>(effect.GetObjectPtr());
        if(impl)
        {
            auto renderType = impl->GetOffScreenRenderableType();
            DALI_LOG_INFO("Render Type: %d\n", static_cast<int>(renderType));
        }
    }
    
private:
    void InitializeDebugTools()
    {
        // 디버그용 셰이더 설정
        SetupDebugShaders();
        
        // 성능 모니터링 시작
        StartPerformanceMonitoring();
    }
    
    bool mDebugMode = false;
};
```

이 아키텍처 문서는 Visual Effects 시스템의 내부 구조와 동작 원리를 상세히 설명하며, 시스템 확장과 최적화를 위한 가이드를 제공합니다.
