// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using UnityEngine.XR.Interaction.Toolkit;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all interactors which offer
    /// variable selection must implement.
    /// </summary>
    public interface IVariableSelectInteractor : IXRSelectInteractor, IXRHoverInteractor
    {
        /// <summary>
        /// Returns a value [0,1] representing the variable
        /// amount of "selection" that this interactor is performing.
        /// </summary>
        /// <remarks>
        /// For gaze-pinch interactors, this is the pinch progress.
        /// For motion controllers, this is the analog trigger press amount.
        /// </remarks>
        float SelectProgress { get; }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
