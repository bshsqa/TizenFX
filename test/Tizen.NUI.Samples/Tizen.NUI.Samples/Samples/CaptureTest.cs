
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using NUnit.Framework;
using Tizen.NUI.Components;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class CaptureTest : IExample
    {
        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.TouchEvent += Win_TouchEvent;

            View view = new View()
            {
                Size = new Size(500.0f, 200.0f),
                PositionUsesPivotPoint = true,
                ParentOrigin = ParentOrigin.Center,
                PivotPoint = PivotPoint.Center,
                BackgroundColor = Color.Blue,
                CornerRadius = 50.0f,
            };
            window.Add(view);

            Visuals.VisualBase shadow = Visuals.ShadowVisualUtility.CreateBoxShadow(50.0f, Color.Black);
            view.AddVisual(shadow);

            View view2 = new View()
            {
                Position = new Position(0.0f, 400.0f),
                Size = new Size(500.0f, 200.0f),
                PositionUsesPivotPoint = true,
                ParentOrigin = ParentOrigin.Center,
                PivotPoint = PivotPoint.Center,
                BackgroundColor = Color.Blue,
                CornerRadius = 50.0f,
            };
            window.Add(view2);

            Visuals.VisualBase innerShadow = Visuals.ShadowVisualUtility.CreateInnerShadow(new UIExtents(0.0f), 50.0f, Color.Black);
            view2.AddVisual(innerShadow);


            //            float blurRadius = 50.0f;
            //            Visuals.ColorVisual shadowVisual1 = new Visuals.ColorVisual()
            //            {
            //                Color = Color.Black,
            //                BlurRadius = blurRadius,
            //                OffsetXPolicy = VisualTransformPolicyType.Absolute,
            //                OffsetYPolicy = VisualTransformPolicyType.Absolute,
            //                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
            //            };
            //            view.AddVisual(shadowVisual1);
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

        private Window window;
    }
}
