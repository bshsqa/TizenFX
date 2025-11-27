# DALi Scene3D vs NUI Scene3D 비교 분석

## 1. 개요

### 1.1 두 프레임워크의 관계

DALi Scene3D와 NUI Scene3D는 동일한 코어 기능을 공유하지만, 다른 개발 환경과 사용 패턴을 target으로 하는 두 개의 별도 API 레이어입니다.

- **DALi Scene3D**: C++ 기반의 네이티브 API
- **NUI Scene3D**: C# 기반의 .NET API 래퍼

### 1.2 아키텍처 관계

```
┌─────────────────────────────────────────┐
│           C# Application Layer          │
├─────────────────────────────────────────┤
│         NUI Scene3D (C# Wrapper)        │
├─────────────────────────────────────────┤
│      C# Binding Layer (P/Invoke)        │
├─────────────────────────────────────────┤
│        DALi Scene3D (C++ Core)          │
├─────────────────────────────────────────┤
│         DALi Core (C++ Framework)       │
├─────────────────────────────────────────┤
│           System Layer (Native)         │
└─────────────────────────────────────────┘
```

## 2. 핵심 차이점

### 2.1 언어 및 플랫폼

| 항목 | DALi Scene3D | NUI Scene3D |
|------|--------------|-------------|
| 언어 | C++ | C# |
| 플랫폼 | Tizen Native | Tizen .NET (Xamarin) |
| 컴파일 | 네이티브 컴파일 | .NET IL → AOT 컴파일 |
| 메모리 관리 | 수동 (RAII) | 자동 (Garbage Collection) |
| 성능 | 최고 성능 | 약간의 오버헤드 있음 |

### 2.2 API 설계 철학

#### DALi Scene3D
- **성능 중심**: 최소한의 오버헤드로 직접적인 메모리 접근
- **제어 중심**: 세밀한 리소스 관리와 생명주기 제어
- **C++ 관용성**: STL 컨테이너, RAII, 스마트 포인터 활용

#### NUI Scene3D
- **생산성 중심**: 쉬운 사용과 빠른 개발
- **안전성 중심**: 타입 안전성과 예외 처리
- **.NET 관용성**: LINQ, async/await, 이벤트 기반 프로그래밍

## 3. API 비교 분석

### 3.1 SceneView 비교

#### DALi C++ API
```cpp
// SceneView 생성
SceneView sceneView = SceneView::New();
sceneView.SetProperty(Actor::Property::SIZE, Vector2(400, 400));

// 카메라 관리
CameraActor camera = CameraActor::New();
sceneView.AddCamera(camera);
sceneView.SelectCamera(0);

// IBL 설정
sceneView.SetImageBasedLightSource("diffuse.ktx", "specular.ktx", 1.0f);

// 시그널 연결
sceneView.CaptureFinishedSignal().Connect(this, &MyClass::OnCaptureFinished);
```

#### NUI C# API
```csharp
// SceneView 생성
SceneView sceneView = new SceneView();
sceneView.Size = new Size(400, 400);

// 카메라 관리
Camera camera = new Camera();
sceneView.AddCamera(camera);
sceneView.SelectCamera(0);

// IBL 설정
sceneView.SetImageBasedLightSource("diffuse.ktx", "specular.ktx", 1.0f);

// 이벤트 연결
sceneView.CaptureFinished += OnCaptureFinished;

// 비동기 캡처
Task<ImageUrl> captureTask = sceneView.CaptureAsync(camera, new Vector2(1920, 1080));
```

### 3.2 Model 비교

#### DALi C++ API
```cpp
// 모델 로딩
Model model = Model::New("model.gltf", "resources/");

// 리소스 상태 확인
Model::ResourceStatus status = model.GetModelResourceStatus();

// 애니메이션
if (model.GetAnimationCount() > 0) {
    Animation anim = model.GetAnimation(0);
    anim.Play();
}

// 시그널
model.LoadCompletedSignal().Connect(this, &MyClass::OnModelLoaded);
model.MeshHitSignal().Connect(this, &MyClass::OnMeshHit);
```

