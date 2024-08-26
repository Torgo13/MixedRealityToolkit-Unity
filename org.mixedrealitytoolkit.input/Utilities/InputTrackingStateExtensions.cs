// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using UnityEngine.XR;

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// Useful extensions for parsing <see cref="InputTrackingState"/> flags.
    /// </summary>
    public static class InputTrackingStateExtensions
    {
        /// <summary>
        /// Returns true iff the state is at least both positionally and rotationally tracked.
        /// </summary>
        public static bool HasPositionAndRotation(this InputTrackingState state)
        {

            return (state & (InputTrackingState.Position | InputTrackingState.Rotation)) ==
                            (InputTrackingState.Position | InputTrackingState.Rotation);
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
