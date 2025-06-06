// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using System;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A Unity subsystem that enables dictation.
    /// </summary>
    [Preserve]
    public class DictationSubsystem :
        MRTKSubsystem<DictationSubsystem, DictationSubsystemDescriptor, DictationSubsystem.Provider>,
        IDictationSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DictationSubsystem"/> class.
        /// </summary>
        public DictationSubsystem()
        { }

        /// <summary>
        /// Interface for providing recognition functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<DictationSubsystem>, IDictationSubsystem
        {
            #region IDictationSubsystem implementation

            /// <inheritdoc/>
            public abstract void StartDictation();

            /// <inheritdoc/>
            public abstract void StopDictation();

            /// <inheritdoc/>
            public event Action<DictationResultEventArgs> Recognizing;

            /// <inheritdoc/>
            public event Action<DictationResultEventArgs> Recognized;

            /// <inheritdoc/>
            public event Action<DictationSessionEventArgs> RecognitionFinished;

            /// <inheritdoc/>
            public event Action<DictationSessionEventArgs> RecognitionFaulted;

            #endregion IDictationSubsystem implementation

            /// <summary>
            /// Trigger for the <see cref="Recognizing"/> Action.
            /// </summary>
            protected void OnRecognizing(DictationResultEventArgs eventArgs)
            {
                Recognizing?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="Recognized"/> Action.
            /// </summary>
            protected void OnRecognized(DictationResultEventArgs eventArgs)
            {
                Recognized?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="RecognitionFinished"/> Action.
            /// </summary>
            protected void OnRecognitionFinished(DictationSessionEventArgs eventArgs)
            {
                RecognitionFinished?.Invoke(eventArgs);
            }

            /// <summary>
            /// Trigger for the <see cref="RecognitionFaulted"/> Action.
            /// </summary>
            protected void OnRecognitionFaulted(DictationSessionEventArgs eventArgs)
            {
                RecognitionFaulted?.Invoke(eventArgs);
            }

        }

        #region IDictationSubsystem implementation

        /// <inheritdoc/>
        public void StartDictation() => provider.StartDictation();

        /// <inheritdoc/>
        public void StopDictation() => provider.StopDictation();

        /// <inheritdoc/>
        public event Action<DictationResultEventArgs> Recognizing
        {
            add => provider.Recognizing += value;
            remove => provider.Recognizing -= value;
        }

        /// <inheritdoc/>
        public event Action<DictationResultEventArgs> Recognized
        {
            add => provider.Recognized += value;
            remove => provider.Recognized -= value;
        }

        /// <inheritdoc/>
        public event Action<DictationSessionEventArgs> RecognitionFinished
        {
            add => provider.RecognitionFinished += value;
            remove => provider.RecognitionFinished -= value;
        }

        /// <inheritdoc/>
        public event Action<DictationSessionEventArgs> RecognitionFaulted
        {
            add => provider.RecognitionFaulted += value;
            remove => provider.RecognitionFaulted -= value;
        }

        #endregion IDictationSubsystem implementation

        /// <summary>
        /// Registers a dictation subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="DictationSubsystemParams">The parameters defining the dictation subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <see langword="true"/> if the subsystem implementation is registered. Otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Register(DictationSubsystemCinfo DictationSubsystemParams)
        {
            var descriptor = DictationSubsystemDescriptor.Create(DictationSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }

}

#endif // ENABLE_VR && ENABLE_XR_MODULE
