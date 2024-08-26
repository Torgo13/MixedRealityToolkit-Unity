// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

namespace MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Helper methods for working with Accessibility components.
    /// </summary>
    public static class AccessibilityHelpers
    {
        private static AccessibilitySubsystem subsystem = null;

        /// <summary>
        /// The first running AccessibilitySubsystem instance.
        /// </summary>
        public static AccessibilitySubsystem Subsystem
        {
            get
            {
                if (subsystem == null || !subsystem.running)
                {
                    subsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<AccessibilitySubsystem>();
                }
                return subsystem;
            }
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
