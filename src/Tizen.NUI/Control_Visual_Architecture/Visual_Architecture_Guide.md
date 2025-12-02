# DALi Visual 아키텍처 가이드

## 개요

DALi의 Visual은 UI 컴포넌트의 렌더링 로직을 캡슐화하는 핵심 아키텍처입니다. Visual은 이미지, 텍스트, 색상, 테두리, 그라데이션 등 다양한 시각적 요소를 효율적으로 렌더링하는 역할을 담당하며, 리소스 관리, 애니메이션 지원, 상태 기반 렌더링 등 고급 기능을 제공합니다.

## Visual 클래스 계층 구조

### 상속 계층

```
BaseObject
└── VisualBaseImpl
    └── VisualBase
        ├── ImageVisual
        ├── ColorVisual
        ├── TextVisual
        ├── BorderVisual
        ├── GradientVisual
        ├── PrimitiveVisual
        ├── AnimatedImageVisual
        ├── AnimatedVectorImageVisual
        ├── SvgVisual
        ├── MeshVisual
        ├── NPatchVisual
        └── ... (기타 Visual들)
```

### 주요 클래스 상세

#### 1. VisualBase (Public API)

모든 Visual의 기반이 되는 추상 기본 클래스입니다.

**주요 속성:**
- `Name`: Visual 이름
- `Type`: Visual 타입
- `DepthIndex`: 렌더링 깊이 인덱스
- `MixColor`: 혼합 색상

**주요 메서드:**
- `SetProperties()`: 속성 설정
- `SetName()`: 이름 설정
- `SetTransformAndSize()`: 변환 및 크기 설정
- `SetDepthIndex()`: 깊이 인덱스 설정
- `SetOnScene()`: 씬에 추가
- `SetOffScene()`: 씬에서 제거
- `CreatePropertyMap()`: 속성 맵 생성
- `DoAction()`: 액션 실행
- `AnimateProperty()`: 속성 애니메이션
- `ResourceReady()`: 리소스 준비 완료
- `IsResourceReady()`: 리소스 준비 상태 확인

#### 2. VisualBaseImpl (Internal Implementation)

Visual의 내부 구현을 담당하는 클래스입니다.

**주요 멤버 변수:**
- `mImpl`: Impl 구조체 포인터
- `mFactoryCache`: VisualFactoryCache 참조

**주요 메서드:**
- `SetProperties()`: 속성 설정
- `OnInitialize()`: 초기화 처리
- `DoSetProperties()`: 속성 설정 처리
- `DoSetOnScene()`: 씬 추가 처리
- `DoSetOffScene()`: 씬 제거 처리
- `OnSetTransform()`: 변환 설정 처리
- `UpdateShader()`: 셰이더 업데이트
- `GenerateShader()`: 셰이더 생성

#### 3. VisualBaseDataImpl (Data Implementation)

Visual의 실제 데이터 관리를 담당하는 구조체입니다.

**주요 멤버 변수:**
- `mRenderer`: VisualRenderer
- `mCustomShaders`: 커스텀 셰이더 배열
- `mEventObserver`: 이벤트 옵저버
- `mName`: Visual 이름
- `mTransform`: Transform 구조체
- `mMixColor`: 혼합 색상
- `mControlSize`: 컨트롤 크기
- `mDecorationData`: 장식 데이터
- `mDepthIndex`: 깊이 인덱스
- `mFittingMode`: 피팅 모드
- `mFlags`: 플래그
- `mResourceStatus`: 리소스 상태
- `mType`: Visual 타입

**주요 플래그:**
- `IS_ON_SCENE`: 씬에 추가된 상태
- `IS_ATLASING_APPLIED`: 아틀라싱 적용 상태
- `IS_PREMULTIPLIED_ALPHA`: 프리멀티플라이드 알파 상태
- `IS_SYNCHRONOUS_RESOURCE_LOADING`: 동기 리소스 로딩 상태

