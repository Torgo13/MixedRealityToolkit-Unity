﻿// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Class which supports components implementing <see cref="IAudioInfluencer"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// AudioInfluencerController requires an <see href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</see> component.
    /// If one is not attached, it will be added automatically.
    /// </para>
    /// <para>
    /// Each sound playing game object should have an AudioInfluencerController
    /// attached in order to have its audio properly influenced.
    /// </para>
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    [AddComponentMenu("MRTK/Audio/Audio Influencer Controller")]
    public class AudioInfluencerController : MonoBehaviour
    {
        /// <summary>
        /// Frequency below the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a high pass filter to allow all
        /// human audible frequencies through the filter.
        /// </remarks>
#if OPTIMISATION_IL2CPP
        public const float NeutralLowFrequency = 10.0f;
#else
        public static readonly float NeutralLowFrequency = 10.0f;
#endif // OPTIMISATION_IL2CPP

        /// <summary>
        /// Frequency above the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a low pass filter to allow all
        /// human audible frequencies through the filter.
        /// </remarks>
#if OPTIMISATION_IL2CPP
        public const float NeutralHighFrequency = 22000.0f;
#else
        public static readonly float NeutralHighFrequency = 22000.0f;
#endif // OPTIMISATION_IL2CPP

        /// <summary>
        /// The source of the audio.
        /// </summary>
        private AudioSource audioSource;

        [Tooltip("Time, in seconds, between audio influence updates. 0 indicates to update every frame.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float updateInterval = 0.25f;

        /// <summary>
        /// Time, in seconds, between audio influence updates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The UpdateInterval range is between 0.0 and 1.0, inclusive. The default
        /// value is 0.25.
        /// </para>
        /// <para>
        /// A value of 0.0 indicates that updates occur every frame.
        /// </para>
        /// </remarks>
        public float UpdateInterval
        {
            get
            {
                 return updateInterval;
            }

            set
            {
                updateInterval = Mathf.Clamp(value, 0.0f, 1.0f);
            }
        }

        [Tooltip("Maximum distance, in meters, to look when attempting to find the user and any influencers.")]
        [Range(1.0f, 50.0f)]
        [SerializeField]
        private float maxDistance = 20.0f;

        /// <summary>
        /// Maximum distance, in meters, to look when attempting to find the user and
        /// any influencer.
        /// </summary>
        /// <remarks>
        /// The max distance range is 1.0 to 50.0, inclusive. The default value is 20.0.
        /// </remarks>
        public float MaxDistance
        {
            get
            {
                 return maxDistance;
            }

            set
            {
                maxDistance = Mathf.Clamp(value, 1.0f, 50.0f);
            }
        }

        /// <summary>
        /// Maximum number of objects that will be considered when looking for an influencer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this value too high may have a negative impact on the performance of your
        /// experience.
        /// </para>
        /// <para>This value can only be set in Unity's inspector windows, in a range from 1 to 25, inclusive.
        /// The default value is 10.
        /// </para>
        /// </remarks>
        [Tooltip("Maximum number of objects that will be considered when looking for influencers.")]
        [Range(1, 25)]
        [SerializeField]
        private int maxObjects = 10;

        /// <summary>
        /// Time of last audio processing update.
        /// </summary>
        private DateTime lastUpdate = DateTime.MinValue;

        /// <summary>
        /// The initial volume level of the audio source.
        /// </summary>
        private float initialAudioSourceVolume;

        /// <summary>
        /// The hits returned by Physics.RaycastAll
        /// </summary>
        private RaycastHit[] hits;

        /// <summary>
        /// The collection of applied audio effects.
        /// </summary>
        private List<IAudioInfluencer> currentEffects = new List<IAudioInfluencer>();

        private float nativeLowPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native low pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeLowPassCutoffFrequency
        {
            get { return nativeLowPassCutoffFrequency; }
            set { nativeLowPassCutoffFrequency = value; }
        }

        private float nativeHighPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native high pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeHighPassCutoffFrequency
        {
            get { return nativeHighPassCutoffFrequency; }
            set { nativeHighPassCutoffFrequency = value; }
        }

        private List<IAudioInfluencer> effectsToApply = null;
        private List<IAudioInfluencer> effectsToRemove = null;

        private float nextUpdate = 0f;

#if OPTIMISATION
        Transform _cameraTransform;
#endif // OPTIMISATION

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            effectsToApply = new List<IAudioInfluencer>(maxObjects);
            effectsToRemove = new List<IAudioInfluencer>(maxObjects);

            audioSource = GetComponent<AudioSource>();

            initialAudioSourceVolume = audioSource.volume;

            // Get initial values that the sound designer / developer
            // may have applied to this game object
            AudioLowPassFilter lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            nativeLowPassCutoffFrequency = (lowPassFilter != null) ? lowPassFilter.cutoffFrequency : NeutralHighFrequency;
            AudioHighPassFilter highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            nativeHighPassCutoffFrequency = (highPassFilter != null) ? highPassFilter.cutoffFrequency : NeutralLowFrequency;

            // Preallocate the array that will be used to collect RaycastHit structures.
            hits = new RaycastHit[maxObjects];

            // Initialize our update time.
            nextUpdate = Time.time;
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
        private void Update()
        {
            // Audio influences are generally not updated every frame.
            if (Time.time < nextUpdate) { return; }

            audioSource.volume = initialAudioSourceVolume;

            // Apply audio influencers to the audio source.
            ApplyEffects();

            // Remove the audio influencers that no longer apply.
            RemoveEffects();

            currentEffects.Clear();
            currentEffects.AddRange(effectsToApply);

            nextUpdate += UpdateInterval;
        }

        /// <summary>
        /// Applies the effects specified by the collection of audio influencers.
        /// </summary>
        private void ApplyEffects()
        {
            effectsToApply.Clear();
            UpdateActiveInfluencerCollection();

            foreach (IAudioInfluencer influencer in effectsToApply)
            {
                influencer.ApplyEffect(gameObject);
            }
        }

        /// <summary>
        /// Removes the effects applied by specified audio influencers.
        /// </summary>
        private void RemoveEffects()
        {
            effectsToRemove.Clear();

            for (int i = 0; i < currentEffects.Count; i++)
            {
                IAudioInfluencer audioInfluencer = currentEffects[i];

                // Find influencers that are no longer in line of sight,
                // have been destroyed, or have been disabled
                if (!effectsToApply.Contains(audioInfluencer) ||
                    !audioInfluencer.TryGetMonoBehaviour(out MonoBehaviour mbPrev) ||
                    !mbPrev.isActiveAndEnabled)
                {
                    effectsToRemove.Add(audioInfluencer);
                }
            }

            foreach (IAudioInfluencer influencer in effectsToRemove)
            {
                influencer.RemoveEffect(gameObject);
            }
        }

        /// <summary>
        /// Finds the IAudioInfluencer objects that are to be applied to the audio source.
        /// </summary>
        private void UpdateActiveInfluencerCollection()
        {
#if OPTIMISATION
            if (_cameraTransform == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    _cameraTransform = mainCamera.transform;
                    if (_cameraTransform == null)
                        return;
                }
            }

            var position = gameObject.transform.position;
            var cameraPosition = _cameraTransform.position;

            // Influencers take effect only when between the emitter and the user.
            // Perform a raycast from the user toward the object.
            Vector3 direction = (position - cameraPosition).normalized;
            float distance = Vector3.Distance(cameraPosition, position);

            int count = UnityPhysics.RaycastNonAlloc(cameraPosition,
#else
            Transform cameraTransform = Camera.main.transform;

            // Influencers take effect only when between the emitter and the user.
            // Perform a raycast from the user toward the object.
            Vector3 direction = (gameObject.transform.position - cameraTransform.position).normalized;
            float distance = Vector3.Distance(cameraTransform.position, gameObject.transform.position);

            int count = UnityPhysics.RaycastNonAlloc(cameraTransform.position,
#endif // OPTIMISATION
                                                direction,
                                                hits,
                                                distance,
                                                UnityPhysics.DefaultRaycastLayers,
                                                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
#if OPTIMISATION
                IAudioInfluencer influencer = hits[i].collider.GetComponentInParent<IAudioInfluencer>();
#else
                IAudioInfluencer influencer = hits[i].collider.gameObject.GetComponentInParent<IAudioInfluencer>();
#endif // OPTIMISATION
                if (influencer != null)
                {
                    effectsToApply.Add(influencer);
                }
            }
        }
    }
}
