# DALi Accessibility 통합 문서

---

## Part 1: DALi Accessibility 현황 분석

# DALi Accessibility 분석

[span_0](start_span)이 문서는 DALi (Dynamic Animation Library)의 Accessibility(접근성) 프레임워크에 대한 상세한 분석을 다룹니다[span_0](end_span).
[span_1](start_span)동작 방식, 전체 구조, 클래스 구조 및 관계를 설명하며, 이해를 돕기 위해 다이어그램을 포함합니다[span_1](end_span).

## 1. 개요 (Overview)

[span_2](start_span)DALi의 Accessibility 프레임워크는 시각 장애인 등을 위한 보조 기술(Assistive Technology, AT) 클라이언트(예: 스크린 리더)와 DALi 애플리케이션 간의 상호작용을 지원합니다[span_2](end_span).
[span_3](start_span)리눅스 환경의 표준 접근성 인터페이스인 **AT-SPI (Assistive Technology Service Provider Interface)** 를 기반으로 구현되어 있으며, DBus를 통해 외부의 접근성 클라이언트와 통신합니다[span_3](end_span).

## 2. 동작 방식 (Operation Mechanism)

DALi Accessibility는 다음과 같은 흐름으로 동작합니다:

1.  **[span_4](start_span)객체 매핑 (Object Mapping)**: DALi의 `Actor`나 `Control`은 내부적으로 `Accessible` 객체와 1:1로 매핑됩니다[span_4](end_span).
    [span_5](start_span)이 `Accessible` 객체는 해당 UI 요소의 정보(이름, 역할, 상태, 자식 관계 등)를 접근성 프레임워크에 제공합니다[span_5](end_span).
2.  **[span_6](start_span)브리지 (Bridge)**: `Bridge` 클래스는 DALi 내부의 `Accessible` 객체들과 외부의 AT-SPI 버스(DBus) 사이를 연결하는 중계자 역할을 합니다[span_6](end_span).
3.  **[span_7](start_span)이벤트 전달 (Event Emission)**: UI 상태가 변경되면(예: 포커스 이동, 버튼 클릭, 값 변경), `Accessible` 객체는 `Bridge`를 통해 AT-SPI 이벤트를 발생시킵니다[span_7](end_span).
4.  **[span_8](start_span)명령 수행 (Action Execution)**: 스크린 리더와 같은 외부 클라이언트가 특정 동작(예: 클릭, 스크롤)을 요청하면, DBus를 통해 `Bridge`로 전달되고, 이는 다시 해당 `Accessible` 객체의 메서드(`DoAction`, `DoGesture` 등)를 호출하여 DALi 내부 로직을 수행합니다[span_8](end_span).

## 3. 전체 구조 (Overall Structure)

[span_9](start_span)전체 구조는 크게 **Application Layer**, **Adaptor/Toolkit Layer**, **System Layer**로 나눌 수 있습니다[span_9](end_span).

* **Application Layer**: 사용자가 작성한 DALi 애플리케이션. [span_10](start_span)`Actor`와 `Control`을 생성하고 배치합니다[span_10](end_span).
* **[span_11](start_span)Adaptor/Toolkit Layer**: Accessibility의 핵심 로직이 존재하는 곳입니다[span_11](end_span).
    * `Accessible`: 모든 접근성 객체의 기본 인터페이스.
    * [span_12](start_span)`ActorAccessible`: `Actor`와 연결된 접근성 객체[span_12](end_span).
    * [span_13](start_span)`ControlAccessible`: `Control`과 연결된 접근성 객체로, `Action` 인터페이스 등을 추가로 구현합니다[span_13](end_span).
    * [span_14](start_span)`Bridge`: AT-SPI 버스와의 통신을 담당하는 싱글톤 객체[span_14](end_span).
* **[span_15](start_span)System Layer**: OS 레벨의 접근성 인프라[span_15](end_span).
    * **[span_16](start_span)AT-SPI Registry**: 접근성 애플리케이션들을 관리[span_16](end_span).
    * **[span_17](start_span)DBus**: 프로세스 간 통신(IPC) 채널[span_17](end_span).
    * **[span_18](start_span)Screen Reader**: 최종 사용자에게 정보를 전달하는 클라이언트 (예: Orca, VoiceOver 등)[span_18](end_span).

