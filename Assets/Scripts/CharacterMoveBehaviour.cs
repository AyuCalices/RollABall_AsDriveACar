using System;
using JetBrains.Annotations;
using UnityEngine;

public class CharacterMoveBehaviour : MonoBehaviour
{
    public static Action onDeath;
    
    [SerializeField] private CharacterController controller;

    [SerializeField] private float deathHeight;
    
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityValue = -9.81f;
    
    [Header("Speed")]
    [SerializeField] private float playerBaseSpeedForward = 2f;
    [SerializeField][Range(0, 1)] private float speedForwardInputMultiplier = 0.8f;
    [SerializeField] private float playerBaseSpeedSide = 4f;
    [SerializeField][Range(0, 10)] private float speedIncreaseOverTime = 0.5f;
    
    [UsedImplicitly] private Vector3 _currentVelocity;
    private float _playerVerticalVelocity;
    private bool _groundedPlayer;
    private float _playerSpeedForward;
    private Vector3 _originPosition;
    private bool _reset;
    
    public float Kmh { get; private set; }
    private int _highestSpeed;

    private void Awake()
    {
        _originPosition = transform.position;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _reset = false;
        transform.position = _originPosition;
        _playerSpeedForward = playerBaseSpeedForward;
        _playerVerticalVelocity = 0f;
    }

    //basis from: https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
    public void Update()
    {
        _groundedPlayer = controller.isGrounded;
        _currentVelocity = controller.velocity;

        _playerSpeedForward += Time.deltaTime * speedIncreaseOverTime;
        
        if (_groundedPlayer && _playerVerticalVelocity < 0)
        {
            _playerVerticalVelocity = -1f;
        }

        float verticalMultiplierByInput = (Input.GetAxis("Vertical") * speedForwardInputMultiplier) + 1;
        float forwardSpeed = verticalMultiplierByInput * Time.deltaTime * _playerSpeedForward;
        float sideSpeed = Input.GetAxis("Horizontal") * Time.deltaTime * playerBaseSpeedSide;
        
        if (Input.GetKeyDown(KeyCode.Space) && _groundedPlayer)
        {
            _playerVerticalVelocity += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
        _playerVerticalVelocity += gravityValue * Time.deltaTime;
        float verticalSpeed = _playerVerticalVelocity * Time.deltaTime;
        
        controller.Move(new Vector3(sideSpeed, verticalSpeed, forwardSpeed));
        
        Vector3 rotationDirection = new Vector3(sideSpeed, 0, forwardSpeed);
        if (rotationDirection != Vector3.zero)
        {
            gameObject.transform.forward = rotationDirection;
        }

        Kmh = _currentVelocity.z / 1000 * 60 * 60;
        if (Kmh > _highestSpeed)
        {
            _highestSpeed = (int)Kmh;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _reset = true;
    }

    private void LateUpdate()
    {
        if (!_reset && !(transform.position.y < deathHeight)) return;
        
        int currentPoints = PlayerPrefs.GetInt("Points");
        if (currentPoints < transform.position.z)
        {
            PlayerPrefs.SetInt("Points", (int) transform.position.z);
        }
        int currentSpeed = PlayerPrefs.GetInt("Kmh");
        if (currentSpeed < _highestSpeed)
        {
            PlayerPrefs.SetInt("Kmh", _highestSpeed);
        }

        Initialize();
        onDeath?.Invoke();
    }
}
