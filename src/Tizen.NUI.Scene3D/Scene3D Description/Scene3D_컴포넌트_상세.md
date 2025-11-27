# Scene3D 컨트롤 및 컴포넌트 상세 가이드

## 1. SceneView 컨트롤

### 1.1 개요

SceneView는 3D 씬을 표시하기 위한 메인 컨테이너로, Dali::Toolkit::Control을 상속받습니다. 각 SceneView는 독립적인 3D 공간을 가지며 내부적으로 Layer3D를 통해 깊이 테스트를 수행합니다.

### 1.2 핵심 기능

#### 카메라 관리

SceneView는 여러 카메라를 관리할 수 있으며, 그중 하나를 선택하여 렌더링에 사용합니다.

```cpp
// 카메라 추가
CameraActor camera = CameraActor::New();
sceneView.AddCamera(camera);

// 카메라 선택
sceneView.SelectCamera(0); // 인덱스로 선택
sceneView.SelectCamera("MainCamera"); // 이름으로 선택

// 선택된 카메라 가져오기
CameraActor selectedCamera = sceneView.GetSelectedCamera();
```

**카메라 속성 자동 조정:**
SceneView 크기가 변경되면 카메라의 다음 속성들이 자동으로 조정됩니다:
- AspectRatio
- LeftPlaneDistance, RightPlaneDistance
- TopPlaneDistance, BottomPlaneDistance

#### Image Based Lighting (IBL)

SceneView에 IBL을 설정하면 추가된 모든 Model에 동일한 환경광이 적용됩니다.

```cpp
// IBL 설정
sceneView.SetImageBasedLightSource(
    "diffuse_cube_map.ktx", 
    "specular_cube_map.ktx", 
    1.0f // scaleFactor
);

// IBL 강도 조절
sceneView.SetImageBasedLightScaleFactor(0.8f);
float intensity = sceneView.GetImageBasedLightScaleFactor();
```

**IBL 텍스처 레이아웃 지원:**
- Vertical Cross Layout
- Horizontal Cross Layout  
- Vertical Array Layout
- Horizontal Array Layout
- KTX Cube Map Format

#### 프레임버퍼 모드

```cpp
// 프레임버퍼 사용 설정
sceneView.UseFramebuffer(true);

// 멀티샘플링 레벨 설정
sceneView.SetFramebufferMultiSamplingLevel(4);

// 해상도 수동 설정
sceneView.SetResolution(1920, 1080);

// 해상도 정보 가져오기
uint32_t width = sceneView.GetResolutionWidth();
uint32_t height = sceneView.GetResolutionHeight();
```

**프레임버퍼 모드 비교:**

| 모드 | 장점 | 단점 | 추천 사용 사례 |
|------|------|------|----------------|
| UseFramebuffer(false) | 높은 성능 | 항상 최상위 렌더링 | 단순 3D 뷰어 |
| UseFramebuffer(true) | 렌더링 순서 제어 | 성능 저하 | 복잡한 UI 통합 |

#### 스카이박스

```cpp
// 스카이박스 설정
sceneView.SetSkybox("skybox_cube_map.jpg");

// 스카이박스 환경맵 타입 설정
sceneView.SetSkyboxEnvironmentMapType(EnvironmentMapType::CUBE_MAP);

// 스카이박스 강도 조절
sceneView.SetSkyboxIntensity(1.2f);

// 스카이박스 방향 설정
Quaternion orientation(Radian(Degree(45)), Vector3::YAXIS);
sceneView.SetSkyboxOrientation(orientation);
```

#### 씬 캡처

```cpp
// 씬 캡처 요청
CameraActor captureCamera = sceneView.GetCamera(1);
int32_t captureId = sceneView.Capture(captureCamera, Vector2(1920, 1080));

// 캡처 완료 시그널 연결
sceneView.CaptureFinishedSignal().Connect(this, &MyClass::OnCaptureFinished);
```

#### 카메라 전환 애니메이션

```cpp
// 부드러운 카메라 전환
sceneView.StartCameraTransition(
    1, // 목표 카메라 인덱스
    2.0f, // 지속 시간 (초)
    AlphaFunction::EASE_IN_OUT // 알파 함수
);

// 카메라 전환 완료 시그널
sceneView.CameraTransitionFinishedSignal().Connect(this, &MyClass::OnCameraTransitionFinished);
```

### 1.3 속성

| 속성명 | 타입 | 기본값 | 설명 |
|--------|------|--------|------|
| alphaMaskUrl | STRING | "" | 알파 마스크 이미지 URL |
| maskContentScale | FLOAT | 1.0f | 마스크 적용 전 콘텐츠 스케일 |
| cropToMask | BOOLEAN | true | 마스크 크기에 맞춰 자를지 여부 |

### 1.4 시그널

