using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public class PathManager
    {
        public List<ObjectPath> Paths { get; private set; }
        private LayerMask _layerMask;
        private const int MAXIMUM_RECURSION_COUNT = 10;
        public PathManager(Vector3 initialPoint, Vector3 initialNormal, Transform initialParent, List<Vector3> initialDirections, float minStepDistance,
            float maxstepDistance, float angleDeviation, LayerMask layerMask)
        {
            Paths = new();
            _layerMask = layerMask;
            foreach (Vector3 initialDirection in initialDirections)
            {
                ObjectPath path = new(initialPoint, initialDirection, initialNormal, initialParent, minStepDistance, maxstepDistance, angleDeviation, this);
                Paths.Add(path);
            }
        }

        public Vector3 ReturnNextPoint(Vector3 initialPoint, Vector3 normal, Vector3 originalDirection, float angleDeviation, float stepDistance,
            out Vector3 newNormal, out Vector3 newDirection, out Transform anchorParent, int iterationCount = 0)
        {
            //This methods needs to be drawn to be understood
            Vector3 direction = originalDirection.normalized;
            Vector3 origin = initialPoint + 0.5f * stepDistance * normal;
            float angleTwist = Random.Range(-angleDeviation, angleDeviation);
            if (Physics.Raycast(origin, direction, out RaycastHit hit, stepDistance, _layerMask.value))
            {
                newNormal = hit.normal;
                newDirection = Geometry_Utility.DirectionFromHitPoint(normal, newNormal, originalDirection.normalized);
                newDirection = Geometry_Utility.RotateDirection(newDirection, newNormal, angleTwist);
                anchorParent = hit.transform;
                return hit.point;
            }
            else
            {
                origin = origin + stepDistance * direction;
                direction = -normal;
                if (Physics.Raycast(origin, direction, out hit, stepDistance, _layerMask.value))
                {
                    newNormal = hit.normal;
                    newDirection = Geometry_Utility.DirectionFromHitPoint(normal, newNormal, originalDirection.normalized);
                    newDirection = Geometry_Utility.RotateDirection(newDirection, newNormal, angleTwist);
                    anchorParent = hit.transform;
                    return hit.point;
                }
                else
                {
                    origin = origin - stepDistance * normal;
                    direction = -originalDirection.normalized;
                    if (Physics.Raycast(origin, direction, out hit, stepDistance, _layerMask.value))
                    {
                        newNormal = hit.normal;
                        newDirection = Geometry_Utility.DirectionFromHitPoint(normal, newNormal, originalDirection.normalized);
                        newDirection = Geometry_Utility.RotateDirection(newDirection, newNormal, angleTwist);
                        anchorParent = hit.transform;
                        return hit.point;
                    }
                }
            }
            if (iterationCount >= MAXIMUM_RECURSION_COUNT)
            {
                Debug.LogWarning("Couldn't find an anchor point, aborting. Make sure you are using the right physics layer in the object spawner.");
                newNormal = Vector3.up;
                newDirection = Vector3.forward;
                anchorParent = null;
                return -1000 * Vector3.up;
            }
            // Alternates between smaller and bigger step distance to stay as close as possible to the preffered step distance
            float newStepDistance = (iterationCount % 2 == 0) ? stepDistance * Mathf.Pow(2, iterationCount + 1) : stepDistance / Mathf.Pow(2, iterationCount + 1);
            //Recursively look for a new point
            return ReturnNextPoint(initialPoint, normal, originalDirection, angleDeviation, newStepDistance, out newNormal, out newDirection, out anchorParent, ++iterationCount);
        }

    }
}
