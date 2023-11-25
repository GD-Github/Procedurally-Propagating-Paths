using UnityEngine;

namespace PPP
{
    public class Canon : MonoBehaviour
    {
        private GameObject _objectSpawnerPrefab;
        [SerializeField]
        private GameObject _objectSpawnerPrefab1;
        [SerializeField]
        private GameObject _objectSpawnerPrefab2;
        [SerializeField]
        private GameObject _objectSpawnerPrefab3;
        [SerializeField]
        private float _power = 50;

        private void Awake()
        {
            _objectSpawnerPrefab = _objectSpawnerPrefab1;
        }
        public void Fire()
        {
            GameObject bullet = Instantiate(_objectSpawnerPrefab);
            bullet.transform.position = transform.position;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(_power * -1f * transform.right, ForceMode.Impulse);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _objectSpawnerPrefab = _objectSpawnerPrefab1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _objectSpawnerPrefab = _objectSpawnerPrefab2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _objectSpawnerPrefab = _objectSpawnerPrefab3;
            }
        }
    }
}

