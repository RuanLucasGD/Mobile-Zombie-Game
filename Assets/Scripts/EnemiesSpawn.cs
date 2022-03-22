using UnityEngine;
using Photon.Pun;

public class EnemiesSpawn : MonoBehaviourPunCallbacks
{
    public Transform spawnPos;

    public string enemy;
    private bool spawned;

    private void Update()
    {
        if (GameManager.Instance.GameStarted)
        {
            if (!spawned)
            {
                spawned = true;
                SpawnEnemey();
            }
        }
    }

    public void SpawnEnemey()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Instantiate(enemy, spawnPos.position, Quaternion.identity);
            PhotonNetwork.Instantiate(enemy, spawnPos.position + Vector3.forward, Quaternion.identity);
        }
    }
}
