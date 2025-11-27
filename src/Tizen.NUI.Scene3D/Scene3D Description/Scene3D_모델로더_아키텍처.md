# Scene3D 모델 로더 아키텍처

## 1. 개요

### 1.1 모델 로더 시스템

Scene3D 모델 로더는 다양한 3D 파일 포맷을 로드하고 DALi Scene3D에서 사용할 수 있는 형태로 변환하는 시스템입니다. 확장 가능한 아키텍처를 통해 새로운 포맷을 쉽게 추가할 수 있도록 설계되었습니다.

### 1.2 지원 포맷

| 포맷 | 파일 확장자 | 설명 | 특징 |
|------|-------------|------|------|
| glTF 2.0 | .gltf, .glb | OpenGL Transmission Format | 표준 3D 포맷, 애니메이션/머티리얼 지원 |
| DLI | .dli | Samsung DALi Internal | Samsung 고유 포맷, 최적화됨 |
| USD | .usd, .usda, .usdc | Universal Scene Description | Pixar 개발, 복잡한 씬 지원 |

**참고**: USD 로더는 동적 라이브러리로 구현되어 있어, 애플리케이션에서 USD 포맷을 실제로 사용할 때만 `dali2-usd-loader` 패키지가 동적으로 로드됩니다. 이를 통해 불필요한 의존성을 줄이고 애플리케이션의 시작 시간을 최적화합니다.

## 2. 아키텍처 설계

### 2.1 설계 원칙

모델 로더 아키텍처는 다음과 같은 설계 원칙을 따릅니다:

1. **확장성**: 새로운 포맷 로더를 쉽게 추가할 수 있는 플러그인 구조
2. **일관성**: 모든 로더가 동일한 인터페이스를 구현
3. **효율성**: 비동기 로딩과 리소스 캐싱 지원
4. **유연성**: 로드 옵션과 커스터마이징 지원

### 2.2 핵심 컴포넌트

```
ModelLoader (퍼사드)
    ↓
ModelLoaderImpl (추상 기반 클래스)
    ↓
┌─────────────┬─────────────┬─────────────┐
│ GLTF2Loader │  DLILoader  │  USDLoader  │
└─────────────┴─────────────┴─────────────┘
    ↓
ResourceBundle (리소스 관리)
    ↓
SceneDefinition (씬 구조)
```

## 3. ModelLoader 상세 분석

### 3.1 ModelLoader 클래스

ModelLoader는 모델 로딩의 진입점으로, 퍼사드 패턴을 통해 복잡한 로딩 과정을 단순화합니다.

```cpp
class ModelLoader
{
public:
    ModelLoader(const std::string& modelUrl, 
                const std::string& resourceDirectoryUrl, 
                LoadResult& loadResult);
    
    bool LoadModel(ResourceBundle::PathProvider& pathProvider, 
                   bool loadOnlyRawResource = false);
    
    SceneDefinition& GetScene();
    ResourceBundle& GetResources();
    std::vector<AnimationDefinition>& GetAnimations();
    std::vector<CameraParameters>& GetCameras();
    
private:
    void CreateModelLoader();
    void LoadResource(ResourceBundle::PathProvider& pathProvider, 
                      bool loadOnlyRawResource);
    
private:
    std::string mModelUrl;
    std::string mResourceDirectoryUrl;
    LoadResult mLoadResult;
    std::unique_ptr<ModelLoaderImpl> mImpl;
};
```

### 3.2 로딩 프로세스

#### 3.2.1 초기화 단계

```cpp
// 1. ModelLoader 생성
LoadResult loadResult;
ModelLoader loader("model.gltf", "resources/", loadResult);

// 2. 포맷에 맞는 로더 생성
loader.CreateModelLoader(); // 내부적으로 파일 확장자 확인
```

#### 3.2.2 포맷 감지 및 동적 로딩

