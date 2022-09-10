using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift;
using DarkRift.Client;
using System;
using UnityEngine.AI;

namespace DarkRiftRPG
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject Enemy;
        public GameObject playerPrefab;
        public GameObject serverPlayerPrefab;
        public Dictionary<ushort, GameObject> ConnectedPlayers = new Dictionary<ushort, GameObject>();
        public ushort LocalPlayerID;
        public int playerCount = 0;
        public ushort enemyCounter = 0;
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

        private void Start()
        {


            ConnectionManager.Instance.SpawnLocalPlayerRequest();
        }

        public void SpawnLocalPlayer(SpawnLocalPlayerResponseData data)
        {
            if (!ConnectedPlayers.ContainsKey(data.ID))
            {
                GameObject go = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
                ConnectedPlayers.Add(data.ID, go);

            }
        }

        public void SpawnPlayer(PlayerSpawnData data)
        {
            LocalPlayerID = ConnectionManager.Instance.LocalClientID;
            //Debug.Log("Local Player ID set to: " + LocalPlayerID);
            if (!ConnectedPlayers.ContainsKey(data.ID))
            {

                if (data.ID == LocalPlayerID)
                {
                    GameObject go = Instantiate(playerPrefab, data.Position, Quaternion.identity);
                    ConnectedPlayers.Add(data.ID, go);
                    //Debug.Log("Data id is " + data.ID);
                }
                else
                {
                    GameObject go = Instantiate(serverPlayerPrefab, data.Position, Quaternion.identity);
                    ConnectedPlayers.Add(data.ID, go);
                    //Debug.Log("Data id is " + data.ID);
                }


            }

        }

        public void RemovePlayerFromGame(PlayerDespawnData data)
        {
            if (ConnectedPlayers.ContainsKey(data.ID))
            {
                Destroy(ConnectedPlayers[data.ID]);
            }
        }

        public void MoveServerPlayer(PlayerMoveData playerMoveData)
        {
            GameObject changePos = ConnectedPlayers[playerMoveData.ID];
            changePos.transform.position = playerMoveData.Position;
            ConnectedPlayers[playerMoveData.ID] = changePos;
        }
        public void MoveServerPlayerPayload(ushort playerID, InputPayload serverState)
        {
            if(playerID != LocalPlayerID) {
                if (ConnectedPlayers.ContainsKey(playerID))
                {
                    ServerPlayerMovement player = (ServerPlayerMovement)ConnectedPlayers[playerID].GetComponent(typeof(ServerPlayerMovement));
                    player.OnClientInput(serverState);
                    //Debug.Log("NEposielaö local");
                }
                
            }
            else
            {
                //Debug.Log("posielaö local");
            }
            
        }

        public void SpawnEnemyWithID(EnemySpawnData enemySpawnData)
        {
            GameObject enemy = Instantiate(Enemy, enemySpawnData.Position, enemySpawnData.Rotation);
            //Debug.Log("Vytvoril som ID: " + playerCount);
            Enemies.Add(enemyCounter, enemy);

            enemyCounter++;
        }

        public void MoveEnemy(EnemyMovement enemyMovement)
        {

            if (Enemies.ContainsKey(enemyMovement.ID)) { 
            EnemyMovementClass enemy = (EnemyMovementClass)Enemies[enemyMovement.ID].GetComponent(typeof(EnemyMovementClass));
            
        
            //Debug.Log("SnaûÌm sa pohn˙ù s ID:" + enemyMovement.ID);
                if (enemyMovement.State == false)
                {
                    enemy.moveToPos(enemyMovement.MovePosition);
                    enemy.rotate(enemyMovement.Rotation);
                } 
                else
                {
                    enemy.moveToPos(enemy.transform.position);
                    enemy.rotate(enemyMovement.Rotation);
                    enemy.attacking();
                }
                

                /*
                 * if (enemyMovement.State == false)
                {
                    NavMeshAgent agent = (NavMeshAgent)Enemies[enemyMovement.ID].GetComponent(typeof(NavMeshAgent));

                    Debug.Log("SnaûÌm sa pohn˙ù s ID:" + enemyMovement.ID);

                    agent.SetDestination(enemyMovement.MovePosition);
                }
                else { 
                
                
                }
                 */
            }

        }
    }
}