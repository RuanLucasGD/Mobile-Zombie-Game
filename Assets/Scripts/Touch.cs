using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float sensitivity;
    [SerializeField] private bool invertVertical;
    [SerializeField] private bool invertHorizontal;

    private bool isPressed;
    private Vector2 lastPointPosition;

    private GameManager gameManager;
    private PointerEventData pointerData;
    private PlayerController playerController;

    private float horizontal;
    private float vertical;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (gameManager.Player)
        {
            if (isPressed)
            {
                var _dir = (pointerData.position - lastPointPosition) * Time.fixedDeltaTime;
                lastPointPosition = pointerData.position;

                horizontal = _dir.x * sensitivity;
                vertical = _dir.y * sensitivity;

                if (invertHorizontal) horizontal *= -1;
                if (invertVertical) vertical *= -1;
            }
            else
            {
                horizontal = 0;
                vertical = 0;
            }

            gameManager.Player.MouseHorizontalInput = horizontal;
            gameManager.Player.MouseVerticalInput = vertical;
        }
    }

    public void OnPointerUp(PointerEventData e)
    {

        isPressed = false;
        pointerData = e;
    }

    public void OnPointerDown(PointerEventData e)
    {
        isPressed = true;
        pointerData = e;
        lastPointPosition = pointerData.position;
    }
}
