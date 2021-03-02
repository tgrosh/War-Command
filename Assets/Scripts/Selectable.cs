using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selectable : NetworkBehaviour
{
    public GameObject selectionMarkerPrefab;
    public float scale;
    public Component selectionType;
    
    GameObject marker;

    private bool isSelected;
    public bool IsSelected { get => isSelected; }

    // Update is called once per frame
    void Update()
    {        
        if (marker && !IsSelected)
        {
            Destroy(marker);
        }

        if (marker == null && IsSelected)
        {
            ShowMarker();
        }
    }

    void ShowMarker()
    {
        marker = Instantiate(selectionMarkerPrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        marker.transform.parent = transform;
        marker.transform.localScale = new Vector3(scale, 1, scale);
    }

    public void Toggle()
    {
        if (isSelected)
        {
            DeSelect();
        } else
        {
            Select();
        }
    }

    public void Select()
    {
        if (hasAuthority) isSelected = true;
    }

    public void DeSelect()
    {
        if (hasAuthority) isSelected = false;
    }
}