### Architecture Diagram (Mermaid)

    ```mermaid
    graph TD
        subgraph "DALi Application"
            App[Application]
            Actor[Actor / Control]
        end

        subgraph "DALi Adaptor / Toolkit"
            AA[ActorAccessible / ControlAccessible]
            Bridge[Accessibility::Bridge]
            AM[AccessibilityManager]
        end

        subgraph "System (OS)"
            DBus((DBus / AT-SPI Bus))
            ScreenReader[Screen Reader / AT Client]
        end

        App --> Actor
        Actor <--> AA
        AA <--> Bridge
        Bridge <--> DBus
        DBus <--> ScreenReader
    ```

## 4. 클래스 구조 및 관계 (Class Structure & Relationships)

[span_19](start_span)DALi Accessibility의 클래스 구조는 상속과 인터페이스 구현을 통해 계층적으로 구성되어 있습니다[span_19](end_span).

### 주요 클래스 (Key Classes)

1.  **`Dali::Accessibility::Accessible`**
    * **[span_20](start_span)정의**: 모든 접근성 객체의 최상위 추상 기본 클래스(Interface)입니다[span_20](end_span).
    * **[span_21](start_span)역할**: 접근성 객체가 갖춰야 할 필수 메서드(`GetName`, `GetRole`, `GetStates`, `GetChildren` 등)를 정의합니다[span_21](end_span).
    * **[span_22](start_span)위치**: `dali-adaptor/dali/devel-api/atspi-interfaces/accessible.h`[span_22](end_span)

2.  **`Dali::Accessibility::ActorAccessible`**
    * **상속**: `Accessible`, `Collection`, `Component`, `ConnectionTracker`, `BaseObjectObserver`
    * **정의**: DALi의 `Actor`와 연결되는 접근성 객체입니다.
    * **역할**:
        * [span_23](start_span)`Actor`의 생명주기를 관찰(`BaseObjectObserver`)하며, `Actor`가 파괴되면 자신도 정리합니다[span_23](end_span).
        * [span_24](start_span)`Actor`의 계층 구조를 `Accessible`의 트리 구조로 매핑합니다 (`GetChildren` 구현)[span_24](end_span).
        * [span_25](start_span)기본적인 화면 좌표(`GetExtents`)와 레이어 정보 등을 제공합니다[span_25](end_span).
    * **[span_26](start_span)위치**: `dali-adaptor/dali/devel-api/adaptor-framework/actor-accessible.h`[span_26](end_span)

3.  **`Dali::Toolkit::DevelControl::ControlAccessible`**
    * **상속**: `ActorAccessible`, `Action` (Virtual inheritance)
    * **[span_27](start_span)정의**: DALi Toolkit의 `Control`을 위한 접근성 객체입니다[span_27](end_span).
    * **역할**:
        * [span_28](start_span)`Control` 특화 기능(예: `GrabFocus`, `DoAction`)을 구현합니다[span_28](end_span).
        * [span_29](start_span)`Action` 인터페이스를 통해 "클릭" 등의 동작을 외부에서 수행할 수 있게 합니다[span_29](end_span).
        * [span_30](start_span)하이라이트(Highlight) 처리를 담당합니다[span_30](end_span).
    * **[span_31](start_span)위치**: `dali-toolkit/dali-toolkit/devel-api/controls/control-accessible.h`[span_31](end_span)

4.  **`Dali::Accessibility::Bridge`**
    * **[span_32](start_span)정의**: AT-SPI 버스와 통신을 관리하는 싱글톤 클래스입니다[span_32](end_span).
    * **역할**:
        * [span_33](start_span)`Accessible` 객체 등록 및 해제 관리[span_33](end_span).
        * [span_34](start_span)DBus를 통한 이벤트 송수신[span_34](end_span).
        * [span_35](start_span)애플리케이션의 루트 객체 관리[span_35](end_span).
    * **[span_36](start_span)위치**: `dali-adaptor/dali/devel-api/adaptor-framework/accessibility-bridge.h`[span_36](end_span)

### Class Diagram (Mermaid)

    ```mermaid
    classDiagram
        class Accessible {
            <<Interface>>
            +GetName() string
            +GetRole() Role
            +GetStates() States
            +GetParent() Accessible*
            +GetChildren() vector~Accessible*~
        }

        class Component {
            <<Interface>>
            +GetExtents() Rect
            +GetLayer() Layer
            +GrabFocus() bool
        }

        class Collection {
            <<Interface>>
        }

        class Action {
            <<Interface>>
            +DoAction(name) bool
            +GetActionCount() int
        }

        class ActorAccessible {
            -WeakHandle~Actor~ mSelf
            +GetInternalActor() Actor
            +OnChildrenChanged()
        }

        class ControlAccessible {
            +GrabFocus() bool
            +DoAction(index) bool
        }

        class Bridge {
            <<Singleton>>
            +Initialize()
            +AddAccessible(id, obj)
            +EmitStateChanged(obj, state)
            +GetCurrentBridge() Bridge*
        }

        Accessible <|-- ActorAccessible
        Component <|-- ActorAccessible
        Collection <|-- ActorAccessible
        
        ActorAccessible <|-- ControlAccessible
        Action <|-- ControlAccessible

        ActorAccessible ..> Bridge : Registers to
        ControlAccessible ..> Bridge : Uses
    ```

