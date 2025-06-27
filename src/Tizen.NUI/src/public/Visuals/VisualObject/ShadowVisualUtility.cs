// Copyright (c) 2024 Samsung Electronics Co., Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Tizen.NUI.Visuals
{
    class ShadowVisualUtility
    {
        static public VisualBase CreateBoxShadow(float blurRadius, Color color, Vector2 offset = null, Vector2 extents = null)
        {
            Visuals.ColorVisual shadowVisual = new Visuals.ColorVisual()
            {
                Color = color,
                BlurRadius = blurRadius,
                OffsetX = offset == null ? 0.0f : offset.X,
                OffsetY = offset == null ? 0.0f : offset.Y,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
                ExtraWidth = extents == null ? 0.0f : extents.Width,
                ExtraHeight = extents == null ? 0.0f : extents.Height,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutViewWithCornerRadius,
                OverrideCorner = true,
            };

            return shadowVisual;
        }

        static public VisualBase CreateInnerShadow(UIExtents insetExtents, float blurRadius, Color color, ColorVisualCutoutPolicyType cutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius)
        {
            Visuals.ColorVisual shadowVisual = new Visuals.ColorVisual()
            {
                Color = Color.Transparent,
                OffsetXPolicy = VisualTransformPolicyType.Absolute,
                OffsetYPolicy = VisualTransformPolicyType.Absolute,
//                CornerRadius = cornerRadius + blurRadius + 1,// + (blurRadius / 2.0f) + 1.0f,
                BorderlineWidth = blurRadius,
                BorderlineColor = color,
                BorderlineOffset = 1.0f,
                BlurRadius = blurRadius,
                CutoutPolicy = ColorVisualCutoutPolicyType.CutoutOutsideWithCornerRadius,
                OverrideCorner = true,
            };

            return shadowVisual;
        }
    }
}