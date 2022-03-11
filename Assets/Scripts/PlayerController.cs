using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
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

    [Header("Weapon")]
    [SerializeField] private GameObject weaponSystem;

    private float currentMouseEulerX;

    private Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    private CharacterController characterController;

    public string VerticalAxis { get => verticalAxis; set => verticalAxis = value; }
    public string HorizontalAxis { get => horizontalAxis; set => horizontalAxis = value; }
    public string MouseVerticalAxis { get => mouseVerticalAxis; set => mouseVerticalAxis = value; }
    public string MouseHorizontalAxis { get => mouseHorizontalAxis; set => mouseHorizontalAxis = value; }
    public float PlayerMoveSpeed { get => playerMoveSpeed; set => playerMoveSpeed = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public float MaxCameraVertical { get => maxCameraVertical; set => maxCameraVertical = value; }
    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public float MouseSensitive { get => mouseSensitive; set => mouseSensitive = value; }
    public GameObject WeaponSystem { get => weaponSystem; set => weaponSystem = value; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        characterController = GetComponent<CharacterController>();

        if (photonView.IsMine)
        {
            if (meshRenderer)
            {
                meshRenderer.enabled = false;
            }
        }
        else
        {
            Destroy(weaponSystem);
            Destroy(playerCamera.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!characterController)
        {
            return;
        }

        if (photonView.IsMine)
        {
            ControlCharacter();
        }
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

        characterController.Move(_moveDirection);
        transform.Rotate(Vector3.up * _mouseHorizontal * MouseSensitive);

        if (PlayerCamera)
        {
            currentMouseEulerX -= _mouseVertical * MouseSensitive;
            currentMouseEulerX = Mathf.Clamp(currentMouseEulerX, -MaxCameraVertical, MaxCameraVertical);
            PlayerCamera.transform.localEulerAngles = Vector3.right * currentMouseEulerX;
        }
    }
}
