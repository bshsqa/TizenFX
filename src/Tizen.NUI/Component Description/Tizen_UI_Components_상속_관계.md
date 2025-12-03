# Tizen.UI.Components 상속 관계

## 소개

이 문서는 Tizen.UI.Components의 전체 상속 관계를 시각적으로 표현하여 컴포넌트 간의 계층 구조와 관계를 명확히 이해할 수 있도록 합니다. 상속 관계는 객체 지향 프로그래밍의 핵심 개념으로, 코드 재사용성과 확장성을 제공합니다.

## 전체 상속 구조

```mermaid
graph TD
    subgraph "Tizen.UI.Components 상속 구조"
        A[NObject] --> B[View]
        B --> C[ViewGroup]
        B --> D[ImageView]
        B --> E[TextView]
        B --> F[TextField]
        B --> G[TextEditor]
        B --> H[LottieAnimationView]
        C --> I[Scrollable]
        I --> J[ScrollView]
        I --> K[RecyclerView]
        B --> L[Navigator]
        C --> M[SelectionGroup]
        B --> N[Progress]
        N --> O[InteractiveProgress]
        B --> P[Clickable]
        P --> Q[ClickableBox]
        C --> R[ClickableGrid]
        C --> S[ClickableHStack]
        C --> T[ClickableVStack]
        B --> U[Pressable]
        U --> V[PressableBox]
        B --> W[Selectable]
        W --> X[SelectableBox]
        W --> Y[SelectableHStack]
        W --> Z[SelectableVStack]
        W --> AA[GroupSelectable]
        AA --> AB[GroupSelectableBox]
    end
    
    subgraph "Theme Implementations"
        AC[Tizen.UI.Components.Material]
        AD[Tizen.UI.Components.OneUI]
        AC --> AE[MaterialButton]
        AC --> AF[MaterialCard]
        AC --> AG[MaterialAppBar]
        AD --> AH[OneUIButton]
        AD --> AI[OneUICard]
        AD --> AJ[OneUIAppBar]
    end
    
    subgraph "Interfaces"
        AI[IClickable]
        AJ[ISelectable]
        AK[IGroupSelectable]
        AL[IPressable]
        AM[ILayoutBox]
        AN[INavigation]
        AO[ISelectionGroup]
        
        P -.-> AI
        Q -.-> AI
        R -.-> AI
        S -.-> AI
        T -.-> AI
        
        W -.-> AJ
        X -.-> AJ
        Y -.-> AJ
        Z -.-> AJ
        
        AA -.-> AK
        AB -.-> AK
        
        U -.-> AL
        V -.-> AL
        
        Q -.-> AM
        X -.-> AM
        V -.-> AM
        AB -.-> AM
        
        L -.-> AN
        
        M -.-> AO
    end
    
    Q --> AE
    Q --> AH
```

## 코어 컴포넌트 상속 구조

### 1. 기본 뷰 계층

```mermaid
classDiagram
    class NObject {
        <<abstract>>
        +Dispose()
        +Handle: IntPtr
    }
    
    class View {
        <<abstract>>
        +Parent: View
        +Children: IList~View~
        +Visibility: Visibility
        +Opacity: float
        +Position: Position
        +Size: Size
        +BackgroundColor: Color
        +AddChild(View)
        +RemoveChild(View)
        +Layout()
    }
    
    class ViewGroup {
        <<abstract>>
        +Layout: LayoutManager
        +AddView(View)
        +RemoveView(View)
        +FindViewById(string)
    }
    
    class ImageView {
        +Source: string
        +Image: Image
        +ScaleType: ScaleType
        +LoadImage(string)
        +SetImage(Image)
    }
    
    class TextView {
        +Text: string
        +TextColor: Color
        +FontSize: float
        +FontFamily: string
        +TextAlignment: TextAlignment
        +SetText(string)
    }
    
    class TextField {
        +Text: string
        +Placeholder: string
        +IsPassword: bool
        +MaxLength: int
        +InputType: InputType
        +TextChanged: EventHandler
        +SetText(string)
    }
    
    class TextEditor {
        +Text: string
        +LineCount: int
        +ScrollPosition: Position
        +EnableScrolling: bool
        +SetText(string)
        +InsertText(string)
        +AppendText(string)
    }
    
    class LottieAnimationView {
        +Source: string
        +IsPlaying: bool
        +Speed: float
        +RepeatCount: int
        +Play()
        +Pause()
        +Stop()
        +SetAnimation(string)
    }
    
    NObject <|-- View
    View <|-- ViewGroup
    View <|-- ImageView
    View <|-- TextView
    View <|-- TextField
    View <|-- TextEditor
    View <|-- LottieAnimationView
```

