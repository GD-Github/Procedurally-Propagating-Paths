using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public class DynamicObjectSpawner : ObjectSpawner
    {
        // An example class to show how instantiate objects that move along the paths

        [Tooltip("The distance between each anchor point of the path, can differ from it if it is impossible to find an anchor point in that range")]
        [Min(0.01f)]
        public float preferedStepDistance = 0.1f;
        [Min(0)]
        [Tooltip("The time in seconds before the object gets destroyed after they spawned")]
        public float objectLifetime = 5;
        [Min(0)]
        [Tooltip("The speed (arbitrary unit) at which the objects travel along the paths")]
        public float objectSpeed = 1f;
        [Min(0)]
        [Tooltip("The time taken for the object to scale down to 0 before getting destroyed")]
        public float destroyDuration = 0.3f;


        private IEnumerator InstantiateObjectsRoutine()
        {
            List<float> randomDelays;
            _objectPaths = _arePathsSorted ? new(_pathManager.Paths) : ShuffledObjectPaths();
            randomDelays = GenerateRandomDelays(_numberOfPaths);
            for (int i = 0; i < _numberOfPaths; i++)
            {
                ObjectPath path = _objectPaths[i];
                if (path.AnchorPoints.Count == 0) continue;
                MovingObject @object = _spawnables.SpawnObject().AddComponent<MovingObject>();
                @object.Initialize(objectLifetime, destroyDuration, path, objectSpeed);
                @object.transform.position = path.AnchorPoints[0].GetPosition();
                yield return new WaitForSeconds(randomDelays[i]);
            }
            yield break;
        }

        protected void InstantiateObjects()
        {
            StartCoroutine(InstantiateObjectsRoutine());
        }


        protected void InstantiateObjectsAndSelfDestroy()
        {
            InstantiateObjects();
            StartDestroy();
        }

        public void StartDestroy()
        {
            StartCoroutine(StartDestroyRoutine());
        }

        private IEnumerator StartDestroyRoutine()
        {
            //Destroys self after the last object disappeared
            yield return new WaitForSeconds(_randomSpawnDelay + objectLifetime);
            Destroy(gameObject);
        }
    }
}