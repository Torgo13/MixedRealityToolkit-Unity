// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif // UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA

namespace MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// A Unity subsystem that extends <see cref="MixedReality.Toolkit.Subsystems.DictationSubsystem">DictationSubsystem</see>
    /// so to expose the dictation services available on Windows platforms. This subsystem is enabled for Windows Standalone and
    /// Universal Windows Applications.
    /// </summary>
    /// <remarks>
    /// This subsystem can be configured using the <see cref="MixedReality.Toolkit.Speech.Windows.WindowsDictationSubsystemConfig">WindowsDictationSubsystemConfig</see> Unity asset.
    /// </remarks>
    [Preserve]
    [MRTKSubsystem(
        Name = "org.mixedrealitytoolkit.windowsdictation",
        DisplayName = "MRTK Windows Dictation Subsystem",
        Author = "Mixed Reality Toolkit Contributors",
        ProviderType = typeof(WindowsDictationProvider),
#if ENABLE_VR && ENABLE_XR_MODULE
        SubsystemTypeOverride = typeof(WindowsDictationSubsystem),
#endif // ENABLE_VR && ENABLE_XR_MODULE
        ConfigType = typeof(WindowsDictationSubsystemConfig))]
#if ENABLE_VR && ENABLE_XR_MODULE
    public class WindowsDictationSubsystem : DictationSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsDictationSubsystem, DictationSubsystemCinfo>();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
#else
            Debug.Log($"Skipping registration of the {cinfo.Name} subsystem due to running on an unsupported platform.");
#endif
        }
#endif // ENABLE_VR && ENABLE_XR_MODULE

        /// <summary>
        /// A subsystem provider used with <see cref="WindowsDictationSubsystem"/> class that exposes methods on Unity's `DictationRecognizer`
        /// on Windows platforms.
        /// </summary>
#if ENABLE_VR && ENABLE_XR_MODULE
        [Preserve]
        class WindowsDictationProvider : Provider
#else
        public class WindowsDictationProvider
#endif // ENABLE_VR && ENABLE_XR_MODULE
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <summary>
            /// The confidence threshold for the recognizer to return its result.
            /// </summary>
            public ConfidenceLevel ConfidenceLevel => confidenceLevel;

            /// <summary>
            /// The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.
            /// </summary>
            public float InitialSilenceTimeoutSeconds
            {
                get => initialSilenceTimeoutSeconds;
                set
                {
                    if (initialSilenceTimeoutSeconds != value)
                    {
                        initialSilenceTimeoutSeconds = value;
                        if (dictationRecognizer != null)
                        {
                            dictationRecognizer.InitialSilenceTimeoutSeconds = value;
                        }
                    }
                }
            }

            /// <summary>
            /// The time length in seconds before dictation recognizer session ends due to lack of audio input.
            /// </summary>
            public float AutoSilenceTimeout
            {
                get => autoSilenceTimeout;
                set
                {
                    if (autoSilenceTimeout != value)
                    {
                        autoSilenceTimeout = value;
                        if (dictationRecognizer != null)
                        {
                            dictationRecognizer.AutoSilenceTimeoutSeconds = value;
                        }
                    }
                }
            }

            private WindowsDictationSubsystemConfig config;
            private DictationRecognizer dictationRecognizer;
            private ConfidenceLevel confidenceLevel;
            private float initialSilenceTimeoutSeconds;
            private float autoSilenceTimeout;
