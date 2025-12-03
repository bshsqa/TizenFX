# Tizen.UI.Components 활용 가이드

## 소개

이 문서는 Tizen.UI.Components를 효과적으로 활용하기 위한 가이드를 제공합니다. 실제 애플리케이션 개발 시나리오를 중심으로 컴포넌트의 사용 방법, 모범 사례, 그리고 고급 기능 활용법을 설명합니다.

## 1. 기본 사용법

### 1.1. 간단한 UI 구성

```csharp
// 기본 뷰 생성
public class SimpleView : View
{
    public SimpleView()
    {
        // 레이아웃 설정
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        // 텍스트 뷰 추가
        var title = new TextView()
        {
            Text = "Welcome to Tizen UI",
            FontSize = 24,
            TextColor = Color.Black,
            Margin = new Thickness(0, 0, 0, 16)
        };
        
        // 이미지 뷰 추가
        var imageView = new ImageView()
        {
            Source = "res/images/welcome.png",
            Size = new Size(200, 200),
            ScaleType = ScaleType.FitCenter,
            Margin = new Thickness(0, 0, 0, 16)
        };
        
        // 버튼 추가
        var actionButton = new ClickableBox()
        {
            Text = "Get Started",
            BackgroundColor = Color.Blue,
            TextColor = Color.White,
            CornerRadius = 8,
            Size = new Size(200, 50)
        };
        
        actionButton.Clicked += OnActionButtonClicked;
        
        // 자식 뷰 추가
        AddChild(title);
        AddChild(imageView);
        AddChild(actionButton);
    }
    
    private void OnActionButtonClicked(object sender, EventArgs e)
    {
        // 버튼 클릭 처리
        Console.WriteLine("Action button clicked!");
    }
}
```

### 1.2. 테마 적용

```csharp
// Material Design 테마 적용
public class ThemedView : View
{
    public ThemedView()
    {
        // Material 테마 설정
        var theme = new MaterialTheme();
        
        // 레이아웃 설정
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        // Material 버튼 생성
        var materialButton = new MaterialButton()
        {
            Text = "Material Button",
            ButtonType = MaterialButtonType.Filled,
            Size = new Size(200, 50)
        };
        
        // Material 카드 생성
        var materialCard = new MaterialCard()
        {
            Elevation = 8,
            CornerRadius = 12,
            Size = new Size(300, 200),
            Margin = new Thickness(0, 16, 0, 16)
        };
        
        // 카드 내용 추가
        var cardLayout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        var cardTitle = new TextView()
        {
            Text = "Material Card",
            FontSize = MaterialTypography.Headline6.FontSize
        };
        
        var cardContent = new TextView()
        {
            Text = "This is a Material Design card component.",
            FontSize = MaterialTypography.Body2.FontSize,
            Margin = new Thickness(0, 8, 0, 16)
        };
        
        cardLayout.AddChild(cardTitle);
        cardLayout.AddChild(cardContent);
        materialCard.AddChild(cardLayout);
        
        // 자식 뷰 추가
        AddChild(materialButton);
        AddChild(materialCard);
        
        // 테마 적용
        theme.ApplyTheme(this);
    }
}
```

## 2. 고급 활용법

### 2.1. 데이터 바인딩

