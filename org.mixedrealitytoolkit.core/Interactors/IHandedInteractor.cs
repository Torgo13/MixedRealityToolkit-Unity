// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using UnityEngine.XR.Interaction.Toolkit;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all interactors with the concept of handedness implement.
    /// </summary>
    public interface IHandedInteractor : IXRInteractor
    {
        /// <summary>
        /// Returns the Handedness of this interactor.
        /// </summary>
        public Handedness Handedness { get; }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
