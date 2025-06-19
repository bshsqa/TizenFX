
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
        private Vector4 ShadowColor = Color.Gray;
        private Vector4 whiteShadowColor = new Vector4(0.9f, 0.9f, 0.9f, 1.0f);
        private Vector4 backgroundColor = Color.LightGray;
        private Vector4 fontColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        private Window window;
        Size shortViewSize = new Size(200.0f, 80.0f);
        Size middleViewSize = new Size(350.0f, 80.0f);
        Size longViewSize = new Size(600.0f, 80.0f);
        Size squareViewSize = new Size(200.0f, 200.0f);
        float cornerRadius = 40.0f;

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;
            window.SetBackgroundColor(backgroundColor);

            View squareButton = CreateUpperButtonStyle(squareViewSize, cornerRadius, "");
            squareButton.Position = new Position(50.0f, 50.0f);
            window.Add(squareButton);
            TextLabel squareText = new TextLabel("Aa")
            {
                Size = squareViewSize,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                PointSize = 50.0f,
            };
            var fontStyle = new Tizen.NUI.Text.FontStyle();
            fontStyle.Weight = FontWeightType.Bold;
            squareText.SetFontStyle(fontStyle);

            View maskView = new View()
            {
                Size = squareViewSize,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center
            };
            PropertyArray stopColor = new PropertyArray();
            stopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(31.0f/255.0f, 120.0f/255.0f, 237.0f/255.0f, 1.0f)));
            stopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(55.0f/255.0f, 219.0f/255.0f, 224.0f/255.0f, 1.0f)));
            GradientVisual gradientVisual = new GradientVisual()
            {
                StartPosition = new Vector2(0.0f, 0.0f),
                EndPosition = new Vector2(1.0f, 0.0f),
                Units = GradientVisualUnitsType.ObjectBoundingBox,
                StopColor = stopColor
            };
            maskView.Background = gradientVisual.OutputVisualMap;

            RenderEffect mask = RenderEffect.CreateMaskEffect(squareText);
            maskView.SetRenderEffect(mask);
            maskView.Add(squareText);
            squareButton.Add(maskView);

            View upperButton = CreateUpperButtonStyle(shortViewSize, cornerRadius, "Button");
            upperButton.Position = new Position(450.0f, 50.0f);
            window.Add(upperButton);

            View circleButton = CreateUpperButtonStyle(new Size(80.0f, 80.0f), cornerRadius, "");
            circleButton.Position = new Position(300.0f, 50.0f);
            window.Add(circleButton);

            View dot = new View()
            {
                Size = new Size(26.0f, 26.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                CornerRadius = 13.0f,
                BackgroundColor = fontColor,
            };
            circleButton.Add(dot);

            View lowerButton = CreateLowerButtonStyle(middleViewSize, cornerRadius, "inset");
            lowerButton.Position = new Position(300.0f, 170.0f);
            window.Add(lowerButton);

            View searchButton = CreateLowerButtonStyle2(longViewSize, cornerRadius, "    Search for ...");
            searchButton.Position = new Position(50.0f, 290.0f);
            window.Add(searchButton);
            (searchButton as TextLabel).HorizontalAlignment = HorizontalAlignment.Begin;

            PropertyArray style2BackgroundStopColor = new PropertyArray();
            style2BackgroundStopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(0.86f, 0.86f, 0.86f, 1.0f)));
            style2BackgroundStopColor.PushBack(new Tizen.NUI.PropertyValue(new Vector4(0.8f, 0.8f, 0.8f, 1.0f)));
            GradientVisual style2BackgroundGradientVisual = new GradientVisual()
            {
                StartPosition = new Vector2(0.0f, 0.0f),
                EndPosition = new Vector2(1.0f, 0.0f),
                Units = GradientVisualUnitsType.ObjectBoundingBox,
                StopColor = style2BackgroundStopColor
            };
            (searchButton as TextLabel).Background = style2BackgroundGradientVisual.OutputVisualMap;

            View slide = CreateLowerButtonStyle2(new Size(50.0f, 320.0f), cornerRadius, "");
            slide.Position = new Position(700.0f, 50.0f);
            window.Add(slide);

            View slideFill = new View()
            {
                Size = new Size(50.0f, 100.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomCenter,
                ParentOrigin = ParentOrigin.BottomCenter,
                CornerRadius = 25.0f,
                BackgroundColor = new Vector4(22.0f/255.0f, 84.0f/255.0f, 240.0f/255.0f, 1.0f),
            };
            slide.Add(slideFill);

            View slide2 = CreateLowerButtonStyle2(new Size(50.0f, 320.0f), cornerRadius, "");
            slide2.Position = new Position(800.0f, 50.0f);
            window.Add(slide2);

            View slideFill2 = new View()
            {
                Size = new Size(50.0f, 250.0f),
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomCenter,
                ParentOrigin = ParentOrigin.BottomCenter,
                CornerRadius = 25.0f,
                BackgroundColor = new Vector4(22.0f/255.0f, 84.0f/255.0f, 240.0f/255.0f, 1.0f),
            };
            slide2.Add(slideFill2);
        }

        private View CreateUpperButtonStyle(Size size, float cornerRadius, string text)
        {
            float blurRadius = 20.0f;
            Visuals.ColorVisual shadowVisual1 = new Visuals.ColorVisual()
            {
                Name = "shadow1",
                Color = ShadowColor,
                BlurRadius = blurRadius,
                OffsetX = 10.0f,
                OffsetY = 10.0f,
                CornerRadius = cornerRadius,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
            };

            Visuals.ColorVisual shadowVisual2 = new Visuals.ColorVisual()
            {
                Name = "shadow2",
                Color = whiteShadowColor,
                BlurRadius = blurRadius,
                OffsetX = -10.0f,
                OffsetY = -10.0f,
                CornerRadius = cornerRadius,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
            };

            TextLabel button = new TextLabel(text)
            {
                Name = "test_root",
                Size = size,
                CornerRadius = Math.Min(size.Width, size.Height) / 2.0f,
                BackgroundColor = backgroundColor,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = fontColor,
            };
            button.AddVisual(shadowVisual1);
            button.AddVisual(shadowVisual2);
            shadowVisual1.LowerToBottom();
            shadowVisual2.LowerToBottom();

            return button;
        }

        private View CreateLowerButtonStyle(Size size, float cornerRadius, string text)
        {
            float blurRadius = 20.0f;
            Visuals.ColorVisual shadowVisual1 = new Visuals.ColorVisual()
            {
                Name = "inner shadow1",
                Color = Color.Transparent,
                //
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,

                OffsetX = 10.0f,
                OffsetY = 10.0f,

                ExtraWidth = 10.0f,
                ExtraHeight = 10.0f,

                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,

                BorderlineWidth = blurRadius + 1.0f,
                BorderlineColor = ShadowColor,
                BorderlineOffset = 1.0f,

                BlurRadius = blurRadius, //(blurRadius / 2.0f),

                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
            };

            Visuals.ColorVisual shadowVisual2 = new Visuals.ColorVisual()
            {
                Name = "inner shadow2",
                Color = Color.Transparent,
                //
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,

                OffsetX = -20.0f,
                OffsetY = -20.0f,

                ExtraWidth = 10.0f,
                ExtraHeight = 10.0f,

                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,

                BorderlineWidth = blurRadius + 1.0f,
                BorderlineColor = whiteShadowColor,
                BorderlineOffset = 1.0f,

                BlurRadius = blurRadius, //(blurRadius / 2.0f),

                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
            };

            TextLabel button = new TextLabel(text)
            {
                Name = "test_root",
                Size = size,
                CornerRadius = cornerRadius,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = fontColor,
            };
            button.AddVisual(shadowVisual1);
            button.AddVisual(shadowVisual2);
            shadowVisual1.LowerToBottom();
            shadowVisual2.LowerToBottom();

            return button;
        }

        private View CreateLowerButtonStyle2(Size size, float cornerRadius, string text)
        {
            float blurRadius = 10.0f;
            Visuals.ColorVisual shadowVisual1 = new Visuals.ColorVisual()
            {
                Name = "inner shadow1",
                Color = Color.Transparent,
                //
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,

                OffsetX = 5.0f,
                OffsetY = 5.0f,

                ExtraWidth = 10.0f,
                ExtraHeight = 10.0f,

                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,

                BorderlineWidth = blurRadius + 1.0f,
                BorderlineColor = ShadowColor,
                BorderlineOffset = 1.0f,

                BlurRadius = blurRadius, //(blurRadius / 2.0f),

                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
            };

            Visuals.ColorVisual shadowVisual2 = new Visuals.ColorVisual()
            {
                Name = "inner shadow2",
                Color = Color.Transparent,
                //
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,

                OffsetX = -15.0f,
                OffsetY = -15.0f,

                ExtraWidth = 10.0f,
                ExtraHeight = 10.0f,

                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,

                BorderlineWidth = blurRadius + 1.0f,
                BorderlineColor = whiteShadowColor,
                BorderlineOffset = 1.0f,

                BlurRadius = blurRadius, //(blurRadius / 2.0f),

                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
            };

            TextLabel button = new TextLabel(text)
            {
                Name = "test_root",
                Size = size,
                CornerRadius = cornerRadius,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = fontColor,
            };
            
            button.AddVisual(shadowVisual1);
            button.AddVisual(shadowVisual2);
            shadowVisual1.LowerToBottom();
            shadowVisual2.LowerToBottom();

            return button;
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
