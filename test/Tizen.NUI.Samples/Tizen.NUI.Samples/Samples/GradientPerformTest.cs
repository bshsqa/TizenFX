
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;
using Tizen.NUI.Text;
using Tizen.NUI.BaseComponents.VectorGraphics;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class GradientPerformTest : IExample
    {
        private Window window;
        private View root;

        GradientVisual gradientVisual;
        private static string resourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;

            ImageView imageView = new ImageView()
            {
                ResourceUrl = resourcePath + "/images/test2.jpg",
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                SynchronousLoading = true,
            };
            window.Add(imageView);

            root = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
            };
            window.Add(root);

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    View cardView = CreateCardViewStyle(new Size(200.0f, 200.0f));
                    cardView.Position = new Position(i * 200, j * 200);
                    root.Add(cardView);
                }
            }

            PropertyArray stopColor = new PropertyArray();
            stopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(31.0f / 255.0f, 120.0f / 255.0f, 237.0f / 255.0f, 1.0f)));
            stopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(55.0f / 255.0f, 219.0f / 255.0f, 224.0f / 255.0f, 1.0f)));
            gradientVisual = new GradientVisual()
            {
                StartPosition = new Vector2(0.0f, 0.0f),
                EndPosition = new Vector2(1.0f, 0.0f),
                Units = GradientVisualUnitsType.ObjectBoundingBox,
                StopColor = stopColor
            };
        }

        private View CreateCardViewStyle(Size size)
        {
            View card = new View()
            {
                Size = size,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
                BackgroundColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
            };

            return card;
        }

        bool created = false;
        private void Win_TouchEvent(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                if (!created)
                {
                    uint childcnt = root.ChildCount;
                    for (uint i = 0; i < childcnt; ++i)
                    {
                        View child = root.GetChildAt(i);
                        child.Background = gradientVisual.OutputVisualMap;
                    }
                    created = true;
                }
            }
        }

        public void Deactivate()
        {
            var root = NUIApplication.GetDefaultWindow().GetRootLayer();
            while (root.ChildCount > 0)
            {
                var child = root.GetChildAt(0);
                root.Remove(child);
                child.DisposeRecursively();
            }
        }
    }
}
