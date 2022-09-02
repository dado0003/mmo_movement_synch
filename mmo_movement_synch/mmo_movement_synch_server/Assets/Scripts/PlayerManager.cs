using System;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Server;

namespace DarkRiftRPG
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        public GameObject ServerPlayerPrefab;

        public Dictionary<ushort, GameObject> CurrentPlayers = new Dictionary<ushort, GameObject>();
        public Dictionary<ushort, GameObject> Enemies = new Dictionary<ushort, GameObject>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SpawnPlayerOnServer(ushort clientID)
        {
            if (!CurrentPlayers.ContainsKey(clientID))
            {
                GameObject go = Instantiate(ServerPlayerPrefab, ServerPlayerPrefab.transform.position, Quaternion.identity);
                CurrentPlayers.Add(clientID, go);
                PlayerSpawnData data = new PlayerSpawnData(clientID, go.transform.position);
                ServerManager.Instance.ConnectedClients[clientID].SpawnLocalPlayerOnClient(data);

                ServerMovement controller = CurrentPlayers[clientID].GetComponent<ServerMovement>();
                ServerManager.Instance.SendNewPlayerToOthers(clientID, controller.transform.position);
                ServerManager.Instance.SendOthersToNewPlayer(clientID, CurrentPlayers);

                foreach (KeyValuePair<ushort, GameObject> enemy in Enemies)
                {
                    EnemySpawnData enemySpawnData = new EnemySpawnData(enemy.Key,enemy.Value.transform.position);
                    ServerManager.Instance.SendToClient(clientID, Tags.SpawnEnemy, enemySpawnData);
                }
            }
        }

        public void MovePlayerOnServer(IClient client, InputPayload inputPayload)
        {
            ServerMovement controller = CurrentPlayers[client.ID].GetComponent<ServerMovement>();
            controller.OnClientInput(client,inputPayload);
            //ServerManager.Instance.SendOthersToPlayer(client.ID, CurrentPlayers);
        }

        
    }
}