- **CaptureFinishedSignalType**: 씬 캡처 완료 시 발생
- **CameraTransitionFinishedSignalType**: 카메라 전환 애니메이션 완료 시 발생

## 2. Model 컨트롤

### 2.1 개요

Model은 3D 모델 파일을 로드하고 표시하는 컨트롤입니다. glTF 2.0, USD, DLI 포맷을 지원하며 물리 기반 렌더링과 애니메이션을 지원합니다.

### 2.2 모델 로딩

```cpp
// 기본 모델 로딩
Model model = Model::New("model.gltf", "resources/");

// 빈 모델 생성 (수동으로 모델 노드 추가)
Model emptyModel = Model::New();
```

### 2.3 리소스 상태 관리

```cpp
// 리소스 상태 확인
Model::ResourceStatus status = model.GetModelResourceStatus();

switch(status) {
    case Model::ResourceStatus::PREPARING:
        // 로딩 중
        break;
    case Model::ResourceStatus::READY:
        // 로딩 완료
        break;
    case Model::ResourceStatus::FAILED:
        // 로딩 실패
        break;
}

// 로딩 완료 시그널
model.LoadCompletedSignal().Connect(this, &MyClass::OnModelLoadCompleted);
```

### 2.4 모델 노드 관리

```cpp
// 루트 노드 가져오기
ModelNode root = model.GetModelRoot();

// 자식 노드 추가
ModelNode newNode = ModelNode::New();
model.AddModelNode(newNode);

// 자식 노드 제거
model.RemoveModelNode(newNode);

// 이름으로 노드 찾기
ModelNode foundNode = model.FindChildModelNodeByName("Armature");
```

### 2.5 애니메이션

```cpp
// 애니메이션 개수 확인
uint32_t animCount = model.GetAnimationCount();

// 인덱스로 애니메이션 가져오기
Animation animation = model.GetAnimation(0);

// 이름으로 애니메이션 가져오기
Animation walkAnim = model.GetAnimation("Walk");

// 애니메이션 재생
animation.Play();
animation.SetLooping(true);
```

### 2.6 블렌드셰이프 (Blend Shape)

```cpp
// 블렌드셰이프 이름 목록 가져오기
std::vector<std::string> blendShapeNames;
model.RetrieveBlendShapeNames(blendShapeNames);

// 특정 블렌드셰이프를 가진 노드 찾기
std::vector<ModelNode> nodes;
model.RetrieveModelNodesByBlendShapeName("Smile", nodes);
```

### 2.7 모션 데이터

```cpp
// 모션 데이터로 애니메이션 생성
MotionData motionData = MotionData::New("motion_data.json");
Animation motionAnim = model.GenerateMotionDataAnimation(motionData);

// 모션 데이터 직접 적용
model.SetMotionData(motionData);
```

### 2.8 섀도우 설정

```cpp
// 그림자 투사 설정
model.CastShadow(true);
bool isCasting = model.IsShadowCasting();

// 그림자 수신 설정
model.ReceiveShadow(true);
bool isReceiving = model.IsShadowReceiving();
```

### 2.9 이벤트 처리

```cpp
// 자식 액터의 이벤트 허용 여부
model.SetChildrenSensitive(true);
bool sensitive = model.GetChildrenSensitive();

// 자식 액터의 포커스 허용 여부
model.SetChildrenFocusable(false);
bool focusable = model.GetChildrenFocusable();
```

### 2.10 카메라 생성

```cpp
// 모델에 정의된 카메라 개수
uint32_t cameraCount = model.GetCameraCount();

// 카메라 액터 생성
CameraActor modelCamera = model.GenerateCamera(0);

// 기존 카메라에 파라미터 적용
CameraActor existingCamera = CameraActor::New();
bool success = model.ApplyCamera(0, existingCamera);
```

### 2.11 머티리얼 및 텍스처

```cpp
// IBL 설정
model.SetImageBasedLightSource("diffuse.ktx", "specular.ktx", 1.0f);

// IBL 강도 조절
model.SetImageBasedLightScaleFactor(0.9f);
float scale = model.GetImageBasedLightScaleFactor();
```

### 2.12 시그널

- **MeshHitSignalType**: 메시 충돌 감지 시 발생
- **LoadCompletedSignalType**: 모델 로딩 완료 시 발생

## 3. Light 컨트롤

### 3.1 개요

Light는 3D 씬을 조명하는 광원 컨트롤입니다. 현재 방향성 광원(Directional Light)을 지원하며 실시간 섀도우 생성이 가능합니다.

### 3.2 기본 사용법

