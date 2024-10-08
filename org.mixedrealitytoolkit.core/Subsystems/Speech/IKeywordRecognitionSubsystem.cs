// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a KeywordRecognitionSubsystem needs to be able to provide.
    /// Both the KeywordRecognitionSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    public interface IKeywordRecognitionSubsystem : ISubsystem
    {
        /// <summary>
        /// Add or update a keyword to recognize.
        /// </summary>
        UnityEvent CreateOrGetEventForKeyword(string keyword);

        /// <summary>
        /// Remove a keyword to recognize.
        /// </summary>
        void RemoveKeyword(string keyword);

        /// <summary>
        /// Remove all keywords to recognize.
        /// </summary>
        void RemoveAllKeywords();

        /// <summary>
        /// Get a read-only reference to the all keywords that are currently registered with the recognizer.
        /// </summary>
        IReadOnlyDictionary<string, UnityEvent> GetAllKeywords();
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
