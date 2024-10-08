// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace MixedReality.Toolkit.Input.Simulation.Editor
{
    /// <summary>
    /// A custom property drawer for <see cref="EyeGazeSimulationSettings"/> fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(EyeGazeSimulationSettings))]
    public class EyeGazeSimulationSettingsDrawer : PropertyDrawer
    {
        /// <summary>
        /// Represents the number of lines taken up by the fields in this property.
        /// </summary>
        /// <remarks>Includes the property header, each SerializedProperty, and any custom LabelFields.</remarks>
        private const int UILineCount = 7;

        private readonly GUIContent simEnabledContent = new GUIContent("Simulation enabled");
        private readonly GUIContent isTrackedContent = new GUIContent("Eyes tracked");

        private readonly GUIContent eyeOffsetContent = new GUIContent("Eye origin offset");

        // private readonly GUIContent sensitivityContent = new GUIContent("Sensitivity");
        // private readonly GUIContent lookSmoothingContent = new GUIContent("Smoothed");
        // private readonly GUIContent saccadeLevelContent = new GUIContent("Saccade level");
        private readonly GUIContent lookHorizontalContent = new GUIContent("Horizontal");
        private readonly GUIContent lookVerticalContent = new GUIContent("Vertical");

        /// <inheritdoc />
        public override float GetPropertyHeight(
            SerializedProperty property,
            GUIContent label)
        {
            return PropertyDrawerUtilities.CalculatePropertyHeight(UILineCount);
        }

        /// <inheritdoc />
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(
                position,
                GUIUtility.GetControlID(FocusType.Passive),
                label,
                EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            int rowMultiplier = 1;

            SerializedProperty simEnabled = property.FindPropertyRelative("simulationEnabled");
            SerializedProperty isTracked = property.FindPropertyRelative("isTracked");
            SerializedProperty eyeOffset = property.FindPropertyRelative("eyeOriginOffset");

            // SerializedProperty sensitivity = property.FindPropertyRelative("sensitivity");
            // SerializedProperty lookSmoothing = property.FindPropertyRelative("isLookSmoothed");
            // SerializedProperty saccadeLevel = new GUIContent("saccadeLevel");
            SerializedProperty lookHorizontal = property.FindPropertyRelative("lookHorizontal");
            SerializedProperty lookVertical = property.FindPropertyRelative("lookVertical");

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    rowMultiplier,
                    PropertyDrawerUtilities.Height),
                simEnabled, simEnabledContent);


            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                isTracked, isTrackedContent);

            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                eyeOffset, eyeOffsetContent);

            EditorGUI.LabelField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                "Look", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            // EditorGUI.PropertyField(
            //    PropertyDrawerUtilities.GetPosition(
            //        position,
            //        PropertyDrawerUtilities.VerticalSpacing,
            //        ++rowMultiplier,
            //        PropertyDrawerUtilities.Height),
            //    sensitivity, sensitivityContent);
            /* todo: needed?
            EditorGUI.PropertyField(
               PropertyDrawerUtilities.GetPosition(
                   position,
                   PropertyDrawerUtilities.VerticalSpacing,
                   ++rowMultiplier,
                   PropertyDrawerUtilities.Height),
               lookSmoothing, lookSmoothingContent);
            */
            // EditorGUI.PropertyField(
            //    PropertyDrawerUtilities.GetPosition(
            //        position,
            //        PropertyDrawerUtilities.VerticalSpacing,
            //        ++rowMultiplier,
            //        PropertyDrawerUtilities.Height),
            //    saccadeLevel, saccadeLevelContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                lookHorizontal, lookHorizontalContent);
            EditorGUI.PropertyField(
                PropertyDrawerUtilities.GetPosition(
                    position,
                    PropertyDrawerUtilities.VerticalSpacing,
                    ++rowMultiplier,
                    PropertyDrawerUtilities.Height),
                lookVertical, lookVerticalContent);
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;

            EditorGUI.EndProperty();
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
