
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

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
            window.SetBackgroundColor(Color.LightGrey);

            View upperBotton = CreateUpperBottonStyle(new Size(500.0f, 100.0f));
            upperBotton.Position = new Position(50.0f, 50.0f);
            window.Add(upperBotton);

            View lowerBotton = CreateLowerBottonStyle(new Size(500.0f, 100.0f));
            lowerBotton.Position = new Position(50.0f, 200.0f);
            window.Add(lowerBotton);
        }

        private View CreateUpperBottonStyle(Size size)
        {
            float blurRadius = 20.0f;
            float cornerRadius = Math.Min(size.Width, size.Height) / 2.0f;
            Visuals.ColorVisual shadowVisual1 = new Visuals.ColorVisual()
            {
                Name = "shadow1",
                Color = Color.Gray,
                BlurRadius = blurRadius,
                OffsetX = 10.0f,
                OffsetY = 10.0f,
                CornerRadius = cornerRadius,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
            };

            Visuals.ColorVisual shadowVisual2 = new Visuals.ColorVisual()
            {
                Name = "shadow2",
                Color = new Vector4(0.9f, 0.9f, 0.9f, 1.0f),
                BlurRadius = blurRadius,
                OffsetX = -10.0f,
                OffsetY = -10.0f,
                CornerRadius = cornerRadius,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
            };

            Visuals.ColorVisual background = new Visuals.ColorVisual()
            {
                Name = "background",
                Color = Color.LightGray,
                CornerRadius = cornerRadius,
            };

            TextLabel botton = new TextLabel("shape case 1")
            {
                Name = "test_root",
                Size = size,
                CornerRadius = Math.Min(size.Width, size.Height) / 2.0f,
                BackgroundColor = Color.LightGrey,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
            };
            botton.AddVisual(shadowVisual1);
            botton.AddVisual(shadowVisual2);
            botton.AddVisual(background);
            shadowVisual1.LowerToBottom();
            shadowVisual2.LowerToBottom();

            return botton;
        }

        private View CreateLowerBottonStyle(Size size)
        {
            float blurRadius = 20.0f;
            float cornerRadius = Math.Min(size.Width, size.Height) / 2.0f;

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
                BorderlineColor = Color.Gray,//new Vector4(0.5f, 0.5f, 0.5f, 0.5f),
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

                OffsetX = -10.0f,
                OffsetY = -10.0f,

                ExtraWidth = 10.0f,
                ExtraHeight = 10.0f,

                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,

                BorderlineWidth = blurRadius + 1.0f,
                BorderlineColor = new Vector4(0.9f, 0.9f, 0.9f, 1.0f),
                BorderlineOffset = 1.0f,

                BlurRadius = blurRadius, //(blurRadius / 2.0f),

                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
            };

            TextLabel botton = new TextLabel("shape case 2")
            {
                Name = "test_root",
                Size = size,
                CornerRadius = cornerRadius,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
            };
            botton.AddVisual(shadowVisual1);
            botton.AddVisual(shadowVisual2);
            shadowVisual1.LowerToBottom();
            shadowVisual2.LowerToBottom();

            return botton;
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