### Class Diagram (PlantUML)

    ```plantuml
    @startuml

    interface Accessible {
        +GetName() : string
        +GetRole() : Role
        +GetStates() : States
        +GetParent() : Accessible*
        +GetChildren() : vector<Accessible*>
    }

    interface Component {
        +GetExtents() : Rect
        +GetLayer() : Layer
        +GrabFocus() : bool
    }

    interface Collection {
    }

    interface Action {
        +DoAction(name) : bool
        +GetActionCount() : int
    }

    class ActorAccessible {
        -mSelf : WeakHandle<Actor>
        +GetInternalActor() : Actor
        +OnChildrenChanged()
    }

    class ControlAccessible {
        +GrabFocus() : bool
        +DoAction(index) : bool
    }

    class Bridge {
        +Initialize()
        +AddAccessible(id, obj)
        +EmitStateChanged(obj, state)
        +{static} GetCurrentBridge() : Bridge*
    }

    Accessible <|-- ActorAccessible
    Component <|-- ActorAccessible
    Collection <|-- ActorAccessible

    ActorAccessible <|-- ControlAccessible
    Action <|-- ControlAccessible

    ActorAccessible ..> Bridge : Registers to

    @enduml
    ```

## 5. 상세 관계 및 상호작용 (Detailed Relationships & Interactions)

### Actor와 Accessible의 관계
* [span_37](start_span)`Actor`는 `Accessible` 객체를 직접 소유하지 않지만, 필요할 때 `Accessible::Get(actor)`를 통해 연관된 `Accessible` 객체를 가져올 수 있습니다[span_37](end_span).
* [span_38](start_span)`ActorAccessible`은 `Actor`에 대한 `WeakHandle`을 가지고 있어, `Actor`가 유효한 동안만 접근성 기능을 제공합니다[span_38](end_span).
* [span_39](start_span)`Actor`의 계층 구조(Parent-Child)는 `ActorAccessible::GetChildren()`을 통해 접근성 트리에 반영됩니다[span_39](end_span).
    [span_40](start_span)기본적으로 `Actor`의 자식들이 접근성 자식이 되지만, `DoGetChildren`을 오버라이딩하여 이를 커스터마이징할 수 있습니다[span_40](end_span).

### Bridge와 Accessible의 관계
* [span_41](start_span)`Accessible` 객체가 생성되거나 초기화될 때, `Bridge::AddAccessible`을 통해 브리지에 등록됩니다[span_41](end_span).
    [span_42](start_span)이때 고유한 ID(주로 Actor ID)가 사용됩니다[span_42](end_span).
* [span_43](start_span)외부(AT-SPI)에서 특정 객체에 대한 요청이 오면, `Bridge`는 등록된 맵에서 해당 ID의 `Accessible` 객체를 찾아 요청을 위임합니다[span_43](end_span).

### Sequence Diagram: 포커스 이동 시나리오 (Mermaid)

[span_44](start_span)사용자가 키보드나 제스처로 포커스를 이동했을 때의 흐름입니다[span_44](end_span).

    ```mermaid
    sequenceDiagram
        participant User
        participant Control as DALi Control
        participant CA as ControlAccessible
        participant Bridge
        participant DBus
        participant ScreenReader

        User->>Control: Focus Gesture / Key
        Control->>Control: OnFocusGained()
        Control->>CA: Notify Accessibility State Change
        CA->>Bridge: EmitStateChanged(FOCUSED, true)
        Bridge->>DBus: Signal: Object:StateChanged:focused(1)
        DBus->>ScreenReader: Receive Signal
        ScreenReader->>DBus: GetName() / GetRole()
        DBus->>Bridge: Call GetName() on Object
        Bridge->>CA: GetName()
        CA-->>Bridge: "Button Label"
        Bridge-->>DBus: "Button Label"
        DBus-->>ScreenReader: "Button Label"
        ScreenReader->>User: "Button Label, Button" (TTS)
    ```

