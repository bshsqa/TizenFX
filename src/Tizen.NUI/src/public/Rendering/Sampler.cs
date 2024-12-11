/*
 * Copyright(c) 2017 Samsung Electronics Co., Ltd.
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
using System.ComponentModel;

namespace Tizen.NUI
{
    public enum SamplerFilterType
    {
        /// <summary>
        /// Filter nearest.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        Nearest,

        /// <summary>
        /// Filter linear.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        Linear,
    }

    public enum SamplerMipmapFilterType
    {
        None,
        Nearest,
        Linear
    }

    /// <summary>
    /// Sampler is a handle to an object that can be used to provide the sampling parameters to sample textures.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public partial class Sampler : BaseHandle
    {
        private SamplerFilterType minificationFilter = SamplerFilterType.Linear;
        private SamplerFilterType magnificationFilter = SamplerFilterType.Linear;
        private SamplerMipmapFilterType mipmapFilter = SamplerMipmapFilterType.None;

        /// <summary>
        /// Create an instance of Sampler.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public Sampler() : this(Interop.Sampler.New(), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        public SamplerFilterType MinificationFilter
        {
            get => minificationFilter;
            set
            {
                if (minificationFilter != value)
                {
                    minificationFilter = value;
                    SetFilterMode(SamplerUtility.GetPresetFilter(minificationFilter, mipmapFilter), SamplerUtility.GetPresetFilter(magnificationFilter));
                }
            }
        }

        public SamplerFilterType MagnificationFilter
        {
            get => magnificationFilter;
            set
            {
                if (magnificationFilter != value)
                {
                    magnificationFilter = value;
                    SetFilterMode(SamplerUtility.GetPresetFilter(minificationFilter, mipmapFilter), SamplerUtility.GetPresetFilter(magnificationFilter));
                }
            }
        }

        public SamplerMipmapFilterType MipmapFilter
        {
            get => mipmapFilter;
            set
            {
                if (mipmapFilter != value)
                {
                    mipmapFilter = value;
                    SetFilterMode(SamplerUtility.GetPresetFilter(minificationFilter, mipmapFilter), SamplerUtility.GetPresetFilter(magnificationFilter));
                }
            }
        }

        /// <summary>
        /// Sets the filter modes for this sampler.
        /// </summary>
        /// <param name="minFilter">The minification filter that will be used.</param>
        /// <param name="magFilter">The magnification filter that will be used.</param>
        private void SetFilterMode(SamplerUtility.PresetSamplerFilter minFilter, SamplerUtility.PresetSamplerFilter magFilter)
        {
            Interop.Sampler.SetFilterMode(SwigCPtr, (int)minFilter, (int)magFilter);
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Sets the wrap modes for this sampler.
        /// </summary>
        /// <param name="uWrap">Wrap mode for u coordinates.</param>
        /// <param name="vWrap">Wrap mode for v coordinates.</param>
        public void SetWrapMode(WrapModeType uWrap, WrapModeType vWrap)
        {
            Interop.Sampler.SetWrapMode(SwigCPtr, (int)uWrap, (int)vWrap);
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Sets the wrap modes for this sampler.
        /// </summary>
        /// <param name="rWrap">Wrap mode for the x direction.</param>
        /// <param name="sWrap">Wrap mode for the y direction.</param>
        /// <param name="tWrap">Wrap mode for the z direction.</param>
        public void SetWrapMode(WrapModeType rWrap, WrapModeType sWrap, WrapModeType tWrap)
        {
            Interop.Sampler.SetWrapMode(SwigCPtr, (int)rWrap, (int)sWrap, (int)tWrap);
            if (NDalicPINVOKE.SWIGPendingException.Pending) throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        internal Sampler(global::System.IntPtr cPtr, bool cMemoryOwn) : base(cPtr, cMemoryOwn)
        {
        }

        /// This will not be public opened.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ReleaseSwigCPtr(System.Runtime.InteropServices.HandleRef swigCPtr)
        {
            Interop.Sampler.DeleteSampler(swigCPtr);
        }
    }
}
