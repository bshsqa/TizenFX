/*
 * Copyright (c) 2026 Samsung Electronics Co., Ltd.
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

using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace Tizen.NUI.Samples
{
    public class GradientLoadingEffectSample : IExample
    {
        private enum MaskGradientType
        {
            Radial,
            Linear
        }

        private const int ImageCount = 5;
        private const int ContentCount = ImageCount + 1;
        private const int TextCardIndex = 3;
        private const int ImageSize = 500;
        private const int TransitionDuration = 1100;
        private const float MaskStartOffset = 0.55f;
        private const float MaskEndOffset = -0.72f;

        private View root;
        private ImageView imageView;
        private View textCardView;
        private View colorGradientView;
        private View maskView;
        private TextLabel helpLabel;
        private TextLabel statusLabel;
        private RenderEffect maskEffect;
        private Animation gradientAnimation;
        private int contentIndex;
        private bool transitionPlaying;
        private MaskGradientType maskGradientType = MaskGradientType.Radial;

        public void Activate()
        {
            Window window = NUIApplication.GetDefaultWindow();
            root = new View()
            {
                Size = new Size(1920, 1080),
                BackgroundColor = Color.White
            };
            window.Add(root);
            window.KeyEvent += OnWindowKeyEvent;

            CreateMaskedGradientEffect();
        }

        public void Deactivate()
        {
            if (gradientAnimation != null)
            {
                gradientAnimation.Dispose();
                gradientAnimation = null;
            }

            if (maskEffect != null)
            {
                maskEffect?.Dispose();
                maskEffect = null;
            }

            if (root != null)
            {
                root.Unparent();
                root.Dispose();
                root = null;
            }
        }

        private string GetImagePath(int index)
        {
            return CommonResource.GetDaliResourcePath() + $"images/CubeTransitionEffect/gallery-medium-{index}.jpg";
        }

        private static int GetImageIndexForContent(int index)
        {
            return index < TextCardIndex ? index + 1 : index;
        }

        private static string GetMaskGradientTypeText(MaskGradientType gradientType)
        {
            return gradientType == MaskGradientType.Radial ? "Mode: Radial" : "Mode: Linear";
        }

        private static PropertyMap CreateColorGradientMap()
        {
            var stopOffsets = new PropertyArray();
            stopOffsets.Add(new PropertyValue(0.00f));
            stopOffsets.Add(new PropertyValue(0.42f));
            stopOffsets.Add(new PropertyValue(0.72f));
            stopOffsets.Add(new PropertyValue(1.00f));

            var stopColors = new PropertyArray();
            stopColors.Add(new PropertyValue(new Vector4(0.18f, 0.95f, 0.62f, 1.00f)));
            stopColors.Add(new PropertyValue(new Vector4(0.05f, 0.58f, 1.00f, 1.00f)));
            stopColors.Add(new PropertyValue(new Vector4(0.08f, 0.48f, 0.92f, 1.00f)));
            stopColors.Add(new PropertyValue(new Vector4(1.00f, 0.88f, 0.24f, 1.00f)));

            var map = new PropertyMap();
            map.Insert((int)Visual.Property.Type, new PropertyValue((int)Visual.Type.Gradient));
            map.Insert((int)GradientVisualProperty.StartPosition, new PropertyValue(new Vector2(0.0f, -0.5f)));
            map.Insert((int)GradientVisualProperty.EndPosition, new PropertyValue(new Vector2(0.0f, 0.5f)));
            map.Insert((int)GradientVisualProperty.StopOffset, new PropertyValue(stopOffsets));
            map.Insert((int)GradientVisualProperty.StopColor, new PropertyValue(stopColors));
            map.Insert((int)GradientVisualProperty.SpreadMethod, new PropertyValue((int)GradientVisualSpreadMethodType.Pad));
            return map;
        }

        private static PropertyMap CreateAlphaMaskGradientMap(MaskGradientType gradientType)
        {
            return gradientType == MaskGradientType.Radial ? CreateRadialAlphaMaskGradientMap() : CreateLinearAlphaMaskGradientMap();
        }

        private static PropertyMap CreateRadialAlphaMaskGradientMap()
        {
            var stopOffsets = new PropertyArray();
            stopOffsets.Add(new PropertyValue(0.00f));
            stopOffsets.Add(new PropertyValue(0.24f));
            stopOffsets.Add(new PropertyValue(0.38f));
            stopOffsets.Add(new PropertyValue(0.54f));
            stopOffsets.Add(new PropertyValue(0.76f));
            stopOffsets.Add(new PropertyValue(1.00f));

            var stopColors = new PropertyArray();
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.48f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.70f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.16f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));

            var map = new PropertyMap();
            map.Insert((int)Visual.Property.Type, new PropertyValue((int)Visual.Type.Gradient));
            map.Insert((int)GradientVisualProperty.Center, new PropertyValue(new Vector2(-0.10f, -1.05f)));
            map.Insert((int)GradientVisualProperty.Radius, new PropertyValue(1.30f));
            map.Insert((int)GradientVisualProperty.StopOffset, new PropertyValue(stopOffsets));
            map.Insert((int)GradientVisualProperty.StopColor, new PropertyValue(stopColors));
            map.Insert((int)GradientVisualProperty.SpreadMethod, new PropertyValue((int)GradientVisualSpreadMethodType.Pad));
            map.Insert((int)GradientVisualProperty.StartOffset, new PropertyValue(MaskStartOffset));
            return map;
        }

        private static PropertyMap CreateLinearAlphaMaskGradientMap()
        {
            var stopOffsets = new PropertyArray();
            stopOffsets.Add(new PropertyValue(0.00f));
            stopOffsets.Add(new PropertyValue(0.20f));
            stopOffsets.Add(new PropertyValue(0.36f));
            stopOffsets.Add(new PropertyValue(0.54f));
            stopOffsets.Add(new PropertyValue(0.78f));
            stopOffsets.Add(new PropertyValue(1.00f));

            var stopColors = new PropertyArray();
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.42f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.70f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.14f)));
            stopColors.Add(new PropertyValue(new Vector4(1.0f, 1.0f, 1.0f, 0.00f)));

            var map = new PropertyMap();
            map.Insert((int)Visual.Property.Type, new PropertyValue((int)Visual.Type.Gradient));
            map.Insert((int)GradientVisualProperty.StartPosition, new PropertyValue(new Vector2(0.0f, -0.5f)));
            map.Insert((int)GradientVisualProperty.EndPosition, new PropertyValue(new Vector2(0.0f, 0.5f)));
            map.Insert((int)GradientVisualProperty.StopOffset, new PropertyValue(stopOffsets));
            map.Insert((int)GradientVisualProperty.StopColor, new PropertyValue(stopColors));
            map.Insert((int)GradientVisualProperty.SpreadMethod, new PropertyValue((int)GradientVisualSpreadMethodType.Pad));
            map.Insert((int)GradientVisualProperty.StartOffset, new PropertyValue(MaskStartOffset));
            return map;
        }

        private static void SetEffectViewGeometry(View view)
        {
            view.Size2D = new Size2D(ImageSize, ImageSize);
            view.ParentOrigin = ParentOrigin.Center;
            view.PivotPoint = PivotPoint.Center;
            view.PositionUsesPivotPoint = true;
        }

        private static TextLabel CreateTextCardLabel(string text, float x, float y, float width, float height, float pointSize, Color color)
        {
            return new TextLabel(text)
            {
                Size2D = new Size2D((int)width, (int)height),
                Position = new Position(x, y, 0.0f),
                ParentOrigin = ParentOrigin.TopLeft,
                PivotPoint = PivotPoint.TopLeft,
                PositionUsesPivotPoint = true,
                TextColor = color,
                PointSize = pointSize,
                MultiLine = true,
                HorizontalAlignment = HorizontalAlignment.Begin,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static View CreateTextCardView()
        {
            var card = new View
            {
                BackgroundColor = new Color(0.86f, 0.80f, 0.68f, 1.0f)
            };
            SetEffectViewGeometry(card);

            card.Add(CreateTextCardLabel("Gradient Loading", 54.0f, 72.0f, 392.0f, 64.0f, 21.0f, new Color(0.14f, 0.12f, 0.10f, 1.0f)));
            card.Add(CreateTextCardLabel("Same transition, different content.", 54.0f, 154.0f, 392.0f, 52.0f, 12.0f, new Color(0.25f, 0.22f, 0.18f, 1.0f)));
            card.Add(CreateTextCardLabel("Radial and linear alpha masks reveal the color gradient before the content switches.", 54.0f, 226.0f, 392.0f, 118.0f, 11.0f, new Color(0.33f, 0.29f, 0.24f, 1.0f)));
            card.Add(CreateTextCardLabel("Press 1 or 2", 54.0f, 382.0f, 392.0f, 42.0f, 10.0f, new Color(0.46f, 0.38f, 0.29f, 1.0f)));

            return card;
        }

        private static TextLabel CreateHelpLabel()
        {
            return new TextLabel("1 Radial  2 Linear\nTouch: next")
            {
                Size2D = new Size2D(260, 56),
                Position = new Position(24.0f, -24.0f, 0.0f),
                ParentOrigin = ParentOrigin.BottomLeft,
                PivotPoint = PivotPoint.BottomLeft,
                PositionUsesPivotPoint = true,
                TextColor = new Color(0.16f, 0.16f, 0.16f, 0.78f),
                PointSize = 9.0f,
                MultiLine = true,
                HorizontalAlignment = HorizontalAlignment.Begin,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static TextLabel CreateStatusLabel(MaskGradientType gradientType)
        {
            return new TextLabel(GetMaskGradientTypeText(gradientType))
            {
                Size2D = new Size2D(260, 42),
                Position = new Position(0.0f, 24.0f, 0.0f),
                ParentOrigin = ParentOrigin.TopCenter,
                PivotPoint = PivotPoint.TopCenter,
                PositionUsesPivotPoint = true,
                TextColor = new Color(0.10f, 0.10f, 0.10f, 0.84f),
                PointSize = 10.0f,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void CreateMaskedGradientEffect()
        {
            maskView = new View
            {
                Background = CreateAlphaMaskGradientMap(maskGradientType)
            };
            SetEffectViewGeometry(maskView);
            root.Add(maskView);
            maskView.Hide();

            imageView = new ImageView(GetImagePath(GetImageIndexForContent(contentIndex)));
            SetEffectViewGeometry(imageView);
            root.Add(imageView);

            textCardView = CreateTextCardView();
            root.Add(textCardView);
            textCardView.Hide();

            colorGradientView = new View
            {
                Background = CreateColorGradientMap()
            };
            SetEffectViewGeometry(colorGradientView);
            colorGradientView.TouchEvent += OnEffectTouched;
            root.Add(colorGradientView);

            maskEffect = RenderEffect.CreateMaskEffect(maskView, MaskEffectMode.Alpha, 0.0f, 0.0f, 1.0f, 1.0f);
            maskEffect.TargetMaskOnce = true;
            maskEffect.SourceMaskOnce = false;
            colorGradientView.SetRenderEffect(maskEffect);

            helpLabel = CreateHelpLabel();
            root.Add(helpLabel);

            statusLabel = CreateStatusLabel(maskGradientType);
            root.Add(statusLabel);
        }

        private void ApplyContent()
        {
            if(contentIndex == TextCardIndex)
            {
                imageView.Hide();
                textCardView.Show();
            }
            else
            {
                imageView.SetImage(GetImagePath(GetImageIndexForContent(contentIndex)));
                textCardView.Hide();
                imageView.Show();
            }
        }

        private bool OnEffectTouched(object sender, View.TouchEventArgs e)
        {
            if(e.Touch.GetState(0) == PointStateType.Down && !transitionPlaying)
            {
                StartTransition();
            }

            return true;
        }

        private void StartTransition()
        {
            transitionPlaying = true;
            maskView.Size2D = new Size2D(ImageSize, ImageSize);
            maskView.PositionY = 0.0f;
            colorGradientView.Size2D = new Size2D(ImageSize, ImageSize);
            maskView.Background = CreateAlphaMaskGradientMap(maskGradientType);
            maskView.Show();

            gradientAnimation?.Dispose();
            gradientAnimation = new Animation(TransitionDuration);
            gradientAnimation.AnimateTo(maskView, "Gradient.StartOffset", MaskEndOffset, new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOut));
            gradientAnimation.Finished += OnTransitionFinished;
            gradientAnimation.Play();
        }

        private void OnTransitionFinished(object sender, System.EventArgs e)
        {
            gradientAnimation.Finished -= OnTransitionFinished;

            contentIndex = (contentIndex + 1) % ContentCount;
            ApplyContent();

            maskView.Background = CreateAlphaMaskGradientMap(maskGradientType);
            maskView.Hide();
            transitionPlaying = false;
        }

        private void StartTransitionWithMaskType(MaskGradientType gradientType)
        {
            maskGradientType = gradientType;
            statusLabel.Text = GetMaskGradientTypeText(maskGradientType);

            if(!transitionPlaying)
            {
                StartTransition();
            }
        }

        private static bool IsKey(Key key, string keyName)
        {
            return key.KeyPressedName == keyName || key.KeyString == keyName || key.KeyPressed == keyName;
        }

        private void OnWindowKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if(e.Key.State != Key.StateType.Down)
            {
                return;
            }

            if(IsKey(e.Key, "1"))
            {
                StartTransitionWithMaskType(MaskGradientType.Radial);
            }
            else if(IsKey(e.Key, "2"))
            {
                StartTransitionWithMaskType(MaskGradientType.Linear);
            }
            else if(e.Key.KeyPressedName == "Escape" || e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "BackSpace")
            {
                Deactivate();
            }
        }
    }
}