```csharp
// 뷰 모델 정의
public class UserViewModel : INotifyPropertyChanged
{
    private string _name;
    private int _age;
    private bool _isActive;
    
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }
    
    public int Age
    {
        get => _age;
        set
        {
            _age = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// 데이터 바인딩 뷰
public class BindingView : View
{
    private readonly UserViewModel _viewModel;
    private readonly TextView _nameView;
    private readonly TextView _ageView;
    private readonly SelectableBox _activeView;
    
    public BindingView()
    {
        _viewModel = new UserViewModel()
        {
            Name = "John Doe",
            Age = 30,
            IsActive = true
        };
        
        // UI 구성
        SetupUI();
        
        // 데이터 바인딩
        BindData();
    }
    
    private void SetupUI()
    {
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        _nameView = new TextView()
        {
            FontSize = 18,
            Margin = new Thickness(0, 0, 0, 8)
        };
        
        _ageView = new TextView()
        {
            FontSize = 16,
            TextColor = Color.Gray,
            Margin = new Thickness(0, 0, 0, 16)
        };
        
        _activeView = new SelectableBox()
        {
            Text = "Active User",
            IsSelectable = false
        };
        
        AddChild(_nameView);
        AddChild(_ageView);
        AddChild(_activeView);
    }
    
    private void BindData()
    {
        // 이름 바인딩
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            handler => _viewModel.PropertyChanged += handler,
            handler => _viewModel.PropertyChanged -= handler)
            .Where(e => e.EventArgs.PropertyName == nameof(UserViewModel.Name))
            .Subscribe(_ => _nameView.Text = _viewModel.Name);
        
        // 나이 바인딩
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            handler => _viewModel.PropertyChanged += handler,
            handler => _viewModel.PropertyChanged -= handler)
            .Where(e => e.EventArgs.PropertyName == nameof(UserViewModel.Age))
            .Subscribe(_ => _ageView.Text = $"Age: {_viewModel.Age}");
        
        // 활성 상태 바인딩
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            handler => _viewModel.PropertyChanged += handler,
            handler => _viewModel.PropertyChanged -= handler)
            .Where(e => e.EventArgs.PropertyName == nameof(UserViewModel.IsActive))
            .Subscribe(_ => _activeView.IsSelected = _viewModel.IsActive);
    }
}
```

### 2.2. 리스트 및 어댑터 패턴

```csharp
// 리스트 아이템 모델
public class ListItemModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconPath { get; set; }
    public bool IsSelected { get; set; }
}

// 커스텀 리스트 아이템 뷰
public class CustomListItem : ClickableBox
{
    private readonly ImageView _iconView;
    private readonly TextView _titleView;
    private readonly TextView _descriptionView;
    
    public CustomListItem()
    {
        // 레이아웃 설정
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Horizontal,
            Padding = new Thickness(16)
        };
        
        // 아이콘
        _iconView = new ImageView()
        {
            Size = new Size(40, 40),
            Margin = new Thickness(0, 0, 16, 0)
        };
        
        // 텍스트 컨테이너
        var textContainer = new ViewGroup()
        {
            Layout = new LinearLayout()
            {
                Orientation = Orientation.Vertical
            }
        };
        
        _titleView = new TextView()
        {
            FontSize = 16,
            TextColor = Color.Black
        };
        
        _descriptionView = new TextView()
        {
            FontSize = 14,
            TextColor = Color.Gray
        };
        
        textContainer.AddChild(_titleView);
        textContainer.AddChild(_descriptionView);
        
        // 자식 뷰 추가
        AddChild(_iconView);
        AddChild(textContainer);
    }
    
    public ListItemModel Model
    {
        set
        {
            _titleView.Text = value.Title;
            _descriptionView.Text = value.Description;
            _iconView.Source = value.IconPath;
            IsSelected = value.IsSelected;
        }
    }
}

// 리스트 어댑터
public class ListAdapter : RecyclerViewAdapter
{
    private readonly List<ListItemModel> _items;
    
    public ListAdapter(List<ListItemModel> items)
    {
        _items = items;
    }
    
    public override int GetItemCount()
    {
        return _items.Count;
    }
    
    public override View OnCreateViewHolder(int viewType)
    {
        return new CustomListItem();
    }
    
    public override void OnBindViewHolder(View holder, int position)
    {
        if (holder is CustomListItem item)
        {
            item.Model = _items[position];
            item.Clicked += (sender, e) => {
                // 아이템 클릭 처리
                OnItemClicked?.Invoke(this, new ItemClickedEventArgs(position, _items[position]));
            };
        }
    }
    
    public event EventHandler<ItemClickedEventArgs> OnItemClicked;
}

// 리스트 뷰
public class ListView : View
{
    private readonly RecyclerView _recyclerView;
    private readonly ListAdapter _adapter;
    
    public ListView()
    {
        // 샘플 데이터 생성
        var items = new List<ListItemModel>
        {
            new ListItemModel { Title = "Item 1", Description = "Description 1", IconPath = "res/icons/item1.png" },
            new ListItemModel { Title = "Item 2", Description = "Description 2", IconPath = "res/icons/item2.png" },
            new ListItemModel { Title = "Item 3", Description = "Description 3", IconPath = "res/icons/item3.png" }
        };
        
        // 어댑터 생성
        _adapter = new ListAdapter(items);
        _adapter.OnItemClicked += OnItemClicked;
        
        // 리사이클러 뷰 설정
        _recyclerView = new RecyclerView()
        {
            Adapter = _adapter,
            LayoutManager = new LinearLayoutManager()
            {
                Orientation = Orientation.Vertical
            }
        };
        
        // 레이아웃 설정
        Layout = new LinearLayout();
        AddChild(_recyclerView);
    }
    
    private void OnItemClicked(object sender, ItemClickedEventArgs e)
    {
        Console.WriteLine($"Item clicked: {e.Item.Title} at position {e.Position}");
    }
}

public class ItemClickedEventArgs : EventArgs
{
    public int Position { get; }
    public ListItemModel Item { get; }
    
    public ItemClickedEventArgs(int position, ListItemModel item)
    {
        Position = position;
        Item = item;
    }
}
```

