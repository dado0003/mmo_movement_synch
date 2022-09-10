using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkRiftRPG { 


    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance;

        // Shared
        private float timer;
        private int currentTick;
        private float minTimeBetweenTicks;
        /*private const float SERVER_TICK_RATE = 30f;*/
        private const float SERVER_TICK_RATE = 60f;
        private const int BUFFER_SIZE = 1024;

        // Client specific
        private StatePayload[] stateBuffer;
        private InputPayload[] inputBuffer;
        private StatePayload latestServerState;
        private StatePayload lastProcessedState;
        private float horizontalInput;
        private float verticalInput;
        private Vector3 forward;
        private Vector3 right;
        private Vector3 forwardRelative;
        private Vector3 rightRelative;
        private Vector3 cameraRelative;

        public Animator animator;

        void Awake()
        {
            Instance = this;
            animator = GetComponent<Animator>();
        }

        void Start()
        {
            minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

            stateBuffer = new StatePayload[BUFFER_SIZE];
            inputBuffer = new InputPayload[BUFFER_SIZE];
        }

        void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");

            forward = Camera.main.transform.forward;
            forward.y = 0;
            forward = forward.normalized;
            right = Camera.main.transform.right;
            right.y = 0;
            right = right.normalized;

            forwardRelative = verticalInput * forward;
            rightRelative = horizontalInput * right;

            cameraRelative = forwardRelative + rightRelative;






            timer += Time.deltaTime;

            while (timer >= minTimeBetweenTicks)
            {
                timer -= minTimeBetweenTicks;
                HandleTick();
                currentTick++;
            }
        }

        public void OnServerMovementState(StatePayload serverState)
        {
            latestServerState = serverState;
        }

        IEnumerator SendToServer(InputPayload inputPayload)
        {
            yield return new WaitForSeconds(0.02f);

            //Server.Instance.OnClientInput(inputPayload);
            ConnectionManager.Instance.SendPosToServer(inputPayload);
        }

        void HandleTick()
        {
            if (!latestServerState.Equals(default(StatePayload)) &&
                (lastProcessedState.Equals(default(StatePayload)) ||
                !latestServerState.Equals(lastProcessedState)))
            {
                HandleServerReconciliation();
            }

            int bufferIndex = currentTick % BUFFER_SIZE;

            // Add payload to inputBuffer
            InputPayload inputPayload = new InputPayload();
            inputPayload.tick = currentTick;
            inputPayload.inputVector = cameraRelative;
            
            
            inputBuffer[bufferIndex] = inputPayload;

            // Add payload to stateBuffer
            stateBuffer[bufferIndex] = ProcessMovement(inputPayload);

            Quaternion lookRotation = Quaternion.LookRotation(inputPayload.inputVector);
            lookRotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.fixedDeltaTime * 360);
            Rigidbody rb = transform.GetComponent<Rigidbody>();

            if (inputPayload.inputVector.y == 0.0 && inputPayload.inputVector.x == 0.0 && inputPayload.inputVector.z == 0.0)
            {
                
            }
            else
            {
                
                rb.MoveRotation(lookRotation);
            }
            inputPayload.lookRotation = lookRotation;
            Debug.Log(lookRotation);
            //Debug.Log("Toto je lookrotation" + inputPayload.lookRotation);
            // Send input to server
            StartCoroutine(SendToServer(inputPayload));
        }

        StatePayload ProcessMovement(InputPayload input)
        {
            // Should always be in sync with same function on Server
            
            if(input.inputVector.y == 0.0 && input.inputVector.x == 0.0 && input.inputVector.z == 0.0)
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                animator.SetBool("isMoving", true);
            }
            transform.Translate(input.inputVector * 5f * minTimeBetweenTicks, Space.World);
            //Debug.Log(input.inputVector);

            return new StatePayload()
            {
                tick = input.tick,
                position = transform.position,
            };
        }

        void HandleServerReconciliation()
        {

            lastProcessedState = latestServerState;

            int serverStateBufferIndex = latestServerState.tick % BUFFER_SIZE;
            float positionError = Vector3.Distance(latestServerState.position, stateBuffer[serverStateBufferIndex].position);

            if (positionError > 0.001f)
            {
                //Debug.Log("Reconcile");
                // Rewind & Replay
                transform.position = latestServerState.position;

                // Update buffer at index of latest server state
                stateBuffer[serverStateBufferIndex] = latestServerState;

                // Now re-simulate the rest of the ticks up to the current tick on the client
                int tickToProcess = latestServerState.tick + 1;

                while (tickToProcess < currentTick)
                {
                    int bufferIndex = tickToProcess % BUFFER_SIZE;

                    // Process new movement with reconciled state
                    StatePayload statePayload = ProcessMovement(inputBuffer[bufferIndex]);

                    // Update buffer with recalculated state
                    stateBuffer[bufferIndex] = statePayload;

                    tickToProcess++;
                }
            }
        }
    }
}