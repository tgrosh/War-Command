using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCommandPlayer : NetworkBehaviour
{
    public GameObject startingUnit;
    CameraController cameraController;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            cameraController = GetComponent<CameraController>();
            Camera.main.transform.SetParent(this.transform, false);
            cameraController.cameraTransform = Camera.main.transform;
        }
    }

    public override void OnStartLocalPlayer()
    {
        CmdSpawnProbe();

        base.OnStartLocalPlayer();
    }

    [Command]
    void CmdSpawnProbe()
    {
        GameObject startingBase = Instantiate(startingUnit, transform.position, transform.rotation);
        NetworkServer.Spawn(startingBase, connectionToClient);
        startingBase.GetComponent<Buildable>().Build();
    }
}