### Sequence Diagram: 포커스 이동 시나리오 (PlantUML)

    ```plantuml
    @startuml
    actor User
    participant "DALi Control" as Control
    participant "ControlAccessible" as CA
    participant "Accessibility::Bridge" as Bridge
    participant "DBus / AT-SPI" as DBus
    participant "Screen Reader" as SR

    User -> Control : Focus Gesture / Key
    activate Control
    Control -> Control : OnFocusGained()
    Control -> CA : Notify Accessibility State Change
    activate CA
    CA -> Bridge : EmitStateChanged(FOCUSED, true)
    activate Bridge
    Bridge -> DBus : Signal: Object:StateChanged:focused(1)
    activate DBus
    DBus -> SR : Receive Signal
    activate SR
    SR -> DBus : GetName() / GetRole()
    DBus -> Bridge : Call GetName() on Object
    Bridge -> CA : GetName()
    CA --> Bridge : "Button Label"
    Bridge --> DBus : "Button Label"
    DBus --> SR : "Button Label"
    SR -> User : "Button Label, Button" (TTS)
    deactivate SR
    deactivate DBus
    deactivate Bridge
    deactivate CA
    deactivate Control
    @enduml
    ```

## 6. 결론 (Conclusion)

[span_45](start_span)DALi의 Accessibility 프레임워크는 `Accessible` 인터페이스를 중심으로 `Actor`와 `Control`을 래핑하여, 내부 UI 구조를 표준 AT-SPI 인터페이스로 노출시키는 구조를 가지고 있습니다[span_45](end_span).
[span_46](start_span)`Bridge`는 이들 객체와 외부 시스템 간의 통신을 담당하는 핵심 허브 역할을 수행합니다[span_46](end_span).
[span_47](start_span)이러한 구조를 통해 DALi 애플리케이션은 별도의 복잡한 설정 없이도 리눅스 데스크탑 환경의 접근성 도구들과 호환될 수 있습니다[span_47](end_span).

---

## Part 2: 연결 방식 비교 분석

# DALi Accessibility: 직접 연결(Direct) vs Bridge 어댑터 비교 분석

[span_48](start_span)이 문서는 DALi Accessibility 아키텍처 리팩토링 시 고려된 두 가지 주요 연결 방식인 **"직접 연결 (Direct Connection)"**과 **"Bridge 어댑터 (Bridge Adapter)"** 방식을 심층 비교 분석합니다[span_48](end_span).
[span_49](start_span)결론적으로, DALi의 **경량화(Lightweighting)**와 **순수성(Purity)** 유지를 위해 **Bridge 어댑터 방식**이 더 우수한 선택임을 설명합니다[span_49](end_span).

## 1. 개요

* **[span_50](start_span)직접 연결 (Direct Connection)**: DALi 엔진이 접근성 코어 라이브러리(`Tizen Accessibility Core`)를 직접 참조하고 호출하는 방식[span_50](end_span).
* **[span_51](start_span)Bridge 어댑터 (Bridge Adapter)**: DALi 엔진은 접근성 코어에 대해 모르며, 중간에 얇은 어댑터(Bridge)를 두어 런타임에 동적으로 연결하는 방식[span_51](end_span).

## 2. 상세 비교 분석

| 비교 항목 | 직접 연결 (Direct Connection) | Bridge 어댑터 (Bridge Adapter) |
| :--- | :--- | :--- |
| **의존성 (Dependency)** | **[span_52](start_span)강한 결합 (Tight Coupling)**<br>DALi 빌드 시 Core 라이브러리 필수.[span_52](end_span) | **느슨한 결합 (Loose Coupling)**<br>DALi는 Core 존재를 모름. [span_53](start_span)런타임 플러그인.[span_53](end_span) |
| **메모리 (Memory)** | **[span_54](start_span)항상 로드됨 (Static Load)**<br>접근성 미사용 시에도 라이브러리가 메모리에 상주.[span_54](end_span) | **[span_55](start_span)필요 시 로드됨 (On-Demand Load)**<br>접근성 활성화 시점에만 `dlopen`으로 로드.[span_55](end_span) |
| **코드 순수성 (Purity)** | **[span_56](start_span)낮음 (Low)**<br>DALi 헤더에 접근성 관련 타입(`IAccessibleNode` 등)이 포함됨.[span_56](end_span) | **높음 (High)**<br>DALi 코드는 렌더링 로직만 유지. [span_57](start_span)접근성 코드는 Bridge에 격리.[span_57](end_span) |
| **초기화 비용 (Startup)** | **[span_58](start_span)높음 (High)**<br>앱 실행 시 접근성 라이브러리도 함께 초기화.[span_58](end_span) | **[span_59](start_span)낮음 (Low)**<br>앱 실행 시에는 영향 없음.[span_59](end_span) |
| **유지보수 (Maintenance)** | [span_60](start_span)Core 변경 시 DALi 재빌드 필요.[span_60](end_span) | Bridge만 재빌드하면 됨. [span_61](start_span)DALi 영향 없음.[span_61](end_span) |
| **확장성 (Extensibility)** | [span_62](start_span)Tizen 외 플랫폼 지원 시 코드 수정 필요.[span_62](end_span) | [span_63](start_span)플랫폼별 Bridge만 교체하면 됨 (예: Android Bridge).[span_63](end_span) |

