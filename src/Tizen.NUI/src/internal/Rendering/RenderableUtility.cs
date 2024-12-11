/*
 * Copyright(c) 2024 Samsung Electronics Co., Ltd.
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
using System.ComponentModel;

namespace Tizen.NUI
{
    /// <summary>
    /// FadeTransitionItem is an object to set Fade transition of a View that will appear or disappear.
    /// FadeTransitionItem object is required to be added to the TransitionSet to play.
    /// </summary>
    internal class RenderableUtility
    {
        public enum BlendingOperationType
        {
            /// <summary>
            /// The source and destination colors are added to each other.
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Add = 0x8006,

            /// <summary>
            /// Use minimum value of the source and the destination.
            /// </summary>
            /// <remark>
            /// It will be supported only if OpenGL es 3.0  or higher version using.
            /// </remark>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Min = 0x8007,

            /// <summary>
            /// Use maximum value of the source and the destination.
            /// </summary>
            /// <remark>
            /// It will be supported only if OpenGL es 3.0  or higher version using.
            /// </remark>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Max = 0x8008,

            /// <summary>
            /// Subtracts the destination from the source.
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Subtract = 0x800A,

            /// <summary>
            /// Subtracts the source from the destination.
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            ReverseSubtract = 0x800B,

            //Advanced Blend Equation
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Multiply = 0x9294,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Screen = 0x9295,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Overlay = 0x9296,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Darken = 0x9297,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Lighten = 0x9298,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            ColorDodge = 0x9299,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            ColorBurn = 0x929A,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            HardLight = 0x929B,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            SoftLight = 0x929C,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Difference = 0x929E,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Exclusion = 0x92A0,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Hue = 0x92AD,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Saturation = 0x92AE,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Color = 0x92AF,
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Luminosity = 0x92B0,
        }

        public static RenderableUtility.BlendingOperationType ConvertBlendingOperationTypeToUtilityProperty(Tizen.NUI.BlendingOperationType blendingOperationType)
        {
            switch (blendingOperationType)
            {
                case Tizen.NUI.BlendingOperationType.Add:
                    {
                        return RenderableUtility.BlendingOperationType.Add;
                    }
                case Tizen.NUI.BlendingOperationType.Min:
                    {
                        return RenderableUtility.BlendingOperationType.Min;
                    }
                case Tizen.NUI.BlendingOperationType.Max:
                    {
                        return RenderableUtility.BlendingOperationType.Max;
                    }
                case Tizen.NUI.BlendingOperationType.Subtract:
                    {
                        return RenderableUtility.BlendingOperationType.Subtract;
                    }
                case Tizen.NUI.BlendingOperationType.ReverseSubtract:
                    {
                        return RenderableUtility.BlendingOperationType.ReverseSubtract;
                    }
                case Tizen.NUI.BlendingOperationType.Multiply:
                    {
                        return RenderableUtility.BlendingOperationType.Multiply;
                    }
                case Tizen.NUI.BlendingOperationType.Screen:
                    {
                        return RenderableUtility.BlendingOperationType.Screen;
                    }
                case Tizen.NUI.BlendingOperationType.Overlay:
                    {
                        return RenderableUtility.BlendingOperationType.Overlay;
                    }
                case Tizen.NUI.BlendingOperationType.Darken:
                    {
                        return RenderableUtility.BlendingOperationType.Darken;
                    }
                case Tizen.NUI.BlendingOperationType.Lighten:
                    {
                        return RenderableUtility.BlendingOperationType.Lighten;
                    }
                case Tizen.NUI.BlendingOperationType.ColorDodge:
                    {
                        return RenderableUtility.BlendingOperationType.ColorDodge;
                    }
                case Tizen.NUI.BlendingOperationType.ColorBurn:
                    {
                        return RenderableUtility.BlendingOperationType.ColorBurn;
                    }
                case Tizen.NUI.BlendingOperationType.HardLight:
                    {
                        return RenderableUtility.BlendingOperationType.HardLight;
                    }
                case Tizen.NUI.BlendingOperationType.SoftLight:
                    {
                        return RenderableUtility.BlendingOperationType.SoftLight;
                    }
                case Tizen.NUI.BlendingOperationType.Difference:
                    {
                        return RenderableUtility.BlendingOperationType.Difference;
                    }
                case Tizen.NUI.BlendingOperationType.Exclusion:
                    {
                        return RenderableUtility.BlendingOperationType.Exclusion;
                    }
                case Tizen.NUI.BlendingOperationType.Hue:
                    {
                        return RenderableUtility.BlendingOperationType.Hue;
                    }
                case Tizen.NUI.BlendingOperationType.Saturation:
                    {
                        return RenderableUtility.BlendingOperationType.Saturation;
                    }
                case Tizen.NUI.BlendingOperationType.Color:
                    {
                        return RenderableUtility.BlendingOperationType.Color;
                    }
                case Tizen.NUI.BlendingOperationType.Luminosity:
                    {
                        return RenderableUtility.BlendingOperationType.Luminosity;
                    }
            }
            return RenderableUtility.BlendingOperationType.Add;
        }

        public static Tizen.NUI.BlendingOperationType ConvertBlendingOperationTypeFromUtilityProperty(RenderableUtility.BlendingOperationType blendingOperationType)
        {
            switch (blendingOperationType)
            {
                case RenderableUtility.BlendingOperationType.Add:
                    {
                        return Tizen.NUI.BlendingOperationType.Add;
                    }
                case RenderableUtility.BlendingOperationType.Min:
                    {
                        return Tizen.NUI.BlendingOperationType.Min;
                    }
                case RenderableUtility.BlendingOperationType.Max:
                    {
                        return Tizen.NUI.BlendingOperationType.Max;
                    }
                case RenderableUtility.BlendingOperationType.Subtract:
                    {
                        return Tizen.NUI.BlendingOperationType.Subtract;
                    }
                case RenderableUtility.BlendingOperationType.ReverseSubtract:
                    {
                        return Tizen.NUI.BlendingOperationType.ReverseSubtract;
                    }
                case RenderableUtility.BlendingOperationType.Multiply:
                    {
                        return Tizen.NUI.BlendingOperationType.Multiply;
                    }
                case RenderableUtility.BlendingOperationType.Screen:
                    {
                        return Tizen.NUI.BlendingOperationType.Screen;
                    }
                case RenderableUtility.BlendingOperationType.Overlay:
                    {
                        return Tizen.NUI.BlendingOperationType.Overlay;
                    }
                case RenderableUtility.BlendingOperationType.Darken:
                    {
                        return Tizen.NUI.BlendingOperationType.Darken;
                    }
                case RenderableUtility.BlendingOperationType.Lighten:
                    {
                        return Tizen.NUI.BlendingOperationType.Lighten;
                    }
                case RenderableUtility.BlendingOperationType.ColorDodge:
                    {
                        return Tizen.NUI.BlendingOperationType.ColorDodge;
                    }
                case RenderableUtility.BlendingOperationType.ColorBurn:
                    {
                        return Tizen.NUI.BlendingOperationType.ColorBurn;
                    }
                case RenderableUtility.BlendingOperationType.HardLight:
                    {
                        return Tizen.NUI.BlendingOperationType.HardLight;
                    }
                case RenderableUtility.BlendingOperationType.SoftLight:
                    {
                        return Tizen.NUI.BlendingOperationType.SoftLight;
                    }
                case RenderableUtility.BlendingOperationType.Difference:
                    {
                        return Tizen.NUI.BlendingOperationType.Difference;
                    }
                case RenderableUtility.BlendingOperationType.Exclusion:
                    {
                        return Tizen.NUI.BlendingOperationType.Exclusion;
                    }
                case RenderableUtility.BlendingOperationType.Hue:
                    {
                        return Tizen.NUI.BlendingOperationType.Hue;
                    }
                case RenderableUtility.BlendingOperationType.Saturation:
                    {
                        return Tizen.NUI.BlendingOperationType.Saturation;
                    }
                case RenderableUtility.BlendingOperationType.Color:
                    {
                        return Tizen.NUI.BlendingOperationType.Color;
                    }
                case RenderableUtility.BlendingOperationType.Luminosity:
                    {
                        return Tizen.NUI.BlendingOperationType.Luminosity;
                    }
            }
            return Tizen.NUI.BlendingOperationType.Add;
        }

        public enum BlendingFactorType
        {
            /// <summary>
            /// Match as GL_ZERO
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            Zero = 0,

            /// <summary>
            /// Match as GL_ONE
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            One = 1,

            /// <summary>
            /// Match as GL_SRC_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            SourceColor = 0x0300,

            /// <summary>
            /// Match as GL_ONE_MINUS_SRC_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusSourceColor = 0x0301,

            /// <summary>
            /// Match as GL_SRC_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            SourceAlpha = 0x0302,

            /// <summary>
            /// Match as GL_ONE_MINUS_SRC_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusSourceAlpha = 0x0303,

            /// <summary>
            /// Match as GL_DST_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            DestinationAlpha = 0x0304,

            /// <summary>
            /// Match as GL_ONE_MINUS_DST_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusDestinationAlpha = 0x0305,

            /// <summary>
            /// Match as GL_DST_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            DestinationColor = 0x0306,

            /// <summary>
            /// Match as GL_ONE_MINUS_DST_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusDestinationColor = 0x0307,

            /// <summary>
            /// Match as GL_SRC_ALPHA_SATURATE
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            SourceAlphaSaturate = 0x0308,

            /// <summary>
            /// Match as GL_CONSTANT_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            ConstantColor = 0x8001,

            /// <summary>
            /// Match as GL_ONE_MINUS_CONSTANT_COLOR
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusConstantColor = 0x8002,

            /// <summary>
            /// Match as GL_CONSTANT_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            ConstantAlpha = 0x8003,

            /// <summary>
            /// Match as GL_ONE_MINUS_CONSTANT_ALPHA
            /// </summary>
            /// This will be public opened in next tizen after ACR done. Before ACR, need to be hidden as inhouse API.
            [EditorBrowsable(EditorBrowsableState.Never)]
            OneMinusConstantAlpha = 0x8004,
        }

        public static RenderableUtility.BlendingFactorType ConvertBlendingFactorTypeToUtilityProperty(Tizen.NUI.BlendingFactorType blendingFactorType)
        {
            switch (blendingFactorType)
            {
                case Tizen.NUI.BlendingFactorType.Zero:
                    {
                        return RenderableUtility.BlendingFactorType.Zero;
                    }
                case Tizen.NUI.BlendingFactorType.One:
                    {
                        return RenderableUtility.BlendingFactorType.One;
                    }
                case Tizen.NUI.BlendingFactorType.SourceColor:
                    {
                        return RenderableUtility.BlendingFactorType.SourceColor;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusSourceColor:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusSourceColor;
                    }
                case Tizen.NUI.BlendingFactorType.SourceAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.SourceAlpha;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusSourceAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusSourceAlpha;
                    }
                case Tizen.NUI.BlendingFactorType.DestinationAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.DestinationAlpha;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusDestinationAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusDestinationAlpha;
                    }
                case Tizen.NUI.BlendingFactorType.DestinationColor:
                    {
                        return RenderableUtility.BlendingFactorType.DestinationColor;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusDestinationColor:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusDestinationColor;
                    }
                case Tizen.NUI.BlendingFactorType.SourceAlphaSaturate:
                    {
                        return RenderableUtility.BlendingFactorType.SourceAlphaSaturate;
                    }
                case Tizen.NUI.BlendingFactorType.ConstantColor:
                    {
                        return RenderableUtility.BlendingFactorType.ConstantColor;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusConstantColor:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusConstantColor;
                    }
                case Tizen.NUI.BlendingFactorType.ConstantAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.ConstantAlpha;
                    }
                case Tizen.NUI.BlendingFactorType.OneMinusConstantAlpha:
                    {
                        return RenderableUtility.BlendingFactorType.OneMinusConstantAlpha;
                    }
            }
            return RenderableUtility.BlendingFactorType.Zero;
        }

        public static Tizen.NUI.BlendingFactorType ConvertBlendingFactorTypeFromUtilityProperty(Tizen.NUI.RenderableUtility.BlendingFactorType blendingFactorType)
        {
            switch (blendingFactorType)
            {
                case Tizen.NUI.RenderableUtility.BlendingFactorType.Zero:
                    {
                        return Tizen.NUI.BlendingFactorType.Zero;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.One:
                    {
                        return Tizen.NUI.BlendingFactorType.One;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.SourceColor:
                    {
                        return Tizen.NUI.BlendingFactorType.SourceColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusSourceColor:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusSourceColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.SourceAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.SourceAlpha;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusSourceAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusSourceAlpha;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.DestinationAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.DestinationAlpha;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusDestinationAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusDestinationAlpha;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.DestinationColor:
                    {
                        return Tizen.NUI.BlendingFactorType.DestinationColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusDestinationColor:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusDestinationColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.SourceAlphaSaturate:
                    {
                        return Tizen.NUI.BlendingFactorType.SourceAlphaSaturate;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.ConstantColor:
                    {
                        return Tizen.NUI.BlendingFactorType.ConstantColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusConstantColor:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusConstantColor;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.ConstantAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.ConstantAlpha;
                    }
                case Tizen.NUI.RenderableUtility.BlendingFactorType.OneMinusConstantAlpha:
                    {
                        return Tizen.NUI.BlendingFactorType.OneMinusConstantAlpha;
                    }
            }
            return Tizen.NUI.BlendingFactorType.Zero;
        }
    }
}