## Visual 관리 시스템

### 1. VisualFactory

Visual 객체를 생성하는 팩토리 클래스입니다.

**주요 메서드:**
- `CreateVisual()`: 타입 기반 Visual 생성
- `CreateImageVisual()`: ImageVisual 생성
- `CreateColorVisual()`: ColorVisual 생성
- `CreateTextVisual()`: TextVisual 생성
- `CreateBorderVisual()`: BorderVisual 생성
- `CreateGradientVisual()`: GradientVisual 생성
- `CreatePrimitiveVisual()`: PrimitiveVisual 생성

### 2. VisualFactoryCache

Visual 생성 시 재사용 가능한 리소스를 캐싱합니다.

**캐시 항목:**
- `mRendererCache`: 렌더러 캐시
- `mShaderCache`: 셰이더 캐시
- `mGeometryCache`: 지오메트리 캐시

**주요 메서드:**
- `GetRenderer()`: 캐시된 렌더러 조회
- `GetShader()`: 캐시된 셰이더 조회
- `GetGeometry()`: 캐시된 지오메트리 조회

### 3. VisualData

Control에 등록된 Visual들을 관리하는 데이터 구조입니다.

**주요 멤버 변수:**
- `mVisuals`: Property::Index -> Visual::Base 맵
- `mVisualEnables`: Property::Index -> bool 맵
- `mVisualResourceStatus`: Property::Index -> ResourceStatus 맵
- `mVisualDepthIndices`: Property::Index -> int 맵

**주요 메서드:**
- `RegisterVisual()`: Visual 등록
- `UnregisterVisual()`: Visual 해제
- `GetVisual()`: Visual 조회
- `EnableVisual()`: Visual 활성화
- `GetVisualResourceStatus()`: 리소스 상태 조회

## 구체적 Visual 구현

### 1. ImageVisual

이미지 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mPixelArea`: 이미지 픽셀 영역
- `mImageUrl`: 이미지 URL
- `mMaskingData`: 마스킹 데이터
- `mDesiredSize`: 원하는 크기
- `mTextureId`: 텍스처 ID
- `mTextures`: 텍스처 세트
- `mNativeTexture`: 네이티브 텍스처
- `mTextureSize`: 텍스처 크기
- `mFittingMode`: 피팅 모드
- `mSamplingMode`: 샘플링 모드
- `mWrapModeU`: U 방향 랩 모드
- `mWrapModeV`: V 방향 랩 모드
- `mLoadPolicy`: 로딩 정책
- `mReleasePolicy`: 해제 정책
- `mAtlasRect`: 아틀라스 사각형
- `mLoadState`: 로딩 상태

**주요 기능:**
- 텍스처 로딩 및 관리
- 아틀라싱 지원
- 마스킹 기능
- YUV to RGB 변환
- 비동기 로딩
- 고속 로딩 지원
- 오류 이미지 표시

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `EnablePreMultipliedAlpha()`: 프리멀티플라이드 알파 활성화
- `OnDoAction()`: 액션 처리
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `DoSetOffScene()`: 씬 제거 처리
- `OnSetTransform()`: 변환 설정 처리
- `UpdateShader()`: 셰이더 업데이트
- `GenerateShader()`: 셰이더 생성
- `UploadCompleted()`: 업로드 완료 처리
- `LoadComplete()`: 로딩 완료 처리
- `FastLoadComplete()`: 고속 로딩 완료 처리

### 2. ColorVisual

단색 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mColor`: 색상

**주요 기능:**
- 간단한 사각형 렌더링
- 혼합 색상 지원
- 코너 반경 지원

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `OnSetTransform()`: 변환 설정 처리

### 3. TextVisual

