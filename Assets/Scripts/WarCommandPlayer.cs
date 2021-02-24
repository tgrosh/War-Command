using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCommandPlayer : NetworkBehaviour
{
    public GameObject startingProbe;
    CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponent<CameraController>();
        Camera.main.transform.SetParent(this.transform, false);
        cameraController.cameraTransform = Camera.main.transform;
    }

    public override void OnStartLocalPlayer()
    {
        CmdSpawnProbe();

        base.OnStartLocalPlayer();
    }

    [Command]
    void CmdSpawnProbe()
    {
        GameObject probe = Instantiate(startingProbe, transform.position, transform.rotation);
        NetworkServer.Spawn(probe, connectionToClient);
    }
}