### 2.3. 내비게이션 및 화면 전환

```csharp
// 홈 화면
public class HomeView : View
{
    private readonly Navigator _navigator;
    
    public HomeView(Navigator navigator)
    {
        _navigator = navigator;
        
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        var title = new TextView()
        {
            Text = "Home Screen",
            FontSize = 24,
            Margin = new Thickness(0, 0, 0, 32)
        };
        
        var detailButton = new ClickableBox()
        {
            Text = "Go to Detail",
            Size = new Size(200, 50)
        };
        
        detailButton.Clicked += OnDetailButtonClicked;
        
        AddChild(title);
        AddChild(detailButton);
    }
    
    private void OnDetailButtonClicked(object sender, EventArgs e)
    {
        var detailView = new DetailView(_navigator);
        _navigator.Push(detailView, new SlideTransition());
    }
}

// 상세 화면
public class DetailView : View
{
    private readonly Navigator _navigator;
    
    public DetailView(Navigator navigator)
    {
        _navigator = navigator;
        
        Layout = new LinearLayout()
        {
            Orientation = Orientation.Vertical,
            Padding = new Thickness(16)
        };
        
        var title = new TextView()
        {
            Text = "Detail Screen",
            FontSize = 24,
            Margin = new Thickness(0, 0, 0, 32)
        };
        
        var backButton = new ClickableBox()
        {
            Text = "Back to Home",
            Size = new Size(200, 50)
        };
        
        backButton.Clicked += OnBackButtonClicked;
        
        AddChild(title);
        AddChild(backButton);
    }
    
    private void OnBackButtonClicked(object sender, EventArgs e)
    {
        _navigator.Pop(new SlideTransition());
    }
}

// 메인 애플리케이션
public class MainApplication : UIApplication
{
    private Navigator _navigator;
    
    public override void OnCreate()
    {
        base.OnCreate();
        
        // 내비게이터 생성
        _navigator = new Navigator();
        
        // 홈 화면 설정
        var homeView = new HomeView(_navigator);
        _navigator.SetRoot(homeView);
        
        // 윈도우에 내비게이터 추가
        MainWindow.AddChild(_navigator);
    }
}
```

## 3. 성능 최적화

### 3.1. 뷰 풀링