#endif

            /// <summary>
            /// Initializes a new instance of the <see cref="WindowsDictationProvider"/> class.
            /// </summary>
            public WindowsDictationProvider()
            {
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA)
                Debug.LogError("Cannot create WindowsDictationSubsystem because WindowsDictationSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
#if ENABLE_VR && ENABLE_XR_MODULE
            /// <inheritdoc/>
            public override void Start()
#else
            public void Start()
#endif // ENABLE_VR && ENABLE_XR_MODULE
            {
#if ENABLE_VR && ENABLE_XR_MODULE
                base.Start();
                config = XRSubsystemHelpers.GetConfiguration<WindowsDictationSubsystemConfig, WindowsDictationSubsystem>();
#endif // ENABLE_VR && ENABLE_XR_MODULE
                confidenceLevel = config.ConfidenceLevel;
                initialSilenceTimeoutSeconds = config.InitialSilenceTimeoutSeconds;
                autoSilenceTimeout = config.AutoSilenceTimeout;
            }

#if ENABLE_VR && ENABLE_XR_MODULE
            public override void Stop()
#else
            public void Stop()
#endif // ENABLE_VR && ENABLE_XR_MODULE
            {
#if ENABLE_VR && ENABLE_XR_MODULE
                base.Stop();
#endif // ENABLE_VR && ENABLE_XR_MODULE
                StopDictation();
            }
#endif

#if ENABLE_VR && ENABLE_XR_MODULE
            /// <inheritdoc/>
            public override void Destroy()
#else
            public void Destroy()
#endif // ENABLE_VR && ENABLE_XR_MODULE
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (dictationRecognizer != null)
                {
                    DeregisterDictationEvents();
                    if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                    {
                        dictationRecognizer.Stop();
                    }
                    dictationRecognizer.Dispose();
                    dictationRecognizer = null;
                }
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <summary>
            /// Start dictation using the specified ConfidenceLevel.
            /// </summary>
            public void StartDictation(ConfidenceLevel confidenceLevel)
            {
                if (dictationRecognizer == null || this.confidenceLevel != confidenceLevel)
                {
                    Destroy();
                    this.confidenceLevel = confidenceLevel;
                    dictationRecognizer = new DictationRecognizer(confidenceLevel);
                    dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeoutSeconds;
                    dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
                }
                if (dictationRecognizer.Status != SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Start();
                    // Deregister first to prevent duplicate
                    DeregisterDictationEvents();
                    RegisterDictationEvents();
                }
            }
#endif

            #region IDictationSubsystem implementation

#if ENABLE_VR && ENABLE_XR_MODULE
            /// <inheritdoc/>
            public override void StartDictation()
#else
            public void StartDictation()
#endif // ENABLE_VR && ENABLE_XR_MODULE
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                StartDictation(confidenceLevel);
#else
                Debug.LogError("Cannot call StartDictation because WindowsDictationSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#if ENABLE_VR && ENABLE_XR_MODULE
            /// <inheritdoc/>
            public override void StopDictation()
#else
            public void StopDictation()
#endif // ENABLE_VR && ENABLE_XR_MODULE
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Stop();
                }
#else
                Debug.LogError("Cannot call StopRecognition because WindowsDictationSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#endregion IDictationSubsystem implementation

            #region Helpers

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA

            private void RegisterDictationEvents()
            {
                dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError += DictationRecognizer_DictationError;
            }

            private void DeregisterDictationEvents()
            {
                dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
            }

            /// <summary>
            /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
            /// </summary>
            /// <param name="text">The currently hypothesized recognition.</param>
#if OPTIMISATION_STATIC
            static
#endif // OPTIMISATION_STATIC
            private void DictationRecognizer_DictationHypothesis(string text)
            {
                DictationResultEventArgs eventArgs = new DictationResultEventArgs(text, null);
#if ENABLE_VR && ENABLE_XR_MODULE
                OnRecognizing(eventArgs);
#endif // ENABLE_VR && ENABLE_XR_MODULE
            }

            /// <summary>
            /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
            /// </summary>
            /// <param name="text">The text that was heard by the recognizer.</param>
            /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
            private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
            {
                DictationResultEventArgs eventArgs = new DictationResultEventArgs(text, ConfidenceLevelToFloat(confidence));
#if ENABLE_VR && ENABLE_XR_MODULE
                OnRecognized(eventArgs);
#endif // ENABLE_VR && ENABLE_XR_MODULE
            }

            /// <summary>
            /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
            /// Typically, this will simply return "Complete".
            /// </summary>
            /// <param name="cause">An enumerated reason for the session completing.</param>
            private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
            {
                DictationSessionEventArgs eventArgs = new DictationSessionEventArgs(ToDictationEventReason(cause),
                    cause.ToString());
#if ENABLE_VR && ENABLE_XR_MODULE
                OnRecognitionFinished(eventArgs);
#endif // ENABLE_VR && ENABLE_XR_MODULE
            }

            /// <summary>
            /// This event is fired when an error occurs.
            /// </summary>
            /// <param name="error">The string representation of the error reason.</param>
            /// <param name="hresult">The int representation of the hresult.</param>
#if OPTIMISATION_STATIC
            static
#endif // OPTIMISATION_STATIC
            private void DictationRecognizer_DictationError(string error, int hresult)
            {
                DictationSessionEventArgs eventArgs = new DictationSessionEventArgs(DictationEventReason.UnknownFailure,
                    error.ToString() + "\nHRESULT: " + hresult);
#if ENABLE_VR && ENABLE_XR_MODULE
                OnRecognitionFaulted(eventArgs);
#endif // ENABLE_VR && ENABLE_XR_MODULE
            }
        
#if OPTIMISATION_STATIC
            static
#endif // OPTIMISATION_STATIC
            private float ConfidenceLevelToFloat(ConfidenceLevel level)
            {
                return level switch
                {
                    ConfidenceLevel.Rejected => 0,
                    ConfidenceLevel.Low => 0.25f,
                    ConfidenceLevel.Medium => 0.5f,
                    ConfidenceLevel.High => 0.75f,
                    _ => 0,
                };
            }
        
#if OPTIMISATION_STATIC
            static
#endif // OPTIMISATION_STATIC
            private DictationEventReason ToDictationEventReason(DictationCompletionCause cause)
            {
                return cause switch
                {
                    DictationCompletionCause.Complete => DictationEventReason.Complete,
                    DictationCompletionCause.AudioQualityFailure => DictationEventReason.AudioQualityFailure,
                    DictationCompletionCause.Canceled => DictationEventReason.Canceled,
                    DictationCompletionCause.TimeoutExceeded => DictationEventReason.TimeoutExceeded,
                    DictationCompletionCause.PauseLimitExceeded => DictationEventReason.PauseLimitExceeded,
                    DictationCompletionCause.NetworkFailure => DictationEventReason.NetworkFailure,
                    DictationCompletionCause.MicrophoneUnavailable => DictationEventReason.MicrophoneUnavailable,
                    DictationCompletionCause.UnknownError => DictationEventReason.UnknownFailure,
                    _ => DictationEventReason.Unknown,
                };
            }
#endif

#endregion Helpers

#if ENABLE_VR && ENABLE_XR_MODULE
        }
#endif // ENABLE_VR && ENABLE_XR_MODULE
    }
}

