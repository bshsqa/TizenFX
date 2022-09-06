
using global::System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Scene3D;
using NUnit.Framework;

namespace Tizen.NUI.Samples
{
    using log = Tizen.Log;
    public class SceneViewTest : IExample
    {
        private Window window;
        private Size windowSize;
        private SceneView sceneView;
        private Color backgroundColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
        private static readonly string resourcePath = "/home/seungho/SharedWork/myModel/";//Tizen.Applications.Application.Current.DirectoryInfo.Resource;
        private float multiplier;

        public void Activate()
        {
            FocusManager.Instance.EnableDefaultAlgorithm(true);
            window = NUIApplication.GetDefaultWindow();
            windowSize = window.Size;
            window.BackgroundColor = backgroundColor;
            window.TouchEvent += OnWindowTouch;
            window.KeyEvent += (s, e) =>
            {
                if(e.Key.KeyPressedName == "1")
                {
                    sceneView.SelectCamera(0u);
                }
                if(e.Key.KeyPressedName == "2")
                {
                    sceneView.SelectCamera(1u);
                }
                if(e.Key.KeyPressedName == "3")
                {
                    // Make orthographic projection
                    sceneView.GetSelectedCamera().OrthographicSize = 5.0f;
                    sceneView.GetSelectedCamera().ProjectionMode = Tizen.NUI.Scene3D.Camera.ProjectionModeType.Orthographic;
                }
                if(e.Key.KeyPressedName == "4")
                {
                    // Make perspective projection
                    sceneView.GetSelectedCamera().ProjectionMode = Tizen.NUI.Scene3D.Camera.ProjectionModeType.Perspective;
                }
                Vector3 direction = new Vector3(0.0f, 0.0f, 1.0f);
                float moveDisplacement = 0.2f;
                Rotation leftDisplacement = new Rotation(new Radian(new Degree(2.0f)), Vector3.YAxis);
                Rotation rightDisplacement = new Rotation(new Radian(new Degree(-2.0f)), Vector3.YAxis);
//                if(e.Key.KeyPressedName == "Up")
//                {
//                    Vector3 rotDirection = sceneView.GetSelectedCamera().Orientation.Rotate(direction);
//                    rotDirection.Normalize();
//                    sceneView.GetSelectedCamera().PositionX += rotDirection.X * moveDisplacement;
//                    sceneView.GetSelectedCamera().PositionZ += rotDirection.Z * moveDisplacement;
//                }
//                if(e.Key.KeyPressedName == "Down")
//                {
//                    Vector3 rotDirection = sceneView.GetSelectedCamera().Orientation.Rotate(direction);
//                    rotDirection.Normalize();
//                    sceneView.GetSelectedCamera().PositionX -= rotDirection.X * moveDisplacement;
//                    sceneView.GetSelectedCamera().PositionZ -= rotDirection.Z * moveDisplacement;
//                }
//                if(e.Key.KeyPressedName == "Right")
//                {
//                    sceneView.GetSelectedCamera().Orientation = rightDisplacement * sceneView.GetSelectedCamera().Orientation;
//                }
//                if(e.Key.KeyPressedName == "Left")
//                {
//                    sceneView.GetSelectedCamera().Orientation = leftDisplacement * sceneView.GetSelectedCamera().Orientation;
//                }
                Tizen.Log.Error("NUI", $"camera Position : {sceneView.GetSelectedCamera().Position.X}, {sceneView.GetSelectedCamera().Position.Y}, {sceneView.GetSelectedCamera().Position.Z}\n");
                Vector3 axis = new Vector3();
                Radian radian = new Radian();
                sceneView.GetSelectedCamera().Orientation.GetAxisAngle(axis, radian);
                Tizen.Log.Error("NUI", $"axis : {axis.X}, {axis.Y}, {axis.Z}, {radian.ConvertToFloat()}\n");
            };
            Tizen.Log.Error("NUI", $"windowSize : {windowSize.Width}, {windowSize.Height}\n");

            multiplier = windowSize.Width / 720.0f;
            MakeScene();
        }

        void MakeScene()
        {
            if (sceneView != null)
            {
                return;
            }

            sceneView = new SceneView()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
                PositionUsesPivotPoint = true,
                UseFramebuffer = true,
            };
            window.Add(sceneView);
            sceneView.GetCamera(0u).PositionX = -2.2f;
            sceneView.GetCamera(0u).PositionY = -1.5f;
            sceneView.GetCamera(0u).PositionZ = 1.0f;
            sceneView.GetCamera(0u).NearPlaneDistance = 0.1f;
            sceneView.GetCamera(0u).FarPlaneDistance = 500.0f;
            sceneView.GetCamera(0u).Orientation = new Rotation(new Radian(1.9560518f), new Vector3(-0.029408932f, 0.9986161f, 0.04360052f));
            sceneView.GetCamera(0u).FieldOfView = new Radian(new Degree(70.0f));
            sceneView.SetImageBasedLightSource(resourcePath + "images/Irradiance.ktx", resourcePath + "images/Radiance.ktx", 0.7f);