```csharp
// 뷰 풀 관리자
public class ViewPool
{
    private readonly Queue<View> _pool = new Queue<View>();
    private readonly Func<View> _createView;
    
    public ViewPool(Func<View> createView)
    {
        _createView = createView;
    }
    
    public View GetView()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }
        return _createView();
    }
    
    public void ReturnView(View view)
    {
        // 뷰 상태 초기화
        ResetView(view);
        _pool.Enqueue(view);
    }
    
    private void ResetView(View view)
    {
        // 뷰의 상태를 초기화
        if (view is TextView textView)
        {
            textView.Text = "";
            textView.TextColor = Color.Black;
        }
        else if (view is ImageView imageView)
        {
            imageView.Source = null;
        }
        // 기타 뷰 타입에 대한 초기화
    }
}

// 풀링을 사용하는 어댑터
public class PooledListAdapter : RecyclerViewAdapter
{
    private readonly List<ListItemModel> _items;
    private readonly ViewPool _viewPool;
    
    public PooledListAdapter(List<ListItemModel> items)
    {
        _items = items;
        _viewPool = new ViewPool(() => new CustomListItem());
    }
    
    public override int GetItemCount()
    {
        return _items.Count;
    }
    
    public override View OnCreateViewHolder(int viewType)
    {
        return _viewPool.GetView();
    }
    
    public override void OnBindViewHolder(View holder, int position)
    {
        if (holder is CustomListItem item)
        {
            item.Model = _items[position];
        }
    }
    
    public override void OnViewRecycled(View holder)
    {
        _viewPool.ReturnView(holder);
    }
}
```

### 3.2. 레이아웃 최적화

```csharp
// 최적화된 뷰
public class OptimizedView : View
{
    private bool _layoutRequested = false;
    
    public override void RequestLayout()
    {
        if (!_layoutRequested)
        {
            _layoutRequested = true;
            base.RequestLayout();
        }
    }
    
    public override void Layout()
    {
        _layoutRequested = false;
        // 레이아웃 계산
        base.Layout();
    }
    
    // 불필요한 렌더링 방지
    public override void OnDraw(Canvas canvas)
    {
        if (Visibility != Visibility.Visible)
            return;
            
        base.OnDraw(canvas);
    }
}
```

## 4. 접근성 지원

### 4.1. 스크린 리더 지원

```csharp
// 접근성 정보를 제공하는 뷰
public class AccessibleView : View
{
    public string AccessibilityName { get; set; }
    public string AccessibilityDescription { get; set; }
    public bool IsAccessibilityFocusable { get; set; } = true;
    
    public override string GetAccessibilityName()
    {
        return AccessibilityName ?? base.GetAccessibilityName();
    }
    
    public override string GetAccessibilityDescription()
    {
        return AccessibilityDescription ?? base.GetAccessibilityDescription();
    }
    
    public override bool IsAccessibilityFocused()
    {
        return IsAccessibilityFocusable && base.IsAccessibilityFocused();
    }
}

// 접근성을 고려한 버튼
public class AccessibleButton : ClickableBox
{
    public AccessibleButton()
    {
        IsAccessibilityFocusable = true;
        AccessibilityRole = AccessibilityRole.Button;
    }
    
    public override string GetAccessibilityName()
    {
        return Text ?? "Button";
    }
    
    public override string GetAccessibilityDescription()
    {
        return "Click to perform action";
    }
}
```

### 4.2. 키보드 내비게이션

```csharp
// 키보드 포커스를 지원하는 뷰
public class FocusableView : View
{
    public View NextFocus { get; set; }
    public View PreviousFocus { get; set; }
    public bool IsFocusable { get; set; } = true;
    
    public virtual bool RequestFocus()
    {
        if (!IsFocusable) return false;
        
        // 포커스 요청 처리
        IsFocused = true;
        OnFocusChanged(true);
        return true;
    }
    
    public virtual void ClearFocus()
    {
        IsFocused = false;
        OnFocusChanged(false);
    }
    
    protected virtual void OnFocusChanged(bool hasFocus)
    {
        // 포커스 상태 변경 시 처리
        if (hasFocus)
        {
            BackgroundColor = Color.LightBlue;
        }
        else
        {
            BackgroundColor = Color.White;
        }
    }
}
```

