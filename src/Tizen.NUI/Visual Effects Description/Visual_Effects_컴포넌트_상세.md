# Visual Effects 컴포넌트 상세 가이드

## 1. RenderEffect

### 1.1 개요

RenderEffect는 모든 비주얼 이펙트의 기본 인터페이스 클래스입니다. 이펙트의 생명주기를 관리하고 Toolkit::Control에 적용되는 방식을 정의합니다. RenderEffect는 추상 클래스이므로 직접 인스턴스화할 수 없으며, MaskEffect와 같은 구체적인 서브클래스를 통해 사용합니다.

### 1.2 주요 메서드

#### 기본 생명주기 메서드

```cpp
// 이펙트 활성화
void Activate();

// 이펙트 비활성화
void Deactivate();

// 이펙트 새로고침
void Refresh();

// 활성화 상태 확인
bool IsActivated();
```

#### Control 적용 메서드

```cpp
// Control에 RenderEffect 설정
control.SetRenderEffect(renderEffect);

// Control에서 RenderEffect 제거
control.ClearRenderEffect();

// 현재 설정된 RenderEffect 가져오기
RenderEffect currentEffect = control.GetRenderEffect();
```

### 1.3 생명주기 관리

RenderEffect의 생명주기는 다음과 같은 단계를 따릅니다:

1. **생성**: 서브클래스의 정적 팩토리 메서드로 생성
2. **설정**: Control.SetRenderEffect()를 통해 Control에 설정
3. **활성화**: 자동 또는 수동으로 Activate() 호출
4. **동작**: 렌더링 파이프라인에서 이펙트 적용
5. **비활성화**: Deactivate() 호출 또는 Control에서 제거
6. **소멸**: 참조 카운트가 0이 되면 자동 소멸

### 1.4 오프스크린 렌더링 타입

각 RenderEffect는 특정 오프스크린 렌더링 타입을 가집니다:

```cpp
enum class OffScreenRenderableType
{
  NONE = 0,      // 오프스크린 렌더링 사용 안 함
  BACKWARD = 1,  // 역방향 오프스크린 렌더링
  FORWARD = 2    // 순방향 오프스크린 렌더링
};
```

## 2. MaskEffect

### 2.1 개요

MaskEffect는 다른 Control을 마스크로 사용하여 소유 Control의 일부를 가리거나 표시하는 효과를 제공합니다. 이를 통해 복잡한 모양의 클리핑이나 독창적인 비주얼 효과를 구현할 수 있습니다.

### 2.2 마스킹 모드

#### Alpha 모드
마스크 텍스처의 알파 채널을 사용하여 마스킹을 적용합니다.

```cpp
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(
    maskControl, 
    Toolkit::MaskEffect::ALPHA
);
```

#### Luminance 모드
마스크 텍스처의 RGB 값을 밝기로 변환하여 마스크 값으로 사용합니다.

```cpp
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(
    maskControl, 
    Toolkit::MaskEffect::LUMINANCE
);
```

### 2.3 생성 및 설정

#### 기본 생성

```cpp
// 기본 설정으로 MaskEffect 생성
Toolkit::Control maskControl = Toolkit::Control::New();
maskControl.SetProperty(Actor::Property::SIZE, Vector2(100, 100));

Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
control.SetRenderEffect(maskEffect);
```

#### 상세 설정

```cpp
// 상세 파라미터로 MaskEffect 생성
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(
    maskControl,                           // 마스크로 사용할 Control
    Toolkit::MaskEffect::ALPHA,            // 마스킹 모드
    Vector2(0.0f, 0.0f),                  // 마스크 위치
    Vector2(1.0f, 1.0f)                   // 마스크 크기 조절
);
```

### 2.4 최적화 옵션

MaskEffect는 성능 최적화를 위한 렌더링 최적화 옵션을 제공합니다:

```cpp
// 타겟 렌더링 최적화
maskEffect.SetTargetMaskOnce(true);  // 타겟을 한 번만 렌더링

// 소스 렌더링 최적화
maskEffect.SetSourceMaskOnce(true);  // 소스를 한 번만 렌더링

// 현재 설정 확인
bool targetOnce = maskEffect.GetTargetMaskOnce();
bool sourceOnce = maskEffect.GetSourceMaskOnce();
```

