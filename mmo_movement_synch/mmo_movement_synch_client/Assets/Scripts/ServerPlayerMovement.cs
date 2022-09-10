using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkRiftRPG
{



    public class ServerPlayerMovement : MonoBehaviour
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
        public Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

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

        public void OnClientInput(InputPayload inputPayload)//call this with data you sent to server
        {
            if(inputQueue != null) { 
                inputQueue.Enqueue(inputPayload);
            }
            

        }

        IEnumerator SendToClientFunc(StatePayload statePayload)
        {
            yield return new WaitForSeconds(0.02f);

            //Client.Instance.OnServerMovementState(statePayload);
            StatePayload clientIdSerialReconcil = new StatePayload(statePayload.tick, statePayload.position);
            //ServerManager.Instance.SendToClient(moveClientID, Tags.StatePayloadTag, clientIdSerialReconcil);
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

        StatePayload ProcessMovement(InputPayload input)
        {
            // Should always be in sync with same function on Client
            if (input.inputVector.y == 0.0 && input.inputVector.x == 0.0 && input.inputVector.z == 0.0)
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                animator.SetBool("isMoving", true);
            }
            transform.Translate(input.inputVector * 5f * minTimeBetweenTicks, Space.World);
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            rb.MoveRotation(input.lookRotation);

            return new StatePayload()
            {
                tick = input.tick,
                position = transform.position,
            };
        }
    }
}