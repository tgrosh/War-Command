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
        marker = Instantiate(selectionMarkerPrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        marker.transform.parent = transform;
        marker.transform.localScale = Vector3.one * GetComponent<Collider>().bounds.size.magnitude;
    }
}