## 3. 심층 분석: 왜 Bridge 어댑터인가?

### 3.1. DALi의 "순수성(Purity)" 보장
DALi는 고성능 UI 렌더링 엔진입니다. [span_64](start_span)본질적인 역할은 화면을 그리고 애니메이션을 처리하는 것입니다[span_64](end_span).
* **[span_65](start_span)Direct 방식**: `Actor` 클래스 내부에 `GetAccessibilityNode()` 같은 메서드가 생기고, `IAccessibleNode` 타입을 알기 위해 헤더를 include 해야 합니다[span_65](end_span).
    [span_66](start_span)이는 렌더링 엔진이 접근성 도메인에 오염되는 결과를 낳습니다[span_66](end_span).
* **[span_67](start_span)Bridge 방식**: DALi는 접근성에 대해 아무것도 모릅니다[span_67](end_span).
    대신 Bridge가 "DALi의 상태를 관찰(Inspect)"하여 접근성 정보를 만들어냅니다. [span_68](start_span)DALi 코드는 깨끗하게 유지됩니다[span_68](end_span).

### 3.2. "경량화(Lightweighting)"의 핵심: On-Demand Loading
[span_69](start_span)임베디드 환경이나 저사양 기기에서는 메모리 1MB, 부팅 시간 10ms가 중요합니다[span_69](end_span).
* **[span_70](start_span)Direct 방식**: 사용자가 접근성 기능을 켜지 않아도, 바이너리가 링크되어 있으므로 OS 로더가 앱 실행 시점에 접근성 라이브러리를 메모리에 올립니다[span_70](end_span).
    [span_71](start_span)불필요한 비용입니다[span_71](end_span).
* **Bridge 방식**: 앱은 가볍게 시작합니다. [span_72](start_span)사용자가 접근성 기능을 켜는 순간(Runtime), DALi는 설정된 플러그인(Bridge)을 동적으로 로드합니다[span_72](end_span).
    **[span_73](start_span)쓰지 않는 기능에 비용을 지불하지 않는(Zero-cost abstraction)** 원칙을 지킬 수 있습니다[span_73](end_span).

### 3.3. "방화벽(Firewall)" 역할
[span_74](start_span)Bridge는 DALi를 외부의 변화로부터 보호합니다[span_74](end_span).
* [span_75](start_span)`Tizen Accessibility Core`의 내부 구현이 바뀌거나, AT-SPI 버전이 업그레이드되어도 DALi 엔진 자체는 안전합니다[span_75](end_span).
    오직 얇은 Bridge 계층만 수정하고 다시 빌드하면 됩니다. [span_76](start_span)이는 거대한 엔진을 유지보수하는 입장에서 빌드 시간과 테스트 비용을 획기적으로 줄여줍니다[span_76](end_span).

## 4. 다이어그램 비교

### Direct Connection (비추천)

    ```mermaid
    graph LR
        subgraph "DALi Engine"
            Actor[Actor]
            Code[Accessibility Code]
        end
        subgraph "Tizen Accessibility Core"
            Lib[Core Library]
        end
        
        Actor <--> Code
        Code == Static Link ==> Lib
        
        style Code fill:#f9f,stroke:#333,stroke-width:2px
        style Lib fill:#f9f,stroke:#333,stroke-width:2px
    ```
* [span_77](start_span)DALi 내부에 접근성 코드가 혼재되어 있고, Core 라이브러리와 강하게 묶여 있습니다[span_77](end_span).

### Bridge Adapter (추천)

    ```mermaid
    graph LR
        subgraph "DALi Engine"
            Actor[Actor]
        end
        subgraph "Bridge Plugin"
        Adapter[Bridge Adapter]
    end
    subgraph "Tizen Accessibility Core"
        Lib[Core Library]
    end
    
    Actor -.-> Adapter : Runtime Inspection
    Adapter == Dynamic Link ==> Lib
    
    style Adapter fill:#bbf,stroke:#333,stroke-width:2px
    style Lib fill:#bbf,stroke:#333,stroke-width:2px
    ```