### 2. 레이아웃 컴포넌트 계층

```mermaid
classDiagram
    class View {
        <<abstract>>
    }
    
    class ViewGroup {
        <<abstract>>
    }
    
    class Scrollable {
        <<abstract>>
        +ScrollX: float
        +ScrollY: float
        +MaxScrollX: float
        +MaxScrollY: float
        +ScrollTo(float, float)
        +SmoothScrollTo(float, float)
    }
    
    class ScrollView {
        +Orientation: Orientation
        +ScrollBarVisible: bool
    }
    
    class RecyclerView {
        +Adapter: RecyclerViewAdapter
        +LayoutManager: LayoutManager
        +RecycledViewPool: RecycledViewPool
    }
    
    class Navigator {
        +NavigationStack: IReadOnlyList~IView~
        +CurrentView: IView
        +Push(IView, INavigationTransition)
        +Pop(INavigationTransition)
        +PopToRoot(INavigationTransition)
        +SetRoot(IView)
    }
    
    View <|-- ViewGroup
    ViewGroup <|-- Scrollable
    Scrollable <|-- ScrollView
    Scrollable <|-- RecyclerView
    View <|-- Navigator
```

### 3. 인터랙티브 컴포넌트 계층

```mermaid
classDiagram
    class View {
        <<abstract>>
    }
    
    class Clickable {
        <<abstract>>
        +Clicked: EventHandler
        +IsClickable: bool
        +OnClicked()
    }
    
    class ClickableBox {
        +Text: string
        +Icon: Image
        +IconPlacement: IconPlacement
    }
    
    class ClickableGrid {
        +Columns: int
        +Spacing: float
    }
    
    class ClickableHStack {
        +Spacing: float
        +Alignment: VerticalAlignment
    }
    
    class ClickableVStack {
        +Spacing: float
        +Alignment: HorizontalAlignment
    }
    
    class Pressable {
        <<abstract>>
        +Pressed: EventHandler
        +Released: EventHandler
        +IsPressed: bool
        +OnPressed()
        +OnReleased()
    }
    
    class PressableBox {
        +Text: string
        +PressedBackgroundColor: Color
        +ReleasedBackgroundColor: Color
    }
    
    class Selectable {
        <<abstract>>
        +SelectionChanged: EventHandler~SelectionChangedEventArgs~
        +IsSelected: bool
        +IsSelectable: bool
        +OnSelected()
        +OnDeselected()
    }
    
    class SelectableBox {
        +Text: string
        +SelectedBackgroundColor: Color
        +UnselectedBackgroundColor: Color
    }
    
    class SelectableHStack {
        +Spacing: float
        +Alignment: VerticalAlignment
    }
    
    class SelectableVStack {
        +Spacing: float
        +Alignment: HorizontalAlignment
    }
    
    class GroupSelectable {
        <<abstract>>
        +SelectionGroup: ISelectionGroup
    }
    
    class GroupSelectableBox {
        +Text: string
        +GroupSelectedBackgroundColor: Color
        +GroupUnselectedBackgroundColor: Color
    }
    
    class SelectionGroup {
        -items: List~IGroupSelectable~
        -selectedItem: IGroupSelectable
        +ItemClicked: EventHandler~SelectionGroupItemClickedEventArgs~
        +AddItem(IGroupSelectable)
        +RemoveItem(IGroupSelectable)
        +ClearSelection()
        +SelectedItem: IGroupSelectable
        +Items: IReadOnlyList~IGroupSelectable~
    }
    
    View <|-- Clickable
    Clickable <|-- ClickableBox
    ViewGroup <|-- ClickableGrid
    ViewGroup <|-- ClickableHStack
    ViewGroup <|-- ClickableVStack
    
    View <|-- Pressable
    Pressable <|-- PressableBox
    
    View <|-- Selectable
    Selectable <|-- GroupSelectable
    Selectable <|-- SelectableBox
    Selectable <|-- SelectableHStack
    Selectable <|-- SelectableVStack
    GroupSelectable <|-- GroupSelectableBox
    
    View <|-- SelectionGroup
```

