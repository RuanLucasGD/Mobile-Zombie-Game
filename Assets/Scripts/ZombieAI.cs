using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class ZombieAI : MonoBehaviourPunCallbacks
{
    [Header("Animation")]
    [SerializeField] private string attackAnimParam;
    [SerializeField] private string walkingAnimParam;

    [Header("Attack")]
    [SerializeField] private float attackDistance;
    [SerializeField] private float fieldOfView;

    private GameManager gameManager;
    private NavMeshAgent agent;
    private Animator animator;
    private Transform target;

    private bool isWalking;
    private bool isAttacking;
    private bool isSeeingTarget;

    public string AttackAnimParam => attackAnimParam;
    public string WalkingAnimParam => walkingAnimParam;

    public NavMeshAgent NavMeshAgent => agent;
    public GameManager GameManager => gameManager;
    public Animator Animator => animator;

    public float FieldOfView { get => fieldOfView; set => fieldOfView = value; }
    public float AttackDistance { get => attackDistance; set => attackDistance = value; }

    public const float GET_NEAR_TARGET_INTERVAL = 1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponentInChildren<Animator>();

        StartCoroutine(GetTargetCoroutine(GET_NEAR_TARGET_INTERVAL));
    }

    void FixedUpdate()
    {
        if (!target || !NavMeshAgent)
        {
            return;
        }

        photonView.RPC(nameof(IsWalking), RpcTarget.AllBuffered, target.position);
        photonView.RPC(nameof(TargetOnView), RpcTarget.AllBuffered, target.position);
        photonView.RPC(nameof(IsAttacking), RpcTarget.AllBuffered, isWalking, isSeeingTarget);

        SetAgentDestination(target.position);

        NavMeshAgent.isStopped = isAttacking;

        // esta parado proximo ao alvo mas n esta o vendo
        if (!isWalking && !isSeeingTarget)
        {
            var _lookAt = Quaternion.LookRotation(target.position - transform.position);
            _lookAt.eulerAngles = Vector3.up * _lookAt.eulerAngles.y;

            transform.rotation = _lookAt;
        }

        if (Animator)
        {
            ControlAnimations(isWalking, isAttacking);
        }
    }

    private IEnumerator GetTargetCoroutine(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (photonView.IsMine)
        {
            if (GameManager.Instance.GameStarted && GameManager.Instance.PlayersOnGame.Length > 0)
            {
                photonView.RPC(nameof(GetNearPlayer), RpcTarget.AllBuffered);
            }
        }

        StartCoroutine(GetTargetCoroutine(interval));
    }

    [PunRPC]
    private void GetNearPlayer()
    {
        var _nearTarget = GameManager.Instance.PlayersOnGame[0].transform;
        var _selfPos = transform.position;

        foreach (var p in GameManager.Instance.PlayersOnGame)
        {
            if (Vector3.Distance(_selfPos, p.transform.position) < Vector3.Distance(_selfPos, _nearTarget.position))
            {
                _nearTarget = p.transform;
            }
        }

        target = _nearTarget;
    }

    [PunRPC]
    private void SetAgentDestination(Vector3 destination)
    {
        NavMeshAgent.destination = destination;
    }

    private void ControlAnimations(bool isWalking, bool isAttacking)
    {
        Animator.SetBool(WalkingAnimParam, isWalking);
        Animator.SetBool(AttackAnimParam, isAttacking);
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Retorna verdadeiro se estiver se movendo até um alvo e falso se estiver proximo do alvo.
    /// </returns>
    [PunRPC]
    private void IsWalking(Vector3 targetPosition)
    {
        isWalking = Vector3.Distance(transform.position, targetPosition) > AttackDistance;
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Retorna verdadeiro se estiver proximo do alvo, se estiver o vendo e tambem estiver parado.
    /// </returns>
    [PunRPC]
    private void IsAttacking(bool isWalking, bool targetOnView)
    {
        isAttacking = !isWalking && targetOnView;
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Retorna verdadeiro se estiver vendo o target.
    /// </returns>
    [PunRPC]
    private void TargetOnView(Vector3 targetPosition)
    {
        isSeeingTarget = (Vector3.Angle(transform.forward, (targetPosition - transform.position).normalized) < FieldOfView);
    }
}