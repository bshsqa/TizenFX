
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
    public class CaptureTest : IExample
    {
        private Window window;

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;

            ImageView imageView = new ImageView()
            {
                ResourceUrl = "/home/seungho/Shared/test2.jpg",
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,                
            };
            window.Add(imageView);

            View cardView = CreateCardViewStyle(new Size(500.0f, 300.0f), 20.0f);
            window.Add(cardView);

            TextLabel name = new TextLabel("Tizen Platform Lab")
            {
                PointSize = 9,
                TextColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
                Position = new Position(30.0f, 120.0f)
            };
            cardView.Add(name);

            TextLabel number = new TextLabel("1 2 3 4    5 6 7 8   9 1 0 1   1 1 2 1")
            {
                PointSize = 12,
                TextColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
                Position = new Position(30.0f, 150.0f)
            };
            cardView.Add(number);

            TextLabel date = new TextLabel("10/30")
            {
                PointSize = 10,
                TextColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
                Position = new Position(30.0f, 250.0f)
            };
            cardView.Add(date);
        }

        private View CreateCardViewStyle(Size size, float cornerRadius)
        {
            float borderlineWidth = 3.0f;
            Visuals.ColorVisual borderLine = new Visuals.ColorVisual()
            {
                Color = Color.Transparent,
                BorderlineColor = new Color(0.7f, 0.7f, 0.7f, 0.5f),
                BorderlineOffset = 1.0f,
                BorderlineWidth = borderlineWidth,
                CornerRadius = cornerRadius + borderlineWidth,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
            };

            Visuals.ColorVisual shadowVisual = new Visuals.ColorVisual()
            {
                Name = "shadow2",
                Color = new Vector4 (0.3f, 0.3f, 0.3f, 1.0f),
                BlurRadius = 100.0f,
                CornerRadius = cornerRadius,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
            };

            View card = new View()
            {
                Name = "test_root",
                Size = size,
                CornerRadius = cornerRadius,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
            };
            card.SetRenderEffect(RenderEffect.CreateBackgroundBlurEffect(150.0f));
            card.AddVisual(borderLine);
            card.AddVisual(shadowVisual);
            borderLine.LowerToBottom();
            shadowVisual.LowerToBottom();

            return card;
        }

        private void Win_TouchEvent(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
            }
        }

        public void Deactivate()
        {
        }
    }
}
