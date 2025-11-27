# DALi Scene3D 완전 문서

## 문서 개요

본 문서 집합은 DALi Scene3D 패키지에 대한 포괄적인 가이드를 제공합니다. Scene3D의 아키텍처, 컴포넌트, 사용법, 그리고 DALi와 NUI 버전의 차이점을 상세히 설명합니다.

## 문서 구조

### 1. [Scene3D 개요](./DALi_Scene3D_개요.md)
Scene3D의 기본 개념과 아키텍처 철학을 소개합니다.

**주요 내용:**
- Scene3D 정의 및 목적
- 주요 특징과 아키텍처 철학
- 핵심 컴포넌트 구조
- 렌더링 파이프라인
- 좌표계 및 공간 관리
- 성능 최적화 기본 원칙
- 호환성 및 요구사항
- 사용 사례 및 제한 사항

### 2. [Scene3D 컴포넌트 상세 가이드](./Scene3D_컴포넌트_상세.md)
Scene3D의 주요 컴포넌트들을 상세하게 설명합니다.

**주요 내용:**
- **SceneView 컨트롤**: 3D 씬 컨테이너, 카메라 관리, IBL, 프레임버퍼, 스카이박스, 씬 캡처
- **Model 컨트롤**: 3D 모델 로딩, 애니메이션, 블렌드셰이프, 모션 데이터, 섀도우 설정
- **Light 컨트롤**: 광원 관리, 섀도우 생성, 섀도우 품질 최적화
- **Model 컴포넌트**: ModelNode, ModelPrimitive, Material의 상세 사용법
- 성능 최적화 가이드
- 디버깅 및 문제 해결
- 모범 사례

### 3. [Scene3D 모델 로더 아키텍처](./Scene3D_모델로더_아키텍처.md)
모델 로더 시스템의 구조와 동작 방식을 상세히 분석합니다.

**주요 내용:**
- 지원 포맷 (glTF 2.0, DLI, USD)
- 아키텍처 설계 원칙
- ModelLoader 상세 분석
- 포맷별 로더 구현 (GLTF2Loader, DLILoader, USDLoader)
- 리소스 관리 시스템
- 비동기 로딩 시스템
- 커스터마이징 및 확장
- 성능 최적화
- 에러 핸들링 및 디버깅

### 4. [Scene3D 아키텍처 결정 분석](./Scene3D_아키텍처_결정_분석.md)
Scene3D의 주요 구조적/아키텍처 결정들을 심층적으로 분석하고 각 결정이 가져온 영향과 설계 철학을 설명합니다.

**주요 내용:**
- 10개 핵심 아키텍처 결정 상세 분석
- glTF 친화적 설계 결정
- 로더 공통 구조 최대화 결정
- Definition 기반 중간 단계 도입
- 패키지 분리 및 선택적 의존성
- OpenUSD 동적 로딩
- 공유 객체 기반 리소스 관리
- 플러그인 기반 확장 아키텍처
- 비동기 로딩 파이프라인
- PBR 중심 렌더링 파이프라인
- 멀티-플랫폼 바인딩 지원
- 성능 영향 분석
- 실제 구현 사례 및 테스트 전략
- 미래 확장 방향
- 교훈 얻은 점
- PlantUML 아키텍처 다이어그램 포함

### 5. [DALi vs NUI Scene3D 비교](./Scene3D_DALi_NUI_비교.md)
두 프레임워크의 관계와 차이점을 종합적으로 분석합니다.

**주요 내용:**
- 아키텍처 관계와 바인딩 레이어
- 핵심 차이점 (언어, 플랫폼, 성능)
- API 비교 분석 (SceneView, Model, Light)
- 메모리 관리 차이
- 성능 비교 및 최적화 전략
- 기능 차이점 (NUI 전용, DALi 전용)
- 사용 사례별 권장 사항
- 마이그레이션 가이드
- 상호 운용성

### 6. [Scene3D 아키텍처 다이어그램](./Scene3D_Architecture_Diagrams.puml)
Scene3D 아키텍처를 시각화한 PlantUML 다이어그램 모음입니다.

**포함된 다이어그램:**
- Scene3D 아키텍처 전체 구조도
- 클래스 상속 계층도
- 모델 로더 작업 흐름
- 렌더링 파이프라인
- DALi vs NUI 관계도

## 빠른 시작 가이드

### Scene3D 기본 사용 예제

