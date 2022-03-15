using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onPlayerFinded;

    private PlayerController player;

    public PlayerController Player => player;
    public UnityEvent OnPlayerFinded { get => onPlayerFinded; set => onPlayerFinded = value; }

    void Update()
    {
        if (!Player)
        {
            FindPlayer();

            if (Player)
            {
                OnPlayerFinded.Invoke();
            }
        }
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
