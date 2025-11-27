# DALi Scene3D 개요

## 1. Scene3D 소개

### 1.1 정의 및 목적

DALi Scene3D는 Tizen 플랫폼에서 3D 씬을 렌더링하고 3D 오브젝트를 제어하기 위한 패키지입니다. 개발자는 2D DALi UI 애플리케이션 내에서 2D UI 컨트롤과 3D 오브젝트를 함께 사용할 수 있어, 풍부하고 인터랙티브한 사용자 경험을 제공할 수 있습니다.

### 1.2 주요 특징

- **하이브리드 렌더링**: 2D UI와 3D 씬의 통합 렌더링
- **물리 기반 렌더링(PBR)**: Image Based Lighting(IBL) 지원
- **다양한 3D 포맷 지원**: glTF 2.0, USD, DLI 포맷
- **멀티 카메라 시스템**: 여러 카메라를 통한 다각적 뷰 제공
- **실시간 섀도우**: 동적 섀도우 맵을 통한 사실적인 그림자 효과
- **애니메이션 시스템**: 모델 애니메이션과 모션 데이터 지원

### 1.3 아키텍처 철학

Scene3D는 다음과 같은 설계 철학을 기반으로 개발되었습니다:

1. **통합성**: 2D UI와 3D 씬의 완벽한 통합
2. **확장성**: 플러그인 기반의 모델 로더 아키텍처
3. **성능**: 최적화된 렌더링 파이프라인
4. **호환성**: 다양한 3D 포맷과 표준 준수

## 2. 핵심 컴포넌트 구조

### 2.1 SceneView

SceneView는 3D 씬을 표시하기 위한 메인 컨테이너입니다. 각 SceneView는 독립적인 3D 공간을 가지며, 내부적으로 3D 레이어를 관리합니다.

```cpp
SceneView sceneView = SceneView::New();
sceneView.SetProperty(Actor::Property::SIZE, Vector2(400, 400));
window.Add(sceneView);
```

#### 주요 기능:
- 다중 카메라 관리
- Image Based Light 소스 설정
- 프레임버퍼 사용 옵션
- 스카이박스 렌더링
- 씬 캡처 기능

### 2.2 Model

Model은 3D 모델 오브젝트를 로드하고 표시하는 컴포넌트입니다. glTF 2.0, USD, DLI 등 다양한 포맷을 지원합니다.

```cpp
Model model = Model::New("model.gltf", "resources/");
sceneView.Add(model);
```

#### 주요 기능:
- 비동기 모델 로딩
- 애니메이션 재생
- 섀도우 설정
- 머티리얼 커스터마이징

### 2.3 Light

Light는 3D 씬을 조명하는 광원 컴포넌트입니다. 현재 방향성 광원(Directional Light)을 지원합니다.

```cpp
Light light = Light::New();
light.SetProperty(Actor::Property::COLOR, Color::WHITE);
light.EnableShadow(true);
sceneView.Add(light);
```

#### 주요 기능:
- 섀도우 생성
- 소프트 섀도우 필터링
- 섀도우 강도 및 바이어스 조절

### 2.4 Model 컴포넌트 계층 구조

```
Model
├── ModelNode (루트)
    ├── ModelNode (자식)
    │   ├── ModelPrimitive
    │   │   ├── Geometry
    │   │   └── Material
    │   └── ModelPrimitive
    └── ModelNode
```

## 3. 렌더링 파이프라인

### 3.1 렌더링 흐름

1. **씬 설정**: SceneView가 3D 레이어를 생성
2. **카메라 설정**: 선택된 카메라의 뷰/투영 행렬 계산
3. **광원 계산**: 활성화된 광원의 셰이더 파라미터 설정
4. **오브젝트 렌더링**: 각 Model의 ModelPrimitive를 순회하며 렌더링
5. **후처리**: 프레임버퍼 사용 시 포스트프로세싱 적용

### 3.2 프레임버퍼 모드

- **UseFramebuffer(false)**: 직접 윈도우 서피스에 렌더링 (고성능, 항상 최상위)
- **UseFramebuffer(true)**: FBO에 렌더링 후 텍스처로 매핑 (유연한 렌더링 순서)

## 4. 좌표계 및 공간

### 4.1 좌표계

Scene3D는 왼손 좌표계를 사용합니다:
- **X축**: 오른쪽 방향
- **Y축**: 아래쪽 방향  
- **Z축**: 앞쪽 방향 (화면 밖)

### 4.2 공간 관리

각 SceneView는 독립적인 3D 공간을 가지며, 내부적으로 Layer3D를 통해 깊이 테스트를 수행합니다. SceneView에 추가된 Actor는 자동으로 3D 레이어의 자식이 됩니다.

## 5. 성능 최적화

### 5.1 렌더링 최적화

- **드로우 콜 최소화**: 동일한 머티리얼을 가진 프리미티브 그룹화
- **오클루전 컬링**: 보이지 않는 오브젝트 렌더링 건너뛰기
- **LOD(Level of Detail)**: 거리에 따른 모델 디테일 조절

### 5.2 메모리 관리

- **리소스 캐싱**: 로드된 텍스처와 지오메트리 캐싱
- **비동기 로딩**: 모델 리소스의 백그라운드 로딩
- **가비지 컬렉션**: 사용하지 않는 리소스 자동 정리

## 6. 호환성 및 요구사항

### 6.1 시스템 요구사항

- **OpenGL ES**: GLSL 버전 3.0 이상
- **메모리**: 최소 128MB VRAM 권장
- **프로세서**: OpenGL ES 3.0 지원 GPU

### 6.2 지원 포맷

#### 모델 포맷
- **glTF 2.0**: .gltf, .glb
- **DLI**: Samsung 고유 포맷
- **USD**: .usd, .usda, .usdc

#### 텍스처 포맷
- **이미지**: PNG, JPG, KTX
- **큐브맵**: Vertical/Horizontal Cross, Vertical/Horizontal Array

## 7. 사용 사례

### 7.1 추천 사용 시나리오

- **3D 제품 뷰어**: 상품의 360도 뷰 제공
- **게임 UI**: 2D UI와 3D 게임 요소의 통합
- **데이터 시각화**: 3D 차트와 그래프
- **AR/VR 애플리케이션**: 혼합 현실 콘텐츠

### 7.2 제한 사항

- **복잡도**: 너무 많은 폴리곤은 성능에 영향을 줄 수 있음
- **광원 수**: SceneView당 최대 5개의 활성 광원
- **텍스처 크기**: GPU 메모리 제한으로 인한 최대 텍스처 크기 제한

## 8. 다음 단계

Scene3D를 효과적으로 사용하기 위해 다음 문서들을 참고하세요:

1. [컨트롤 및 컴포넌트 상세 가이드](./Scene3D_컴포넌트_상세.md)
2. [모델 로더 아키텍처](./Scene3D_모델로더_아키텍처.md)
3. [DALi vs NUI Scene3D 비교](./Scene3D_DALi_NUI_비교.md)
4. [아키텍처 다이어그램](./Scene3D_Architecture_Diagrams.puml)
