using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxLookUpAngle = 80f;
    [SerializeField] private float maxLookDownAngle = 80f;
    private float _camSensitivity = 2f;

    [Header("Footstep Settings")]
    [SerializeField] private float stepInterval = 0.5f;
    [SerializeField] private AudioClip[] footstepClips;         // Default footsteps
    [SerializeField] private AudioClip[] gravelFootstepClips;   // Gravel footsteps
    [SerializeField] private AudioSource audioSource;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private LayerMask groundLayer;

    private float stepTimer = 0f;

    private Camera _cam;
    private Rigidbody _rb;
    private PlayerControl _playerControl;

    private bool isOnGravel = false;

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

        if (audioSource == null)
            Debug.LogWarning("PlayerMovement: No AudioSource assigned for footsteps.");
    }

    private void Update()
    {
        HandleCamera(_playerControl.GetGameManager().CanPlayerMove());
        if (!_playerControl.GetGameManager().CanPlayerMove())
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        CheckGroundMaterial();
        HandleMovement();
    }

    private float _smoothTime = 0.002f;
    private float _rotationX, _rotationY;
    private float _currentRotationX, _currentRotationY;
    private float _velocityX, _velocityY;

    private void HandleCamera(bool acceptNewMovements = true)
    {
        if (acceptNewMovements) _rotationX += Input.GetAxis("Mouse X") * _camSensitivity;
        if (acceptNewMovements) _rotationY += Input.GetAxis("Mouse Y") * _camSensitivity;

        _rotationY = Mathf.Clamp(_rotationY, -maxLookUpAngle, maxLookDownAngle);

        _currentRotationX = Mathf.SmoothDamp(_currentRotationX, _rotationX, ref _velocityX, _smoothTime);
        _currentRotationY = Mathf.SmoothDamp(_currentRotationY, _rotationY, ref _velocityY, _smoothTime);

        _cam.transform.localRotation = Quaternion.Euler(-_currentRotationY, 0, 0);
        transform.localRotation = Quaternion.Euler(0, _currentRotationX, 0);
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput);
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f); // Normalize diagonal movement

        Vector3 moveDirection = Vector3.zero;

        if (inputDirection.sqrMagnitude > 0f)
        {
            Vector3 cameraForward = _cam.transform.forward;
            Vector3 cameraRight = _cam.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;
        }

        Vector3 newVelocity = new Vector3(moveDirection.x * moveSpeed, _rb.linearVelocity.y, moveDirection.z * moveSpeed);
        _rb.linearVelocity = newVelocity;

        Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        bool isActuallyMoving = horizontalVelocity.magnitude > 0.1f;

        if (isActuallyMoving)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstepSound();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval;
        }
    }

    private void PlayFootstepSound()
    {
        if (audioSource == null) return;

        AudioClip[] clipsToUse = isOnGravel && gravelFootstepClips.Length > 0
            ? gravelFootstepClips
            : footstepClips;

        if (clipsToUse == null || clipsToUse.Length == 0)
            return;

        AudioClip clip = clipsToUse[Random.Range(0, clipsToUse.Length)];

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clip);
    }

    private void CheckGroundMaterial()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            isOnGravel = hit.collider.CompareTag("Gravel");
        }
        else
        {
            isOnGravel = false;
        }
    }
}