```cpp
void ModelLoader::CreateModelLoader()
{
    std::string extension = GetFileExtension(mModelUrl);
    
    if (extension == "gltf" || extension == "glb") {
        mImpl = std::make_unique<GLTF2Loader>(mModelUrl, mResourceDirectoryUrl, mLoadResult);
    } else if (extension == "dli") {
        mImpl = std::make_unique<DLILoader>(mModelUrl, mResourceDirectoryUrl, mLoadResult);
    } else if (extension == "usd" || extension == "usda" || extension == "usdc") {
        // USD 로더는 동적 라이브러리로 로드
        mImpl = CreateUSDLoaderDynamic();
    } else {
        throw std::runtime_error("Unsupported model format: " + extension);
    }
}

std::unique_ptr<ModelLoaderImpl> ModelLoader::CreateUSDLoaderDynamic()
{
    // 동적 라이브러리 로드 시도
    void* handle = dlopen("libdali-usd-loader.so", RTLD_LAZY);
    if (!handle) {
        DALI_LOG_ERROR("Failed to load USD loader library: %s\n", dlerror());
        throw std::runtime_error("USD loader not available");
    }
    
    // 팩토리 함수 타입 정의
    using CreateUSDLoaderFunc = std::unique_ptr<ModelLoaderImpl>(*)(
        const std::string&, const std::string&, LoadResult&);
    
    // 팩토리 함수 주소 가져오기
    CreateUSDLoaderFunc createFunc = (CreateUSDLoaderFunc)dlsym(handle, "CreateUSDLoader");
    if (!createFunc) {
        dlclose(handle);
        throw std::runtime_error("USD loader factory function not found");
    }
    
    // USD 로더 인스턴스 생성
    return createFunc(mModelUrl, mResourceDirectoryUrl, mLoadResult);
}
```

#### USD 동적 로딩의 장점

1. **의존성 최소화**: USD를 사용하지 않는 애플리케이션은 불필요한 USD 라이브러리를 로드하지 않음
2. **시작 시간 최적화**: 애플리케이션 시작 시 동적 라이브러리 로딩 오버헤드 제거
3. **메모리 사용량 감소**: 실제 사용 시에만 USD 관련 메모리 할당
4. **유연한 배포**: USD 기능이 선택적인 애플리케이션에 대한 경량화된 배포 가능

#### 3.2.3 리소스 로딩

```cpp
bool ModelLoader::LoadModel(ResourceBundle::PathProvider& pathProvider, bool loadOnlyRawResource)
{
    // 1. 원시 리소스 로딩
    if (!mImpl->LoadModel()) {
        return false;
    }
    
    // 2. DALi 리소스 생성 (선택적)
    if (!loadOnlyRawResource) {
        LoadResource(pathProvider, loadOnlyRawResource);
    }
    
    return true;
}
```

## 4. ModelLoaderImpl 추상 클래스

### 4.1 인터페이스 정의

모든 포맷 로더는 ModelLoaderImpl을 상속받아 동일한 인터페이스를 구현해야 합니다.

```cpp
class ModelLoaderImpl
{
public:
    virtual ~ModelLoaderImpl() = default;
    virtual bool LoadModel() = 0;
    
protected:
    ModelLoaderImpl(const std::string& modelUrl,
                    const std::string& resourceDirectoryUrl,
                    LoadResult& loadResult);
    
protected:
    std::string mModelUrl;
    std::string mResourceDirectoryUrl;
    LoadResult& mLoadResult;
};
```

### 4.2 공통 기능

ModelLoaderImpl은 모든 로더에 필요한 공통 기능을 제공합니다:

- 파일 경로 해석
- 리소스 디렉토리 관리
- 기본적인 에러 핸들링
- 로드 결과 저장

## 5. 포맷별 로더 구현

### 5.1 GLTF2Loader

#### 5.1.1 개요

GLTF2Loader는 glTF 2.0 포맷을 로드하는 구현체입니다. JSON 기반의 .gltf 파일과 바이너리 .glb 파일을 모두 지원합니다.

#### 5.1.2 핵심 구조

```cpp
class GLTF2Loader : public ModelLoaderImpl
{
public:
    bool LoadModel() override;
    
private:
    bool LoadGLTFFile();
    bool ParseScene();
    bool LoadMeshes();
    bool LoadMaterials();
    bool LoadAnimations();
    bool LoadSkins();
    
private:
    gltf2::Asset mAsset;
    std::vector<gltf2::Mesh> mMeshes;
    std::vector<gltf2::Material> mMaterials;
    std::vector<gltf2::Animation> mAnimations;
};
```