#### NUI C# API
```csharp
// 모델 로딩
Model model = new Model("model.gltf", "resources/");

// 리소스 상태 확인
Model.ResourceStatus status = model.GetModelResourceStatus();

// 애니메이션
if (model.GetAnimationCount() > 0) {
    Animation anim = model.GetAnimation(0);
    anim.Play();
}

// 이벤트
model.LoadCompleted += OnModelLoaded;
model.MeshHit += OnMeshHit;
```

### 3.3 Light 비교

#### DALi C++ API
```cpp
// 광원 생성
Light light = Light::New();
light.SetProperty(Actor::Property::COLOR, Color::WHITE);
light.Enable(true);
light.EnableShadow(true);

// 섀도우 설정
light.SetShadowIntensity(0.5f);
light.SetShadowBias(0.001f);
light.EnableShadowSoftFiltering(true);
```

#### NUI C# API
```csharp
// 광원 생성
Light light = new Light();
light.Color = Color.White;
light.Enable(true);
light.EnableShadow(true);

// 섀도우 설정
light.ShadowIntensity = 0.5f;
light.ShadowBias = 0.001f;
light.EnableShadowSoftFiltering(true);
```

## 4. 바인딩 레이어 상세 분석

### 4.1 P/Invoke 기반 바인딩

NUI Scene3D는 P/Invoke(Platform Invoke)를 통해 C++ DALi API를 호출합니다.

```cpp
// C++ 바인딩 함수 (scene-view-wrap.cpp)
DALI_EXPORT_API void scene_view_add_camera(Dali_Handle sceneView, Dali_Handle camera)
{
    SceneView* scene = reinterpret_cast<SceneView*>(sceneView);
    CameraActor* cam = reinterpret_cast<CameraActor*>(camera);
    
    if (scene && cam) {
        scene->AddCamera(*cam);
    }
}
```

```csharp
// C# 래퍼 (SceneView.cs)
[DllImport("dali-scene3d.dll", CallingConvention = CallingConvention.Cdecl)]
internal static extern void scene_view_add_camera(IntPtr sceneView, IntPtr camera);

public void AddCamera(Camera camera)
{
    if (camera != null) {
        Interop.SceneView.AddCamera(SwigCPtr, camera.SwigCPtr);
    }
}
```

### 4.2 메모리 관리 차이

#### DALi C++ 메모리 관리
```cpp
// RAII를 통한 자동 메모리 관리
{
    SceneView sceneView = SceneView::New(); // 자원 할당
    Model model = Model::New("model.gltf"); // 자원 할당
    
    sceneView.Add(model);
    
} // 스코프 종료 시 자동으로 자원 해제
```

#### NUI C# 메모리 관리
```csharp
// Garbage Collector에 의한 자동 메모리 관리
void CreateScene()
{
    SceneView sceneView = new SceneView(); // 객체 생성
    Model model = new Model("model.gltf"); // 객체 생성
    
    sceneView.Add(model);
    
    // GC가 적절한 시점에 메모리 해제
}

// 명시적 해제도 가능
void Cleanup()
{
    sceneView?.Dispose();
    model?.Dispose();
}
```

## 5. 성능 비교

### 5.1 렌더링 성능

| 항목 | DALi Scene3D | NUI Scene3D | 차이 |
|------|--------------|-------------|------|
| 렌더링 속도 | 100% | 95-98% | 2-5% 느림 |
| 메모리 사용량 | 기준 | +10-15% | 약간 더 많음 |
| 초기 로딩 | 빠름 | 약간 느림 | .NET 런타임 로딩 |

### 5.2 API 호출 오버헤드

```cpp
// DALi: 직접 호출
sceneView.SetProperty(Actor::Property::SIZE, Vector2(400, 400));
// 오버헤드: ~0.001ms
```

```csharp
// NUI: P/Invoke 호출
sceneView.Size = new Size(400, 400);
// 오버헤드: ~0.01-0.02ms (10-20배 더 느림)
```

### 5.3 성능 최적화 전략

