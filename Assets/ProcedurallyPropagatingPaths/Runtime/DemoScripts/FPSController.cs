using UnityEngine;


namespace PPP
{
    [RequireComponent(typeof(CharacterController))]

    public class FPSController : MonoBehaviour
    {
        public float mouseSensitivity = 2f;
        public float speed = 5f;
        public float angleLimit = 30f;
        public Camera mainCamera;
        public Canon canon;

        CharacterController characterController;
        Vector3 moveDirection = Vector3.zero;
        float rotationXAxis = 0;

        void Awake()
        {
            characterController = GetComponent<CharacterController>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float verticalInput = speed * Input.GetAxis("Vertical");
            float horizontalInput = speed * Input.GetAxis("Horizontal");
            moveDirection = (forward * verticalInput) + (right * horizontalInput);
            characterController.Move(moveDirection * Time.deltaTime);
            rotationXAxis += -Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationXAxis = Mathf.Clamp(rotationXAxis, -angleLimit, angleLimit);
            mainCamera.transform.localRotation = Quaternion.Euler(rotationXAxis, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
            if (Input.GetMouseButtonDown(0))
            {
                canon.Fire();
            }
        }
    }
}