### 2.5 사용 예제

#### 원형 마스킹

```cpp
// 원형 마스크 생성
Toolkit::Control circleMask = Toolkit::Control::New();
circleMask.SetProperty(Actor::Property::SIZE, Vector2(150, 150));

// 원형 Visual 설정
Property::Map circleVisual;
circleVisual.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
circleVisual.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, Color::WHITE);
circleVisual.Insert(Toolkit::DevelVisual::Property::CORNER_RADIUS, Vector3(75, 75, 0));
circleMask.SetProperty(Toolkit::Control::Property::BACKGROUND, circleVisual);

// MaskEffect 적용
Toolkit::MaskEffect circleMaskEffect = Toolkit::MaskEffect::New(
    circleMask, 
    Toolkit::MaskEffect::ALPHA
);

// 이미지에 원형 마스크 적용
Toolkit::ImageView imageView = Toolkit::ImageView::New("image.jpg");
imageView.SetProperty(Actor::Property::SIZE, Vector2(200, 200));
imageView.SetRenderEffect(circleMaskEffect);
```

#### 그라데이션 마스킹

```cpp
// 그라데이션 마스크 생성
Toolkit::Control gradientMask = Toolkit::Control::New();
gradientMask.SetProperty(Actor::Property::SIZE, Vector2(200, 200));

// 그라데이션 Visual 설정
Property::Map gradientVisual;
gradientVisual.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::GRADIENT);

Property::Array stopOffsets;
stopOffsets.PushBack(0.0f);
stopOffsets.PushBack(1.0f);

Property::Array stopColors;
stopColors.Push_back(Color::WHITE);
stopColors.Push_back(Color::TRANSPARENT);

gradientVisual.Insert(Toolkit::GradientVisual::Property::STOP_OFFSET, stopOffsets);
gradientVisual.Insert(Toolkit::GradientVisual::Property::STOP_COLOR, stopColors);
gradientMask.SetProperty(Toolkit::Control::Property::BACKGROUND, gradientVisual);

// MaskEffect 적용
Toolkit::MaskEffect gradientMaskEffect = Toolkit::MaskEffect::New(
    gradientMask, 
    Toolkit::MaskEffect::ALPHA
);
```

## 3. BackgroundBlurEffect

### 3.1 개요

BackgroundBlurEffect는 Control의 배경을 블러 처리하여 Glassmorphism 효과를 구현합니다. 이 이펙트는 Control 뒤에 있는 콘텐츠를 블러하여 반투명 유리 효과를 만듭니다.

### 3.2 생성 및 설정

```cpp
// BackgroundBlurEffect 생성
float blurRadius = 15.0f;
Toolkit::BackgroundBlurEffect blurEffect = 
    Toolkit::BackgroundBlurEffect::New(blurRadius);

// Control에 적용
Toolkit::Control glassPanel = Toolkit::Control::New();
glassPanel.SetProperty(Actor::Property::SIZE, Vector2(300, 200));
glassPanel.SetRenderEffect(blurEffect);
```

### 3.3 블러 강도 조절

블러 강도는 blurRadius 파라미터로 조절합니다:

```cpp
// 약한 블러
Toolkit::BackgroundBlurEffect lightBlur = 
    Toolkit::BackgroundBlurEffect::New(5.0f);

// 강한 블러
Toolkit::BackgroundBlurEffect strongBlur = 
    Toolkit::BackgroundBlurEffect::New(25.0f);
```

### 3.4 Glassmorphism 구현

```cpp
// Glassmorphism 패널 생성
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
                     Vector4(1.0f, 1.0f, 1.0f, 0.2f));
glassPanel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);

// 테두리
Property::Map borderMap;
borderMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::BORDER);
borderMap.Insert(Toolkit::BorderVisual::Property::COLOR, Color::WHITE);
borderMap.Insert(Toolkit::BorderVisual::Property::SIZE, 1.0f);
glassPanel.SetProperty(Toolkit::Control::Property::BORDER, borderMap);

// CornerRadius
glassPanel.SetProperty(Toolkit::Control::Property::CORNER_RADIUS, Vector3(15, 15, 0));
```

