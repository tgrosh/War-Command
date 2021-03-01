using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
	public Transform camTransform;

    void Update()
    {
        transform.LookAt(transform.position + camTransform.rotation * Vector3.forward,
            camTransform.rotation * Vector3.up); 
    }
}