### 4. 진행 상태 컴포넌트 계층

```mermaid
classDiagram
    class View {
        <<abstract>>
    }
    
    class Progress {
        +Value: float
        +MinValue: float
        +MaxValue: float
        +IsIndeterminate: bool
        +ProgressColor: Color
        +BackgroundColor: Color
    }
    
    class InteractiveProgress {
        +UserInteractionEnabled: bool
        +ValueChanged: EventHandler
        +OnValueChanged(float)
    }
    
    View <|-- Progress
    Progress <|-- InteractiveProgress
```

## 테마 구현체 상속 구조

### 1. Material Design 구현체

```mermaid
classDiagram
    class ClickableBox {
        <<abstract>>
    }
    
    class MaterialButton {
        +ButtonType: MaterialButtonType
        +Elevation: float
        +RippleColor: Color
        +CornerRadius: float
        +Icon: Image
        +IconPlacement: IconPlacement
        +ApplyElevation()
        +CreateRippleEffect()
    }
    
    class MaterialCard {
        +Elevation: float
        +CornerRadius: float
        +StrokeColor: Color
        +StrokeWidth: float
        +ApplyElevation()
        +ApplyCornerRadius()
    }
    
    class MaterialAppBar {
        +Title: string
        +NavigationIcon: Image
        +BackgroundColor: Color
        +Elevation: float
        +AddAction(MaterialAppBarAction)
        +SetTitle(string)
        +SetNavigationIcon(Image)
    }
    
    ClickableBox <|-- MaterialButton
    ViewGroup <|-- MaterialCard
    ViewGroup <|-- MaterialAppBar
```

### 2. OneUI 구현체

```mermaid
classDiagram
    class ClickableBox {
        <<abstract>>
    }
    
    class OneUIButton {
        +ButtonType: OneUIButtonType
        +Style: OneUIButtonStyle
        +RippleColor: Color
        +CornerRadius: float
        +Icon: Image
        +IconPlacement: IconPlacement
        +ApplyStyle()
        +CreateRippleEffect()
    }
    
    class OneUICard {
        +Elevation: float
        +CornerRadius: float
        +StrokeColor: Color
        +StrokeWidth: float
        +ApplyElevation()
        +ApplyCornerRadius()
    }
    
    class OneUIAppBar {
        +Title: string
        +NavigationIcon: Image
        +BackgroundColor: Color
        +Elevation: float
        +AddAction(OneUIAppBarAction)
        +SetTitle(string)
        +SetNavigationIcon(Image)
    }
    
    ClickableBox <|-- OneUIButton
    ViewGroup <|-- OneUICard
    ViewGroup <|-- OneUIAppBar
```

## 인터페이스 구현 관계

### 1. 클릭 가능 인터페이스

