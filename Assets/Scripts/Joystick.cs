using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform center;
    public RectTransform joystick;
    public float radius;

    private bool isPressed;
    private Vector3 joystickDirection;

    private GameManager gameManager;
    private PointerEventData pointerData;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void LateUpdate()
    {
        if (!center || !joystick || !gameManager.Player)
        {
            return;
        }

        ControlJoystick(out joystickDirection);

        gameManager.Player.HorizontalInput = joystickDirection.x;
        gameManager.Player.VerticalInput = joystickDirection.z;
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
    }

    private void ControlJoystick(out Vector3 outDirection)
    {
        outDirection = new Vector3();

        if (isPressed)
        {
            var _dir = (Vector3)pointerData.position - center.position;


            joystick.position = center.position + _dir;
            if (joystick.anchoredPosition.magnitude > radius)
            {
                joystick.anchoredPosition = joystick.anchoredPosition.normalized * radius;
            }
        }
        else
        {
            joystick.position = center.position;
        }

        outDirection.x = joystick.anchoredPosition.x;
        outDirection.z = joystick.anchoredPosition.y;

        outDirection.x /= radius;
        outDirection.z /= radius;
    }
}