* [span_78](start_span)DALi는 독립적이며, Bridge가 중간에서 런타임에 연결해줍니다[span_78](end_span).

## 5. 결론

[span_79](start_span)DALi가 지향하는 **고성능, 경량화, 모듈화** 목표를 달성하기 위해서는 **Bridge 어댑터 패턴**이 필수적입니다[span_79](end_span).
[span_80](start_span)이는 단순히 코드를 분리하는 것을 넘어, DALi를 플랫폼 종속성으로부터 해방시키고 본연의 렌더링 기능에 집중하게 만드는 아키텍처적 결단입니다[span_80](end_span).

---

## Part 3: 아키텍처 개선 제안

# DALi 경량화 및 Tizen 접근성 통합을 위한 아키텍처 개선 제안

[span_81](start_span)이 문서는 DALi (Dynamic Animation Library)의 **경량화(Lightweighting) 및 효율화(Efficiency)**를 달성하고, 동시에 Tizen 생태계의 접근성 파편화 문제를 해결하기 위한 아키텍처 개선 제안서입니다[span_81](end_span).

## 1. 배경 및 목적 (Background & Objectives)

### 1.1. DALi의 비대화 문제 (The Bloat Problem)
[span_82](start_span)현재 DALi의 Accessibility 구현체(`ActorAccessible`, `ControlAccessible`)는 `Actor` 및 `Control`과 강하게 결합되어 있습니다[span_82](end_span).
* **[span_83](start_span)메모리 낭비**: 접근성 기능이 필요 없는 환경(예: 저사양 기기, 접근성 미사용 사용자)에서도 관련 객체 생성 및 초기화 로직이 수행되거나, 최소한의 메모리 공간을 점유합니다[span_83](end_span).
* **[span_84](start_span)초기화 비용**: 앱 구동 시점에 Accessibility Bridge 및 관련 리소스를 준비하는 과정이 DALi 코어 초기화 단계에 포함되어 있어, 부팅 속도(Startup Time)에 영향을 줍니다[span_84](end_span).
* **[span_85](start_span)바이너리 크기**: Accessibility 관련 코드가 DALi Core/Toolkit 라이브러리에 포함되어 있어, 전체 바이너리 크기를 증가시킵니다[span_85](end_span).

### 1.2. Tizen 생태계의 확장 (Ecosystem Expansion)
[span_86](start_span)Tizen은 이제 DALi뿐만 아니라 Web, Flutter 등 다양한 UI Toolkit을 지원합니다[span_86](end_span).
[span_87](start_span)현재 DALi에 종속된 Accessibility 구조는 다른 Toolkit에서 재사용할 수 없으며, 이는 플랫폼 전체의 비효율을 초래합니다[span_87](end_span).
**[span_88](start_span)따라서, 본 제안의 핵심 목표는 "Accessibility를 DALi에서 분리하여 DALi를 가볍게 만들고, 분리된 모듈을 공용화하는 것"입니다[span_88](end_span).**

## 2. 아키텍처 옵션 비교 (Architectural Options Comparison)

[span_89](start_span)Accessibility 모듈을 DALi에서 분리하는 방식에 대해 3가지 옵션을 비교 분석합니다[span_89](end_span).

### Option A: Bridge Adapter (추천)
[span_90](start_span)DALi와 Core 사이에 얇은 어댑터(Bridge)를 두고, 이를 런타임에 동적으로 로드하는 방식입니다[span_90](end_span).
* **[span_91](start_span)구조**: `DALi` <-> `Bridge (Plugin)` <-> `Tizen Accessibility Core`[span_91](end_span)
* **장점**:
    * **[span_92](start_span)경량화 최적**: 접근성 미사용 시 DALi에 오버헤드가 거의 없음 (Zero-cost abstraction)[span_92](end_span).
    * **[span_93](start_span)유연성**: DALi 코드를 수정하지 않고도 접근성 구현체를 교체 가능[span_93](end_span).
* **[span_94](start_span)단점**: 플러그인 로딩 구조 설계가 필요함[span_94](end_span).

#### Diagram (Mermaid)
    ```mermaid
    graph LR
        subgraph DALi
            Actor[Actor / Control]
        end
        subgraph "Bridge (Dynamic Lib)"
            Adapter[DALi Accessibility Adapter]
        end
        subgraph "Tizen Accessibility Core"
            Core[Core Logic]
        end
        Actor -.-> Adapter : Inspects (Runtime)
        Adapter <--> Core : Implements Interface
    ```

