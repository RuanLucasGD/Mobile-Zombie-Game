using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public enum TransformAxis
    {
        X,
        Y,
        Z,
        X_NEGATIVE,
        Y_NEGATIVE,
        Z_NEGATIVE
    }

    [Header("Inputs")]
    [SerializeField] private string verticalAxis;
    [SerializeField] private string horizontalAxis;
    [SerializeField] private string mouseVerticalAxis;
    [SerializeField] private string mouseHorizontalAxis;

    [Header("Movimentation")]
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private bool disableDefaultControl;

    [Header("Camera Control")]
    [SerializeField] private float maxCameraVertical;
    [SerializeField] private float mouseSensitive;
    [SerializeField] private Camera playerCamera;

    [Header("Weapon")]
    [SerializeField] private GameObject weaponSystem;
    [SerializeField] private GameObject weaponMesh;

    [Header("Animations")]
    [SerializeField] private string runAnimationParameter;

    [Header("Armature Moviment")]
    public Transform rootBone;
    public Transform upperBodyBone;
    public TransformAxis bodyRight;

    [Header("Aim")]
    public float aimDistance;
    public LayerMask aimLayer;

    private float currentMouseEulerX;

    private float verticalInput;
    private float horizontalInput;
    private float mouseHorizontalInput;
    private float mouseVerticalInput;

    private Vector3 playerMoveDirection;
    private Vector3 aimPosition;

    private Animator animator;
    private SkinnedMeshRenderer meshRenderer;
    private CharacterController characterController;

    private Quaternion currentBodyRotation;
    private Quaternion initialRootBoneRotation;
    private Quaternion initialUpperBodyBoneRotation;

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
    public GameObject WeaponMesh { get => weaponMesh; set => weaponMesh = value; }
    public bool DisableDefaultControl { get => disableDefaultControl; set => disableDefaultControl = value; }
    public float VerticalInput { get => verticalInput; set => verticalInput = value; }
    public float HorizontalInput { get => horizontalInput; set => horizontalInput = value; }
    public float MouseHorizontalInput { get => mouseHorizontalInput; set => mouseHorizontalInput = value; }
    public float MouseVerticalInput { get => mouseVerticalInput; set => mouseVerticalInput = value; }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        characterController = GetComponent<CharacterController>();

        if (rootBone && upperBodyBone)
        {
            initialRootBoneRotation = rootBone.localRotation;
            initialUpperBodyBoneRotation = upperBodyBone.localRotation;
        }

        if (photonView.IsMine)
        {
            var _audioListener = GetComponentInChildren<AudioListener>();

            if (_audioListener) _audioListener.enabled = false;
            if (meshRenderer) meshRenderer.enabled = false;
            if (weaponMesh) Destroy(WeaponMesh);
        }
        else
        {
            Destroy(weaponSystem);
            playerCamera.enabled = false;
        }
    }

    private void Update()
    {
        if (!DisableDefaultControl)
        {
            HorizontalInput = Input.GetAxis(HorizontalAxis);
            VerticalInput = Input.GetAxis(VerticalAxis);
            MouseHorizontalInput = Input.GetAxis(MouseHorizontalAxis) * mouseSensitive;
            MouseVerticalInput = Input.GetAxis(MouseVerticalAxis) * mouseSensitive;
        }
    }

    private void LateUpdate()
    {
        upperBodyBone.localRotation = initialUpperBodyBoneRotation;

        AnimArmatureLookToTarget(aimPosition);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!characterController)
        {
            return;
        }

        aimPosition = GetAimPosition(PlayerCamera, aimDistance, aimLayer);

        if (photonView.IsMine)
        {
            var _input = Mathf.Clamp(Mathf.Abs(VerticalInput) + Mathf.Abs(HorizontalInput), 0, 1);
            var _relativeMoveDir = transform.InverseTransformDirection(playerMoveDirection);

            if (_relativeMoveDir.z < 0)
            {
                _input *= -1;
            }

            ControlCharacter();

            ControlMovimentAnimation(runAnimationParameter, _input);
        }
    }

    private void ControlCharacter()
    {
        var _inputDirection = ((transform.forward * VerticalInput) + (transform.right * HorizontalInput)).normalized;
        playerMoveDirection = _inputDirection * PlayerMoveSpeed;

        playerMoveDirection *= Time.fixedDeltaTime;

        characterController.Move(playerMoveDirection + Vector3.down * Gravity);
        transform.Rotate(Vector3.up * MouseHorizontalInput);

        if (PlayerCamera)
        {
            currentMouseEulerX -= MouseVerticalInput;
            currentMouseEulerX = Mathf.Clamp(currentMouseEulerX, -MaxCameraVertical, MaxCameraVertical);
            PlayerCamera.transform.localEulerAngles = Vector3.right * currentMouseEulerX;
        }
    }

    private void AnimArmatureLookToTarget(Vector3 aimPosition)
    {
        upperBodyBone.localRotation = initialUpperBodyBoneRotation;

        var _upBody = GetAxisDirection(bodyRight);
        var _angleToTarget = Vector3.Angle(upperBodyBone.forward, (aimPosition - upperBodyBone.position));
        var _rootRot = rootBone.rotation;

        if (aimPosition.y > upperBodyBone.position.y)
        {
            _angleToTarget = 360 - _angleToTarget;
        }

        upperBodyBone.rotation = Quaternion.RotateTowards(upperBodyBone.rotation, _rootRot, 0) * Quaternion.Euler(_upBody * _angleToTarget);
    }

    private Vector3 GetAxisDirection(TransformAxis axis)
    {
        var _axis = Vector3.up;

        switch (axis)
        {
            case TransformAxis.X:
                _axis = Vector3.right;
                break;
            case TransformAxis.Y:
                _axis = Vector3.up;
                break;
            case TransformAxis.Z:
                _axis = Vector3.forward;
                break;
            case TransformAxis.X_NEGATIVE:
                _axis = Vector3.left;
                break;
            case TransformAxis.Y_NEGATIVE:
                _axis = Vector3.down;
                break;
            case TransformAxis.Z_NEGATIVE:
                _axis = Vector3.back;
                break;
            default:
                break;
        }

        return _axis;
    }

    private void ControlMovimentAnimation(string animationParameter, float input)
    {
        animator.SetFloat(animationParameter, input);
    }

    private Vector3 GetAimPosition(Camera playerCamera, float aimDistance, LayerMask aimCollisionLayer)
    {
        var aimPosition = playerCamera.transform.position + (playerCamera.transform.forward * aimDistance);

        if (Physics.Raycast(playerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, aimDistance, aimCollisionLayer))
        {
            aimPosition = hit.point;
        }

        return aimPosition;
    }
}
