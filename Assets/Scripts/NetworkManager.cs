using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private bool isConnected;
    private bool isJoinedOnRoom;

    public delegate void ConnectionTest();

    private ConnectionTest onConnectedOnMaster;
    private ConnectionTest onJoinedOnRoom;
    private ConnectionTest onConnectionOnMasterFail;
    private ConnectionTest onJoinOnRoomFail;

    public bool IsConnected => isConnected;
    public bool IsJoinedOnRoom => isJoinedOnRoom;
    public bool IsMaster => PhotonNetwork.IsMasterClient;

    public ConnectionTest OnConnectedOnMaster { get => onConnectedOnMaster; set => onConnectedOnMaster = value; }
    public ConnectionTest OnJoinedOnRoom { get => onJoinedOnRoom; set => onJoinedOnRoom = value; }
    public ConnectionTest OnConnectionOnMasterFail { get => onConnectionOnMasterFail; set => onConnectionOnMasterFail = value; }
    public ConnectionTest OnJoinOnRoomFail { get => onJoinOnRoomFail; set => onJoinOnRoomFail = value; }

    private static NetworkManager instance;
    public static NetworkManager Instance { get => instance; private set => instance = value; }

    private void Awake()
    {
        if (instance) return;

        instance = this;

        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(this.gameObject);
    }

    public override void OnConnectedToMaster()
    {
        isConnected = true;

        if (OnConnectedOnMaster != null) OnConnectedOnMaster();
    }

    public override void OnJoinedRoom()
    {
        isJoinedOnRoom = true;

        if (OnJoinedOnRoom != null) OnJoinedOnRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (OnJoinOnRoomFail != null) OnJoinOnRoomFail();
    }

    public void CreateRoom(string name)
    {
        PhotonNetwork.CreateRoom(name);
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public void LoadScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
