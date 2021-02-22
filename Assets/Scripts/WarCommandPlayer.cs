using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCommandPlayer : NetworkBehaviour
{
    public GameObject startingProbe;

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
            Camera.main.transform.SetParent(this.transform, false);
            GetComponent<CameraController>().cameraTransform = Camera.main.transform;
            CmdSpawnProbe();
        }

        base.OnStartClient();
    }

    [Command]
    void CmdSpawnProbe()
    {
        GameObject probe = Instantiate(startingProbe, transform.position, transform.rotation);
        NetworkServer.Spawn(probe, connectionToClient);
    }
}
