
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
    public class BlurPerformTest : IExample
    {
        private Window window;
        private View root;
        private Animation animation;
        private static string resourcePath = Tizen.Applications.Application.Current.DirectoryInfo.Resource;

        public void Activate()
        {
            window = NUIApplication.GetDefaultWindow();

            ImageView imageView = new ImageView()
            {
                ResourceUrl = resourcePath + "/images/test2.jpg",
                WidthResizePolicy = ResizePolicyType.FillToParent,
                HeightResizePolicy = ResizePolicyType.FillToParent,
            };
            window.Add(imageView);

            root = new View()
            {
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
            };
            window.Add(root);

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    View cardView = CreateCardViewStyle(new Size(400.0f, 300.0f));
                    cardView.Position = new Position(i * 400, j * 300);
                    root.Add(cardView);
                }
            }

            View cardView10 = CreateCardViewStyle(new Size(400.0f, 300.0f));
            cardView10.Position = new Position(1300, 300);
            root.Add(cardView10);

            animation = new Animation(2000);
            animation.Looping = true;
            animation.AnimateTo(root, "positionY", 500.0f);
            animation.Play();
        }

        private View CreateCardViewStyle(Size size)
        {
            View card = new View()
            {
                Name = "test_root",
                Size = size,
                PositionUsesPivotPoint = true,
                PivotPoint = PivotPoint.TopLeft,
                ParentOrigin = ParentOrigin.TopLeft,
            };
            card.SetRenderEffect(RenderEffect.CreateBackgroundBlurEffect(50.0f));

            return card;
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
