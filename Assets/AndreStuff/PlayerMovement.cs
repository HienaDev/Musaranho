using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float maxLookDownAngle = 80f;

    private Camera _cam;
    private Rigidbody _rb;
    private float cameraPitch = 0f;
    private PlayerControl _playerControl;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerControl = GetComponent<PlayerControl>();
    }

    void Start()
    {
        _cam = Camera.main;
        
        _rb.freezeRotation = true;
        _rb.useGravity = true;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    
    void Update()
    {
        if (!_playerControl.GetGameManager().CanPlayerMove())
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }
        HandleMouseLook();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;
        
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookUpAngle, maxLookDownAngle);
        
        _cam.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        
        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = _cam.transform.forward;
            Vector3 cameraRight = _cam.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            Vector3 desiredMoveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
            
            _rb.linearVelocity = new Vector3(desiredMoveDirection.x * moveSpeed, _rb.linearVelocity.y, desiredMoveDirection.z * moveSpeed);
        }
        else
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
        }
    }
}
