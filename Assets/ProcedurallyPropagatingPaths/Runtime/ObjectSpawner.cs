using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    public abstract class ObjectSpawner : MonoBehaviour
    {

        [Header("Path Finding Parameters")]
        [Tooltip("The layer on which the raycast will be performed to find the anchor points for the paths.")]
        [SerializeField]
        protected LayerMask _layerMask;
        [Tooltip("The number of paths that will be created from the initial point")]
        [SerializeField]
        protected uint _numberOfPaths = 10;
        [Range(0, 1)]
        [Tooltip("Closer to 0 means that the angle between the path will be initially evenly distributed, closer to 1 makes it more random")]
        [SerializeField]
        protected float _divergence;
        [Range(1, 360)]
        [Tooltip("Defines the portion of the circle that will be used to create the paths")]
        [SerializeField]
        protected float _angleRange = 360;
        [Range(0, 360)]
        [Tooltip("Defines the maximum angle between two steps of a path")]
        [SerializeField]
        protected float _iterationAngleDivergence = 10;
        [Tooltip("Defines if the paths should be used in the geometrical order or if they should be randomly shuffled")]
        [SerializeField]
        protected bool _arePathsSorted = true;

        [Space]
        [Header("Spawned Objects Parameters")]
        [SerializeField]
        [Tooltip("The objects that can be spawned and used along the paths")]
        protected Spawnables _spawnables;
        [Min(0)]
        [Tooltip("The objects will spawn between a delay of 0 and this value")]
        [SerializeField]
        protected float _randomSpawnDelay = 0.5f;

        protected PathManager _pathManager;
        protected List<ObjectPath> _objectPaths;
        protected List<Color> _colors;
        protected void DefineGizmoColors()
        {
            _colors = new();
            for (int i = 0; i < _pathManager.Paths.Count; i++)
            {
                _colors.Add(new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            }
        }

        private void OnDrawGizmos()
        {
            if (_objectPaths == null) return;
            for (int i = 0; i < _objectPaths.Count; i++)
            {
                ObjectPath path = _objectPaths[i];
                for (int j = 0; j < path.AnchorPoints.Count - 1; j++)
                {
                    Debug.DrawLine(path.AnchorPoints[j].GetPosition(), path.AnchorPoints[j + 1].GetPosition(), _colors[i]);
                }
            }
        }
        protected List<ObjectPath> ShuffledObjectPaths()
        {
            //Shuffles the paths in a random order
            List<ObjectPath> shuffledPaths = new();
            List<ObjectPath> pathsCopy = new(_pathManager.Paths);
            for (int i = 0; i < _numberOfPaths; i++)
            {
                int index = Random.Range(0, pathsCopy.Count - 1);
                ObjectPath path = pathsCopy[index];
                shuffledPaths.Add(path);
                pathsCopy.Remove(path);
            }
            return shuffledPaths;
        }
        protected List<float> GenerateRandomDelays(uint size)
        {
            //Returns a sorted list with delays contained between 0 and randomSpawnDelay 
            //The sum of the list's value is equal to randomSpawnDelay
            List<float> randomDelays = new();
            if (_randomSpawnDelay == 0)
            {
                for (int i = 0; i < size; i++)
                {
                    randomDelays.Add(0);
                }
                return randomDelays;
            }
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                float delay = Random.Range(0, _randomSpawnDelay);
                randomDelays.Add(delay);
                sum += delay;
            }
            float conversionValue = _randomSpawnDelay / sum;
            for (int i = 0; i < size; i++)
            {
                randomDelays[i] *= conversionValue;
            }
            randomDelays.Sort();
            return randomDelays;
        }
    }
}
