// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

using MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using UnityEngine;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// A build-target-specific profile that determines which subsystems are launched,
    /// and which configurations are bound to them.
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "MRTKProfile.asset", menuName = "MRTK/MRTKProfile")]
    public class MRTKProfile : BaseMRTKProfile
    {
        private static MRTKProfile instance = null;

        /// <summary>
        /// Static instance that will hold the runtime asset instance we created in our build process.
        /// </summary>
        public static MRTKProfile Instance
        {
            get => instance;
#if UNITY_EDITOR
            set => instance = value;
#endif
        }

        [SerializeField]
        [Tooltip("The list of subsystems intended to be started at runtime.")]
        [Implements(typeof(IMRTKManagedSubsystem), TypeGrouping.ByNamespaceFlat)]
        private List<SystemType> loadedSubsystems = new List<SystemType>();

        /// <summary>
        /// The list of subsystems intended to be started at runtime.
        /// </summary>
        /// <remarks>
        /// Subsystems not on this list may still be started at a later point, manually.
        /// </remarks>
        public List<SystemType> LoadedSubsystems
        {
            get => loadedSubsystems;
            protected set => loadedSubsystems = value;
        }

        [SerializeField]
        [Tooltip("The collection of configuration mapped to the corresponding subsystem.")]
        private SerializableDictionary<SystemType, BaseSubsystemConfig> subsystemConfigs = new SerializableDictionary<SystemType, BaseSubsystemConfig>();

        /// <summary>
        /// The collection of <see cref="BaseSubsystemConfig"/> mapped to the corresponding subsystem.
        /// </summary>
        protected SerializableDictionary<SystemType, BaseSubsystemConfig> SubsystemConfigs
        {
            get => subsystemConfigs;
            set => subsystemConfigs = value;
        }

        /// <summary>
        /// Attempts to retrieve the specified <see cref="BaseSubsystemConfig"/> for a given subsystem type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if there is a registered configuration for the specified subsystem, <see langword="false"/> otherwise.
        /// </returns>
        public bool TryGetConfigForSubsystem(SystemType subsystemType, out BaseSubsystemConfig config)
        {
#if OPTIMISATION
            if (subsystemConfigs.TryGetValue(subsystemType, out BaseSubsystemConfig subsystemConfig))
            {
                config = subsystemConfig;
                return true;
            }
#else
            if (subsystemConfigs.ContainsKey(subsystemType))
            {
                config = subsystemConfigs[subsystemType];
                return true;
            }
#endif // OPTIMISATION
            else
            {
                config = null;
                return false;
            }
        }

#if !UNITY_EDITOR
        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
#endif
    }
}
