using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Scene3D;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class CaptureTest : IExample
    {
        private Window window;
        private SceneView sceneView;
        private TextLabel textLabel;
        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();
            window.BackgroundColor = Color.White;
            Size2D windowSize = window.Size;
            
            sceneView = new SceneView()
            {
                Size = new Size(windowSize.Width / 2, windowSize.Height / 2),
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                PositionUsesPivotPoint = true,
                BackgroundColor = Color.Beige,
                UseFramebuffer = true,
            };
            window.Add(sceneView);

            textLabel = new TextLabel("SampleText")
            {
                Size = new Size(250, 200),
                Position = new Position(1.0f, 1.0f, 1.0f),
                Orientation = new Rotation(new Radian(new Degree(-30.0f)), Vector3.ZAxis),
                Scale = new Vector3(1.0f/50.0f, 1.0f/50.0f, 1.0f),
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                PositionUsesPivotPoint = true,
            };
            sceneView.Add(textLabel);

            Scene3D.Camera camera = sceneView.GetSelectedCamera();
            camera.Position = new Position(2.0f, 2.0f, 2.0f);
            camera.LookAt(Vector3.One);
            camera.NearPlaneDistance = 0.5f;
            camera.FarPlaneDistance = 10.0f;
        }

        private void WindowKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down)
            {
            }
        }

        public void Deactivate()
        {
            window.KeyEvent -= WindowKeyEvent;
        }
    }
}
