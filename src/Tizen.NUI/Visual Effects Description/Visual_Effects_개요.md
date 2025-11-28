# DALi Visual Effects 개요

## 1. Visual Effects 소개

### 1.1 정의 및 목적

DALi Visual Effects는 Tizen 플랫폼에서 현대적 UI 디자인 트렌드를 구현하기 위한 비주얼 이펙트 시스템입니다. 개발자는 DALi UI 애플리케이션에 Neumorphism, Glassmorphism과 같은 최신 디자인 효과를 적용할 수 있어, 풍부하고 현대적인 사용자 경험을 제공할 수 있습니다.

기존 DALi는 빠르고 성숙한 UI 프레임워크였지만, 현대적 디자인 트렌드를 구현하기 위한 비주얼 이펙트가 부족했습니다. BoxShadow는 지원했지만 InnerShadow는 없었고, 하나의 그림자만 등록할 수 있었습니다. GaussianBlurView라는 간단한 블러 뷰가 있었지만, Glassmorphism을 위한 Background Blur나 텍스트 효과는 부족했습니다. 이러한 제한사항을 해결하기 위해 Visual Effects 시스템이 도입되었습니다.

### 1.2 주요 특징

- **다양한 비주얼 이펙트**: RenderEffect를 통한 확장 가능한 이펙트 시스템
- **다중 Shadow 지원**: InnerShadow와 여러 Shadow의 동시 적용
- **고급 블러 효과**: BackgroundBlurEffect와 GaussianBlurEffect
- **텍스트 Cutout**: 텍스트 모양을 통한 배경 표시
- **마스킹 시스템**: MaskEffect를 통한 정교한 마스킹
- **오프스크린 렌더링**: 통일된 corner radius, clipping 표현
- **자동 RenderTask 재정렬**: BackgroundBlurEffect를 위한 최적화

### 1.3 아키텍처 철학

Visual Effects는 다음과 같은 설계 철학을 기반으로 개발되었습니다:

1. **확장성**: RenderEffect 인터페이스를 통한 새로운 이펙트의 쉬운 추가
2. **성능**: 오프스크린 렌더링과 RenderTask 최적화를 통한 효율적인 처리
3. **통합성**: DALi Control과의 완벽한 통합
4. **호환성**: 기존 DALi 시스템과의 하위 호환성 유지

## 2. 핵심 컴포넌트 구조

### 2.1 RenderEffect

RenderEffect는 모든 비주얼 이펙트의 기본 인터페이스입니다. 이펙트의 생명주기를 관리하고 Control에 적용되는 방식을 정의합니다.

```cpp
// RenderEffect 기본 사용법
Toolkit::RenderEffect effect = Toolkit::MaskEffect::New(maskControl);
control.SetRenderEffect(effect);  // 이펙트 활성화
effect.Activate();                // 수동 활성화
effect.Deactivate();              // 비활성화
effect.Refresh();                 // 이펙트 새로고침
control.ClearRenderEffect();      // 이펙트 제거
```

#### 주요 기능:
- 이펙트 활성화/비활성화 관리
- 소유 Control과의 생명주기 동기화
- 오프스크린 렌더링 타입 관리
- 이펙트 상태 조회 및 제어

### 2.2 MaskEffect

MaskEffect는 다른 Control을 마스크로 사용하여 소유 Control의 일부를 가리거나 표시하는 효과를 제공합니다.

```cpp
// MaskEffect 생성
Toolkit::Control maskControl = Toolkit::Control::New();
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(
    maskControl, 
    Toolkit::MaskEffect::ALPHA,    // 또는 LUMINANCE
    Vector2(0.0f, 0.0f),           // maskPosition
    Vector2(1.0f, 1.0f)            // maskScale
);
control.SetRenderEffect(maskEffect);
```

#### 주요 기능:
- Alpha/Luminance 마스킹 모드
- 마스크 위치 및 크기 조절
- 타겟/소스 렌더링 최적화 옵션

### 2.3 Shadow 시스템

Shadow 시스템은 BoxShadow와 InnerShadow를 모두 지원하며, 여러 그림자를 동시에 적용할 수 있습니다.

