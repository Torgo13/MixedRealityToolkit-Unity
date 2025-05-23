﻿// Copyright (c) Mixed Reality Toolkit Contributors
// Licensed under the BSD 3-Clause

#if ENABLE_VR && ENABLE_XR_MODULE

using Unity.Profiling;
using UnityEngine;

namespace MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Provides a solver that overlaps with the tracked object.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Solvers/Overlap")]
    public class Overlap : Solver
    {
        private static readonly ProfilerMarker SolverUpdatePerfMarker =
            new ProfilerMarker("[MRTK] Overlap.SolverUpdate");

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            using (SolverUpdatePerfMarker.Auto())
            {
                Transform target = SolverHandler.TransformTarget;
                if (target != null)
                {
                    GoalPosition = target.position;
                    GoalRotation = target.rotation;
                }
            }
        }
    }
}

#endif // ENABLE_VR && ENABLE_XR_MODULE
