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
                       
            GameObject probe = Instantiate(probePrefab, startPositions[startPositionIndex].transform.position, startPositions[startPositionIndex].transform.rotation);
            NetworkServer.Spawn(probe, conn);
        }
    }
}