## 4. GaussianBlurEffect

### 4.1 개요

GaussianBlurEffect는 Control 자체를 가우시안 블러 처리합니다. BackgroundBlurEffect와 달리 배경이 아닌 Control의 콘텐츠를 블러합니다.

### 4.2 생성 및 설정

```cpp
// GaussianBlurEffect 생성
float blurRadius = 8.0f;
Toolkit::GaussianBlurEffect gaussianBlur = 
    Toolkit::GaussianBlurEffect::New(blurRadius);

// Control에 적용
Toolkit::Control blurControl = Toolkit::Control::New();
blurControl.SetProperty(Actor::Property::SIZE, Vector2(200, 200));
blurControl.SetRenderEffect(gaussianBlur);
```

### 4.3 사용 예제

```cpp
// 블러된 배경 위에 텍스트 표시
Toolkit::Control background = Toolkit::Control::New();
background.SetProperty(Actor::Property::SIZE, Vector2(300, 200));

// 배경 이미지
Property::Map backgroundImage;
backgroundImage.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::IMAGE);
backgroundImage.Insert(Toolkit::ImageVisual::Property::URL, "background.jpg");
background.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundImage);

// 가우시안 블러 적용
Toolkit::GaussianBlurEffect blurEffect = 
    Toolkit::GaussianBlurEffect::New(12.0f);
background.SetRenderEffect(blurEffect);

// 텍스트 추가
Toolkit::TextLabel textLabel = Toolkit::TextLabel::New();
textLabel.SetProperty(Toolkit::TextLabel::Property::TEXT, "Blurred Background");
textLabel.SetProperty(Actor::Property::POSITION, Vector2(0, 0));
textLabel.SetProperty(Actor::Property::SIZE, Vector2(300, 200));
background.Add(textLabel);
```

## 5. Shadow 시스템

### 5.1 개요

Shadow 시스템은 BoxShadow와 InnerShadow를 모두 지원하며, 여러 그림자를 동시에 적용할 수 있습니다. 이를 통해 Neumorphism과 같은 현대적 디자인 효과를 구현할 수 있습니다.

### 5.2 Shadow 속성

#### 기본 속성

```cpp
// Shadow 생성
Toolkit::Shadow shadow = Toolkit::Shadow::New();

// 색상 설정
shadow.SetProperty(Toolkit::Shadow::Property::COLOR, Color::BLACK);

// 오프셋 설정
shadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(5.0f, 5.0f));

// 블러 반경 설정
shadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 10.0f);
```

#### 고급 속성

```cpp
// 확장 설정
shadow.SetProperty(Toolkit::Shadow::Property::SPREAD, 2.0f);

// 투명도 설정
Vector4 shadowColor = shadow.GetProperty<Vector4>(Toolkit::Shadow::Property::COLOR);
shadowColor.a = 0.5f;  // 50% 투명도
shadow.SetProperty(Toolkit::Shadow::Property::COLOR, shadowColor);
```

### 5.3 InnerShadow

InnerShadow는 Control의 내부에 그림자 효과를 적용합니다.

```cpp
// InnerShadow 생성
Toolkit::InnerShadow innerShadow = Toolkit::InnerShadow::New();

// 밝은 내부 그림자 (Neumorphism 효과)
innerShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Color::WHITE);
innerShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-2.0f, -2.0f));
innerShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 5.0f);
```

### 5.4 다중 Shadow 적용

여러 그림자를 동시에 적용하여 복잡한 효과를 만들 수 있습니다.

