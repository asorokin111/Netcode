using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Base variables")]
    public float speed = 7.5f;
    public float sprintSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 9.8f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float _cameraYOffset;
    [SerializeField]
    private Camera _playerCam;
    [SerializeField]
    private CharacterController _characterController;

    private Vector3 _moveDir = Vector3.zero;
    private float _rotationX = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            _playerCam = Camera.main;
            _playerCam.transform.position = new Vector3(transform.position.x,
                transform.position.y + _cameraYOffset,
                transform.position.z);
            _playerCam.transform.SetParent(transform);
        }
        else
        {
            enabled = false;
        }
    }

    private void Start()
    {
        _characterController ??= GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? sprintSpeed : speed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? sprintSpeed : speed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDir.y;
        _moveDir = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            _moveDir.y = jumpSpeed;
        }
        else
        {
            _moveDir.y = movementDirectionY;
        }

        if (!_characterController.isGrounded)
        {
            _moveDir.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        _characterController.Move(_moveDir * Time.deltaTime); // TODO: fix this throwing warnings on init by calling on an inactive controller

        // Player and Camera rotation
        if (canMove && _playerCam != null)
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            _playerCam.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
