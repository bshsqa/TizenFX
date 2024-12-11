/*
 * Copyright(c) 2021 Samsung Electronics Co., Ltd.
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Tizen.NUI
{
    /// <summary>
    /// Renderer is a handle to an object used to show content by combining a Geometry, a TextureSet and a shader.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public partial class Renderable : Animatable
    {
        private Geometry geometry = null;
        private Shader shader = null;
        private int firstIndexElement = 0;
        private int indexElementCount = 0;

        /// <summary>
        /// Gets and Sets DepthIndex property.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int DepthIndex
        {
            get
            {
                int temp = 0;
                Tizen.NUI.PropertyValue pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.DepthIndex);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.DepthIndex, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets FaceCullingMode.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public FaceCullingModeType FaceCullingMode
        {
            get => (FaceCullingModeType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.FaceCullingMode);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.FaceCullingMode, (int)value);
        }

        /// <summary>
        /// Gets and Sets BlendMode.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendModeType BlendMode
        {
            get => (BlendModeType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendMode);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendMode, (int)value);
        }

        /// <summary>
        /// Gets and Sets BlendEquationRgb.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingOperationType BlendEquationRgb
        {
            get => RenderableUtility.ConvertBlendingOperationTypeFromUtilityProperty((RenderableUtility.BlendingOperationType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendEquationRgb));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendEquationRgb, (int)RenderableUtility.ConvertBlendingOperationTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendEquationAlpha.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingOperationType BlendEquationAlpha
        {
            get => RenderableUtility.ConvertBlendingOperationTypeFromUtilityProperty((RenderableUtility.BlendingOperationType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendEquationAlpha));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendEquationAlpha, (int)RenderableUtility.ConvertBlendingOperationTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendFactorSrcRgb.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingFactorType BlendFactorSrcRgb
        {
            get => RenderableUtility.ConvertBlendingFactorTypeFromUtilityProperty((RenderableUtility.BlendingFactorType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendFactorSrcRgb));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendFactorSrcRgb, (int)RenderableUtility.ConvertBlendingFactorTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendFactorDestRgb.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingFactorType BlendFactorDestRgb
        {
            get => RenderableUtility.ConvertBlendingFactorTypeFromUtilityProperty((RenderableUtility.BlendingFactorType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendFactorDestRgb));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendFactorDestRgb, (int)RenderableUtility.ConvertBlendingFactorTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendFactorSrcAlpha.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingFactorType BlendFactorSrcAlpha
        {
            get => RenderableUtility.ConvertBlendingFactorTypeFromUtilityProperty((RenderableUtility.BlendingFactorType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendFactorSrcAlpha));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendFactorSrcAlpha, (int)RenderableUtility.ConvertBlendingFactorTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendFactorDestAlpha.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public BlendingFactorType BlendFactorDestAlpha
        {
            get => RenderableUtility.ConvertBlendingFactorTypeFromUtilityProperty((RenderableUtility.BlendingFactorType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.BlendFactorDestAlpha));
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.BlendFactorDestAlpha, (int)RenderableUtility.ConvertBlendingFactorTypeToUtilityProperty(value));
        }

        /// <summary>
        /// Gets and Sets BlendColor.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Vector4 BlendColor
        {
            get
            {
                Vector4 temp = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.BlendColor);
                pValue.Get(temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.BlendColor, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets BlendPreMultipliedAlpha.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public bool BlendPreMultipliedAlpha
        {
            get
            {
                bool temp = false;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.BlendPreMultipliedAlpha);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.BlendPreMultipliedAlpha, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets IndexRangeFirst.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int IndexRangeFirst
        {
            get
            {
                int temp = 0;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.IndexRangeFirst);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.IndexRangeFirst, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets IndexRangeCount.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int IndexRangeCount
        {
            get
            {
                int temp = 0;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.IndexRangeCount);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.IndexRangeCount, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets DepthWriteMode.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public DepthWriteModeType DepthWriteMode
        {
            get => (DepthWriteModeType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.DepthWriteMode);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.DepthWriteMode, (int)value);
        }

        /// <summary>
        /// Gets and Sets DepthFunction.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public DepthFunctionType DepthFunction
        {
            get => (DepthFunctionType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.DepthFunction);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.DepthFunction, (int)value);
        }

        /// <summary>
        /// Gets and Sets DepthTestMode.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public DepthTestModeType DepthTestMode
        {
            get => (DepthTestModeType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.DepthTestMode);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.DepthTestMode, (int)value);
        }

        /// <summary>
        /// Gets and Sets RenderMode.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public RenderModeType RenderMode
        {
            get => (RenderModeType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.RenderMode);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.RenderMode, (int)value);
        }

        /// <summary>
        /// Gets and Sets Stencil   Function.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public StencilFunctionType StencilFunction
        {
            get => (StencilFunctionType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.StencilFunction);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.StencilFunction, (int)value);
        }

        /// <summary>
        /// Gets and Sets StencilFunctionMask.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int StencilFunctionMask
        {
            get
            {
                int temp = 0;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.StencilFunctionMask);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.StencilFunctionMask, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets StencilFunctionReference.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int StencilFunctionReference
        {
            get
            {
                int temp = 0;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.StencilFunctionReference);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.StencilFunctionReference, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets StencilMask.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public int StencilMask
        {
            get
            {
                int temp = 0;
                var pValue = Tizen.NUI.Object.GetProperty(SwigCPtr, RendererProperty.StencilMask);
                pValue.Get(out temp);
                pValue.Dispose();
                return temp;
            }
            set
            {
                var temp = new Tizen.NUI.PropertyValue(value);
                Tizen.NUI.Object.SetProperty(SwigCPtr, RendererProperty.StencilMask, temp);
                temp.Dispose();
            }
        }

        /// <summary>
        /// Gets and Sets StencilOperationOnFail.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public StencilOperationType StencilOperationOnFail
        {
            get => (StencilOperationType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnFail);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnFail, (int)value);
        }

        /// <summary>
        /// Gets and Sets StencilOperationOnZFail.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public StencilOperationType StencilOperationOnZFail
        {
            get => (StencilOperationType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnZFail);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnZFail, (int)value);
        }

        /// <summary>
        /// Gets and Sets StencilOperationOnZPass property.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public StencilOperationType StencilOperationOnZPass
        {
            get => (StencilOperationType)Object.InternalGetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnZPass);
            set => Object.InternalSetPropertyInt(SwigCPtr, RendererProperty.StencilOperationOnZPass, (int)value);
        }

        public int FirstIndexElement
        {
            get => firstIndexElement;
            set => SetIndexRange(value, IndexElementCount);
        }

        public int IndexElementCount
        {
            get => indexElementCount;
            set => SetIndexRange(FirstIndexElement, value);
        }

        public Geometry Geometry
        {
            get => geometry;
            set => SetGeometry(value);
        }

        public Shader Shader
        {
            get => shader;
            set => SetShader(value);
        }

        public TextureSet TextureSet
        {
            get => GetTextures();
            set => SetTextures(value);
        }

        /// <summary>
        /// Sets the geometry to be used by this renderer.
        /// </summary>
        /// <param name="geometry">The geometry to be used by this renderer.</param>
        /// <since_tizen> 3 </since_tizen>
        private void SetGeometry(Geometry geometry)
        {
            this.geometry = geometry;
            Interop.Renderer.SetGeometry(SwigCPtr, Geometry.getCPtr(this.geometry));
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Sets effective range of indices to draw from bound index buffer.
        /// </summary>
        /// <param name="firstElement">The First element to draw.</param>
        /// <param name="elementsCount">The number of elements to draw.</param>
        /// <since_tizen> 3 </since_tizen>
        private void SetIndexRange(int firstElement, int elementsCount)
        {
            if (firstIndexElement != firstElement || indexElementCount != elementsCount)
            {
                firstIndexElement = firstElement;
                indexElementCount = elementsCount;
                Interop.Renderer.SetIndexRange(SwigCPtr, firstElement, elementsCount);
                if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
            }
        }

        /// <summary>
        /// Sets the shader used by this renderer.
        /// </summary>
        /// <param name="shader">The shader to be used by this renderer.</param>
        /// <since_tizen> 3 </since_tizen>
        private void SetShader(Shader shader)
        {
            this.shader = shader;
            Interop.Renderer.SetShader(SwigCPtr, Shader.getCPtr(this.shader));
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Sets the texture set to be used by this renderer.
        /// </summary>
        /// <param name="textureSet">The texture set to be used by this renderer.</param>
        /// <since_tizen> 3 </since_tizen>
        private void SetTextures(TextureSet textureSet)
        {
            Interop.Renderer.SetTextures(SwigCPtr, TextureSet.getCPtr(textureSet));
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Gets the texture set used by this renderer.
        /// </summary>
        /// <returns>The texture set used by the renderer.</returns>
        /// <since_tizen> 3 </since_tizen>
        private TextureSet GetTextures()
        {
            global::System.IntPtr cPtr = Interop.Renderer.GetTextures(SwigCPtr);
            TextureSet ret = Registry.GetManagedBaseHandleFromNativePtr(cPtr) as TextureSet;
            if (ret != null)
            {
                Interop.BaseHandle.DeleteBaseHandle(new global::System.Runtime.InteropServices.HandleRef(this, cPtr));
            }
            else
            {
                ret = new TextureSet(cPtr, true);
            }
            NDalicPINVOKE.ThrowExceptionIfExists();
            return ret;
        }

        internal Renderable(global::System.IntPtr cPtr, bool cMemoryOwn) : base(cPtr, cMemoryOwn)
        {
        }

        /// This will not be public opened.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ReleaseSwigCPtr(System.Runtime.InteropServices.HandleRef swigCPtr)
        {
            Interop.Renderer.DeleteRenderer(swigCPtr);
        }
    }
}