#### 5.1.3 로딩 과정

```cpp
bool GLTF2Loader::LoadModel()
{
    // 1. glTF 파일 파싱
    if (!LoadGLTFFile()) {
        return false;
    }
    
    // 2. 씬 구조 파싱
    if (!ParseScene()) {
        return false;
    }
    
    // 3. 메시 데이터 로딩
    if (!LoadMeshes()) {
        return false;
    }
    
    // 4. 머티리얼 로딩
    if (!LoadMaterials()) {
        return false;
    }
    
    // 5. 애니메이션 로딩
    if (!LoadAnimations()) {
        return false;
    }
    
    // 6. 스킨 데이터 로딩
    if (!LoadSkins()) {
        return false;
    }
    
    return true;
}
```

#### 5.1.4 메시 로딩 상세

```cpp
bool GLTF2Loader::LoadMeshes()
{
    for (const auto& gltfMesh : mAsset.meshes) {
        MeshDefinition meshDef;
        
        for (const auto& primitive : gltfMesh.primitives) {
            GeometryDefinition geoDef;
            
            // 정점 속성 로딩
            if (primitive.attributes.find("POSITION") != primitive.attributes.end()) {
                LoadAccessor(primitive.attributes.at("POSITION"), geoDef.positions);
            }
            
            if (primitive.attributes.find("NORMAL") != primitive.attributes.end()) {
                LoadAccessor(primitive.attributes.at("NORMAL"), geoDef.normals);
            }
            
            if (primitive.attributes.find("TEXCOORD_0") != primitive.attributes.end()) {
                LoadAccessor(primitive.attributes.at("TEXCOORD_0"), geoDef.texCoords);
            }
            
            // 인덱스 로딩
            if (primitive.indices >= 0) {
                LoadAccessor(primitive.indices, geoDef.indices);
            }
            
            meshDef.geometries.push_back(geoDef);
        }
        
        mLoadResult.resources.AddMesh(meshDef);
    }
    
    return true;
}
```

### 5.2 DLILoader

#### 5.2.1 개요

DLILoader는 Samsung 고유의 DLI 포맷을 로드하는 구현체입니다. 최적화된 바이너리 포맷으로 빠른 로딩 속도를 제공합니다.

#### 5.2.2 특징

- 바이너리 포맷으로 빠른 로딩
- Samsung 기기 최적화
- 압축된 데이터 지원
- DALi 전용 기능 지원

#### 5.2.3 구현 예시

```cpp
class DLILoader : public ModelLoaderImpl
{
public:
    bool LoadModel() override;
    
private:
    bool ReadHeader();
    bool ReadSceneData();
    bool ReadMeshData();
    bool ReadMaterialData();
    bool ReadAnimationData();
    
private:
    DliHeader mHeader;
    std::ifstream mFileStream;
};
```

### 5.3 USDLoader

#### 5.3.1 개요

USDLoader는 Universal Scene Description 포맷을 로드하는 구현체입니다. 복잡한 3D 씬과 계층 구조를 지원합니다.

#### 5.3.2 특징

- 복잡한 씬 계층 구조 지원
- 레이어링 및 컴포지션 기능
- 애니메이션과 시변 데이터 지원
- Pixar USD 라이브러리 기반

#### 5.3.3 구현 예시

```cpp
class USDLoader : public ModelLoaderImpl
{
public:
    bool LoadModel() override;
    
private:
    bool LoadUSDStage();
    bool TraverseSceneHierarchy();
    bool ConvertPrims();
    bool ExtractMaterials();
    bool ExtractAnimations();
    
private:
    UsdStageRefPtr mStage;
    SdfPathVector mPrims;
};
```

## 6. 리소스 관리 시스템

### 6.1 ResourceBundle

ResourceBundle은 로드된 모든 리소스를 중앙에서 관리하는 컴포넌트입니다.

