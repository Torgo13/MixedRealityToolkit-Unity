// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using MixedReality.Toolkit.Editor;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace MixedReality.Toolkit
{
    /// <summary>
    /// A class helping users with validating the configuration of the project for use with MRTK3
    /// </summary>
    public static class MRTKProjectValidation
    {
        private const string XRProjectValidationSettingsPath = "Project/XR Plug-in Management/Project Validation";
        private const string DefaultMRTKProfileGuid = "c677e5c4eb85b7849a8da406775c299d";
        private static readonly Dictionary<BuildTargetGroup, List<BuildValidationRule>> validationRulesDictionary = new Dictionary<BuildTargetGroup, List<BuildValidationRule>>();

        /// <summary>
        /// Get all the possible <see cref="BuildTargetGroup"/> enumeration values.
        /// </summary>
        /// <remarks>
        /// Build targets currently not supported by MRTK will be filtered out.
        /// </remarks>
        public static readonly BuildTargetGroup[] BuildTargetGroups = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.WSA };

        [MenuItem("Mixed Reality/MRTK3/Utilities/Project Validation", priority = 0)]
        private static void MenuItem()
        {
            SettingsService.OpenProjectSettings(XRProjectValidationSettingsPath);
        }

        [InitializeOnLoadMethod]
        private static void MRTKProjectValidationCheck()
        {
            AddMRTKCoreValidationRules();
            // Add a delay to wait for all rules to be added (potentially from other MRTK scripts registered with InitializeOnLoadMethod)
            EditorApplication.delayCall += AddMRTKValidationRules;
        }

        private static void AddMRTKValidationRules()
        {
            foreach (BuildTargetGroup buildTargetGroup in validationRulesDictionary.Keys)
            {
                BuildValidator.AddRules(buildTargetGroup, validationRulesDictionary[buildTargetGroup]);
            }
        }

        private static void AddMRTKCoreValidationRules()
        {
            List<BuildValidationRule> mrtkCoreTargetIndependentRules = new List<BuildValidationRule>
            {
                // Always ensure the standalone target has a profile assigned no matter what target is being targeted
                GenerateProfileRule(BuildTargetGroup.Standalone),
                GenerateVisibleMetaFilesRule(),
                GenerateForceTextSerializationRule(),
                GenerateSpatializerRule(),
                GenerateLinearColorSpaceRule()
            };
            AddTargetIndependentRules(mrtkCoreTargetIndependentRules);

            // Add target-specific rules
            foreach (BuildTargetGroup buildTargetGroup in BuildTargetGroups)
            {
                // Skip the standalone target as the profile rule for it is already present for all build targets
                if (buildTargetGroup != BuildTargetGroup.Standalone)
                {
                    AddTargetDependentRules(new List<BuildValidationRule>() { GenerateProfileRule(buildTargetGroup) }, buildTargetGroup);
                }
            }
        }

        /// <summary>
        /// Add a build target independent rule for project configuration validation (i.e. a rule that applies to all targets)
        /// </summary>
        public static void AddTargetIndependentRules(List<BuildValidationRule> rules)
        {
            foreach (BuildTargetGroup buildTargetGroup in BuildTargetGroups)
            {
                AddTargetDependentRules(rules, buildTargetGroup);
            }
        }

        /// <summary>
        /// Add a build target dependent rule (e.g. specific to Standalone) for project configuration validation
        /// </summary>
        public static void AddTargetDependentRules(List<BuildValidationRule> rules, BuildTargetGroup buildTargetGroup)
        {
            if (validationRulesDictionary.TryGetValue(buildTargetGroup, out List<BuildValidationRule> rulesList))
            {
                rulesList.AddRange(rules);
            }
            else
            {
                validationRulesDictionary.Add(buildTargetGroup, new List<BuildValidationRule>(rules));
            }
        }

        /// <summary>
        /// Retrieve a list of loaded subsystems specified in the profile for a given BuildTargetGroup
        /// </summary>
        public static List<SystemType> GetLoadedSubsystemsForBuildTarget(BuildTargetGroup buildTargetGroup)
        {
            MRTKProfile profile = MRTKSettings.ProfileForBuildTarget(buildTargetGroup);
            if (profile != null)
            {
                return profile.LoadedSubsystems;
            }
            return null;
        }

        private static BuildValidationRule GenerateProfileRule(BuildTargetGroup buildTargetGroup)
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = $"MRTK3 profile may need to be assigned for the {buildTargetGroup} build target.",
                CheckPredicate = () => MRTKSettings.ProfileForBuildTarget(buildTargetGroup) != null,
                FixIt = () => MRTKSettings.GetOrCreateSettings().SetProfileForBuildTarget(buildTargetGroup,
                AssetDatabase.LoadAssetAtPath<MRTKProfile>(AssetDatabase.GUIDToAssetPath(DefaultMRTKProfileGuid))),
                FixItMessage = $"Assign the default MRTK3 profile for the {buildTargetGroup} build target",
                Error = false,
                HelpLink = "https://learn.microsoft.com/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/setting-up/setup-new-project#4-configure-mrtk-profile-after-import"
            };
        }

        private static BuildValidationRule GenerateVisibleMetaFilesRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "Visible meta files is recommended for Version Control settings.",
                CheckPredicate = () => VersionControlSettings.mode.Equals("Visible Meta Files"),
                FixIt = () => VersionControlSettings.mode = "Visible Meta Files",
                FixItMessage = "Change the mode under Project Settings -> Version Control to Visible Meta Files",
                Error = false
            };
        }

        private static BuildValidationRule GenerateForceTextSerializationRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "Force text serialization is recommended for Editor settings.",
                CheckPredicate = () => EditorSettings.serializationMode == SerializationMode.ForceText,
                FixIt = () => EditorSettings.serializationMode = SerializationMode.ForceText,
                FixItMessage = "Change the mode under Project Settings -> Editor -> Asset serialization to Force Text",
                Error = false
            };
        }

        private static BuildValidationRule GenerateSpatializerRule()
        {
            return new BuildValidationRule()
            {
                IsRuleEnabled = () => AudioSettings.GetSpatializerPluginNames().Length > 0,
                Category = "MRTK3",
                Message = "It is recommended to enable an audio spatializer in the project.",
                CheckPredicate = () => !string.IsNullOrEmpty(AudioSettings.GetSpatializerPluginName()),
                FixIt = () => SettingsService.OpenProjectSettings("Project/Audio"),
                FixItMessage = "Specify a spatializer under Project Settings -> Audio -> Spatializer plugin",
                FixItAutomatic = false,
                Error = false
            };
        }

        private static BuildValidationRule GenerateLinearColorSpaceRule()
        {
            return new BuildValidationRule()
            {
                Category = "MRTK3",
                Message = "For MRTK UI and other visuals to appear correctly, we strongly recommend Linear color space.",
                CheckPredicate = () => PlayerSettings.colorSpace == ColorSpace.Linear,
                FixIt = () => PlayerSettings.colorSpace = ColorSpace.Linear,
                FixItMessage = "Change the color space from Gamma to Linear under Project Settings -> Player Settings.",
                Error = false
            };
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
