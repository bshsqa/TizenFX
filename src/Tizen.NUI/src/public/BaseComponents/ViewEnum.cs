/*
 * Copyright(c) 2019-2022 Samsung Electronics Co., Ltd.
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

namespace Tizen.NUI.BaseComponents
{
    /// <summary>
    /// The View layout Direction type.
    /// </summary>
    /// <since_tizen> 4 </since_tizen>
    public enum ViewLayoutDirectionType
    {
        /// <summary>
        /// Left to right.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        LTR,
        /// <summary>
        /// Right to left.
        /// </summary>
        /// <since_tizen> 4 </since_tizen>
        RTL
    }

    /// <summary>
    /// Layout policies to decide the size of View when the View is laid out in its parent View.
    /// LayoutParamPolicies.MatchParent and LayoutParamPolicies.WrapContent can be assigned to <see cref="View.WidthSpecification"/> and <see cref="View.HeightSpecification"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// // matchParentView matches its size to its parent size.
    /// matchParentView.WidthSpecification = LayoutParamPolicies.MatchParent;
    /// matchParentView.HeightSpecification = LayoutParamPolicies.MatchParent;
    ///
    /// // wrapContentView wraps its children with their desired size.
    /// wrapContentView.WidthSpecification = LayoutParamPolicies.WrapContent;
    /// wrapContentView.HeightSpecification = LayoutParamPolicies.WrapContent;
    /// </code>
    /// </example>
    /// <since_tizen> 9 </since_tizen>
    public static class LayoutParamPolicies
    {
        /// <summary>
        /// Constant which indicates child size should match parent size.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public const int MatchParent = -1;
        /// <summary>
        /// Constant which indicates parent should take the smallest size possible to wrap its children with their desired size.
        /// </summary>
        /// <since_tizen> 9 </since_tizen>
        public const int WrapContent = -2;
    }

    internal enum ResourceLoadingStatusType
    {
        Invalid = -1,
        Preparing = 0,
        Ready,
        Failed,
    }

    /// <summary>
    /// View is the base class for all views.
    /// </summary>
    /// <since_tizen> 3 </since_tizen>
    public partial class View
    {
        /// <summary>
        /// Enumeration for describing the states of the view.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names")]
        public enum States
        {
            /// <summary>
            /// The normal state.
            /// </summary>
            [Description("NORMAL")]
            Normal,
            /// <summary>
            /// The focused state.
            /// </summary>
            [Description("FOCUSED")]
            Focused,
            /// <summary>
            /// The disabled state.
            /// </summary>
            [Description("DISABLED")]
            Disabled
        }

        /// <summary>
        /// Describes the direction to move the focus towards.
        /// </summary>
        /// <since_tizen> 3 </since_tizen>
        public enum FocusDirection
        {
            /// <summary>
            /// Move keyboard focus towards the left direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            Left,
            /// <summary>
            /// Move keyboard focus towards the right direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            Right,
            /// <summary>
            /// Move keyboard focus towards the up direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            Up,
            /// <summary>
            /// Move keyboard focus towards the down direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            Down,
            /// <summary>
            /// Move keyboard focus towards the previous page direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            PageUp,
            /// <summary>
            /// Move keyboard focus towards the next page direction.
            /// </summary>
            /// <since_tizen> 3 </since_tizen>
            PageDown,
            /// <summary>
            /// Move keyboard focus towards the forward direction.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            Forward,
            /// <summary>
            /// Move keyboard focus towards the backward direction.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            Backward,
            /// <summary>
            /// Move focus towards the Clockwise direction by rotary wheel.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            Clockwise,
            /// <summary>
            /// Move focus towards the CounterClockwise direction by rotary wheel.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            CounterClockwise,
        }

        /// <summary>
        /// Describes offscreen rendering types.
        /// View with this property enabled renders at offscreen buffer, with all its children.
        /// The property expects to reduce many repetitive render calls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public enum OffScreenRenderingType
        {
            /// <summary>
            /// No offscreen rendering.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            None,
            /// <summary>
            /// Draw offscreen only once.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            RefreshOnce,
            /// <summary>
            /// Draw offscreen every frame.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            RefreshAlways,
        };


        /// <summary>
        /// Actions property value to update visual property.
        /// Note : Only few kind of properies can be update. Update with invalid property action is undefined.
        /// </summary>
        internal static readonly int ActionUpdateProperty = Interop.Visual.GetActionUpdateProperty();

        internal enum PropertyRange
        {
            PROPERTY_START_INDEX = PropertyRanges.PROPERTY_REGISTRATION_START_INDEX,
            CONTROL_PROPERTY_START_INDEX = PROPERTY_START_INDEX,
            CONTROL_PROPERTY_END_INDEX = CONTROL_PROPERTY_START_INDEX + 1000
        }

        internal class Property
        {
            internal static readonly int TOOLTIP = 10000005;
            internal static readonly int STATE = 10000006;
            internal static readonly int SubState = 10000007;
            internal static readonly int LeftFocusableViewId = 10000008;
            internal static readonly int RightFocusableViewId = 10000009;
            internal static readonly int UpFocusableViewId = 10000010;
            internal static readonly int DownFocusableViewId = 10000011;
            internal static readonly int ClockwiseFocusableViewId = 10000021;
            internal static readonly int CounterClockwiseFocusableViewId = 10000022;
            internal static readonly int StyleName = 10000000;
            internal static readonly int KeyInputFocus = 10000001;
            internal static readonly int BACKGROUND = 10000002;
            internal static readonly int SiblingOrder = 66;
            internal static readonly int OPACITY = 55;
            internal static readonly int ScreenPosition = 56;
            internal static readonly int PositionUsesAnchorPoint = 57;
            internal static readonly int ParentOrigin = 0;
            internal static readonly int ParentOriginX = 1;
            internal static readonly int ParentOriginY = 2;
            internal static readonly int ParentOriginZ = 3;
            internal static readonly int AnchorPoint = 4;
            internal static readonly int AnchorPointX = 5;
            internal static readonly int AnchorPointY = 6;
            internal static readonly int AnchorPointZ = 7;
            internal static readonly int SIZE = 8;
            internal static readonly int SizeWidth = 9;
            internal static readonly int SizeHeight = 10;
            internal static readonly int SizeDepth = 11;
            internal static readonly int POSITION = 12;
            internal static readonly int PositionX = 13;
            internal static readonly int PositionY = 14;
            internal static readonly int PositionZ = 15;
            internal static readonly int WorldPosition = 16;
            internal static readonly int WorldPositionX = 17;
            internal static readonly int WorldPositionY = 18;
            internal static readonly int WorldPositionZ = 19;
            internal static readonly int ORIENTATION = 20;
            internal static readonly int WorldOrientation = 21;
            internal static readonly int SCALE = 22;
            internal static readonly int ScaleX = 23;
            internal static readonly int ScaleY = 24;
            internal static readonly int ScaleZ = 25;
            internal static readonly int WorldScale = 26;
            internal static readonly int VISIBLE = 27;
            internal static readonly int COLOR = 28;
            internal static readonly int ColorRed = 29;
            internal static readonly int ColorGreen = 30;
            internal static readonly int ColorBlue = 31;
            internal static readonly int WorldColor = 33;
            internal static readonly int WorldMatrix = 34;
            internal static readonly int NAME = 35;
            internal static readonly int SENSITIVE = 36;
            internal static readonly int UserInteractionEnabled = 72;
            internal static readonly int LeaveRequired = 37;
            internal static readonly int InheritOrientation = 38;
            internal static readonly int InheritScale = 39;
            internal static readonly int DrawMode = 41;
            internal static readonly int SizeModeFactor = 42;
            internal static readonly int WidthResizePolicy = 43;
            internal static readonly int HeightResizePolicy = 44;
            internal static readonly int SizeScalePolicy = 45;
            internal static readonly int WidthForHeight = 46;
            internal static readonly int HeightForWidth = 47;
            internal static readonly int MinimumSize = 49;
            internal static readonly int MaximumSize = 50;
            internal static readonly int InheritPosition = 51;
            internal static readonly int ClippingMode = 52;
            internal static readonly int InheritLayoutDirection = 54;
            internal static readonly int LayoutDirection = 53;
            internal static readonly int MARGIN = 10000003;
            internal static readonly int PADDING = 10000004;
            internal static readonly int SHADOW = 10000012;
            internal static readonly int CaptureAllTouchAfterStart = 67;
            internal static readonly int AllowOnlyOwnTouch = 73;
            internal static readonly int BlendEquation = 69;
            internal static readonly int Culled = 58;
            internal static readonly int AccessibilityName = 10000013;
            internal static readonly int AccessibilityDescription = 10000014;
            internal static readonly int AccessibilityTranslationDomain = 10000015;
            internal static readonly int AccessibilityRole = 10000016;
            internal static readonly int AccessibilityHighlightable = 10000017;
            internal static readonly int AccessibilityAttributes = 10000018;
            internal static readonly int DispatchKeyEvents = 10000019;
            internal static readonly int AccessibilityHidden = 10000020;
            internal static readonly int AutomationId = 10000023;
            internal static readonly int AccessibilityState = 10000026;
            internal static readonly int AccessibilityIsModal = 10000027;
            internal static readonly int AccessibilityValue = 10000024;
            internal static readonly int AccessibilityScrollable = 10000025;
            internal static readonly int UpdateAreaHint = 65;
            internal static readonly int DispatchTouchMotion = 75;
            internal static readonly int DispatchHoverMotion = 76;
            internal static readonly int OffScreenRendering = 10000028;
            internal static readonly int InnerShadow = 10000029;
            internal static readonly int Borderline = 10000030;
            internal static readonly int CornerRadiusPolicy = 25000001;
            internal static readonly int CornerRadius = 25000000;
            internal static readonly int CornerSquareness = 25000002;
            internal static readonly int BorderlineWidth = 25000003;
            internal static readonly int BorderlineColor = 25000004;
            internal static readonly int BorderlineOffset = 25000005;
        }
    }
}
