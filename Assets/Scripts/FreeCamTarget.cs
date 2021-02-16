using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCamTarget : MonoBehaviour
{
    public float moveSpeed;
    public float zoomAmount;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (Keyboard.current.wKey.isPressed)
        {
            pos.x -= moveSpeed * Time.deltaTime;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            pos.x += moveSpeed * Time.deltaTime;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            pos.z -= moveSpeed * Time.deltaTime;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            pos.z += moveSpeed * Time.deltaTime;
        }

        if (Mouse.current.scroll.y.ReadValue() > 0)
        {
            pos.y -= zoomAmount;
        }
        else if (Mouse.current.scroll.y.ReadValue() < 0)
        {
            pos.y += zoomAmount;
        }

        transform.position = pos;
    }
}
