using UnityEngine;

namespace PPP
{
    public class XSinusMovement : MonoBehaviour
    {
        [SerializeField]
        private float _frequency;
        [SerializeField]
        private float _amplitude;
        [SerializeField]
        private float _phase;

        void Update()
        {
            Vector3 newPosition = transform.position;
            newPosition.x = _amplitude * Mathf.Sin(_frequency * Time.time + _phase) ;
            transform.position = newPosition;
        }
    }
}