```mermaid
classDiagram
    class IClickable {
        <<interface>>
        +Clicked: EventHandler
        +IsClickable: bool
        +OnClicked()
    }
    
    class Clickable {
        <<abstract>>
    }
    
    class ClickableBox {
        <<abstract>>
    }
    
    class ClickableGrid {
        <<abstract>>
    }
    
    class ClickableHStack {
        <<abstract>>
    }
    
    class ClickableVStack {
        <<abstract>>
    }
    
    class MaterialButton {
        <<abstract>>
    }
    
    class OneUIButton {
        <<abstract>>
    }
    
    Clickable ..|> IClickable
    ClickableBox ..|> IClickable
    ClickableGrid ..|> IClickable
    ClickableHStack ..|> IClickable
    ClickableVStack ..|> IClickable
    MaterialButton ..|> IClickable
    OneUIButton ..|> IClickable
```

### 2. 선택 가능 인터페이스

```mermaid
classDiagram
    class ISelectable {
        <<interface>>
        +SelectionChanged: EventHandler~SelectionChangedEventArgs~
        +IsSelected: bool
        +IsSelectable: bool
        +OnSelected()
        +OnDeselected()
    }
    
    class Selectable {
        <<abstract>>
    }
    
    class SelectableBox {
        <<abstract>>
    }
    
    class SelectableHStack {
        <<abstract>>
    }
    
    class SelectableVStack {
        <<abstract>>
    }
    
    Selectable ..|> ISelectable
    SelectableBox ..|> ISelectable
    SelectableHStack ..|> ISelectable
    SelectableVStack ..|> ISelectable
```

### 3. 그룹 선택 가능 인터페이스

```mermaid
classDiagram
    class IGroupSelectable {
        <<interface>>
        +SelectionGroup: ISelectionGroup
    }
    
    class GroupSelectable {
        <<abstract>>
    }
    
    class GroupSelectableBox {
        <<abstract>>
    }
    
    GroupSelectable ..|> IGroupSelectable
    GroupSelectableBox ..|> IGroupSelectable
```

## 컴포넌트 조합 예제

### 1. 복합 컴포넌트 상속 구조

```mermaid
classDiagram
    class View {
        <<abstract>>
    }
    
    class ViewGroup {
        <<abstract>>
    }
    
    class ClickableBox {
        <<abstract>>
    }
    
    class SelectableBox {
        <<abstract>>
    }
    
    class PressableBox {
        <<abstract>>
    }
    
    class InteractiveListItem {
        +Title: string
        +Subtitle: string
        +Icon: Image
        +IsSelected: bool
        +IsPressed: bool
        +Clicked: EventHandler
        +SelectionChanged: EventHandler
        +Pressed: EventHandler
        +Released: EventHandler
    }
    
    View <|-- ClickableBox
    View <|-- SelectableBox
    View <|-- PressableBox
    ClickableBox <|-- InteractiveListItem
    SelectableBox <|-- InteractiveListItem
    PressableBox <|-- InteractiveListItem
```

### 2. 커스텀 컴포넌트 상속 구조

```mermaid
classDiagram
    class View {
        <<abstract>>
    }
    
    class CustomComponent {
        +CustomProperty: string
        +CustomMethod()
    }
    
    class ThemedCustomComponent {
        +ApplyTheme()
    }
    
    View <|-- CustomComponent
    CustomComponent <|-- ThemedCustomComponent
```

## 결론

Tizen.UI.Components의 상속 구조는 다음과 같은 특징을 가지고 있습니다:

1. **계층적 구조**: 명확한 계층 구조를 통해 코드의 조직화와 관리 용이성 제공
2. **확장성**: 추상 클래스와 인터페이스를 통해 새로운 컴포넌트의 추가가 용이
3. **재사용성**: 기존 컴포넌트를 상속받아 새로운 기능을 추가하거나 수정 가능
4. **유연성**: 인터페이스 기반 설계를 통해 다형성과 유연한 구성 가능
5. **테마 지원**: 테마 구현체를 통해 다양한 디자인 시스템 적용 가능

이러한 상속 구조는 Tizen.UI.Components가 강력하고 유연한 UI 프레임워크로 동작할 수 있도록 하며, 개발자들이 일관되고 효율적인 방식으로 UI 컴포넌트를 개발하고 확장할 수 있도록 지원합니다.
