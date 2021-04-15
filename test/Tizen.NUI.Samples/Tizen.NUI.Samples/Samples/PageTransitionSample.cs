using System;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace Tizen.NUI.Samples
{
    public class PageTransitionSample : IExample
    {
        private readonly string[,] Keywords = new string[3, 2]
        {
            {"red", "redGrey"},
            {"green", "greenGrey"},
            {"blue", "blueGrey"}
        };
        private readonly string totalGreyTag = "totalGrey";

        private Navigator navigator;
        private ContentPage mainPage;
        private ContentPage redPage, greenPage, bluePage, totalPage;

        private readonly Vector4 ColorGrey = new Vector4(0.82f, 0.80f, 0.78f, 1.0f);
        private readonly Vector4 ColorBackground = new Vector4(0.99f, 0.94f, 0.83f, 1.0f);

        private readonly Vector4[] TileColor = { new Color("#F5625D"), new Color("#7DFF83"), new Color("#7E72DF") };
        private readonly Vector4[] PageColor = { new Color("#F5625D"), new Color("#7DFF83"), Color.Cyan };

        private readonly Vector2 baseSize = new Vector2(720.0f, 1280.0f);
        private Vector2 contentSize;
        private Vector2 windowSize;

        private float magnification;

        private float convertSize(float size)
        {
            return size * magnification;
        }


        public void Activate()
        {
            Window window = NUIApplication.GetDefaultWindow();
            windowSize = new Vector2((float)(window.Size.Width), (float)(window.Size.Height));
            magnification = Math.Min(windowSize.X / baseSize.X, windowSize.Y / baseSize.Y);
            contentSize = baseSize * magnification;

            navigator = new Navigator()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(contentSize.Width, contentSize.Height),
                Transition = new Transition()
                {
                    TimePeriod = new TimePeriod(0.4f),
                    AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                },
            };
            navigator.TransitionFinished += (object sender, EventArgs e) =>
            {
                navigator.Transition = new Transition()
                {
                    TimePeriod = new TimePeriod(0.4f),
                    AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                };
            };
            window.Add(navigator);

            View mainRoot = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent
            };

            View layoutView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopCenter,
                ParentOrigin = ParentOrigin.TopCenter,
                Layout = new LinearLayout()
                {
                    LinearAlignment = LinearLayout.Alignment.Center,
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    CellPadding = new Size(convertSize(90), convertSize(90)),
                },
                Position = new Position(0, convertSize(90))
            };
            mainRoot.Add(layoutView);

            View redButton = CreateButton(0, redPage, new Radian(-(float)Math.PI / 2.0f), 0.0f);
            View greenButton = CreateButton(1, greenPage, new Radian(0.0f), 10.0f);
            View blueButton = CreateButton(2, bluePage, new Radian(0.0f), 0.0f);

            layoutView.Add(redButton);
            layoutView.Add(greenButton);
            layoutView.Add(blueButton);

            mainPage = new ContentPage()
            {
                Content = mainRoot,
            };
            navigator.Push(mainPage);

            View totalGreyView = new View()
            {
                Size = new Size(convertSize(75), convertSize(75)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionWithChild = true,
                    TransitionTag = totalGreyTag,
                }
            };

            View innerView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(60), convertSize(60)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.Wheat,
            };
            totalGreyView.Add(innerView);

            totalGreyView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.Transition = new Transition()
                    {
                        TimePeriod = new TimePeriod(0.8f),
                        AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                    };
                    navigator.PushWithTransition(totalPage);
                }
                return true;
            };
            layoutView.Add(totalGreyView);


            // ------------------------------------------------------


            View totalPageRoot = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                SizeHeight = contentSize.Height,
            };

            View totalLayoutView = new View()
            {
                Layout = new GridLayout()
                {
                    Rows = 2,
                    GridOrientation = GridLayout.Orientation.Vertical,
                },
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
            };
            totalPageRoot.Add(totalLayoutView);

            for (int i = 0; i < 3; ++i)
            {
                View sizeView = new View()
                {
                    Size = new Size(contentSize.Width / 2.0f, contentSize.Height / 2.0f),
                };
                View smallView = CreatePageScene(i, (i==1)?10.0f:0.0f);
                smallView.Scale = new Vector3(0.45f, 0.45f, 1.0f);
                smallView.PositionUsesPivotPoint = true;
                smallView.PivotPoint = PivotPoint.Center;
                smallView.ParentOrigin = ParentOrigin.Center;
                sizeView.Add(smallView);
                totalLayoutView.Add(sizeView);
            }

            View sizeGreyView = new View()
            {
                Size = new Size(contentSize.Width / 2.0f, contentSize.Height / 2.0f),
            };

            View totalGreyReturnView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(105), convertSize(105)),
                CornerRadius = 0.28f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionWithChild = true,
                    TransitionTag = totalGreyTag,
                }
            };

            View innerReturnView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(60), convertSize(60)),
                CornerRadius = 0.5f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.Wheat,
            };
            totalGreyReturnView.Add(innerReturnView);
            sizeGreyView.Add(totalGreyReturnView);
            totalLayoutView.Add(sizeGreyView);

            totalGreyReturnView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.Transition = new Transition()
                    {
                        TimePeriod = new TimePeriod(0.8f),
                        AlphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseInOutSine),
                    };
                    navigator.PopWithTransition();
                }
                return true;
            };

            totalPage = new ContentPage()
            {
                Content = totalPageRoot,
            };
        }

        private View CreateButton(int index, Page secondPage, Radian angle, float shadowRadius)
        {
            View colorView = new View()
            {
                Size = new Size(convertSize(75), convertSize(75)),
                CornerRadius = 0.45f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = TileColor[index],
                Orientation = new Rotation(angle, Vector3.ZAxis),
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 0],
                },
            };

            View greyView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(convertSize(40), convertSize(40)),
                CornerRadius = 0.45f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                InheritOrientation = false,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 1],
                }
            };

            if(shadowRadius > 0.0f)
            {
                greyView.BoxShadow = new Shadow(shadowRadius, null);
                greyView.InheritScale = false;
            }

            secondPage = CreatePage(index, shadowRadius);

            colorView.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.PushWithTransition(secondPage);
                }
                return true;
            };
            colorView.Add(greyView);
            return colorView;
        }

        private View CreatePageScene(int index, float shadowRadius)
        {
            View pageBackground = new View()
            {
                SizeWidth = contentSize.Width,
                SizeHeight = contentSize.Height,
            };

            View colorView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.Center,
                ParentOrigin = ParentOrigin.Center,
                Size = new Size(windowSize.Width, windowSize.Height),
                BackgroundColor = PageColor[index],
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 0]
                }
            };

            View greyView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopCenter,
                ParentOrigin = ParentOrigin.TopCenter,
                Position = new Position(0, convertSize(120)),
                SizeWidth = contentSize.Width * 0.35f,
                SizeHeight = contentSize.Height * 0.03f,
                Scale = new Vector3(2.0f, 2.0f, 1.0f),
                CornerRadius = 0.1f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = ColorGrey,
                TransitionOptions = new TransitionOptions()
                {
                    TransitionTag = Keywords[index, 1]
                }
            };

            if(shadowRadius > 0.0f)
            {
                greyView.BoxShadow = new Shadow(shadowRadius, null);
            }

            View whiteView = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.BottomCenter,
                ParentOrigin = ParentOrigin.BottomCenter,
                Position = new Position(0, -convertSize(90)),
                SizeWidth = contentSize.Width * 0.65f,
                SizeHeight = contentSize.Height * 0.7f,
                CornerRadius = 0.1f,
                CornerRadiusPolicy = VisualTransformPolicyType.Relative,
                BackgroundColor = Color.AntiqueWhite,
            };
            pageBackground.Add(colorView);
            pageBackground.Add(whiteView);
            pageBackground.Add(greyView);

            return pageBackground;
        }

        private Page CreatePage(int index, float shadowRadius)
        {
            View pageRoot = new View()
            {
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent
            };

            View pageBackground = CreatePageScene(index, shadowRadius);
            pageBackground.TouchEvent += (object sender, View.TouchEventArgs e) =>
            {
                if (e.Touch.GetState(0) == PointStateType.Down)
                {
                    navigator.PopWithTransition();
                }
                return true;
            };
            pageRoot.Add(pageBackground);

            Page page = new ContentPage()
            {
                Content = pageRoot,
            };
            return page;
        }

        public void Deactivate()
        {
        }
    }
}
