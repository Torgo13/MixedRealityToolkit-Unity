// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR;

namespace MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Subsystem for aggregating skeletal hand joint data from all available sources.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations can aggregate hand joint data from multiple APIs, or from multiple
    /// <see cref="HandsSubsystem"/> objects, or from any other source they choose.
    /// </para>
    /// <para>
    /// Recommended use is for aggregating from all loaded <see cref="HandsSubsystem"/> object. See <c>MRTKHandsAggregatorSubsystem</c> for the MRTK implementation.
    /// </para>
    /// </remarks>
    [Preserve]
    public class HandsAggregatorSubsystem :
        MRTKSubsystem<HandsAggregatorSubsystem,
                      MRTKSubsystemDescriptor<HandsAggregatorSubsystem,
                                              HandsAggregatorSubsystem.Provider>,
                      HandsAggregatorSubsystem.Provider>,
        IHandsAggregatorSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandsAggregatorSubsystem"/> class.
        /// </summary>
        public HandsAggregatorSubsystem()
        { }

        /// <summary>
        /// Interface for providing hand functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<HandsAggregatorSubsystem>, IHandsAggregatorSubsystem
        {
            #region IHandsAggregatorSubsystem implementation

            /// <inheritdoc/>
            public abstract bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose);

            /// <inheritdoc/>
            public abstract bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose);

            /// <inheritdoc/>
            public abstract bool TryGetPinchProgress(XRNode hand, out bool isReadyToPinch, out bool isPinching, out float pinchAmount);

            /// <inheritdoc/>
            public abstract bool TryGetPalmFacingAway(XRNode hand, out bool palmFacingAway);

            /// <inheritdoc/>
            public abstract bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose);

            ///<inheritdoc/>
            public abstract bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses);

            #endregion IHandsAggregatorSubsystem implementation
        }

        #region IHandsAggregatorSubsystem implementation

        /// <inheritdoc/>
        public bool TryGetNearInteractionPoint(XRNode hand, out HandJointPose jointPose)
            => provider.TryGetNearInteractionPoint(hand, out jointPose);

        /// <inheritdoc/>
        public bool TryGetPinchingPoint(XRNode hand, out HandJointPose jointPose)
            => provider.TryGetPinchingPoint(hand, out jointPose);

        /// <inheritdoc/>
        public bool TryGetPinchProgress(XRNode hand, out bool isReadyToPinch, out bool isPinching, out float pinchAmount)
            => provider.TryGetPinchProgress(hand, out isReadyToPinch, out isPinching, out pinchAmount);

        /// <inheritdoc/>
        public bool TryGetPalmFacingAway(XRNode hand, out bool isPalmFacingAway)
            => provider.TryGetPalmFacingAway(hand, out isPalmFacingAway);

        /// <inheritdoc/>
        public bool TryGetJoint(TrackedHandJoint joint, XRNode hand, out HandJointPose jointPose)
            => provider.TryGetJoint(joint, hand, out jointPose);

        /// <inheritdoc/>
        public bool TryGetEntireHand(XRNode hand, out IReadOnlyList<HandJointPose> jointPoses) => provider.TryGetEntireHand(hand, out jointPoses);

        #endregion IHandsAggregatorSubsystem implementation

        /// <summary>
        /// Registers a hands subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="cinfo">
        /// The parameters defining the hands subsystem functionality implemented
        /// by the subsystem provider.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the subsystem implementation is registered. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Register(MRTKSubsystemCinfo cinfo)
        {
            var descriptor = MRTKSubsystemDescriptor<HandsAggregatorSubsystem, Provider>.Create(cinfo);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