#### C++ (DALi)
```cpp
#include <dali-scene3d/public-api/controls/scene-view/scene-view.h>
#include <dali-scene3d/public-api/controls/model/model.h>

// SceneView 생성
SceneView sceneView = SceneView::New();
sceneView.SetProperty(Actor::Property::SIZE, Vector2(400, 400));
window.Add(sceneView);

// 모델 로딩
Model model = Model::New("model.gltf", "resources/");
sceneView.Add(model);

// 광원 설정
Light light = Light::New();
light.SetProperty(Actor::Property::COLOR, Color::WHITE);
sceneView.Add(light);

// IBL 설정
sceneView.SetImageBasedLightSource("diffuse.ktx", "specular.ktx", 1.0f);
```

#### C# (NUI)
```csharp
using Tizen.NUI.Scene3D;

// SceneView 생성
SceneView sceneView = new SceneView();
sceneView.Size = new Size(400, 400);
window.Add(sceneView);

// 모델 로딩
Model model = new Model("model.gltf", "resources/");
sceneView.Add(model);

// 광원 설정
Light light = new Light();
light.Color = Color.White;
sceneView.Add(light);

// IBL 설정
sceneView.SetImageBasedLightSource("diffuse.ktx", "specular.ktx", 1.0f);
```

## 기술 사양

### 시스템 요구사항
- **OpenGL ES**: GLSL 버전 3.0 이상
- **메모리**: 최소 128MB VRAM 권장
- **프로세서**: OpenGL ES 3.0 지원 GPU

### 지원 포맷
- **모델**: glTF 2.0 (.gltf, .glb), DLI (.dli), USD (.usd, .usda, .usdc)
- **텍스처**: PNG, JPG, KTX
- **큐브맵**: Vertical/Horizontal Cross, Vertical/Horizontal Array

### 성능 특성
- **최대 광원 수**: SceneView당 5개
- **좌표계**: 왼손 좌표계 (X: 오른쪽, Y: 아래, Z: 앞)
- **렌더링 모드**: 프레임버퍼 사용 여부 선택 가능

## 선택 가이드

### DALi Scene3D를 선택해야 하는 경우
- 최고 성능이 요구되는 3D 게임
- 실시간 렌더링 애플리케이션 (AR/VR)
- 메모리가 제한적인 임베디드 시스템
- 저수준 메모리 제어가 필요한 경우

### NUI Scene3D를 선택해야 하는 경우
- 빠른 개발과 높은 생산성이 중요한 경우
- 비즈니스/엔터프라이즈 애플리케이션
- 3D 제품 뷰어, 데이터 시각화
- .NET 생태계를 활용해야 하는 경우

## 학습 경로

### 초급
1. [Scene3D 개요](./DALi_Scene3D_개요.md) - 기본 개념 이해
2. [Scene3D 컴포넌트 상세 가이드](./Scene3D_컴포넌트_상세.md) - 기본 컴포넌트 사용법

### 중급
1. [Scene3D 모델 로더 아키텍처](./Scene3D_모델로더_아키텍처.md) - 모델 로딩 시스템 이해
2. [Scene3D 아키텍처 다이어그램](./Scene3D_Architecture_Diagrams.puml) - 아키텍처 시각화

### 고급
1. [DALi vs NUI Scene3D 비교](./Scene3D_DALi_NUI_비교.md) - 두 프레임워크 심층 비교
2. 성능 최적화 및 커스터마이징

## 참고 자료

### 공식 문서
- [DALi 공식 문서](https://docs.tizen.org/application/native/api/6.0/group__dali.html)
- [Tizen .NET 문서](https://docs.tizen.org/application/dotnet/api/6.0/)

### 관련 기술
- [glTF 2.0 사양](https://github.com/KhronosGroup/glTF/tree/main/specification/2.0)
- [USD 사양](https://graphics.pixar.com/usd/release/index.html)
- [OpenGL ES 3.0](https://www.khronos.org/registry/OpenGL/index_es.php)

### 도구
- [PlantUML](https://plantuml.com/) - 아키텍처 다이어그램 생성
- [glTF Viewer](https://gltf-viewer.donmccurdy.com/) - glTF 모델 확인

## 기여 및 피드백

본 문서는 DALi Scene3D 소스 코드 분석을 기반으로 작성되었습니다. 오타, 오류, 개선 사항이 있으면 알려주시기 바랍니다.

### 문서 버전
- **버전**: 1.0
- **작성일**: 2025년 11월
- **기반 소스**: DALi Scene3D v2.1.38 이상

### 라이선스
본 문서는 Apache License 2.0을 따릅니다.

---

**참고**: 이 문서 집합은 DALi Scene3D의 현재 버전을 기준으로 작성되었으며, 버전 업데이트에 따라 내용이 변경될 수 있습니다. 최신 정보는 공식 문서를 참고하시기 바랍니다.
