using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selectable : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public bool isSelected;
    
    GameObject marker;


    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isSelected = true;
                }
                else
                {
                    isSelected = false;
                }
            }
        }

        if (marker && !isSelected)
        {
            Destroy(marker);
        }

        if (marker == null && isSelected)
        {
            ShowMarker();
        }
    }

    void ShowMarker()
    {
        marker = Instantiate(selectionMarkerPrefab, (Vector3.up * .6f), Quaternion.Euler(Vector3.right * 90), transform);
    }
}