```cpp
// 다중 Shadow 적용
Toolkit::Shadow shadow1 = Toolkit::Shadow::New();
shadow1.SetProperty(Toolkit::Shadow::Property::COLOR, Color::BLACK);
shadow1.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(2.0f, 2.0f));

Toolkit::InnerShadow shadow2 = Toolkit::InnerShadow::New();
shadow2.SetProperty(Toolkit::Shadow::Property::COLOR, Color::WHITE);
shadow2.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-1.0f, -1.0f));

control.AddShadow(shadow1);
control.AddShadow(shadow2);
```

#### 주요 기능:
- BoxShadow와 InnerShadow 지원
- 다중 Shadow 적용
- CornerRadius 자동 상속
- Neumorphism 구현 지원

### 2.4 Text Cutout 효과

Text Cutout은 텍스트 모양을 통해 배경이 보이도록 하는 효과로, 현대적 디자인에서 자주 사용됩니다.

```cpp
// TextLabel에서 Cutout 효과 사용
Toolkit::TextLabel textLabel = Toolkit::TextLabel::New();
textLabel.SetProperty(Toolkit::TextLabel::Property::TEXT, "Hello World");
textLabel.SetProperty(Toolkit::DevelTextLabel::Property::CUTOUT, true);

// 배경 설정
textLabel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundColorMap);
```

#### 주요 기능:
- 텍스트 모양의 배경 표시
- MaskEffect를 통한 내부 구현
- 비동기 렌더링 지원

## 3. 렌더링 파이프라인

### 3.1 렌더링 흐름

1. **이펙트 적용**: Control에 RenderEffect 설정
2. **오프스크린 렌더링**: 필요시 Framebuffer에 렌더링
3. **이펙트 처리**: 셰이더를 통한 비주얼 이펙트 적용
4. **결과 합성**: 처리된 결과를 최종 화면에 합성
5. **RenderTask 재정렬**: BackgroundBlurEffect를 위한 자동 정렬

### 3.2 오프스크린 렌더링

Visual Effects는 Framebuffer Object(FBO)를 사용한 오프스크린 렌더링을 지원합니다.

```cpp
// 오프스크린 렌더링 설정
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::ALWAYS);

// 렌더링 결과 가져오기
if(control.GetProperty<DevelControl::OffScreenRenderingType>() == 
   DevelControl::OffScreenRenderingType::REFRESH_ONCE)
{
    Dali::Texture outputTexture = control.GetOffScreenRenderingOutput();
}
```

#### 오프스크린 렌더링 모드:
- **DISABLED**: 오프스크린 렌더링 사용 안 함
- **ALWAYS**: 매 프레임 오프스크린 렌더링
- **REFRESH_ONCE**: 한 번만 렌더링 후 텍스처로 재사용

### 3.3 RenderTask 재정렬

BackgroundBlurEffect와 같은 일부 이펙트는 올바른 렌더링을 위해 RenderTask의 순서를 자동으로 재정렬합니다.

```cpp
// RenderTaskList에서 자동 재정렬 수행
void RenderTaskList::ReorderTasks(Dali::Internal::LayerList& layerList)
{
    // OffScreenRenderable을 기반으로 RenderTask 순서 재정렬
    // BackgroundBlurEffect가 올바르게 렌더링되도록 보장
}
```

## 4. 좌표계 및 공간

### 4.1 좌표계

Visual Effects는 DALi의 표준 좌표계를 따릅니다:
- **X축**: 오른쪽 방향
- **Y축**: 아래쪽 방향
- **Z축**: 앞쪽 방향 (화면 밖)

### 4.2 공간 관리

각 Control은 독립적인 렌더링 공간을 가지며, Visual Effects는 Control의 로컬 좌표계에서 작동합니다. 오프스크린 렌더링 시 Control의 크기와 위치가 정확하게 보존됩니다.

## 5. 성능 최적화

### 5.1 렌더링 최적화

- **오프스크린 렌더링 최적화**: 필요한 경우에만 FBO 사용
- **RenderTask 재정렬**: 최소한의 재정렬만 수행
- **셰이더 최적화**: 효율적인 셰이더 코드 사용
- **텍스처 캐싱**: 반복 사용되는 텍스처 캐싱

### 5.2 메모리 관리

