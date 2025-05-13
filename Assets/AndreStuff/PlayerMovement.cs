using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float maxLookDownAngle = 80f;
    private float _camSensitivity = 2f;

    private Camera _cam;
    private Rigidbody _rb;
    private PlayerControl _playerControl;

    public void UpdateSensitivity(float value) => _camSensitivity = value;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerControl = GetComponent<PlayerControl>();
        _playerControl.SetupPlayerMovement(this);
    }

    private void Start()
    {
        _cam = Camera.main;
    }
    
    private void Update()
    {
        if (!_playerControl.GetGameManager().CanPlayerMove())
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        HandleCamera();
        HandleMovement();
    }

    private float _smoothTime = 0.002f;
    private float _rotationX, _rotationY;
    private float _currentRotationX, _currentRotationY;
    private float _velocityX, _velocityY;
    private void HandleCamera()
    {
        
        _rotationX += Input.GetAxis("Mouse X") * _camSensitivity;
        _rotationY += Input.GetAxis("Mouse Y") * _camSensitivity;
        
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);
        
        _currentRotationX = Mathf.SmoothDamp(_currentRotationX, _rotationX, ref _velocityX, _smoothTime);
        _currentRotationY = Mathf.SmoothDamp(_currentRotationY, _rotationY, ref _velocityY, _smoothTime);
        
        _cam.transform.localRotation = Quaternion.Euler(-_currentRotationY, _currentRotationX, 0);
    }
    
    private void HandleMovement()
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
