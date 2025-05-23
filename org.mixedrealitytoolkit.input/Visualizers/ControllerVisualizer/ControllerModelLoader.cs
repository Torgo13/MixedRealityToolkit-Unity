// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT
using Microsoft.MixedReality.OpenXR;
using GLTFast;
#endif // MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT

namespace MixedReality.Toolkit.Input
{
    /// <summary>
    /// A helper class which loads and caches controller models fetched from the controller's platform SDK.
    /// </summary>
    public static class ControllerModelLoader
    {
        /// <summary>
        /// A dictionary which caches the controller model GameObject associated with a specified model key
        /// </summary>
        private static Dictionary<ulong, GameObject> controllerModelDictionary = new Dictionary<ulong, GameObject>();

        /// <summary>
        /// Stores a boolean indicating whether a warning was recently raised when trying to get the model key for this input device.
        /// </summary>
        private static Dictionary<string, bool> warningCache = new Dictionary<string, bool>();

        /// <summary>
        /// Stores a boolean indicating whether an error was recently raised when trying to get the model for this model key.
        /// </summary>
        private static Dictionary<ulong, bool> errorCache = new Dictionary<ulong, bool>();

        /// <summary>
        /// Tries to load the controller model game object of the specified input device with the corresponding handedness.
        /// Requires the MR OpenXR plugin to work.
        /// </summary>
        /// <param name="inputDeviceKey">A deterministic key representing the current input device.</param>
        /// <param name="handedness">The handedness of the input device requesting the controller model.</param>
        /// <returns>A GameObject representing the generated controller model in the scene.</returns>
        public async static Task<GameObject> TryGenerateControllerModelFromPlatformSDK(string inputDeviceKey, Handedness handedness)
        {
            // Proceed with trying to load the model from the platform
            GameObject gltfGameObject = null;

#if MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT
            ControllerModel controllerModelProvider = handedness == Handedness.Left ? ControllerModel.Left :
                                                      handedness == Handedness.Right ? ControllerModel.Right :
                                                      null;
            if (controllerModelProvider == null)
            {
                Debug.Log("Controller model provider does not exist for this handedness");
                return null;
            }

            // Try to obtain a model key
            if (!controllerModelProvider.TryGetControllerModelKey(out ulong modelKey))
            {
                if (!warningCache.TryGetValue(inputDeviceKey, out bool warningLogged) || !warningLogged)
                {
                    Debug.LogFormat("{0} didn't provide a key for a controller model from the platform.", inputDeviceKey);
                }

                // Make sure we record in our cache that we've previously raised a warning for this input device
                warningCache[inputDeviceKey] = true;

                return null;
            }

            // Make sure we record in our cache that we've succeeded in loading this model without warnings
            warningCache[inputDeviceKey] = false;

            // Check if a GameObject already exists for this model key
            if (controllerModelDictionary.TryGetValue(modelKey, out gltfGameObject) && gltfGameObject != null)
            {
                gltfGameObject.SetActive(true);
#if MROPENXR_ANIM_PRESENT
                // Try to add an animator to the controller model
                ControllerModelArticulator controllerModelArticulator = gltfGameObject.EnsureComponent<ControllerModelArticulator>();
                if (!controllerModelArticulator.TryStartArticulating(controllerModelProvider, modelKey))
                {
                    Debug.LogError("Unable to load model animation.");
                }
#endif
                return gltfGameObject;
            }

            // Otherwise, try to load the model from the model provider
            byte[] modelStream = await controllerModelProvider.TryGetControllerModel(modelKey);
            if (modelStream == null || modelStream.Length == 0)
            {
                if (!errorCache.TryGetValue(modelKey, out bool errorLogged) || !errorLogged)
                {
                    Debug.LogErrorFormat("Failed to obtain controller model from platform for model key {0}.", modelKey);
                }

                // Make sure we record in our cache that we've previously raised a warning for this input device
                errorCache[modelKey] = true;

                return null;
            }
            // Make sure we record in our cache that we've succeeded in loading this model without warnings
            errorCache[modelKey] = false;

            // Finally try to create a GameObject from the model data
            GltfImport gltf = new GltfImport();
            bool success = await gltf.LoadGltfBinary(modelStream);
            gltfGameObject = new GameObject(modelKey.ToString());
            if (success && await gltf.InstantiateMainSceneAsync(gltfGameObject.transform))
            {
                // After all the awaits, double check that another task didn't finish earlier
                if (controllerModelDictionary.TryGetValue(modelKey, out GameObject existingGameObject) && existingGameObject != null)
                {
                    Object.Destroy(gltfGameObject);
                    return existingGameObject;
                }

#if MROPENXR_ANIM_PRESENT
                // Try to add an animator to the controller model
                ControllerModelArticulator controllerModelArticulator = gltfGameObject.EnsureComponent<ControllerModelArticulator>();
                if (!controllerModelArticulator.TryStartArticulating(controllerModelProvider, modelKey))
                {
                    Debug.LogError("Unable to load model animation.");
                }
#endif

                controllerModelDictionary[modelKey] = gltfGameObject;
            }
            else
            {
                Object.Destroy(gltfGameObject);
            }
#else
            await Task.CompletedTask;
#endif // MROPENXR_PRESENT && (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && GLTFAST_PRESENT

            return gltfGameObject;
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
