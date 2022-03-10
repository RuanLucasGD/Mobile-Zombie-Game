using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private string verticalAxis;
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string mouseVerticalAxis;
    [SerializeField] private string mouseHorizontalAxis;

    [Header("Movimentation")]
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float gravity;

    [Header("Camera Control")]
    [SerializeField] private float maxCameraVertical;
    [SerializeField] private float mouseSensitive;
    [SerializeField] private Camera playerCamera;


    private float currentMouseEulerX;

    private Animator _animator;
    private SkinnedMeshRenderer _meshRenderer;
    private CharacterController _characterController;

    public string VerticalAxis { get => verticalAxis; set => verticalAxis = value; }
    public string HorizontalAxis { get => horizontalAxis; set => horizontalAxis = value; }
    public string MouseVerticalAxis { get => mouseVerticalAxis; set => mouseVerticalAxis = value; }
    public string MouseHorizontalAxis { get => mouseHorizontalAxis; set => mouseHorizontalAxis = value; }
    public float PlayerMoveSpeed { get => playerMoveSpeed; set => playerMoveSpeed = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public float MaxCameraVertical { get => maxCameraVertical; set => maxCameraVertical = value; }
    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public float MouseSensitive { get => mouseSensitive; set => mouseSensitive = value; }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _characterController = GetComponent<CharacterController>();

        if (_meshRenderer)
        {
            _meshRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_characterController)
        {
            return;
        }

        ControlCharacter();
    }

    private void ControlCharacter()
    {
        var _horizontal = Input.GetAxis(HorizontalAxis);
        var _vertical = Input.GetAxis(VerticalAxis);
        var _mouseHorizontal = Input.GetAxis(MouseHorizontalAxis);
        var _mouseVertical = Input.GetAxis(MouseVerticalAxis);

        var _inputDirection = ((transform.forward * _vertical) + (transform.right * _horizontal)).normalized;
        var _moveDirection = _inputDirection * PlayerMoveSpeed;

        _moveDirection += Vector3.down * Gravity;
        _moveDirection *= Time.fixedDeltaTime;

        currentMouseEulerX -= _mouseVertical * MouseSensitive;
        currentMouseEulerX = Mathf.Clamp(currentMouseEulerX, -MaxCameraVertical, MaxCameraVertical);
        PlayerCamera.transform.localEulerAngles = Vector3.right * currentMouseEulerX;

        _characterController.Move(_moveDirection);
        transform.Rotate(Vector3.up * _mouseHorizontal * MouseSensitive);
    }
}
