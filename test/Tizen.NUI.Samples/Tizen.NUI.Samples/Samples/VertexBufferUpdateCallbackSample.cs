/*
 * Copyright (c) 2025 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Runtime.InteropServices;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace VertexBufferUpdateCallbackSample
{
    class Program : NUIApplication
    {
        private Window window;
        private View rootView;
        private Renderer renderer;
        private Geometry geometry;
        private VertexBuffer vertexBuffer;
        private VertexBufferUpdateCallback updateCallback;
        private GeometryUpdater geometryUpdater;

        // Vertex structure for animated quad
        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;
        }

        // Geometry updater class
        public class GeometryUpdater
        {
            private float time = 0.0f;

            public uint UpdateVertices(IntPtr ptr, ulong size, float elapsedSeconds)
            {
                unsafe
                {
                    Vertex* vertices = (Vertex*)ptr;
                    uint vertexCount = (uint)(size / (ulong)Marshal.SizeOf<Vertex>());

                    time += elapsedSeconds;
                    float blend = (float)(Math.Sin(time) + 1.0f) * 0.5f; // 0.0 ~ 1.0

                    if (vertexCount >= 4)
                    {
                        Vector3 bottomLeft = new Vector3(-0.5f, 0.5f, 0.0f);
                        Vector3 bottomRight = new Vector3(0.5f, 0.5f, 0.0f);
                        Vector3 topLeft = new Vector3(-0.5f, -0.5f, 0.0f);
                        Vector3 topRight = new Vector3(0.5f, -0.5f, 0.0f);

                        Vector3 animatedTopLeft = topLeft + (topRight - topLeft) * blend;
                        vertices[2].Position = animatedTopLeft;
                    }

                    return size;
                }
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        private void Initialize()
        {
            window = NUIApplication.GetDefaultWindow();
            window.BackgroundColor = Color.White;
            window.TouchEvent += OnTouchEvent;

            // Create root view
            rootView = new View()
            {
                Size = new Size(300.0f, 300.0f),
                ParentOrigin = ParentOrigin.Center,
                PivotPoint = PivotPoint.Center,
                PositionUsesPivotPoint = true
            };
            window.Add(rootView);

            // Create geometry updater
            geometryUpdater = new GeometryUpdater();

            // Create vertex buffer format
            PropertyMap vertexFormat = new PropertyMap();
            vertexFormat.Add("aPosition", PropertyType.Vector3);
            vertexFormat.Add("aTexCoord", PropertyType.Vector2);

            // Create vertex buffer
            vertexBuffer = new VertexBuffer(vertexFormat);

            // Set initial vertex data
            Vertex[] initialVertices = new Vertex[]
            {
                new Vertex { Position = new Vector3(-0.5f, 0.5f, 0.0f), TexCoord = new Vector2(0.0f, 1.0f) }, // bottom left
                new Vertex { Position = new Vector3(0.5f, 0.5f, 0.0f), TexCoord = new Vector2(1.0f, 1.0f) },  // bottom right
                new Vertex { Position = new Vector3(-0.5f, -0.5f, 0.0f), TexCoord = new Vector2(0.0f, 0.0f) }, // top left
                new Vertex { Position = new Vector3(0.5f, -0.5f, 0.0f), TexCoord = new Vector2(1.0f, 0.0f) }   // top right
            };
            vertexBuffer.SetData(initialVertices);

            // Create vertex buffer update callback
            updateCallback = new VertexBufferUpdateCallback();
            updateCallback.Connect(geometryUpdater.UpdateVertices);
            vertexBuffer.SetVertexBufferUpdateCallback(updateCallback);

            // Create geometry
            geometry = new Geometry();
            geometry.AddVertexBuffer(vertexBuffer);
            geometry.PrimitiveType = PrimitiveType.TriangleStrip;

            // Create shader
            PropertyMap shaderMap = new PropertyMap();
            string vertexShader = @"
                INPUT mediump vec2 aPosition;
                INPUT mediump vec2 aTexCoord;
                OUTPUT mediump vec2 vTexCoord;
                UNIFORM highp mat4 uMvpMatrix;
                UNIFORM highp vec3 uSize;
                void main()
                {
                    vTexCoord = aTexCoord;
                    gl_Position = uMvpMatrix * vec4(uSize.xy * aPosition, 0.0, 1.0);
                }";
            string fragmentShader =@"
                INPUT mediump vec2 vTexCoord;
                UNIFORM sampler2D sTexture;
                void main()
                {
                    mediump vec2 texCoord = vTexCoord;
                    lowp vec4 textureColor = TEXTURE(sTexture, texCoord);
                    gl_FragColor = textureColor;
                }";

            Shader shader = new Shader(vertexShader, fragmentShader);

            // Create renderer
            renderer = new Renderer(geometry, shader);
            rootView.AddRenderer(renderer);

            // Create and set texture
            TextureSet textureSet = new TextureSet();

            var DEMO_IMAGE_DIR = Tizen.Applications.Application.Current.DirectoryInfo.Resource + "images/Dali/ContactCard/gallery-small-2.jpg";
            PixelBuffer pixelBuffer = ImageLoader.LoadImageFromFile(DEMO_IMAGE_DIR);
            PixelData pixelData = PixelBuffer.Convert(pixelBuffer);
            Texture texture = new Texture(TextureType.TEXTURE_2D, pixelData.GetPixelFormat(), pixelData.GetWidth(), pixelData.GetHeight());
            texture.Upload(pixelData);
            
            textureSet.SetTexture(0, texture);
            renderer.SetTextures(textureSet);
        }

        private void OnTouchEvent(object source, Window.TouchEventArgs e)
        {
            if (e.Touch.GetState(0) == PointStateType.Down)
            {
                // Quit application on touch
                Exit();
            }
        }

        static void Main(string[] args)
        {
            NUIApplication.IsUsingXaml = false;
            var app = new Program();
            app.Run(args);
        }
    }
}
