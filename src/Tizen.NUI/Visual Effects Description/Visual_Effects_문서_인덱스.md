# DALi Visual Effects 완전 문서

## 문서 개요

본 문서 집합은 DALi Visual Effects 시스템에 대한 포괄적인 가이드를 제공합니다. Visual Effects의 아키텍처, 컴포넌트, 사용법, 그리고 DALi와 NUI 버전의 차이점을 상세히 설명합니다.

## 문서 구조

### 1. [Visual Effects 개요](./Visual_Effects_개요.md)
Visual Effects의 기본 개념과 아키텍처 철학을 소개합니다.

**주요 내용:**
- Visual Effects 정의 및 목적
- 주요 특징과 아키텍처 철학
- 핵심 컴포넌트 구조
- 렌더링 파이프라인
- 오프스크린 렌더링 시스템
- 성능 최적화 기본 원칙
- 호환성 및 요구사항
- 사용 사례 및 제한 사항

### 2. [Visual Effects 컴포넌트 상세 가이드](./Visual_Effects_컴포넌트_상세.md)
Visual Effects의 주요 컴포넌트들을 상세하게 설명합니다.

**주요 내용:**
- **RenderEffect**: 비주얼 이펙트의 기본 인터페이스
- **MaskEffect**: 마스킹 효과 구현
- **BackgroundBlurEffect**: 배경 블러 효과
- **GaussianBlurEffect**: 가우시안 블러 효과
- **Shadow 시스템**: InnerShadow와 다중 Shadow 지원
- **Text Cutout 효과**: 텍스트 모양의 배경 표시
- **OffScreenRendering**: 오프스크린 렌더링 옵션
- 성능 최적화 가이드
- 디버깅 및 문제 해결
- 모범 사례

### 3. [Visual Effects 아키텍처](./Visual_Effects_아키텍처.md)
Visual Effects 시스템의 구조와 동작 방식을 상세히 분석합니다.

**주요 내용:**
- 아키텍처 설계 원칙
- RenderEffect 상세 분석
- 오프스크린 렌더링 시스템
- RenderTask 재정렬 메커니즘
- 이펙트 체이닝 및 조합
- 메모리 관리 시스템
- 커스터마이징 및 확장
- 성능 최적화
- 에러 핸들링 및 디버깅

### 4. [DALi vs NUI Visual Effects 비교](./Visual_Effects_DALi_NUI_비교.md)
두 프레임워크의 관계와 차이점을 종합적으로 분석합니다.

**주요 내용:**
- 아키텍처 관계와 바인딩 레이어
- 핵심 차이점 (언어, 플랫폼, 성능)
- API 비교 분석 (RenderEffect, MaskEffect, Shadow)
- 메모리 관리 차이
- 성능 비교 및 최적화 전략
- 기능 차이점 (NUI 전용, DALi 전용)
- 사용 사례별 권장 사항
- 마이그레이션 가이드
- 상호 운용성

### 5. [Visual Effects 아키텍처 다이어그램](./Visual_Effects_Architecture_Diagrams.puml)
Visual Effects 아키텍처를 시각화한 PlantUML 다이어그램 모음입니다.

**포함된 다이어그램:**
- Visual Effects 아키텍처 전체 구조도
- 클래스 상속 계층도
- 오프스크린 렌더링 흐름
- RenderTask 재정렬 작업 흐름
- DALi vs NUI 관계도

## 빠른 시작 가이드

### Visual Effects 기본 사용 예제

#### C++ (DALi)
```cpp
#include <dali-toolkit/public-api/controls/render-effects/render-effect.h>
#include <dali-toolkit/public-api/controls/render-effects/mask-effect.h>

// RenderEffect 생성 및 적용
Toolkit::Control control = Toolkit::Control::New();
Toolkit::MaskEffect maskEffect = Toolkit::MaskEffect::New(maskControl);
control.SetRenderEffect(maskEffect);

// Shadow 추가
control.AddShadow(Dali::Toolkit::Shadow::New());
control.AddShadow(Dali::Toolkit::InnerShadow::New());

// OffScreenRendering 활성화
control.SetProperty(DevelControl::Property::OFFSCREEN_RENDERING, 
                    DevelControl::OffScreenRenderingType::ALWAYS);
```

