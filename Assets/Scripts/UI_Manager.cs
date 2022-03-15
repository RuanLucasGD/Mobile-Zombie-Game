using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;

    private GameManager gameManager;

    void Start()
    {
        mobileUI.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();
    }

    public void EnableMobileControls(bool enabled)
    {
        mobileUI.SetActive(enabled);
        if (gameManager && gameManager.Player)
        {
            gameManager.Player.DisableDefaultControl = enabled;
        }
    }
}
