using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarCommandPlayer : NetworkBehaviour
{
    public GameObject probePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameObject go = Instantiate(probePrefab);
        NetworkServer.Spawn(go, connectionToServer);
    }
}