#### NUI에서의 성능 최적화
```csharp
// 1. 배치 호출 사용
public void OptimizeSceneView()
{
    // 여러 속성을 한 번에 설정
    sceneView.Size = new Size(400, 400);
    sceneView.Position = new Position(0, 0);
    sceneView.UseFramebuffer = true;
}

// 2. 비동기 작업 활용
public async Task LoadModelAsync()
{
    var model = new Model("complex_model.gltf");
    
    // 로딩 완료까지 기다리지 않고 다른 작업 수행
    await Task.Run(() => {
        // 백그라운드에서 리소스 로딩
    });
}

// 3. 객체 풀링
private readonly Queue<Model> modelPool = new Queue<Model>();

public Model GetPooledModel()
{
    if (modelPool.Count > 0) {
        return modelPool.Dequeue();
    }
    return new Model();
}
```

## 6. 기능 차이점

### 6.1 NUI 전용 기능

#### 6.1.1 비동기 프로그래밍 지원
```csharp
// 비동기 캡처
public async Task CaptureSceneAsync()
{
    try {
        ImageUrl capturedImage = await sceneView.CaptureAsync(camera, new Vector2(1920, 1080));
        // 캡처된 이미지 처리
        ProcessCapturedImage(capturedImage);
    } catch (Exception ex) {
        // 에러 처리
        HandleCaptureError(ex);
    }
}
```

#### 6.1.2 .NET 이벤트 시스템
```csharp
// 이벤트 기반 프로그래밍
sceneView.CameraTransitionFinished += (sender, e) => {
    // 카메라 전환 완료 처리
    UpdateUI();
};

sceneView.CaptureFinished += (sender, e) => {
    if (e.CapturedImageUrl != null) {
        // 캡처 성공 처리
        ShowCapturedImage(e.CapturedImageUrl);
    }
};
```

#### 6.1.3 속성 접근자
```csharp
// 프로퍼티를 통한 직관적인 접근
public class SceneView : View
{
    public float ImageBasedLightScaleFactor
    {
        get => GetImageBasedLightScaleFactor();
        set => SetImageBasedLightScaleFactor(value);
    }
    
    public bool UseFramebuffer
    {
        get => IsUsingFramebuffer();
        set => SetUseFramebuffer(value);
    }
    
    public uint FramebufferMultiSamplingLevel
    {
        get => GetFramebufferMultiSamplingLevel();
        set => SetFramebufferMultiSamplingLevel(value);
    }
}
```

### 6.2 DALi 전용 기능

#### 6.2.1 저수준 메모리 제어
```cpp
// 직접적인 메모리 관리
class CustomModel : public Model
{
public:
    CustomModel() {
        // 커스텀 메모리 풀 사용
        mCustomAllocator = std::make_unique<CustomAllocator>();
    }
    
    ~CustomModel() {
        // 명시적 리소스 해제
        mCustomAllocator->ReleaseAll();
    }
    
private:
    std::unique_ptr<CustomAllocator> mCustomAllocator;
};
```

#### 6.2.2 템플릿 기반 프로그래밍
```cpp
// 템플릿을 통한 타입 안전한 프로그래밍
template<typename T>
class Scene3DComponent : public Control
{
public:
    void SetCustomProperty(const std::string& name, const T& value) {
        mProperties[name] = value;
    }
    
    T GetCustomProperty(const std::string& name) const {
        auto it = mProperties.find(name);
        return (it != mProperties.end()) ? it->second : T{};
    }
    
private:
    std::unordered_map<std::string, T> mProperties;
};
```

## 7. 사용 사례별 권장 사항

### 7.1 DALi Scene3D가 적합한 경우

#### 7.1.1 고성능 애플리케이션
- **3D 게임**: 60FPS 이상의 고주사율이 필요한 경우
- **실시간 렌더링**: AR/VR 애플리케이션
- **대규모 시뮬레이션**: 수많은 3D 오브젝트를 동시에 처리

```cpp
// 고성능 게임 루프 예시
void GameLoop()
{
    while (mRunning) {
        // 최소한의 오버헤드로 렌더링
        mSceneView.Render();
        
        // 직접적인 메모리 관리
        ProcessGameObjects();
        
        // 정밀한 타이밍 제어
        mTimer.Update();
    }
}
```

#### 7.1.2 임베디드 시스템
- **저사양 기기**: 메모리와 CPU가 제한적인 환경
- **실시간 시스템**: 결정론적인 동작이 요구되는 시스템

### 7.2 NUI Scene3D가 적합한 경우