```cpp
Toolkit::Control control = Toolkit::Control::New();
control.SetProperty(Actor::Property::SIZE, Vector2(200, 200));

// 첫 번째 그림자 (어두운 외부 그림자)
Toolkit::Shadow shadow1 = Toolkit::Shadow::New();
shadow1.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.3f));
shadow1.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(8, 8));
shadow1.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 15.0f);
control.AddShadow(shadow1);

// 두 번째 그림자 (밝은 내부 그림자)
Toolkit::InnerShadow shadow2 = Toolkit::InnerShadow::New();
shadow2.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(1, 1, 1, 0.5f));
shadow2.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-4, -4));
shadow2.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 8.0f);
control.AddShadow(shadow2);

// 세 번째 그림자 (미세한 외부 그림자)
Toolkit::Shadow shadow3 = Toolkit::Shadow::New();
shadow3.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.2f));
shadow3.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(2, 2));
shadow3.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 3.0f);
control.AddShadow(shadow3);
```

### 5.5 Neumorphism 구현

```cpp
// Neumorphism 버튼 생성
Toolkit::Control neuButton = Toolkit::Control::New();
neuButton.SetProperty(Actor::Property::SIZE, Vector2(120, 50));

// 배경색
Property::Map backgroundMap;
backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                     Vector4(0.9f, 0.9f, 0.95f, 1.0f));
neuButton.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);

// CornerRadius
neuButton.SetProperty(Toolkit::Control::Property::CORNER_RADIUS, Vector3(25, 25, 0));

// 밝은 내부 그림자 (왼쪽 위)
Toolkit::InnerShadow lightShadow = Toolkit::InnerShadow::New();
lightShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(1, 1, 1, 0.8f));
lightShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-3, -3));
lightShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 5.0f);
neuButton.AddShadow(lightShadow);

// 어두운 외부 그림자 (오른쪽 아래)
Toolkit::Shadow darkShadow = Toolkit::Shadow::New();
darkShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.3f));
darkShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(5, 5));
darkShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 8.0f);
neuButton.AddShadow(darkShadow);
```

## 6. Text Cutout 효과

### 6.1 개요

Text Cutout은 텍스트 모양을 통해 배경이 보이도록 하는 효과입니다. 이는 MaskEffect를 내부적으로 사용하여 구현됩니다.

### 6.2 TextLabel에서의 사용

```cpp
// TextLabel 생성
Toolkit::TextLabel textLabel = Toolkit::TextLabel::New();
textLabel.SetProperty(Actor::Property::SIZE, Vector2(300, 100));
textLabel.SetProperty(Toolkit::TextLabel::Property::TEXT, "CUTOUT TEXT");
textLabel.SetProperty(Toolkit::TextLabel::Property::POINT_SIZE, 24.0f);

// Cutout 효과 활성화
textLabel.SetProperty(Toolkit::DevelTextLabel::Property::CUTOUT, true);

// 배경 설정
Property::Map backgroundMap;
backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                     Vector4(0.2f, 0.3f, 0.8f, 1.0f));
textLabel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);
```

### 6.3 고급 Cutout 효과

```cpp
// 그라데이션 배경과 Cutout 텍스트
Toolkit::TextLabel cutoutText = Toolkit::TextLabel::New();
cutoutText.SetProperty(Actor::Property::SIZE, Vector2(400, 150));
cutoutText.SetProperty(Toolkit::TextLabel::Property::TEXT, "GRADIENT CUTOUT");
cutoutText.SetProperty(Toolkit::TextLabel::Property::POINT_SIZE, 32.0f);
cutoutText.SetProperty(Toolkit::DevelTextLabel::Property::CUTOUT, true);

// 그라데이션 배경
Property::Map gradientBackground;
gradientBackground.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::GRADIENT);

Property::Array stopOffsets;
stopOffsets.PushBack(0.0f);
stopOffsets.PushBack(0.5f);
stopOffsets.PushBack(1.0f);

Property::Array stopColors;
stopColors.PushBack(Vector4(1.0f, 0.2f, 0.4f, 1.0f));  // 빨강
stopColors.Push_back(Vector4(0.2f, 0.8f, 1.0f, 1.0f));  // 파랑
stopColors.Push_back(Vector4(1.0f, 0.8f, 0.2f, 1.0f));  // 노랑

gradientBackground.Insert(Toolkit::GradientVisual::Property::STOP_OFFSET, stopOffsets);
gradientBackground.Insert(Toolkit::GradientVisual::Property::STOP_COLOR, stopColors);
cutoutText.SetProperty(Toolkit::Control::Property::BACKGROUND, gradientBackground);
```