텍스트 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mText`: 텍스트 내용
- `mFontFamily`: 폰트 패밀리
- `mFontStyle`: 폰트 스타일
- `mPointSize`: 폰트 크기
- `mTextColor`: 텍스트 색상
- `mMultiLine`: 다중 라인 여부
- `mHorizontalAlignment`: 수평 정렬
- `mVerticalAlignment`: 수직 정렬
- `mEnableMarkup`: 마크업 활성화

**주요 기능:**
- 폰트 렌더링
- 다중 라인 지원
- 마크업 처리
- 텍스트 스타일링

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `OnSetTransform()`: 변환 설정 처리

### 4. BorderVisual

테두리 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mColor`: 테두리 색상
- `mSize`: 테두리 크기
- `mCornerColor`: 코너 색상
- `mCornerSize`: 코너 크기

**주요 기능:**
- 테두리 색상 및 크기
- 코너 테두리 지원
- 안티앨리어싱

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `OnSetTransform()`: 변환 설정 처리

### 5. GradientVisual

그라데이션 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mStartColor`: 시작 색상
- `mEndColor`: 끝 색상
- `mStartPosition`: 시작 위치
- `mEndPosition`: 끝 위치
- `mGradientUnits`: 그라데이션 단위

**주요 기능:**
- 선형 그라데이션
- 방사형 그라데이션
- 다중 색상 지원

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `OnSetTransform()`: 변환 설정 처리

### 6. PrimitiveVisual

기본 도형 렌더링을 전문으로 하는 Visual입니다.

**주요 속성:**
- `mShape`: 도형 타입
- `mColor`: 색상
- `mSlices`: 슬라이스 수
- `mRings`: 링 수
- `mBevelPercentage`: 베벨 비율
- `mScaleDimensions`: 스케일 차원
- `mThreeDimensions`: 3D 여부

**주요 기능:**
- 원, 사각형, 삼각형
- 3D 기본 도형 지원
- 베벨 효과 지원

**주요 메서드:**
- `GetNaturalSize()`: 자연 크기 조회
- `DoCreatePropertyMap()`: 속성 맵 생성
- `DoSetProperties()`: 속성 설정
- `DoSetOnScene()`: 씬 추가 처리
- `OnSetTransform()`: 변환 설정 처리

## Visual 동작 원리

### 1. 생명주기

Visual의 생명주기는 다음과 같은 단계를 거칩니다:

1. **생성**: VisualFactory를 통해 Visual 생성
2. **초기화**: `OnInitialize()` 가상 함수 호출
3. **속성 설정**: `SetProperties()`로 속성 설정
4. **리소스 준비**: 필요한 리소스 비동기 로딩
5. **씬 연결**: `SetOnScene()`으로 렌더러 추가
6. **렌더링**: 렌더링 루프에 참여
7. **상태 변경**: 속성 변경에 따른 재렌더링
8. **씬 해제**: `SetOffScene()`으로 렌더러 제거
9. **소멸**: 리소스 정리 및 소멸

### 2. 리소스 관리

Visual은 효율적인 리소스 관리를 지원합니다:

- **비동기 로딩**: 리소스를 비동기적으로 로딩하여 UI 응답성 유지
- **리소스 상태**: PREPARING, READY, FAILED 상태로 리소스 상태 관리
- **캐싱**: 자주 사용하는 리소스는 캐시에서 재사용
- **아틀라싱**: 작은 이미지들을 하나의 텍스처로 묶어 메모리 최적화

### 3. 애니메이션 지원

Visual은 속성 애니메이션을 지원합니다:

- **속성 애니메이션**: `AnimateProperty()`로 속성 애니메이션
- **전환 효과**: 상태 변경 시 부드러운 전환
- **셰이더 애니메이션**: 커스텀 셰이더를 통한 고급 애니메이션

## Visual과 렌더링 시스템

### 1. VisualRenderer

Visual의 렌더링을 담당하는 클래스입니다.

**주요 구성요소:**
- `mRenderer`: DALi Core Renderer
- `mGeometry`: 지오메트리
- `mTextureSet`: 텍스처 세트
- `mShader`: 셰이더

**주요 메서드:**
- `SetRenderer()`: 렌더러 설정
- `SetGeometry()`: 지오메트리 설정
- `SetTextureSet()`: 텍스처 세트 설정
- `SetShader()`: 셰이더 설정
- `RegisterVisualTransformUniform()`: 변환 유니폼 등록

### 2. Transform 시스템

Visual의 변환을 관리하는 시스템입니다.

**Transform 구조체:**
- `mOffset`: 오프셋
- `mSize`: 크기
- `mExtraSize`: 추가 크기
- `mOffsetSizeMode`: 오프셋 크기 모드
- `mOrigin`: 원점
- `mAnchorPoint`: 앵커 포인트

**주요 메서드:**
- `SetPropertyMap()`: 속성 맵 설정
- `GetPropertyMap()`: 속성 맵 조회
- `UpdatePropertyMap()`: 속성 맵 업데이트
- `SetUniforms()`: 유니폼 설정
- `GetVisualSize()`: 비주얼 크기 계산

### 3. CustomShader 시스템

Visual에 커스텀 셰이더를 적용하는 시스템입니다.

**CustomShader 구조체:**
- `mVertexShader`: 정점 셰이더
- `mFragmentShader`: 프래그먼트 셰이더
- `mGridSize`: 그리드 크기
- `mHints`: 셰이더 힌트
- `mRenderPassTag`: 렌더 패스 태그
- `mName`: 셰이더 이름

**주요 기능:**
- 다중 셰이더 지원
- 동적 셰이더 생성
- 셰이더 캐싱
- 런타임 셰이더 교체

## Visual과 리소스 관리

### 1. TextureManager

텍스처 리소스를 관리하는 시스템입니다.

**주요 기능:**
- 텍스처 로딩 및 캐싱
- 아틀라싱 최적화
- 메모리 관리
- 비동기 업로드 지원

**주요 메서드:**
- `LoadTexture()`: 텍스처 로딩
- `UploadTexture()`: 텍스처 업로드
- `RemoveTexture()`: 텍스처 제거
- `GetTexture()`: 텍스처 조회
- `AddObserver()`: 옵저버 추가
- `RemoveObserver()`: 옵저버 제거

### 2. ImageLoader

이미지 파일을 로딩하는 시스템입니다.

**지원 형식:**
- JPEG, PNG, GIF, BMP, WebP 등
- 다양한 픽셀 포맷 지원
- 메타데이터 추출

**주요 메서드:**
- `LoadImage()`: 이미지 로딩
- `DecodeImage()`: 이미지 디코딩
- `GetImageSize()`: 이미지 크기 조회
- `GetPixelFormat()`: 픽셀 포맷 조회

### 3. FontManager

폰트 리소스를 관리하는 시스템입니다.

**주요 기능:**
- 폰트 로딩 및 캐싱
- 글리프 렌더링
- 폰트 메트릭 관리

**주요 메서드:**
- `LoadFont()`: 폰트 로딩
- `GetFontMetrics()`: 폰트 메트릭 조회
- `GetGlyphBitmap()`: 글리프 비트맵 조회
- `GetGlyphMetrics()`: 글리프 메트릭 조회

## Visual의 확장

### 1. 새로운 Visual 생성

새로운 Visual을 생성하려면 다음 단계를 따릅니다:

1. **VisualBase 상속**: `class MyVisual : public VisualBase`
2. **필수 가상 함수 구현**:
   - `OnInitialize()`: 초기화 처리
   - `DoSetProperties()`: 속성 설정 처리
   - `DoSetOnScene()`: 씬 추가 처리
   - `DoSetOffScene()`: 씬 제거 처리
   - `OnSetTransform()`: 변환 설정 처리
   - `DoCreatePropertyMap()`: 속성 맵 생성
   - `DoCreateInstancePropertyMap()`: 인스턴스 속성 맵 생성
3. **VisualFactory에 등록**: 새로운 Visual 타입 등록
4. **렌더러 설정**: 필요한 렌더러 설정
5. **리소스 관리**: 필요한 리소스 로딩 및 해제

### 2. VisualFactory 확장

새로운 Visual 타입을 지원하려면 VisualFactory를 확장해야 합니다:

```cpp
// VisualFactory에 새로운 생성 메서드 추가
Visual::BasePtr VisualFactory::CreateMyVisual(const Property::Map& properties)
{
    // Visual 생성 및 속성 설정
    auto visual = MyVisual::New(mFactoryCache);
    visual->SetProperties(properties);
    return visual;
}
```

## 성능 최적화

### 1. 렌더링 최적화

- **드로우 콜 최소화**: 비슷한 Visual들을 그룹화하여 드로우 콜 최소화
- **배치 렌더링**: 동일한 타입의 Visual들을 배치 처리
- **오클루전 컬링**: 보이지 않는 Visual은 렌더링 건너뛰기
- **LOD (Level of Detail):**: 거리에 따른 상세 레벨 조정

### 2. 메모리 최적화

- **텍스처 아틀라싱**: 작은 텍스처들을 하나로 묶어 메모리 절약
- **리소스 풀링**: 자주 사용하는 리소스는 풀에서 재사용
- **지연 로딩**: 필요한 시점에 리소스 로딩
- **가비지 컬렉션**: 사용하지 않는 리소스 자동 해제

### 3. CPU 최적화

- **멀티스레딩**: 리소스 로딩을 백그라운드 스레드에서 처리
- **캐싱**: 계산 결과를 캐싱하여 중복 계산 방지
- **업데이트 최적화**: 변경된 Visual만 업데이트
- **이벤트 최적화**: 불필요한 이벤트 필터링

## Visual과 애니메이션

### 1. 속성 애니메이션

Visual은 다양한 속성 애니메이션을 지원합니다:

- **위치 애니메이션**: Transform의 위치 속성
- **크기 애니메이션**: Transform의 크기 속성
- **색상 애니메이션**: MixColor 속성
- **투명도 애니메이션**: Alpha 속성
- **회전 애니메이션**: Transform의 회전 속성

### 2. 셰이더 애니메이션

커스텀 셰이더를 통한 고급 애니메이션:

- **정점 애니메이션**: 정점 셰이더를 통한 형태 변형
- **프래그먼트 애니메이션**: 프래그먼트 셰이더를 통한 색상/효과 애니메이션
- **시간 기반 애니메이션**: 유니폼 타임 변수를 활용한 애니메이션

### 3. 전환 효과

상태 변경 시 부드러운 전환 효과:

- **페이드 인/아웃**: 투명도를 이용한 부드러운 전환
- **슬라이드**: 위치를 이용한 슬라이드 효과
- **스케일**: 크기를 이용한 확대/축소 효과
- **회전**: 회전을 이용한 동적 전환

## 결론

DALi의 Visual 아키텍처는 유연하고 효율적인 렌더링 시스템을 제공합니다. VisualBase를 통한 일관된 인터페이스와 각 전문 Visual을 통한 특화된 기능 제공으로 개발자는 쉽게 시각적 요소를 구현할 수 있습니다. 리소스 관리, 애니메이션 지원, 성능 최적화 등 고급 기능을 통해 고품질의 UI를 구현할 수 있으며, 확장 가능한 아키텍처로 새로운 Visual 타입을 쉽게 추가할 수 있습니다.

---

## 관련 다이어그램

- [Visual 클래스 계층 다이어그램](Visual_Class_Hierarchy.puml)
- [Control과 Visual 관계 다이어그램](Control_Visual_Relationship.puml)
- [DALi 전체 아키텍처 다이어그램](DALi_Overall_Architecture.puml)
