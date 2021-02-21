using Mirror;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class WarCommandNetworkManager : NetworkManager
    {
        public GameObject probePrefab;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
           
            GameObject probe = Instantiate(probePrefab);
            NetworkServer.Spawn(probe, conn);            
        }
    }
}