## 7. OffScreenRendering

### 7.1 개요

OffScreenRendering은 Control을 Framebuffer에 렌더링하여 통일된 corner radius, clipping 등의 효과를 적용할 수 있게 합니다.

### 7.2 렌더링 모드

```cpp
enum class OffScreenRenderingType
{
  DISABLED = 0,    // 오프스크린 렌더링 사용 안 함
  ALWAYS = 1,      // 매 프레임 오프스크린 렌더링
  REFRESH_ONCE = 2 // 한 번만 렌더링 후 텍스처로 재사용
};
```

### 7.3 설정 및 사용

```cpp
// 항상 오프스크린 렌더링
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::ALWAYS);

// 한 번만 렌더링
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::REFRESH_ONCE);

// 렌더링 결과 가져오기
if(control.GetProperty<DevelControl::OffScreenRenderingType>() == 
   DevelControl::OffScreenRenderingType::REFRESH_ONCE)
{
    Dali::Texture outputTexture = control.GetOffScreenRenderingOutput();
    // 텍스처를 다른 곳에서 재사용
}
```

### 7.4 사용 사례

#### 복잡한 Clipping

```cpp
// 복잡한 모양의 Control
Toolkit::Control complexControl = Toolkit::Control::New();
complexControl.SetProperty(Actor::Property::SIZE, Vector2(200, 200));

// 오프스크린 렌더링 활성화
complexControl.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                          DevelControl::OffScreenRenderingType::ALWAYS);

// 복잡한 CornerRadius
complexControl.SetProperty(Toolkit::Control::Property::CORNER_RADIUS, 
                          Vector3(50, 30, 20));

// 자식 Control들도 정확하게 클리핑됨
Toolkit::Control child1 = Toolkit::Control::New();
child1.SetProperty(Actor::Property::SIZE, Vector2(180, 80));
child1.SetProperty(Actor::Property::POSITION, Vector2(10, 10));
complexControl.Add(child1);
```

## 8. 성능 최적화 가이드

### 8.1 렌더링 최적화

#### 오프스크린 렌더링 최적화

```cpp
// 정적인 콘텐츠는 REFRESH_ONCE 사용
staticControl.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                          DevelControl::OffScreenRenderingType::REFRESH_ONCE);

// 동적인 콘텐츠만 ALWAYS 사용
dynamicControl.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                          DevelControl::OffScreenRenderingType::ALWAYS);
```

#### MaskEffect 최적화

```cpp
// 변하지 않는 마스크는 최적화 옵션 사용
maskEffect.SetTargetMaskOnce(true);   // 타겟을 한 번만 렌더링
maskEffect.SetSourceMaskOnce(true);   // 소스를 한 번만 렌더링
```

#### Shadow 최적화

```cpp
// 너무 많은 Shadow는 성능에 영향을 줌
// 필요한 최소한의 Shadow만 사용
control.AddShadow(essentialShadow1);
control.AddShadow(essentialShadow2);
// 과도한 Shadow 추가는 피할 것
```

### 8.2 메모리 관리

#### RenderEffect 생명주기 관리

```cpp
{
    // 스코프 내에서 RenderEffect 사용
    Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
    control.SetRenderEffect(maskEffect);
    
    // 사용 후 명시적 제거
    control.ClearRenderEffect();
} // maskEffect는 자동으로 소멸
```

#### 오프스크린 텍스처 관리

```cpp
// REFRESH_ONCE 모드 사용 시 텍스처 메모리 관리
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::REFRESH_ONCE);

// 더 이상 필요 없을 때 오프스크린 렌더링 비활성화
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::DISABLED);
```

## 9. 디버깅 및 문제 해결

### 9.1 일반적인 문제

#### RenderEffect가 적용되지 않는 경우

```cpp
// Control이 스테이지에 추가되었는지 확인
if(control.GetProperty<bool>(Actor::Property::CONNECTED_TO_SCENE))
{
    control.SetRenderEffect(effect);
}

// 이펙트가 올바르게 활성화되었는지 확인
if(effect && effect.IsActivated())
{
    // 이펙트가 정상적으로 동작
}
```

