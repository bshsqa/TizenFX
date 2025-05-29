
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class CaptureTest : IExample
    {
        View viewB;

        bool isGenB = false;

        int cellCount = 50;

        Animation animation;

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;

            Vector2 windowHalfSize = new Vector2(window.Size.Width/2.0f, window.Size.Height);

            View viewA = new View()
            {
                PositionUsesPivotPoint = true,
                Size = new Size(windowHalfSize.X, windowHalfSize.Y),
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft
            };
            window.Add(viewA);

            animation = new Animation(5000)
            {
                Looping = true,
            };
            animation.AnimateTo(viewA, "PositionX", 100.0f);
            animation.Play();

            viewB = new View()
            {
                PositionUsesPivotPoint = true,
                Size = new Size(windowHalfSize.X, windowHalfSize.Y),
                PivotPoint = PivotPoint.TopRight,
                ParentOrigin = ParentOrigin.TopRight
            };
            window.Add(viewB);
            viewB.Ignored = true;

            Size viewSize = new Size(windowHalfSize.X / cellCount, windowHalfSize.Y / cellCount);
            for (int i = 0; i < cellCount; ++i)
            {
                for (int j = 0; j < cellCount; ++j)
                {
                    View view = new View()
                    {
                        BackgroundColor = Color.Red,
                        BorderlineWidth = 3.0f,
                        PivotPoint = PivotPoint.TopLeft,
                        ParentOrigin = ParentOrigin.TopLeft,
                        Position = new Position(viewSize.Width * i, viewSize.Height * j),
                        Size = viewSize
                    };
                    viewA.Add(view);
                }
            }
        }

        private void GenB()
        {
            if(isGenB)
            {
                return;
            }

            Vector2 windowHalfSize = new Vector2(window.Size.Width/2.0f, window.Size.Height);
            Size viewSize = new Size(windowHalfSize.X / cellCount, windowHalfSize.Y / cellCount);
            for (int i = 0; i < cellCount; ++i)
            {
                for (int j = 0; j < cellCount; ++j)
                {
                    View view = new View()
                    {
                        BackgroundColor = Color.Blue,
                        BorderlineWidth = 3.0f,
                        PivotPoint = PivotPoint.TopLeft,
                        ParentOrigin = ParentOrigin.TopLeft,
                        Position = new Position(viewSize.Width * i, viewSize.Height * j),
                        Size = viewSize
                    };
                    viewB.Add(view);
                }
            }
            isGenB = true;
        }

        private void Win_TouchEvent(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                GenB();
                viewB.Ignored = !viewB.Ignored;
            }
        }

        public void Deactivate()
        {
        }

        private Window window;
    }
}
