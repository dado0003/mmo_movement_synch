using System;
using System.Net;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkRiftRPG
{
    public class ConnectionManager : MonoBehaviour
    {
        //We want a static reference to ConnectionManager so it can be called directly from other scripts
        public static ConnectionManager Instance;
        //A reference to the Client component on this game object. 
        public UnityClient Client { get; private set; }

        public ushort LocalClientID;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(this);
        }
        
        public void SpawnLocalPlayerRequest()
        {
            using (Message message = Message.CreateEmpty((ushort)Tags.SpawnLocalPlayerRequest))
            {
                Client.SendMessage(message, SendMode.Reliable);
            }
        }
        
        void Start()
        {
            Client = GetComponent<UnityClient>();
            Client.ConnectInBackground(IPAddress.Loopback, Client.Port, false, ConnectCallback);
            Client.MessageReceived += OnMessage;
        }
        private void ConnectCallback(Exception e)
        {
            if (Client.ConnectionState == ConnectionState.Connected)
            {
                //Debug.Log("Connected to server!");
                OnConnectedToServer();

            }
            else
            {
                //Debug.LogError($"Unable to connect to server. Reason: {e.Message} ");
            }
        }

        private void OnConnectedToServer()
        {
            using (Message message = Message.CreateEmpty((ushort)Tags.JoinGameRequest))
            {
                Client.SendMessage(message, SendMode.Reliable);
            }
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            using (Message m = e.GetMessage())
            {
                switch ((Tags)m.Tag)
                {
                    case Tags.JoinGameResponse:
                        OnJoinGameResponse(m.Deserialize<JoinGameResponseData>());
                        break;
                        /*
                    case Tags.SpawnLocalPlayerResponse:
                        OnSpawnLocalPlayerResponse(m.Deserialize<SpawnLocalPlayerResponseData>());
                        break;
                        */
                    case Tags.StatePayloadTag:
                        StatePayloadTagFunc(m.Deserialize<StatePayload>());
                        break;
                    case Tags.SpawnPlayer:
                        OnSpawnPlayer(m.Deserialize<PlayerSpawnData>());
                        break;
                    case Tags.DespawnPlayer:
                        OnDespawnPlayer(m.Deserialize<PlayerDespawnData>());
                        break;
                    case Tags.LocalPlayerID:
                        ReceiveLocalPlayerID(m.Deserialize<PlayerIdSerial>());
                        break;
                    /*
                case Tags.MovePlayer:
                    MoveServerPlayers(m.Deserialize<PlayerMoveData>());
                    break;
                    */

                    case Tags.MovePlayerPayload:
                        MoveServerPlayersPayload(m.Deserialize<InputPayloadWithID>());
                        break;
                    case Tags.SpawnEnemy:
                        SpawnEnemy(m.Deserialize<EnemySpawnData>());
                        break;
                    case Tags.EnemyMove:
                        MoveEnemy(m.Deserialize<EnemyMovement>());
                        break;
                }
            }   
        }

        private void MoveEnemy(EnemyMovement enemyMovement)
        {
            if(GameManager.Instance != null) { 
            GameManager.Instance.MoveEnemy(enemyMovement);
            }
        }

        private void SpawnEnemy(EnemySpawnData enemySpawnData)
        {
            GameManager.Instance.SpawnEnemyWithID(enemySpawnData);
        }

        private void MoveServerPlayers(PlayerMoveData playerMoveData)
        {
            GameManager.Instance.MoveServerPlayer(playerMoveData);
        }

        private void MoveServerPlayersPayload(InputPayloadWithID playerInputPayload)
        {
            InputPayload sendPayload = new InputPayload();
            sendPayload.tick = playerInputPayload.tick;
            sendPayload.inputVector = playerInputPayload.inputVector;
            sendPayload.lookRotation = playerInputPayload.lookRotation;
            //Debug.Log("ID" + playerInputPayload.id + "Tick:" + sendPayload.tick + "Vector:" + sendPayload.inputVector);
            if (GameManager.Instance != null) { 
            GameManager.Instance.MoveServerPlayerPayload(playerInputPayload.id, sendPayload);
            }
        }

        private void ReceiveLocalPlayerID(PlayerIdSerial playerIdSerial)
        {
            LocalClientID = playerIdSerial.ID;
            //Debug.Log("Connection Manager Local Client ID = "+LocalClientID);
        }

        /*
private void OnSpawnLocalPlayerResponse(SpawnLocalPlayerResponseData data)
{
   GameManager.Instance.SpawnLocalPlayer(data);
}*/
        private void OnDespawnPlayer(PlayerDespawnData data)
        {
            GameManager.Instance.RemovePlayerFromGame(data);
        }
        private void OnSpawnPlayer(PlayerSpawnData data)
        {
            GameManager.Instance.SpawnPlayer(data);
        }
        private void StatePayloadTagFunc(StatePayload statePayload)
        {
            
            PlayerMovement.Instance.OnServerMovementState(statePayload);
        }   
        
        private void OnJoinGameResponse(JoinGameResponseData data)
        {
            if (!data.JoinGameRequestAccepted)
            {
               
                return;
            }

            SceneManager.LoadScene("Game");
        }
        public void SendPosToServer(InputPayload inputPayload)
        {
            InputPayload newInputPayload = new InputPayload();
            newInputPayload.tick = inputPayload.tick;
            newInputPayload.inputVector = inputPayload.inputVector;
            newInputPayload.lookRotation = inputPayload.lookRotation;
            using (Message message = Message.Create((ushort)Tags.InputPayloadTag, newInputPayload))
            {
                Client.SendMessage(message, SendMode.Reliable);
            }
        }

        private void OnDestroy()
        {
            Client.MessageReceived -= OnMessage;
        }
    }
}