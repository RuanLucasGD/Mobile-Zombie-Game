using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameScene;
    [SerializeField] private InputField roomName;
    [SerializeField] private Button enterRoom;
    [SerializeField] private Button createRoom;

    private NetworkManager network;

    public string GameScene { get => gameScene; set => gameScene = value; }
    public InputField RoomName { get => roomName; set => roomName = value; }
    public Button EnterRoomButton { get => enterRoom; set => enterRoom = value; }
    public Button CreateRoomButton { get => createRoom; set => createRoom = value; }

    private void Start()
    {
        network = NetworkManager.Instance;
        EnterRoomButton.interactable = false;
        CreateRoomButton.interactable = false;

        network.OnConnectedOnMaster = delegate
        {
            EnterRoomButton.interactable = true;
            CreateRoomButton.interactable = true;
        };

        network.OnJoinedOnRoom = () => LoadGameScene();
        network.OnConnectionOnMasterFail = () => { };
        network.OnJoinOnRoomFail = ()=> { };
    }

    public void EnterRoom()
    {
        network.JoinRoom(RoomName.text);
    }

    public void CreateRoom()
    {
        network.CreateRoom(RoomName.text);
    }

    private void LoadGameScene()
    {
        network.LoadScene(gameScene);
    }
}
