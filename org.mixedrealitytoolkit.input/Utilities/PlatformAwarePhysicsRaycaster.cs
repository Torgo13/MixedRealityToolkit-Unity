// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using UnityEngine.EventSystems;

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// A wrapper around <see cref="UnityEngine.EventSystems.PhysicsRaycaster"/>, which
    /// will automatically disable itself if it detects the application is running on an
    /// XR device (i.e., a <see cref="UnityEngine.XR.XRDisplaySubsystem"/> is present and running).
    /// </summary>
    /// <remarks>
    /// This is useful for automatically enabling UGUI-event-based UI with mouse/touchscreen
    /// input on flat/2D platforms, while saving performance on XR devices that don't need
    /// 2D input processing.
    /// </remarks>
    internal class PlatformAwarePhysicsRaycaster : PhysicsRaycaster
    {
        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Are we on an XR device? If so, we don't want to
            // use camera raycasting at all.
            if (XRDisplaySubsystemHelpers.AreAnyActive())
            {
                enabled = false;
            }
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