#### 7.2.1 비즈니스 애플리케이션
- **3D 제품 뷰어**: 쇼핑몰, 전자 카탈로그
- **데이터 시각화**: 3D 차트, 그래프
- **교육용 앱**: 3D 모델을 활용한 학습 자료

```csharp
// 비즈니스 애플리케이션 예시
public class ProductViewer : ContentPage
{
    public ProductViewer()
    {
        var sceneView = new SceneView();
        var model = new Model("product.gltf");
        
        sceneView.AddCamera(new Camera());
        sceneView.Add(model);
        
        // 비동기 로딩으로 UX 개선
        LoadModelAsync(model);
        
        Content = sceneView;
    }
    
    private async Task LoadModelAsync(Model model)
    {
        // 로딩 인디케이터 표시
        ShowLoadingIndicator();
        
        try {
            await Task.Run(() => {
                // 백그라운드 로딩
                model.LoadResources();
            });
            
            HideLoadingIndicator();
        } catch (Exception ex) {
            ShowError(ex.Message);
        }
    }
}
```

#### 7.2.2 프로토타이핑 및 개발
- **빠른 개발**: 생산성이 중요한 초기 개발 단계
- **UI 중심 앱**: 3D가 부가적인 기능인 경우

## 8. 마이그레이션 가이드

### 8.1 DALi → NUI 마이그레이션

#### 8.1.1 기본적인 변환 패턴

```cpp
// DALi C++
SceneView sceneView = SceneView::New();
sceneView.SetProperty(Actor::Property::SIZE, Vector2(400, 400));
sceneView.SetProperty(Actor::Property::POSITION, Vector3(0, 0, 0));

CameraActor camera = CameraActor::New();
sceneView.AddCamera(camera);
sceneView.SelectCamera(0);

Model model = Model::New("model.gltf", "resources/");
sceneView.Add(model);

sceneView.CaptureFinishedSignal().Connect(this, &MyClass::OnCaptureFinished);
```

```csharp
// NUI C#
SceneView sceneView = new SceneView();
sceneView.Size = new Size(400, 400);
sceneView.Position = new Position(0, 0);

Camera camera = new Camera();
sceneView.AddCamera(camera);
sceneView.SelectCamera(0);

Model model = new Model("model.gltf", "resources/");
sceneView.Add(model);

sceneView.CaptureFinished += OnCaptureFinished;
```

#### 8.1.2 시그널 → 이벤트 변환

```cpp
// DALi 시그널
class MyClass
{
public:
    MyClass() {
        mSceneView.CaptureFinishedSignal().Connect(this, &MyClass::OnCaptureFinished);
    }
    
    void OnCaptureFinished(SceneView sceneView, int32_t captureId, const ImageUrl& imageUrl) {
        // 처리 로직
    }
    
private:
    SceneView mSceneView;
};
```

```csharp
// NUI 이벤트
class MyClass
{
    public MyClass()
    {
        sceneView.CaptureFinished += OnCaptureFinished;
    }
    
    private void OnCaptureFinished(object sender, CaptureFinishedEventArgs e)
    {
        // 처리 로직
        int captureId = e.CaptureId;
        ImageUrl imageUrl = e.CapturedImageUrl;
    }
    
    private SceneView sceneView;
}
```

### 8.2 NUI → DALi 마이그레이션

#### 8.2.1 비동기 → 동기 변환

```csharp
// NUI 비동기
public async Task LoadModelAsync()
{
    Model model = new Model("model.gltf");
    
    await Task.Run(() => {
        // 로딩 작업
    });
    
    sceneView.Add(model);
}
```

```cpp
// DALi 동기
void LoadModel()
{
    Model model = Model::New("model.gltf");
    
    // 동기 로딩
    model.LoadCompletedSignal().Connect(this, &MyClass::OnModelLoaded);
    
    sceneView.Add(model);
}

void MyClass::OnModelLoaded(Model model, bool success)
{
    // 로딩 완료 처리
}
```

## 9. 상호 운용성

### 9.1 혼합 사용 시나리오

#### 9.1.1 C# 앱에서 C++ 컴포넌트 사용

