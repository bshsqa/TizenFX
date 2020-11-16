using System.Collections.Generic;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.NUI.Samples
{
    using tlog = Tizen.Log;
    public class AnimatedImageViewTest1 : IExample
    {
        Window window;
        ScrollableBase scrollable;
        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            scrollable = new ScrollableBase()
            {
                Padding = new Extents(5),
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = 480,

                ScrollingDirection = ScrollableBase.Direction.Vertical,
//                Layout = new LinearLayout()
//                {
//                    LinearOrientation = LinearLayout.Orientation.Vertical,
//                },
                Layout = new GridLayout()
                {
                    Columns = 4,
                    GridOrientation = GridLayout.Orientation.Horizontal,
                }
            };
            window.Add(scrollable);

            for(int i=0; i<20000; ++i)
            {
                TextLabel text = new TextLabel()
                {
                    Margin = new Extents(10),
                    Text = i.ToString(),
                    Size = new Size(100, 100),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    PointSize = 12,
                    BackgroundColor = Color.Blue,
                };
                scrollable.Add(text);
            }

            TextLabel text2 = new TextLabel()
            {
                Text = i.ToString(),
                Size = new Size(100, 100),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                PointSize = 12,
                BackgroundColor = new Vector4(1.0f, 0.0f, 1.0f, 0.5f),
            };
            window.Add(text2);

            window.TouchEvent += OnTouch;
        }

        int i=0;
        private void OnTouch(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                if(i++ == 0)
                {
                    scrollable.ControlManyItem = true;
                }
            }
        }

        public void Deactivate()
        {
        }
    }
}