```cpp
class ResourceBundle
{
public:
    // 리소스 추가
    void AddGeometry(uint32_t id, Geometry geometry);
    void AddTexture(uint32_t id, Texture texture);
    void AddMaterial(uint32_t id, Material material);
    
    // 리소스 조회
    Geometry GetGeometry(uint32_t id);
    Texture GetTexture(uint32_t id);
    Material GetMaterial(uint32_t id);
    
    // 경로 제공자
    using PathProvider = std::function<std::string(const std::string&)>;
    
private:
    std::vector<Geometry> mGeometries;
    std::vector<Texture> mTextures;
    std::vector<Material> mMaterials;
    std::unordered_map<std::string, uint32_t> mResourceMap;
};
```

### 6.2 리소스 캐싱

```cpp
class ResourceCache
{
public:
    static ResourceCache& GetInstance();
    
    Texture GetOrCreateTexture(const std::string& path);
    Geometry GetOrCreateGeometry(const std::string& key);
    
    void ClearCache();
    
private:
    std::unordered_map<std::string, Texture> mTextureCache;
    std::unordered_map<std::string, Geometry> mGeometryCache;
};
```

## 7. 씬 정의 시스템

### 7.1 SceneDefinition의 역할

SceneDefinition은 다양한 파일 포맷에서 로드된 데이터를 DALi Model 구조로 변환하기 위한 **중간 단계(Intermediate Stage)**로 사용됩니다. 이 설계는 여러 종류의 파일 포맷으로부터 통합된 인터페이스를 통해 Model을 생성할 수 있게 해주는 핵심적인 아키텍처 패턴입니다.

#### SceneDefinition의 필요성

1. **통합된 인터페이스 제공**: glTF, DLI, USD 등 각기 다른 포맷의 데이터 구조를 표준화된 형태로 변환
2. **느슨한 결합**: 특정 포맷에 종속되지 않는 유연한 아키텍처
3. **확장성**: 새로운 포맷 추가 시 기존 코드 수정 최소화
4. **데이터 변환 최적화**: 포맷별 특성을 고려한 최적의 변환 로직 적용

#### 데이터 흐름

```
파일 포맷 (glTF/DLI/USD)
    ↓
포맷별 로더 (GLTF2Loader/DLILoader/USDLoader)
    ↓
SceneDefinition (중간 표현)
    ↓
DALi Model (최종 구조)
```

### 7.2 SceneDefinition 클래스

SceneDefinition은 로드된 3D 씬의 구조를 표준화된 형태로 정의합니다.

```cpp
class SceneDefinition
{
public:
    NodeDefinition& GetRootNode();
    const std::vector<AnimationDefinition>& GetAnimations() const;
    const std::vector<CameraParameters>& GetCameras() const;
    
    void SetRootNode(const NodeDefinition& rootNode);
    void AddAnimation(const AnimationDefinition& animation);
    void AddCamera(const CameraParameters& camera);
    
private:
    NodeDefinition mRootNode;
    std::vector<AnimationDefinition> mAnimations;
    std::vector<CameraParameters> mCameras;
};
```

### 7.2 NodeDefinition

```cpp
class NodeDefinition
{
public:
    std::string name;
    Vector3 position;
    Quaternion rotation;
    Vector3 scale;
    
    std::vector<uint32_t> meshIndices;
    std::vector<uint32_t> childIndices;
    std::vector<uint32_t> materialIndices;
    
    // 애니메이션 관련
    std::vector<AnimationTarget> animationTargets;
};
```

## 8. 비동기 로딩 시스템

### 8.1 로딩 태스크

```cpp
class ModelLoadTask
{
public:
    ModelLoadTask(const std::string& modelUrl,
                  const std::string& resourceDirectoryUrl,
                  std::function<void(bool)> callback);
    
    void Execute();
    bool IsCompleted() const;
    
private:
    std::string mModelUrl;
    std::string mResourceDirectoryUrl;
    std::function<void(bool)> mCallback;
    std::atomic<bool> mCompleted{false};
    std::future<bool> mFuture;
};
```

### 8.2 비동기 로딩 사용 예시

```cpp
// 비동기 모델 로딩
void LoadModelAsync(const std::string& modelUrl)
{
    auto task = std::make_shared<ModelLoadTask>(
        modelUrl,
        "resources/",
        [this](bool success) {
            if (success) {
                DALI_LOG_INFO("Model loaded successfully\n");
            } else {
                DALI_LOG_ERROR("Model loading failed\n");
            }
        }
    );
    
    // 백그라운드 스레드에서 실행
    std::thread([task]() {
        task->Execute();
    }).detach();
}
```