            Tizen.NUI.Scene3D.Camera camera = new Tizen.NUI.Scene3D.Camera()
            {
                PositionX = -2.5f,
                PositionY = -5.5f,
                PositionZ = -2.0f,
                NearPlaneDistance = 0.1f,
                FarPlaneDistance = 500.0f,
                Orientation = new Rotation(new Radian(0.5996008f), new Vector3(-0.14123423f, 0.9890342f, 0.043179594f)),
                FieldOfView = new Radian(new Degree(70.0f)),
            };
            sceneView.AddCamera(camera);

            FocusManager.Instance.FocusIndicator = null;
            ImageView focusImage = new ImageView();
            Model bicycle = new Model(resourcePath + "models/bicycle/Old Bicycle.gltf")
            {
                Position = new Vector3(4, 0, -2),
                Focusable = true,
            };
            bicycle.ResourcesLoaded += (e, s) =>
            {
                Tizen.Log.Error("NUI", $"Bicycle size : {bicycle.Size.Width}, {bicycle.Size.Height}\n");
                focusImage = new ImageView(resourcePath + "images/floor.png")
                {
                    Size = new Size(bicycle.Size.Width, bicycle.Size.Depth),
                    Scale = new Vector3(1.2f, 1.2f, 1.2f),
                    Orientation = new Rotation(new Radian(new Degree(90.0f)), Vector3.XAxis),
                    ParentOrigin = ParentOrigin.BottomCenter,
                    PivotPoint = PivotPoint.Center,
                    PositionUsesPivotPoint = true,
                };
            };
            bicycle.FocusGained += (s, e) =>
            {
                bicycle.Add(focusImage);
            };
            sceneView.Add(bicycle);

            Model bicycle2 = new Model(resourcePath + "models/bicycle/Old Bicycle.gltf")
            {
                Position = new Vector3(4, 0, -1),
                Focusable = true,
            };
            bicycle2.ResourcesLoaded += (s, e) =>
            {
                Tizen.Log.Error("NUI", $"Bicycle2 size : {bicycle2.Size.Width}, {bicycle2.Size.Height}, {bicycle2.Size.Depth}\n");
            };
            bicycle2.FocusGained += (s, e) =>
            {
                bicycle2.Add(focusImage);
            };
            sceneView.Add(bicycle2);

            ImageView frame = new ImageView(resourcePath + "images/lake_front.jpg")
            {
                FittingMode = FittingModeType.ScaleToFill,
                Size = new Size(1.0f, 1.5f),
                Position = new Position(0.0f, 1.0f, -2.5f),
                ParentOrigin = ParentOrigin.Center,
                PivotPoint = PivotPoint.Center,
                PositionUsesPivotPoint = true,
            };
            sceneView.Add(frame);

            TextLabel livingRoomText = new TextLabel()
            {
                Text = "Living Room",
                Size = new Size(200, 100),
                Position = new Vector3(1, 0, -1),
                Orientation = new Rotation(new Radian(new Degree(-90.0f)), Vector3.YAxis) *
                              new Rotation(new Radian(new Degree(90.0f)), Vector3.XAxis),
                PointSize = 10,
                Scale = new Vector3(0.02f, 0.02f, 1),
                ParentOrigin = ParentOrigin.Center,
                PivotPoint = PivotPoint.Center,
                PositionUsesPivotPoint = true,
            };
            sceneView.Add(livingRoomText);

//            Model home = new Model(resourcePath + "models/staircase/staircase.gltf")
//            {
//                Position = new Vector3(0, 0, 0),
//            };
//            home.ResourcesLoaded += (e, s) =>
//            {
//                Tizen.Log.Error("NUI", $"Staircase size : {home.Size.Width}, {home.Size.Height}\n");
//            };
//            sceneView.Add(home);
        }

        private void OnWindowTouch(object sender, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                Radian fov = sceneView.GetCamera(0u).FieldOfView;
                Vector3 cameraPosition = sceneView.GetCamera(0u).Position;
                float nearPlain = sceneView.GetCamera(0u).NearPlaneDistance;
                float farPlain = sceneView.GetCamera(0u).FarPlaneDistance;

                Tizen.Log.Error("NUI", $"cpos : {cameraPosition.Z}\n");
                Tizen.Log.Error("NUI", $"fov :  {fov.ConvertToFloat()}\n");
                Tizen.Log.Error("NUI", $"near : {nearPlain}\n");
                Tizen.Log.Error("NUI", $"far  : {farPlain}\n");
            }
        }

        public void Deactivate()
        {
        }
    }
}
