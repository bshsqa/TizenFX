/*
 * Copyright(c) 2025 Samsung Electronics Co., Ltd.
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
using System.Runtime.InteropServices;

namespace Tizen.NUI
{
    /// <summary>
    /// Delegate for vertex buffer update callback.
    /// </summary>
    /// <param name="ptr">Pointer to vertex data buffer.</param>
    /// <param name="size">Size of the buffer in bytes.</param>
    /// <param name="elapsedSeconds">Time elapsed since last update in seconds.</param>
    /// <returns>Size of the buffer that was updated.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate uint VertexUpdateCallback(IntPtr ptr, ulong size, float elapsedSeconds);

    /// <summary>
    /// VertexBufferUpdateCallback is a handle to an object that provides callback functionality for dynamic vertex buffer updates.<br />
    /// This allows real-time modification of vertex data during rendering.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class VertexBufferUpdateCallback : BaseHandle
    {
        private static int aliveCount;
        private VertexUpdateCallback callback;
        private GCHandle? callbackHandle;

        /// <summary>
        /// The constructor to create a VertexBufferUpdateCallback.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public VertexBufferUpdateCallback() : this(Interop.VertexBufferUpdateCallback.New(), true)
        {
            if (NDalicPINVOKE.SWIGPendingException.Pending)
                throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        internal VertexBufferUpdateCallback(global::System.IntPtr cPtr, bool cMemoryOwn) : base(cPtr, cMemoryOwn)
        {
            ++aliveCount;
        }

        /// <summary>
        /// Gets the number of currently alived VertexBufferUpdateCallback object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int AliveCount => aliveCount;

        /// <summary>
        /// Connects a vertex update callback to this VertexBufferUpdateCallback.
        /// </summary>
        /// <param name="callback">The callback function to be invoked during vertex buffer updates.</param>
        /// <exception cref="ArgumentNullException"> Thrown when callback is null. </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Connect(VertexUpdateCallback callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            // Store the callback to prevent garbage collection
            this.callback = callback;

            // Pin the callback delegate to prevent it from being garbage collected
            callbackHandle = GCHandle.Alloc(callback);

            // Get function pointer and connect to native callback
            IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            Interop.VertexBufferUpdateCallback.Connect(SwigCPtr, callbackPtr);

            if (NDalicPINVOKE.SWIGPendingException.Pending)
                throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// <summary>
        /// Disconnects the currently connected vertex update callback.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Disconnect()
        {
            if (callbackHandle.HasValue)
            {
                callbackHandle.Value.Free();
                callbackHandle = null;
            }

            callback = null;
            Interop.VertexBufferUpdateCallback.Disconnect(SwigCPtr);

            if (NDalicPINVOKE.SWIGPendingException.Pending)
                throw NDalicPINVOKE.SWIGPendingException.Retrieve();
        }

        /// This will not be public opened.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void Dispose(DisposeTypes type)
        {
            if (Disposed)
            {
                return;
            }

            // Clean up callback handle
            if (callbackHandle.HasValue)
            {
                callbackHandle.Value.Free();
                callbackHandle = null;
            }

            --aliveCount;

            base.Dispose(type);
        }

        /// This will not be public opened.
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ReleaseSwigCPtr(System.Runtime.InteropServices.HandleRef swigCPtr)
        {
            Interop.VertexBufferUpdateCallback.DeleteVertexBufferUpdateCallback(swigCPtr);
        }
    }
}