## 9. 커스터마이징 및 확장

### 9.1 새로운 포맷 로더 추가

#### 9.1.1 로더 클래스 구현

```cpp
class CustomFormatLoader : public ModelLoaderImpl
{
public:
    CustomFormatLoader(const std::string& modelUrl,
                       const std::string& resourceDirectoryUrl,
                       LoadResult& loadResult)
    : ModelLoaderImpl(modelUrl, resourceDirectoryUrl, loadResult)
    {
    }
    
    bool LoadModel() override
    {
        // 커스텀 포맷 로딩 로직 구현
        if (!ParseCustomFormat()) {
            return false;
        }
        
        if (!ConvertToSceneDefinition()) {
            return false;
        }
        
        return true;
    }
    
private:
    bool ParseCustomFormat()
    {
        // 커스텀 포맷 파싱
        return true;
    }
    
    bool ConvertToSceneDefinition()
    {
        // SceneDefinition으로 변환
        return true;
    }
};
```

#### 9.1.2 ModelLoader에 등록

```cpp
void ModelLoader::CreateModelLoader()
{
    std::string extension = GetFileExtension(mModelUrl);
    
    if (extension == "custom") {
        mImpl = std::make_unique<CustomFormatLoader>(mModelUrl, mResourceDirectoryUrl, mLoadResult);
    }
    // 기존 포맷들...
}
```

### 9.2 로딩 옵션 커스터마이징

```cpp
struct LoadOptions
{
    bool generateTangents = true;
    bool mergeMeshes = false;
    bool optimizeGeometry = true;
    float scale_factor = 1.0f;
    bool flipUVs = false;
};

class ModelLoader
{
public:
    bool LoadModel(const LoadOptions& options = LoadOptions{});
    
private:
    LoadOptions mOptions;
};
```

## 10. 성능 최적화

### 10.1 로딩 성능 최적화

#### 10.1.1 스트리밍 로딩

```cpp
class StreamingLoader
{
public:
    void LoadModelProgressive(const std::string& modelUrl)
    {
        // 1. 메시 데이터 먼저 로딩
        LoadMeshData(modelUrl);
        
        // 2. 저해상도 텍스처 로딩
        LoadLowResTextures(modelUrl);
        
        // 3. 고해상도 텍스처 비동기 로딩
        std::thread([this, modelUrl]() {
            LoadHighResTextures(modelUrl);
        }).detach();
    }
};
```

#### 10.1.2 메모리 풀링

```cpp
class GeometryPool
{
public:
    static GeometryPool& GetInstance()
    {
        static GeometryPool instance;
        return instance;
    }
    
    Geometry* AcquireGeometry()
    {
        if (!mPool.empty()) {
            auto* geom = mPool.back();
            mPool.pop_back();
            return geom;
        }
        return new Geometry();
    }
    
    void ReleaseGeometry(Geometry* geometry)
    {
        geometry->Reset();
        mPool.push_back(geometry);
    }
    
private:
    std::vector<Geometry*> mPool;
};
```

### 10.2 렌더링 최적화

#### 10.2.1 지오메트리 병합

```cpp
class GeometryMerger
{
public:
    static Geometry MergeGeometries(const std::vector<Geometry>& geometries)
    {
        Geometry merged;
        
        for (const auto& geom : geometries) {
            // 정점 데이터 병합
            merged.vertices.insert(merged.vertices.end(),
                                  geom.vertices.begin(), geom.vertices.end());
            
            // 인덱스 데이터 조정 및 병합
            AdjustAndMergeIndices(merged, geom);
        }
        
        return merged;
    }
};
```

#### 10.2.2 LOD 생성

```cpp
class LODGenerator
{
public:
    static std::vector<Geometry> GenerateLODChain(const Geometry& highRes)
    {
        std::vector<Geometry> lodChain;
        
        lodChain.push_back(highRes); // LOD 0
        
        // LOD 1: 50% 폴리곤
        lodChain.push_back(GenerateLOD(highRes, 0.5f));
        
        // LOD 2: 25% 폴리곤
        lodChain.push_back(GenerateLOD(highRes, 0.25f));
        
        return lodChain;
    }
    
private:
    static Geometry GenerateLOD(const Geometry& source, float ratio)
    {
        // 간단한 폴리곤 리덕션 알고리즘
        return SimplifyGeometry(source, ratio);
    }
};
```

