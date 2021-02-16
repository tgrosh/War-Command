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
