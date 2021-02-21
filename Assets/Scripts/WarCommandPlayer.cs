using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCommandPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            Camera.main.transform.SetParent(this.transform);
            GetComponent<CameraController>().cameraTransform = Camera.main.transform;
        }

        base.OnStartClient();
    }

}