## 5. 테스트 및 디버깅

### 5.1. 단위 테스트

```csharp
// 컴포넌트 단위 테스트
[TestFixture]
public class ComponentTests
{
    [Test]
    public void TestButtonClick()
    {
        // Given
        var button = new ClickableBox()
        {
            Text = "Test Button"
        };
        
        var clicked = false;
        button.Clicked += (sender, e) => clicked = true;
        
        // When
        button.OnClicked();
        
        // Then
        Assert.IsTrue(clicked);
    }
    
    [Test]
    public void TestSelection()
    {
        // Given
        var selectable = new SelectableBox()
        {
            Text = "Test Selectable"
        };
        
        // When
        selectable.IsSelected = true;
        
        // Then
        Assert.IsTrue(selectable.IsSelected);
    }
}
```

### 5.2. UI 테스트

```csharp
// UI 테스트
[TestFixture]
public class UITests
{
    [Test]
    public void TestListViewInteraction()
    {
        // Given
        var listView = new ListView();
        var adapter = listView.Adapter as ListAdapter;
        
        // When
        adapter.OnItemClicked += (sender, e) => {
            // 아이템 클릭 시 처리
        };
        
        // Then
        // UI 상태 검증
        Assert.IsNotNull(listView);
        Assert.IsTrue(adapter.GetItemCount() > 0);
    }
}
```

## 6. 모범 사례

### 6.1. 컴포넌트 설계 원칙

1. **단일 책임 원칙**: 각 컴포넌트는 하나의 책임만을 가져야 합니다.
2. **개방-폐쇄 원칙**: 확장에는 열려있고 수정에는 닫혀 있어야 합니다.
3. **리스코프 치환 원칙**: 자식 클래스는 부모 클래스를 대체할 수 있어야 합니다.
4. **인터페이스 분리 원칙**: 클라이언트가 사용하지 않는 메서드에 의존하지 않아야 합니다.
5. **의존성 역전 원칙**: 고수준 모듈은 저수준 모듈에 의존하지 않아야 합니다.

### 6.2. 성능 고려사항

1. **뷰 풀링**: 재사용 가능한 뷰는 풀링하여 메모리 사용 최적화
2. **레이아웃 최적화**: 불필요한 레이아웃 요청 최소화
3. **이벤트 처리**: 이벤트 디바운싱을 통해 성능 향상
4. **리소스 관리**: 이미지 및 리소스의 적절한 로딩 및 해제

### 6.3. 유지보수성

1. **명확한 명명 규칙**: 직관적인 클래스 및 메서드 이름 사용
2. **문서화**: 중요한 클래스와 메서드에 대한 문서 제공
3. **테스트 가능성**: 테스트 가능한 구조로 설계
4. **확장성**: 새로운 기능 추가를 고려한 설계

## 결론

Tizen.UI.Components는 현대적인 UI 개발을 위한 강력한 프레임워크를 제공합니다. 이 가이드에서 설명한 개념과 패턴을 활용하면 다음과 같은 이점을 얻을 수 있습니다:

1. **효율적인 개발**: 재사용 가능한 컴포넌트를 통해 개발 시간 단축
2. **일관된 UX**: 표준화된 컴포넌트를 통해 일관된 사용자 경험 제공
3. **높은 성능**: 최적화된 구조를 통해 우수한 성능 보장
4. **쉬운 유지보수**: 명확한 구조와 문서화를 통해 유지보수 용이성 향상

이 가이드를 참고하여 Tizen.UI.Components를 효과적으로 활용하고, 고품질의 Tizen 애플리케이션을 개발하시기 바랍니다.
