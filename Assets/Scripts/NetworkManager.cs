using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject spawnArea;

    [Space]
    [SerializeField] private string playerPath;

    [Header("Spawn area")]
    [SerializeField] private float maxSpawnDistance;

    [Header("network")]
    [SerializeField] private string roomName;

    public GameObject SpawnArea { get => spawnArea; set => spawnArea = value; }
    public string PlayerPath { get => playerPath; set => playerPath = value; }
    public string RoomName { get => roomName; set => roomName = value; }
    public float MaxSpawnDistance { get => maxSpawnDistance; set => maxSpawnDistance = value; }

    private static NetworkManager instance;
    public static NetworkManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if (instance)
        {
            return;
        }

        instance = this;

        PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        EnableMouseCursor(true);
    }

    public override void OnConnectedToMaster()
    {
        JoinRoom(RoomName);
    }

    public override void OnJoinedRoom()
    {
        DisableSpawnArea();
        SpawnPlayer();
        //EnableMouseCursor(false);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        CreateRoom(RoomName);
    }

    public void CreateRoom(string name)
    {
        PhotonNetwork.CreateRoom(name);
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    private void DisableSpawnArea()
    {
        SpawnArea.SetActive(false);
    }

    private void SpawnPlayer()
    {
        var _randomX = UnityEngine.Random.Range(-1f, 1f);
        var _randomz = UnityEngine.Random.Range(-1f, 1f);
        var _spawnPos = SpawnArea.transform.position + (new Vector3(_randomX, 0, _randomz).normalized * MaxSpawnDistance);

        PhotonNetwork.Instantiate(PlayerPath, _spawnPos, Quaternion.identity);
    }

    private void EnableMouseCursor(bool enabled)
    {
        Cursor.visible = enabled;
        Cursor.lockState = !enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