```cpp
// 광원 생성
Light light = Light::New();

// 광원 속성 설정
light.SetProperty(Actor::Property::COLOR, Color::WHITE);
light.SetProperty(Actor::Property::POSITION, Vector3(10.0f, 10.0f, 10.0f));

// 광원 방향 설정 (LookAt 사용)
DevelActor::LookAt(light, Vector3(0.0f, 0.0f, 0.0f));

// SceneView에 추가
sceneView.Add(light);
```

### 3.3 광원 활성화

```cpp
// 광원 활성화/비활성화
light.Enable(true);
bool isEnabled = light.IsEnabled();

// 최대 활성 광원 수 확인
uint32_t maxLights = Light::GetMaximumEnabledLightCount(); // 현재 5개
```

### 3.4 섀도우 설정

```cpp
// 섀도우 활성화
light.EnableShadow(true);
bool shadowEnabled = light.IsShadowEnabled();

// 섀도우 강도 설정 (0.0 ~ 1.0)
light.SetShadowIntensity(0.7f);
float intensity = light.GetShadowIntensity();

// 섀도우 바이어스 설정
light.SetShadowBias(0.005f);
float bias = light.GetShadowBias();

// 소프트 섀도우 필터링
light.EnableShadowSoftFiltering(true);
bool softFiltering = light.IsShadowSoftFilteringEnabled();
```

### 3.5 섀도우 품질 최적화

```cpp
// 섀도우 품질을 위한 권장 설정
light.EnableShadow(true);
light.SetShadowIntensity(0.5f);
light.SetShadowBias(0.001f);
light.EnableShadowSoftFiltering(true);
```

## 4. Model 컴포넌트

### 4.1 ModelNode

#### 개요
ModelNode는 3D 모델의 계층 구조를 나타내는 노드입니다. 트리 구조로 자식 노드를 가질 수 있습니다.

#### 주요 기능
```cpp
// 모델 프리미티브 추가
ModelPrimitive primitive = ModelPrimitive::New();
node.AddModelPrimitive(primitive);

// 자식 노드 찾기
ModelNode child = node.FindChildModelNodeByName("ChildNode");

// 트랜스폼 설정
node.SetProperty(Actor::Property::POSITION, Vector3(1.0f, 0.0f, 0.0f));
node.SetProperty(Actor::Property::ORIENTATION, Quaternion(Radian(Degree(45)), Vector3::YAXIS));
node.SetProperty(Actor::Property::SCALE, Vector3(1.0f, 1.0f, 1.0f));
```

### 4.2 ModelPrimitive

#### 개요
ModelPrimitive는 렌더링할 지오메트리와 머티리얼을 포함하는 가장 작은 단위입니다.

#### 지오메트리 설정
```cpp
// 지오메트리 생성 및 설정
Geometry geometry = Geometry::New();
// 지오메트리 정점 데이터 설정...
primitive.SetGeometry(geometry);

// 지오메트리 가져오기
Geometry currentGeometry = primitive.GetGeometry();
```

#### 머티리얼 설정
```cpp
// 머티리얼 생성 및 설정
Material material = Material::New();
material.SetProperty(Material::Property::ALBEDO_COLOR, Color::RED);
primitive.SetMaterial(material);

// 머티리얼 가져오기
Material currentMaterial = primitive.GetMaterial();
```

### 4.3 Material

#### 개요
Material은 모델의 시각적 속성을 정의합니다. PBR(Physically Based Rendering)을 지원합니다.

#### PBR 속성 설정
```cpp
// 기본 PBR 속성
material.SetProperty(Material::Property::ALBEDO_COLOR, Color::WHITE);
material.SetProperty(Material::Property::METALLIC, 0.0f);
material.SetProperty(Material::Property::ROUGHNESS, 0.5f);

// 텍스처 설정
Texture albedoTexture = Texture::New("albedo.jpg");
material.SetProperty(Material::Property::ALBEDO_MAP, albedoTexture);

Texture normalTexture = Texture::New("normal.jpg");
material.SetProperty(Material::Property::NORMAL_MAP, normalTexture);
```

#### 텍스처 관리
```cpp
// 텍스처 설정
material.SetTexture("albedo", albedoTexture);
material.SetTexture("normal", normalTexture);

// 텍스처 가져오기
Texture texture = material.GetTexture("albedo");
```

## 5. 성능 최적화 가이드

### 5.1 SceneView 최적화

#### 프레임버퍼 사용 결정
```cpp
// 단순한 3D 뷰어 - 성능 우선
sceneView.UseFramebuffer(false);

// 복잡한 UI와 통합 - 렌더링 순서 제어 필요
sceneView.UseFramebuffer(true);
```

#### 해상도 최적화
```cpp
// 화면 해상도보다 낮게 설정하여 성능 향상
Vector2 viewSize = sceneView.GetProperty<Vector2>(Actor::Property::SIZE);
sceneView.SetResolution(viewSize.width * 0.8f, viewSize.height * 0.8f);
```

