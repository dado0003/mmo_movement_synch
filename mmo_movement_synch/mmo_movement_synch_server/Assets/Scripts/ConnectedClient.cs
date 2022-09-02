using System.Collections;
using System.Collections.Generic;
using System;
using DarkRift.Server;
using DarkRift;
using UnityEngine;

namespace DarkRiftRPG
{   
        
    public struct PlayerStruct
    {
        public ushort pID;
        public Queue<InputPayload> inputQueue;

        public PlayerStruct(ushort ID)
        {
            pID = ID;
            inputQueue = new Queue<InputPayload>();
        }
    }
    public class ConnectedClient
    {

        public ushort ClientID;
        public IClient Client;
        public IDictionary<int, PlayerStruct> PlayersWíthPayload = new Dictionary<int, PlayerStruct>();

        public ConnectedClient(IClient client)
        {
            Client = client;
            ClientID = client.ID;

            Client.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;
            using (Message message = e.GetMessage())
            {
                switch ((Tags)message.Tag)
                {
                    case Tags.SpawnLocalPlayerRequest:
                        OnSpawnLocalPlayerRequest();
                        break;
                    case Tags.InputPayloadTag:
                        InputPayloadTagFunc(client, message.Deserialize<InputPayload>());
                        break;
                }
            }
        }

        private void InputPayloadTagFunc(IClient client, InputPayload inputPayload)
        {
            //ServerMovement.Instance.OnClientInput(client, inputPayload);
            PlayerManager.Instance.MovePlayerOnServer(client, inputPayload);
            Debug.Log(client.ID + " " + inputPayload.inputVector);
        }

        private void OnSpawnLocalPlayerRequest()
        {
            PlayerManager.Instance.SpawnPlayerOnServer(ClientID);
            
            
        }

        public void SpawnLocalPlayerOnClient(PlayerSpawnData data)
        {
            PlayerIdSerial clientIdSerial = new PlayerIdSerial(ClientID);
            Debug.Log("TOTO JE CLIENT ID XDD" + ClientID);
            ServerManager.Instance.SendToClient(ClientID, Tags.LocalPlayerID, clientIdSerial);
            ServerManager.Instance.SendToClient(data.ID, Tags.SpawnPlayer, data);
        }

        public void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessage;
        }
    }
}