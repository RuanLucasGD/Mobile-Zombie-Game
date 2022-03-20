using UnityEngine;
using Photon.Pun;

public class EnemiesSpawn : MonoBehaviourPunCallbacks
{
    public Transform spawnPos;
    public string enemy;


    private bool spawned;

    void Start()
    {

    }

    private void Update()
    {
        if (NetworkManager.Instance.GameStarted)
        {
            if (!spawned)
            {
                spawned = true;
                SpawnEnemey(enemy);
            }
        }
    }

    public void SpawnEnemey(string path)
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate(path, spawnPos.position, Quaternion.identity);
        }
    }
}
