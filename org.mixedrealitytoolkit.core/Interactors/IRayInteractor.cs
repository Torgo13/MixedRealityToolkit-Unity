// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using UnityEngine.XR.Interaction.Toolkit;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all far-ray-like interactors implement.
    /// Interactors that implement this interface are expected to use
    /// the <see cref="IXRInteractor"/> attachTransform to specify
    /// the impact point of the ray on the interactable.
    /// </summary>
    public interface IRayInteractor : IXRInteractor
    {
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
