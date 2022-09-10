using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkRiftRPG
{

    
    
    public class ServerMovement : MonoBehaviour
    {
       


        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks;
        /*private const float SERVER_TICK_RATE = 30f;*/
        private const float SERVER_TICK_RATE = 60f;
        private const int BUFFER_SIZE = 1024;
        public ushort moveClientID;

        private StatePayload[] stateBuffer;
        private Queue<InputPayload> inputQueue;

        

        void Start()
        {
            minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

            stateBuffer = new StatePayload[BUFFER_SIZE];
            inputQueue = new Queue<InputPayload>();
        }

        void Update()
        {
            timer += Time.deltaTime;

            while (timer >= minTimeBetweenTicks)
            {
                timer -= minTimeBetweenTicks;
                HandleTick();
                currentTick++;
            }
        }

        public void OnClientInput(IClient client, InputPayload inputPayload)//call this with data you sent to server
        {
            InputPayloadWithID serverPlayerMovement = new InputPayloadWithID(moveClientID, inputPayload.tick, inputPayload.inputVector, inputPayload.lookRotation);
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            //Debug.Log("Toto je lookrotation" + inputPayload.lookRotation);
            rb.MoveRotation(inputPayload.lookRotation);
            Debug.Log(inputPayload.lookRotation);
            ServerManager.Instance.SendToAll(Tags.MovePlayerPayload, serverPlayerMovement);
            inputQueue.Enqueue(inputPayload);
            //Debug.Log(client.ID);//zisti client ID aby si vedel poslaù sp‰ù pozicie hr·Ëov a zobraziù ich na clientovi
            moveClientID = client.ID;
        }

        IEnumerator SendToClientFunc(StatePayload statePayload)
        {
            yield return new WaitForSeconds(0.02f);

            //Client.Instance.OnServerMovementState(statePayload);
            StatePayload clientIdSerialReconcil = new StatePayload(statePayload.tick, statePayload.position);
            ServerManager.Instance.SendToClient(moveClientID, Tags.StatePayloadTag, clientIdSerialReconcil);
        }

        void HandleTick()
        {
            // Process the input queue
            int bufferIndex = -1;
            while (inputQueue.Count > 0)
            {
                InputPayload inputPayload = inputQueue.Dequeue();

                bufferIndex = inputPayload.tick % BUFFER_SIZE;

                StatePayload statePayload = ProcessMovement(inputPayload);
                stateBuffer[bufferIndex] = statePayload;
            }

            if (bufferIndex != -1)
            {
                StartCoroutine(SendToClientFunc(stateBuffer[bufferIndex]));
            }
        }
        /*ZISTI KDE POSLAç INPUT S PAYLOADOM DO MoveServerPlayersPayload FUKNCIE NA CLIENTOVI KçOR¡ PRIRADZUJE SERVERMOVEMENT SCRIPT NA GAMEOBJECTY
            A H›BE ICH CEZ ONCLIENTINPUT*/
        StatePayload ProcessMovement(InputPayload input)
        {

            // Should always be in sync with same function on Client
            transform.Translate(input.inputVector * 5f * minTimeBetweenTicks, Space.World);

            return new StatePayload()
            {
                tick = input.tick,
                position = transform.position,
            };
        }
    }
}