#### Diagram (PlantUML)
    ```plantuml
    @startuml
    component "DALi Engine" as DALi
    component "DALi Accessibility Bridge\n(Dynamic Library)" as Bridge
    component "Tizen Accessibility Core\n(Shared Library)" as Core

    DALi ..> Bridge : Load on Demand (dlopen)
    Bridge -> DALi : Inspects Actor State
    Bridge -right-> Core : Implements IAccessibleNode
    @enduml
    ```

### Option B: External Service Delegation
[span_95](start_span)접근성 로직을 별도의 프로세스(Service)로 완전히 분리하고 IPC로 통신하는 방식입니다[span_95](end_span).
* **[span_96](start_span)구조**: `DALi Process` <-> `IPC` <-> `Accessibility Service Process`[span_96](end_span)
* **장점**: 완벽한 프로세스 격리. [span_97](start_span)DALi 크래시가 접근성에 영향을 주지 않음[span_97](end_span).
* **단점**:
    * **[span_98](start_span)성능 저하**: UI 트리가 변할 때마다 방대한 데이터를 IPC로 전송해야 함 (Serialization 비용)[span_98](end_span).
    * **[span_99](start_span)동기화 문제**: 화면은 갱신되었는데 접근성 트리는 갱신되지 않는 타이밍 이슈 발생 가능[span_99](end_span).

#### Diagram (Mermaid)
    ```mermaid
    graph LR
        subgraph "DALi Process"
            Actor[Actor]
            IPC_Client[IPC Client]
        end
        subgraph "Accessibility Service Process"
            IPC_Server[IPC Server]
            Mirror[Mirror Tree]
            Core[Core Logic]
        end
        Actor --> IPC_Client : Serialize Tree
        IPC_Client <--> IPC_Server : IPC (Socket/Pipe)
        IPC_Server --> Mirror : Update
        Mirror <--> Core
    ```

#### Diagram (PlantUML)
    ```plantuml
    @startuml
    package "DALi Process" {
        [Actor]
        [IPC Sender]
    }
    package "Accessibility Service" {
        [IPC Receiver]
        [Shadow DOM]
        [Core Logic]
    }

    [Actor] -> [IPC Sender] : Tree Updates
    [IPC Sender] <-> [IPC Receiver] : Heavy IPC Traffic
    [IPC Receiver] -> [Shadow DOM] : Reconstruct
    [Shadow DOM] <-> [Core Logic]
    @enduml
    ```

### Option C: Direct Connection
[span_100](start_span)DALi가 `Tizen Accessibility Core` 라이브러리를 직접 링크(Link)하여 사용하는 방식입니다[span_100](end_span).
* **[span_101](start_span)구조**: `DALi` <-> `Tizen Accessibility Core`[span_101](end_span)
* **장점**: 구현이 가장 단순함. [span_102](start_span)호출 오버헤드가 가장 적음[span_102](end_span).
* **단점**:
    * **[span_103](start_span)강한 결합**: DALi 빌드 시 Core 라이브러리가 필수[span_103](end_span).
    * **메모리 낭비**: 접근성 미사용 시에도 라이브러리가 로드됨. [span_104](start_span)DALi 코드 내에 접근성 타입이 혼재됨[span_104](end_span).

#### Diagram (Mermaid)
    ```mermaid
    graph LR
        subgraph "DALi Binary"
            Actor[Actor]
            Impl[Accessible Impl]
        end
        subgraph "Tizen Accessibility Core"
            Core[Core Logic]
        end
        Actor <--> Impl
        Impl <--> Core : Direct Function Call
    ```

#### Diagram (PlantUML)
    ```plantuml
    @startuml
    component "DALi Engine" {
        [Actor]
        [Accessible Implementation]
    }
    component "Tizen Accessibility Core" as Core

    [Accessible Implementation] -right-> Core : Static Link / Direct Call
    [Actor] <-> [Accessible Implementation] : Tightly Coupled
    @enduml
    ```

---

## 3. 통신 모델 비교 (Communication Model Comparison)

[span_105](start_span)UI 스레드와 접근성 로직(DBus 통신) 간의 상호작용 방식에 대한 3가지 옵션입니다[span_105](end_span).