## 11. 에러 핸들링 및 디버깅

### 11.1 에러 타입

```cpp
enum class LoaderError
{
    SUCCESS,
    FILE_NOT_FOUND,
    INVALID_FORMAT,
    PARSE_ERROR,
    RESOURCE_LOAD_FAILED,
    OUT_OF_MEMORY,
    UNSUPPORTED_FEATURE
};

class LoaderException : public std::exception
{
public:
    LoaderException(LoaderError error, const std::string& message)
    : mError(error), mMessage(message)
    {
    }
    
    const char* what() const noexcept override
    {
        return mMessage.c_str();
    }
    
    LoaderError GetError() const { return mError; }
    
private:
    LoaderError mError;
    std::string mMessage;
};
```

### 11.2 디버깅 지원

```cpp
class LoaderDebugger
{
public:
    static void EnableDebugLogging(bool enable)
    {
        sDebugEnabled = enable;
    }
    
    static void LogLoadingProgress(const std::string& stage, float progress)
    {
        if (sDebugEnabled) {
            DALI_LOG_INFO("Loading: %s - %.1f%%\n", stage.c_str(), progress * 100.0f);
        }
    }
    
    static void DumpSceneStructure(const SceneDefinition& scene)
    {
        if (sDebugEnabled) {
            DumpNode(scene.GetRootNode(), 0);
        }
    }
    
private:
    static void DumpNode(const NodeDefinition& node, int depth)
    {
        std::string indent(depth * 2, ' ');
        DALI_LOG_INFO("%sNode: %s\n", indent.c_str(), node.name.c_str());
        
        for (uint32_t childIndex : node.childIndices) {
            // 자식 노드 덤프
        }
    }
    
    static bool sDebugEnabled = false;
};
```

## 12. 모범 사례

### 12.1 리소스 관리

```cpp
// 좋은 예시: RAII를 통한 자원 관리
class ModelManager
{
public:
    Model LoadModel(const std::string& path)
    {
        LoadResult result;
        ModelLoader loader(path, "resources/", result);
        
        if (!loader.LoadModel(GetPathProvider())) {
            throw LoaderException(LoaderError::RESOURCE_LOAD_FAILED, 
                                "Failed to load model: " + path);
        }
        
        return CreateModelFromResult(result);
    }
    
private:
    ResourceBundle::PathProvider GetPathProvider()
    {
        return [this](const std::string& relativePath) {
            return mResourceBasePath + "/" + relativePath;
        };
    }
    
    std::string mResourceBasePath;
};
```

### 12.2 비동기 로딩

```cpp
// 좋은 예시: 비동기 로딩과 상태 관리
class AsyncModelLoader
{
public:
    std::future<Model> LoadModelAsync(const std::string& path)
    {
        return std::async(std::launch::async, [this, path]() {
            try {
                return mModelManager.LoadModel(path);
            } catch (const LoaderException& e) {
                DALI_LOG_ERROR("Model loading failed: %s\n", e.what());
                return Model{};
            }
        });
    }
    
private:
    ModelManager mModelManager;
};
```

### 12.3 메모리 최적화

```cpp
// 좋은 예시: 메모리 풀과 캐싱
class OptimizedLoader
{
public:
    Model LoadModel(const std::string& path)
    {
        // 캐시 확인
        auto it = mModelCache.find(path);
        if (it != mModelCache.end()) {
            return it->second;
        }
        
        // 로딩 및 캐싱
        Model model = LoadModelInternal(path);
        mModelCache[path] = model;
        
        return model;
    }
    
    void ClearCache()
    {
        mModelCache.clear();
        ResourceCache::GetInstance().ClearCache();
    }
    
private:
    std::unordered_map<std::string, Model> mModelCache;
};
```

이 아키텍처 문서는 Scene3D 모델 로더 시스템의 전체적인 구조와 동작 방식을 상세히 설명하며, 실제 개발에서 모델 로더를 확장하거나 최적화할 때 참고할 수 있는 가이드를 제공합니다.
