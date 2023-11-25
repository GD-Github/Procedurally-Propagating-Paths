using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public class StaticObjectSpawner : ObjectSpawner
    {
        // An example class to show how to use the paths to instantiate static objects in the scene
        [Tooltip("The amount of objects that will be spread among all the paths")]
        public uint ObjectsCountToSpawn = 10;
        [Min(0.01f)]
        [Tooltip("The distance between each step will be, if possible, between the min and max step distance." +
            " It may go out of bounds if it is impossible to find an anchor point within this range")]
        public float MinPreferedStepDistance = 0.5f;
        [Min(0.01f)]
        [Tooltip("The distance between each step will be, if possible, between the min and max step distance." +
            " It may go out of bounds if it is impossible to find an anchor point within this range")]
        public float MaxPreferedStepDistance = 1;
        [Tooltip("If true, will add a random rotation to the spawned object along the up axis")]
        public bool ShouldRandomizeRotation = false;

        [Tooltip("If true, the spawned objects will be a child of the object where the anchor point was found")]
        public bool ParentSpawnedObject = true;
        [Tooltip("Use this to give an offset along the normal to the spawned objects. Note that there is already a constant offset that is setup in the AnchorPoint.cs script")]
        public float NormalShift;
        [Header("Filtering anchor points")]
        [Tooltip("Can be used" +
            " to make object spawn on a specific sides of objects")]
        public Transform NormalAxisForSpawnedObjects;
        [Range(-1, 1)]
        [Tooltip("The closer to 1, the more the anchor points' normal has to be close to the NormalAxisForSpawnedObjects for the object to be spawned")]
        public float ScalarProductTolerance = -1;
        private void OnValidate()
        {
            if (MinPreferedStepDistance > MaxPreferedStepDistance) MaxPreferedStepDistance = MinPreferedStepDistance;
        }

        private IEnumerator InstantiateObjectsRoutine()
        {
            _objectPaths = _arePathsSorted ? new(_pathManager.Paths) : ShuffledObjectPaths();
            List<float> randomDelays = GenerateRandomDelays(ObjectsCountToSpawn);
            int objectSpawned = 0;
            int pathIndex = 0;
            while (objectSpawned < ObjectsCountToSpawn)
            {
                foreach (var path in _objectPaths)
                {
                    if (objectSpawned >= ObjectsCountToSpawn) break;
                    AnchorPoint anchorPoint = path.GetAnchorPoint(pathIndex);
                    if (NormalAxisForSpawnedObjects && (Vector3.Dot(anchorPoint.GetNormal(), NormalAxisForSpawnedObjects.up) < ScalarProductTolerance)) continue;
                    GameObject @object = _spawnables.SpawnObject();
                    @object.transform.up = anchorPoint.GetNormal();
                    @object.transform.position = anchorPoint.GetPosition() + NormalShift * @object.transform.up;

                    if (ShouldRandomizeRotation)
                    {
                        @object.transform.Rotate(@object.transform.up, Random.Range(0, 360), Space.World);
                    }
                    if (ParentSpawnedObject)
                    {
                        @object.transform.SetParent(anchorPoint.Parent);
                    }

                    yield return new WaitForSeconds(randomDelays[objectSpawned]);
                    objectSpawned++;
                    if (pathIndex == 0) break;
                }
                pathIndex++;
            }
            yield break;
        }
        protected void InstantiateObjects()
        {
            StartCoroutine(InstantiateObjectsRoutine());
        }
    }
}