#### C# (NUI)
```csharp
using Tizen.NUI;
using Tizen.NUI.RenderEffects;

// RenderEffect 생성 및 적용
View view = new View();
MaskEffect maskEffect = RenderEffect.CreateMaskEffect(maskView);
view.SetRenderEffect(maskEffect);

// Shadow 추가
view.AddShadow(new Shadow());
view.AddShadow(new InnerShadow());

// OffScreenRendering 활성화
view.OffScreenRendering = View.OffScreenRenderingType.Always;
```

## 기술 사양

### 시스템 요구사항
- **OpenGL ES**: GLSL 버전 3.0 이상
- **메모리**: 최소 128MB VRAM 권장
- **프로세서**: OpenGL ES 3.0 지원 GPU

### 지원 이펙트
- **마스킹**: Alpha, Luminance 모드
- **블러**: Background Blur, Gaussian Blur
- **그림자**: Box Shadow, Inner Shadow, 다중 Shadow
- **텍스트 효과**: Cutout, Masking

### 성능 특성
- **최대 Shadow 수**: View당 제한 없음
- **오프스크린 렌더링**: Framebuffer 기반
- **RenderTask 재정렬**: 자동 정렬 시스템

## 선택 가이드

### DALi Visual Effects를 선택해야 하는 경우
- 최고 성능이 요구되는 그래픽 애플리케이션
- 실시간 비주얼 이펙트 처리
- 메모리가 제한적인 임베디드 시스템
- 저수준 메모리 제어가 필요한 경우

### NUI Visual Effects를 선택해야 하는 경우
- 빠른 개발과 높은 생산성이 중요한 경우
- 비즈니스/엔터프라이즈 애플리케이션
- 현대적 UI 디자인 구현
- .NET 생태계를 활용해야 하는 경우

## 학습 경로

### 초급
1. [Visual Effects 개요](./Visual_Effects_개요.md) - 기본 개념 이해
2. [Visual Effects 컴포넌트 상세 가이드](./Visual_Effects_컴포넌트_상세.md) - 기본 컴포넌트 사용법

### 중급
1. [Visual Effects 아키텍처](./Visual_Effects_아키텍처.md) - 시스템 구조 이해
2. [Visual Effects 아키텍처 다이어그램](./Visual_Effects_Architecture_Diagrams.puml) - 아키텍처 시각화

### 고급
1. [DALi vs NUI Visual Effects 비교](./Visual_Effects_DALi_NUI_비교.md) - 두 프레임워크 심층 비교
2. 성능 최적화 및 커스터마이징

## 참고 자료

### 공식 문서
- [DALi 공식 문서](https://docs.tizen.org/application/native/api/6.0/group__dali.html)
- [Tizen .NET 문서](https://docs.tizen.org/application/dotnet/api/6.0/)

### 관련 기술
- [OpenGL ES 3.0](https://www.khronos.org/registry/OpenGL/index_es.php)
- [Frame Buffer Object](https://www.khronos.org/registry/OpenGL/extensions/EXT/EXT_framebuffer_object.txt)

### 도구
- [PlantUML](https://plantuml.com/) - 아키텍처 다이어그램 생성

## 기여 및 피드백

본 문서는 DALi Visual Effects 소스 코드 분석을 기반으로 작성되었습니다. 오타, 오류, 개선 사항이 있으면 알려주시기 바랍니다.

### 문서 버전
- **버전**: 1.0
- **작성일**: 2025년 11월
- **기반 소스**: DALi Visual Effects v2.3.28 이상

### 라이선스
본 문서는 Apache License 2.0을 따릅니다.

---

**참고**: 이 문서 집합은 DALi Visual Effects의 현재 버전을 기준으로 작성되었으며, 버전 업데이트에 따라 내용이 변경될 수 있습니다. 최신 정보는 공식 문서를 참고하시기 바랍니다.