### 5.2 Model 최적화

#### LOD (Level of Detail) 구현
```cpp
// 거리에 따른 모델 교체
void UpdateModelLOD(Model& model, float distance) {
    if (distance < 10.0f) {
        // 고해상도 모델
        model.SetProperty(Actor::Property::VISIBILITY, true);
    } else if (distance < 50.0f) {
        // 중해상도 모델
        // 다른 모델로 교체
    } else {
        // 저해상도 모델 또는 숨김
        model.SetProperty(Actor::Property::VISIBILITY, false);
    }
}
```

#### 애니메이션 최적화
```cpp
// 필요한 애니메이션만 활성화
for (uint32_t i = 0; i < model.GetAnimationCount(); ++i) {
    Animation anim = model.GetAnimation(i);
    if (IsRequiredAnimation(anim)) {
        anim.Play();
    } else {
        anim.Stop();
    }
}
```

### 5.3 Light 최적화

#### 광원 수 제한
```cpp
// SceneView당 최대 5개의 활성 광원
std::vector<Light> lights = {light1, light2, light3, light4, light5, light6};

// 가장 중요한 5개만 활성화
for (size_t i = 0; i < lights.size(); ++i) {
    lights[i].Enable(i < 5);
}
```

#### 섀도우 최적화
```cpp
// 성능이 중요할 때는 소프트 필터링 비활성화
light.EnableShadowSoftFiltering(false);

// 섀도우 바이어스 튜닝으로 아티팩트 감소
light.SetShadowBias(0.002f);
```

## 6. 디버깅 및 문제 해결

### 6.1 일반적인 문제

#### 모델이 보이지 않을 때
```cpp
// 1. 모델 리소스 상태 확인
Model::ResourceStatus status = model.GetModelResourceStatus();
if (status != Model::ResourceStatus::READY) {
    // 리소스 로딩 대기
}

// 2. 모델 크기 및 위치 확인
Vector3 size = model.GetProperty<Vector3>(Actor::Property::SIZE);
Vector3 position = model.GetProperty<Vector3>(Actor::Property::POSITION);

// 3. 카메라 설정 확인
CameraActor camera = sceneView.GetSelectedCamera();
Vector3 cameraPos = camera.GetProperty<Vector3>(Actor::Property::POSITION);
```

#### 섀도우가 보이지 않을 때
```cpp
// 1. 광원 섀도우 활성화 확인
light.EnableShadow(true);

// 2. 모델 섀도우 수신 설정 확인
model.ReceiveShadow(true);

// 3. 섀도우 강도 확인
light.SetShadowIntensity(0.5f);
```

### 6.2 성능 프로파일링

```cpp
// 렌더링 성능 모니터링
void OnRenderFrame() {
    static uint32_t frameCount = 0;
    static auto startTime = std::chrono::high_resolution_clock::now();
    
    frameCount++;
    
    if (frameCount % 60 == 0) {
        auto endTime = std::chrono::high_resolution_clock::now();
        auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(endTime - startTime);
        
        float fps = 60.0f * 1000.0f / duration.count();
        DALI_LOG_ERROR("FPS: %.2f\n", fps);
        
        startTime = endTime;
    }
}
```

## 7. 모범 사례

### 7.1 리소스 관리

```cpp
// 리소스 로딩 완료까지 대기
model.LoadCompletedSignal().Connect(this, &MyClass::OnModelLoaded);

void MyClass::OnModelLoaded(Model model, bool success) {
    if (success) {
        // 애니메이션 시작
        if (model.GetAnimationCount() > 0) {
            Animation anim = model.GetAnimation(0);
            anim.Play();
        }
    } else {
        // 에러 처리
        DALI_LOG_ERROR("Model loading failed\n");
    }
}
```

### 7.2 메모리 관리

```cpp
// 사용하지 않는 리소스 즉시 해제
{
    Model tempModel = Model::New("temp_model.gltf");
    sceneView.Add(tempModel);
    // 임시 모델 사용
} // tempModel이 스코프를 벗어나면 자동으로 해제
```

### 7.3 에러 핸들링

```cpp
try {
    Model model = Model::New("model.gltf", "resources/");
    
    // 리소스 상태 확인
    if (model.GetModelResourceStatus() == Model::ResourceStatus::FAILED) {
        throw std::runtime_error("Model resource loading failed");
    }
    
    sceneView.Add(model);
} catch (const std::exception& e) {
    DALI_LOG_ERROR("Error loading model: %s\n", e.what());
    // 대체 모델 로드 또는 에러 UI 표시
}
```

이 가이드는 Scene3D의 주요 컴포넌트들을 상세히 설명하며, 실제 개발에서 발생할 수 있는 다양한 상황에 대한 해결책을 제공합니다.