### Model 1: Fully Synchronous (동기식)
[span_106](start_span)모든 요청(정보 조회, 액션 수행)을 UI 스레드에서 동기적으로 처리합니다[span_106](end_span).
* **[span_107](start_span)장점**: 구현 단순, 데이터 정합성(Consistency) 완벽 보장[span_107](end_span).
* **[span_108](start_span)단점**: DBus 통신이 지연되면 UI 프레임 드랍(Jank) 발생[span_108](end_span).

#### Diagram (Mermaid)
    ```mermaid
    sequenceDiagram
        participant ScreenReader
        participant Core
        participant DALi
        ScreenReader->>Core: GetName()
        Core->>DALi: GetName() (Block UI)
        DALi-->>Core: "Button"
        Core-->>ScreenReader: "Button"
    ```

### Model 2: Fully Asynchronous (비동기식)
[span_109](start_span)모든 요청을 별도 스레드나 작업 큐를 통해 비동기로 처리합니다[span_109](end_span).
* **[span_110](start_span)장점**: UI 스레드 블로킹 절대 없음[span_110](end_span).
* **단점**:
    * **[span_111](start_span)복잡성**: 콜백 지옥, 레이스 컨디션 관리 필요[span_111](end_span).
    * **[span_112](start_span)사용자 경험 저하**: 스크린 리더가 "현재 포커스"를 물었는데, 응답이 오기 전에 포커스가 이미 이동해버릴 수 있음 (Stale Data)[span_112](end_span).

#### Diagram (Mermaid)
    ```mermaid
    sequenceDiagram
        participant ScreenReader
        participant Core
        participant DALi
        ScreenReader->>Core: GetName()
        Core->>DALi: Post Task: GetName()
        Note right of DALi: UI continues...
        DALi-->>Core: Callback("Button")
        Core-->>ScreenReader: "Button"
    ```

### Model 3: Hybrid (혼합형) - **추천**
[span_113](start_span)정보 조회(Getters)는 **Sync (with Caching)**, 액션/이벤트(Actions)는 **Async**로 처리합니다[span_113](end_span).
* **장점**:
    * **[span_114](start_span)반응성**: `GetName` 등은 즉시 응답하여 스크린 리더의 답답함 해소[span_114](end_span).
    * **[span_115](start_span)성능**: 무거운 작업(이벤트 전파, 액션 수행)은 UI를 방해하지 않음[span_115](end_span).
    * **[span_116](start_span)캐싱 전략**: Sync 조회 시 DBus 타임아웃을 방지하기 위해 Core 레벨에서 적극적인 캐싱 사용[span_116](end_span).

#### Diagram (Mermaid)
    ```mermaid
    sequenceDiagram
        participant ScreenReader
        participant Core
        participant DALi
        
        Note over ScreenReader, DALi: Read Info (Sync + Cache)
        ScreenReader->>Core: GetName()
        alt Cached
            Core-->>ScreenReader: "Button" (Instant)
        else Not Cached
            Core->>DALi: GetName() (Fast)
            DALi-->>Core: "Button"
            Core-->>ScreenReader: "Button"
        end

        Note over ScreenReader, DALi: Perform Action (Async)
        ScreenReader->>Core: DoAction(Click)
        Core->>DALi: Post Task: Click
        Core-->>ScreenReader: Ack (Immediate)
        DALi->>DALi: Process Click
    ```

#### Diagram (PlantUML)
    ```plantuml
    @startuml
    participant "Screen Reader" as SR
    participant "Tizen Core" as Core
    participant "DALi Main Thread" as DALi

    == Information Retrieval (Sync) ==
    SR -> Core : GetName()
    activate Core
        alt Cache Hit
            Core --> SR : Return Cached Value
        else Cache Miss
            Core -> DALi : Read Property
            activate DALi
            DALi --> Core : Return Value
            deactivate DALi
            Core --> SR : Return Value
        end
    deactivate Core

    == Action Execution (Async) ==
    SR -> Core : DoAction("Click")
    activate Core
        Core -> DALi : Post Event (Async)
        Core --> SR : Acknowledge
    deactivate Core
    activate DALi
        DALi -> DALi : Handle Click
    deactivate DALi
    @enduml
    ```

## 4. 최종 제안 (Final Recommendation)

**[span_117](start_span)Option A (Bridge Adapter)** 구조와 **Model 3 (Hybrid Communication)** 모델을 결합하는 것을 최종 제안합니다[span_117](end_span).
[span_118](start_span)이 조합은 **DALi의 경량화**라는 최우선 목표를 달성하면서도, **사용자 경험(반응성)**을 해치지 않는 최적의 균형점입니다[span_118](end_span).