```csharp
// C#에서 C++ 라이브러리 직접 호출
[DllImport("CustomScene3D.dll", CallingConvention = CallingConvention.Cdecl)]
internal static extern IntPtr CreateCustomModel(string modelPath);

[DllImport("CustomScene3D.dll", CallingConvention = CallingConvention.Cdecl)]
internal static extern void ProcessCustomModel(IntPtr model);

public class AdvancedModel
{
    private IntPtr nativeModel;
    
    public AdvancedModel(string modelPath)
    {
        nativeModel = CreateCustomModel(modelPath);
    }
    
    public void Process()
    {
        ProcessCustomModel(nativeModel);
    }
}
```

### 9.2 데이터 공유

#### 9.2.1 공통 파일 포맷 사용

```csharp
// C#에서 모델 저장
public void SaveModel(Model model, string filePath)
{
    // 공통 포맷으로 저장 (glTF)
    model.SaveAsGLTF(filePath);
}
```

```cpp
// C++에서 모델 로드
void LoadSharedModel(const std::string& filePath)
{
    Model model = Model::New(filePath);
    // 공유 모델 사용
}
```

## 10. 모범 사례 및 권장 사항

### 10.1 선택 가이드

| 요구사항 | DALi Scene3D | NUI Scene3D |
|----------|--------------|-------------|
| 최고 성능 | ✅ 추천 | ⚠️ 제한적 |
| 빠른 개발 | ⚠️ 복잡함 | ✅ 추천 |
| 메모리 제약 | ✅ 추천 | ⚠️ 주의 필요 |
| 팀 생산성 | ⚠️ C++ 전문가 필요 | ✅ 쉬운 학습 곡선 |
| 유지보수 | ⚠️ 복잡함 | ✅ 상대적 용이 |
| .NET 생태계 | ❌ 불가능 | ✅ 완벽 지원 |

### 10.2 성능 최적화 팁

#### DALi 최적화
```cpp
// 1. 메모리 풀 사용
class MemoryPool
{
public:
    static Geometry* AcquireGeometry() {
        if (!mPool.empty()) {
            auto* geom = mPool.back();
            mPool.pop_back();
            return geom;
        }
        return new Geometry();
    }
    
    static void ReleaseGeometry(Geometry* geom) {
        geom->Reset();
        mPool.push_back(geom);
    }
};

// 2. 렌더링 배치
void OptimizeRendering()
{
    // 동일한 머티리얼을 가진 오브젝트 그룹화
    GroupByMaterial(mRenderables);
    
    // 드로우 콜 최소화
    MinimizeDrawCalls();
}
```

#### NUI 최적화
```csharp
// 1. 비동기 패턴 활용
public async Task InitializeSceneAsync()
{
    // 병렬 리소스 로딩
    var modelTask = LoadModelAsync("model.gltf");
    var textureTask = LoadTextureAsync("texture.jpg");
    
    await Task.WhenAll(modelTask, textureTask);
    
    // 로딩 완료 후 씬 설정
    sceneView.Add(modelTask.Result);
}

// 2. 객체 풀링
public class ModelPool
{
    private readonly ConcurrentQueue<Model> pool = new();
    
    public Model GetModel()
    {
        return pool.TryDequeue(out var model) ? model : new Model();
    }
    
    public void ReturnModel(Model model)
    {
        model.Reset();
        pool.Enqueue(model);
    }
}
```

## 11. 결론

DALi Scene3D와 NUI Scene3D는 각각의 장단점을 가지며, 프로젝트의 요구사항에 따라 적절한 선택이 필요합니다.

### 11.1 핵심 요약

1. **성능**: DALi가 약 2-5% 더 빠름
2. **생산성**: NUI가 개발 속도와 유지보수 면에서 우월
3. **생태계**: NUI가 .NET 생태계를 완벽히 지원
4. **제어**: DALi가 저수준 제어 기능 제공

### 11.2 최종 권장사항

- **고성능 3D 애플리케이션**: DALi Scene3D
- **비즈니스/엔터프라이즈 앱**: NUI Scene3D
- **프로토타이핑**: NUI Scene3D로 시작 후 필요시 DALi로 전환
- **팀 역량**: C++ 전문가 팀 → DALi, C# 팀 → NUI

두 프레임워크 모두 동일한 코어를 공유하므로, 프로젝트의 중간에 전환하는 것도 가능하며, 실제 개발에서는 프로젝트의 특성과 팀의 역량을 고려하여 최적의 선택을 하는 것이 중요합니다.
