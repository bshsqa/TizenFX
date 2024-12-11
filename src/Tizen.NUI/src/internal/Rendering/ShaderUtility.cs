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
    internal class ShaderUtility
    {
        internal enum PresetShaderHint
        {
            /// <summary>
            /// No hints.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Might generate transparent alpha from opaque inputs
            /// </summary>
            TransparentOutput = 0x01,

            /// <summary>
            /// Might change position of vertices, this option disables any culling optimizations
            /// </summary>
            ModifiesGeometry = 0x02
        }

        public static PresetShaderHint GetPresetFilter(ShaderHint shaderHint)
        {
            switch (shaderHint)
            {
                case ShaderHint.None:
                    {
                        return PresetShaderHint.None;
                    }
                case ShaderHint.TransparentOutput:
                    {
                        return PresetShaderHint.TransparentOutput;
                    }
                case ShaderHint.ModifiesGeometry:
                    {
                        return PresetShaderHint.ModifiesGeometry;
                    }
            }
            return PresetShaderHint.None;
        }
    }
}