- **자동 메모리 정리**: RenderEffect 제거 시 관련 리소스 자동 정리
- **WeakReference 사용**: NUI에서 순환 참조 방지
- **오프스크린 텍스처 관리**: 불필요한 텍스처 즉시 해제

## 6. 호환성 및 요구사항

### 6.1 시스템 요구사항

- **OpenGL ES**: GLSL 버전 3.0 이상
- **메모리**: 최소 128MB VRAM 권장
- **프로세서**: OpenGL ES 3.0 지원 GPU

### 6.2 지원 이펙트

#### 마스킹 이펙트
- **MaskEffect**: Alpha/Luminance 모드 지원

#### 블러 이펙트
- **BackgroundBlurEffect**: 배경 블러
- **GaussianBlurEffect**: 가우시안 블러

#### 그림자 이펙트
- **BoxShadow**: 외부 그림자
- **InnerShadow**: 내부 그림자
- **다중 Shadow**: 여러 그림자 동시 적용

#### 텍스트 이펙트
- **Text Cutout**: 텍스트 모양의 배경 표시
- **Text Masking**: 텍스트를 통한 마스킹

## 7. 사용 사례

### 7.1 추천 사용 시나리오

- **Neumorphism**: 부드러운 외부/내부 그림자 조합
- **Glassmorphism**: 배경 블러와 투명도 조합
- **현대적 UI**: 다양한 비주얼 이펙트의 조합
- **텍스트 디자인**: Cutout과 마스킹을 통한 창의적 텍스트 표현

### 7.2 제한 사항

- **성능**: 복잡한 이펙트는 성능에 영향을 줄 수 있음
- **메모리**: 오프스크린 렌더링은 추가 메모리 사용
- **호환성**: 일부 구형 GPU에서는 지원되지 않을 수 있음

## 8. 다음 단계

Visual Effects를 효과적으로 사용하기 위해 다음 문서들을 참고하세요:

1. [컴포넌트 상세 가이드](./Visual_Effects_컴포넌트_상세.md)
2. [아키텍처 상세 분석](./Visual_Effects_아키텍처.md)
3. [DALi vs NUI 비교](./Visual_Effects_DALi_NUI_비교.md)
4. [아키텍처 다이어그램](./Visual_Effects_Architecture_Diagrams.puml)

## 9. 코드 예제

### 9.1 기본 Visual Effects 사용

```cpp
#include <dali-toolkit/public-api/controls/render-effects/render-effect.h>
#include <dali-toolkit/public-api/controls/render-effects/mask-effect.h>

// Control 생성
Toolkit::Control control = Toolkit::Control::New();
control.SetProperty(Actor::Property::SIZE, Vector2(200, 200));
window.Add(control);

// MaskEffect 적용
Toolkit::Control maskControl = Toolkit::Control::New();
maskControl.SetProperty(Actor::Property::SIZE, Vector2(100, 100));
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
control.SetRenderEffect(maskEffect);

// Shadow 추가
Toolkit::Shadow shadow = Toolkit::Shadow::New();
shadow.SetProperty(Toolkit::Shadow::Property::COLOR, Color::BLACK);
shadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(5, 5));
control.AddShadow(shadow);
```

### 9.2 고급 Visual Effects 조합

```cpp
// Glassmorphism 효과 구현
Toolkit::Control glassPanel = Toolkit::Control::New();
glassPanel.SetProperty(Actor::Property::SIZE, Vector2(300, 200));

// 배경 블러 효과
Toolkit::BackgroundBlurEffect blurEffect = 
    Toolkit::BackgroundBlurEffect::New(10.0f);
glassPanel.SetRenderEffect(blurEffect);

// 반투명 배경
Property::Map backgroundMap;
backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                     Vector4(1.0f, 1.0f, 1.0f, 0.3f));
glassPanel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);

// 부드러운 그림자
Toolkit::Shadow softShadow = Toolkit::Shadow::New();
softShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.2f));
softShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(0, 10));
softShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 15.0f);
glassPanel.AddShadow(softShadow);

window.Add(glassPanel);
```

이 예제들은 Visual Effects 시스템의 기본 사용법과 고급 기능들을 보여주며, 현대적 UI 디자인을 구현하는 방법을 설명합니다.
