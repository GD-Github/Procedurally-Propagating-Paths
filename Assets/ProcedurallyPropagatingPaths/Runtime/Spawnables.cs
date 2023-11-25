using System.Collections.Generic;
using UnityEngine;

namespace PPP
{
    [System.Serializable]
    public class Spawnable
    {
        public const float MIN_PROBA_VALUE = 0.001f;
        public GameObject ObjectToSpawn;

        [Range(MIN_PROBA_VALUE, 1)]
        public float SpawnProbability;

        [Tooltip("Locks the probability to the current value")]
        public bool Locked;
        public void SetSpawnProbability(float f, float maxValue)
        {
            f = Mathf.Clamp(f, MIN_PROBA_VALUE, maxValue);
            SpawnProbability = Locked ? SpawnProbability : f;
        }
    }

    public class Spawnables : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of objects that can be spawned with their associated probabilities")]
        private List<Spawnable> _spawnableList = new();
        private void OnValidate()
        {
            //This code isn't perfect and can be broken, but pretty much tries to hold the sum of the probabilities to 1 in the inspector
            if (_spawnableList.Count == 1)
            {
                _spawnableList[0].SetSpawnProbability(1, 1);
                return;
            }
            float totalLockedWeight = 0;
            float totalUnlockedWeight = 0;
            int lockedCount = 0;
            float maxValue = 1 - Spawnable.MIN_PROBA_VALUE * (_spawnableList.Count - 1);
            foreach (var spawnable in _spawnableList)
            {
                if (spawnable.Locked)
                {
                    totalLockedWeight += spawnable.SpawnProbability;
                    lockedCount++;
                }
                else
                {
                    totalUnlockedWeight += spawnable.SpawnProbability;
                }

            }
            float f = totalUnlockedWeight / (totalLockedWeight + totalUnlockedWeight);
            if (totalUnlockedWeight == 0) return;

            foreach (var spawnable in _spawnableList)
            {
                if (!spawnable.Locked)
                {
                    if (lockedCount == _spawnableList.Count - 1)
                    {
                        spawnable.SetSpawnProbability(1 - totalLockedWeight, maxValue);
                        continue;
                    }
                    spawnable.SetSpawnProbability(f * spawnable.SpawnProbability / totalUnlockedWeight, Mathf.Min(1 - totalLockedWeight - (Spawnable.MIN_PROBA_VALUE * (_spawnableList.Count - lockedCount - 1)), maxValue));

                }
                else
                {
                    spawnable.SetSpawnProbability(spawnable.SpawnProbability, Mathf.Min(1 - totalUnlockedWeight - (Spawnable.MIN_PROBA_VALUE * (_spawnableList.Count - lockedCount - 1)), maxValue));
                }
            }
        }

        public GameObject SpawnObject()
        {
            //Spawns one of the spawnable object according to the probabilities
            float totalWeight = 0;
            foreach (Spawnable item in _spawnableList)
            {
                totalWeight += item.SpawnProbability;
            }
            float rand = Random.Range(0f, totalWeight);
            float tempSum = 0;
            GameObject objectToSpawn = null;
            foreach (Spawnable spawnable in _spawnableList)
            {
                tempSum += spawnable.SpawnProbability;
                if (rand <= tempSum)
                {
                    objectToSpawn = spawnable.ObjectToSpawn;
                    break;
                }
            }
            return Instantiate(objectToSpawn);
        }
    }
}
