
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class CaptureTest : IExample
    {
        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;

            window.VisibilityChanged += (s, e) =>
            {
                Tizen.Log.Error("NUI", $"window visibility : {e.Visibility}\n");
            };

            parent = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
            };
            window.Add(parent);

            textLabel = new TextLabel("Hello World")
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
            };
            textLabel.AggregatedVisibilityChanged += (s, e) =>
            {
                Tizen.Log.Error("NUI", $"visibility : {e.Visibility}\n");
            };
            parent.Add(textLabel);
        }

        private void Win_TouchEvent(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Tizen.Log.Error("NUI", "Hide - Window\n");
                window.Hide();

                Tizen.Log.Error("NUI", "Hide - Parent\n");
                parent.Hide();

                Tizen.Log.Error("NUI", "Show - Window\n");
                window.Show();

                Tizen.Log.Error("NUI", "Show - Parent\n");
                parent.Show();

                Tizen.Log.Error("NUI", "Hide - Window\n");
                window.Hide();

                Tizen.Log.Error("NUI", "Show - Window\n");
                window.Show();

                Tizen.Log.Error("NUI", "Hide - textLabel\n");
                textLabel.Hide();

                Tizen.Log.Error("NUI", "Show - textLabel\n");
                textLabel.Show();

                Tizen.Log.Error("NUI", "Hide - textLabel\n");
                textLabel.Hide();

                Tizen.Log.Error("NUI", "Hide - Window\n");
                window.Hide();

                Tizen.Log.Error("NUI", "Show - Window\n");
                window.Show();

                Tizen.Log.Error("NUI", "Show - textLabel\n");
                textLabel.Show();

                Tizen.Log.Error("NUI", "Hide - textLabel\n");
                textLabel.Hide();

                Tizen.Log.Error("NUI", "Hide - Window\n");
                window.Hide();

                Tizen.Log.Error("NUI", "Show - textLabel\n");
                textLabel.Show();

                Tizen.Log.Error("NUI", "Show - Window\n");
                window.Show();
            }
        }

        public void Deactivate()
        {
        }

        private Window window;
        private TextLabel textLabel;
        private View parent;
    }
}