#### 오프스크린 렌더링 문제

```cpp
// 오프스크린 렌더링 타입 확인
auto renderType = control.GetProperty<DevelControl::OffScreenRenderingType>(
    DevelControl::Property::OFFSCREEN_RENDERING);

if(renderType == DevelControl::OffScreenRenderingType::DISABLED)
{
    // 오프스크린 렌더링이 비활성화됨
}
```

### 9.2 성능 모니터링

```cpp
// 렌더링 성능 확인
Dali::Integration::RenderTracker& renderTracker = 
    Dali::Integration::RenderTracker::Get();

// 프레임 시간 모니터링
float frameTime = renderTracker.GetFrameTime();
if(frameTime > 16.67f)  // 60fps 이하
{
    // 성능 최적화 필요
}
```

## 10. 모범 사례

### 10.1 디자인 패턴

#### Glassmorphism 패턴

```cpp
Toolkit::Control CreateGlassPanel(Vector2 size, float blurRadius, float opacity)
{
    Toolkit::Control panel = Toolkit::Control::New();
    panel.SetProperty(Actor::Property::SIZE, size);
    
    // 배경 블러
    Toolkit::BackgroundBlurEffect blurEffect = 
        Toolkit::BackgroundBlurEffect::New(blurRadius);
    panel.SetRenderEffect(blurEffect);
    
    // 반투명 배경
    Property::Map backgroundMap;
    backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
    backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                         Vector4(1.0f, 1.0f, 1.0f, opacity));
    panel.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);
    
    return panel;
}
```

#### Neumorphism 패턴

```cpp
Toolkit::Control CreateNeuButton(Vector2 size, Vector3 cornerRadius)
{
    Toolkit::Control button = Toolkit::Control::New();
    button.SetProperty(Actor::Property::SIZE, size);
    button.SetProperty(Toolkit::Control::Property::CORNER_RADIUS, cornerRadius);
    
    // 배경색
    Property::Map backgroundMap;
    backgroundMap.Insert(Toolkit::Visual::Property::TYPE, Toolkit::Visual::COLOR);
    backgroundMap.Insert(Toolkit::ColorVisual::Property::MIX_COLOR, 
                         Vector4(0.9f, 0.9f, 0.95f, 1.0f));
    button.SetProperty(Toolkit::Control::Property::BACKGROUND, backgroundMap);
    
    // 밝은 내부 그림자
    Toolkit::InnerShadow lightShadow = Toolkit::InnerShadow::New();
    lightShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(1, 1, 1, 0.8f));
    lightShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(-3, -3));
    lightShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 5.0f);
    button.AddShadow(lightShadow);
    
    // 어두운 외부 그림자
    Toolkit::Shadow darkShadow = Toolkit::Shadow::New();
    darkShadow.SetProperty(Toolkit::Shadow::Property::COLOR, Vector4(0, 0, 0, 0.3f));
    darkShadow.SetProperty(Toolkit::Shadow::Property::OFFSET, Vector2(5, 5));
    darkShadow.SetProperty(Toolkit::Shadow::Property::BLUR_RADIUS, 8.0f);
    button.AddShadow(darkShadow);
    
    return button;
}
```

### 10.2 성능 최적화 패턴

#### 이펙트 풀링

```cpp
class EffectPool
{
public:
    static Toolkit::MaskEffect GetMaskEffect(Toolkit::Control maskControl)
    {
        // 재사용 가능한 MaskEffect 반환 또는 새로 생성
        for(auto& effect : mMaskEffects)
        {
            if(!effect.IsUsed())
            {
                effect.Reset(maskControl);
                return effect.GetHandle();
            }
        }
        
        // 새로운 MaskEffect 생성
        auto newEffect = PooledMaskEffect::New(maskControl);
        mMaskEffects.push_back(newEffect);
        return newEffect.GetHandle();
    }
    
private:
    std::vector<PooledMaskEffect> mMaskEffects;
};
```

이 가이드는 Visual Effects 시스템의 각 컴포넌트를 상세히 설명하며, 효과적인 사용법과 최적화 방법을 제공합니다.
