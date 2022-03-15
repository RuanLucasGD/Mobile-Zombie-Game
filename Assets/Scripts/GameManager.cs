using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onPlayerFinded;

    private PlayerController player;

    public PlayerController Player => player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Player)
        {
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        var _players = FindObjectsOfType<PlayerController>();

        foreach (var p in _players)
        {
            if (p.photonView.IsMine)
            {
                player = p;
                onPlayerFinded.Invoke();
            }
        }
    }
}
