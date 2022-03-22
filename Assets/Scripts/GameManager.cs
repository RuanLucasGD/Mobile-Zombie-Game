using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private EnemiesSpawn enemiesSpawn;
    [SerializeField] private Transform playerSpawn;


    [SerializeField] private string playerPath;
    [SerializeField] private UnityEvent onPlayerFinded;


    [Header("Player spawn area")]
    [SerializeField] private float maxSpawnDistance;

    private PlayerController[] playersOnGame;

    private PlayerController player;
    private bool gameStarted;

    public bool GameStarted => gameStarted;
    public PlayerController Player => player;
    public EnemiesSpawn EnemiesSpawn => enemiesSpawn;
    public UnityEvent OnPlayerFinded { get => onPlayerFinded; set => onPlayerFinded = value; }
    public float MaxSpawnDistance { get => maxSpawnDistance; set => maxSpawnDistance = value; }
    public string PlayerPath { get => playerPath; set => playerPath = value; }
    public PlayerController[] PlayersOnGame => playersOnGame;

    private static GameManager gameManager;
    public static GameManager Instance { get => gameManager; private set => gameManager = value; }
    public Transform PlayerSpawn { get => playerSpawn; set => playerSpawn = value; }

    private void Awake()
    {
        enemiesSpawn.enabled = false;
        gameManager = this;
    }

    private void Start()
    {
        playersOnGame = new PlayerController[0];
        StartCoroutine(nameof(StartGame), 2);
    }

    void Update()
    {
        if (!Player)
        {
            FindPlayer();

            if (Player)
            {
                OnPlayerFinded.Invoke();
                enemiesSpawn.enabled = true;
                enemiesSpawn.SpawnEnemey();
            }
        }
    }

    public IEnumerator StartGame(float time)
    {
        yield return new WaitForSeconds(time);

        SpawnPlayer();
        DisableSpawnArea();
        DisablePlayerSpawn();

        photonView.RPC(nameof(UpdatePlayersList), RpcTarget.AllBuffered);
        gameStarted = true;
    }

    [PunRPC]
    private void UpdatePlayersList()
    {
        playersOnGame = FindObjectsOfType<PlayerController>();
    }

    [PunRPC]
    private void SpawnPlayer()
    {
        var _randomX = UnityEngine.Random.Range(-1f, 1f);
        var _randomz = UnityEngine.Random.Range(-1f, 1f);
        var _spawnPos = PlayerSpawn.transform.position + (new Vector3(_randomX, 0, _randomz).normalized * MaxSpawnDistance);

        PhotonNetwork.Instantiate(PlayerPath, _spawnPos, Quaternion.identity);
    }

    private void DisableSpawnArea()
    {
        EnemiesSpawn.gameObject.SetActive(false);
    }

    private void DisablePlayerSpawn()
    {
        PlayerSpawn.gameObject.SetActive(false);
    }

    private void FindPlayer()
    {
        var _players = FindObjectsOfType<PlayerController>();

        foreach (var p in _players)
        {
            if (p.photonView.IsMine) player = p;
        }
    }
}
