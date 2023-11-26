using UnityEngine;
namespace PPP
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotateSpeed = 200f;
        public float zoomSpeed = 10f;

        private Vector3 previousPosition;

        void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                previousPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                Vector3 direction = previousPosition - Input.mousePosition;
                Vector3 move = new Vector3(direction.x, 0, direction.y) * moveSpeed * Time.deltaTime;
                Camera.main.transform.Translate(move, Space.Self);
                previousPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonDown(1))
            {
                previousPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 rotation = new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0) * rotateSpeed * Time.deltaTime;
                Camera.main.transform.eulerAngles -= rotation;
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.transform.position += Camera.main.transform.forward * scroll * zoomSpeed;
        }
    }
}