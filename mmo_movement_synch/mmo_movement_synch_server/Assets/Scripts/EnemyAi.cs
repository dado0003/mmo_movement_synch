
using UnityEngine;
using UnityEngine.AI;


namespace DarkRiftRPG
{
    public class EnemyAi : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player;

        public LayerMask Ground, Player;

        
        public ushort enemyID;

        //Patroling
        public Vector3 walkPoint;
        bool walkPointSet;
        public float walkPointRange;

        

        //States
        public float sightRange;
        public bool playerInSightRange;

        private void Awake()
        {

            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (GameObject.Find("ServerPlayerPrefab(Clone)") != null)
            {
                player = GameObject.Find("ServerPlayerPrefab(Clone)").transform;
            }

            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
            
            if (!playerInSightRange) Patroling();
            if (playerInSightRange) ChasePlayer();
        }
        private void Patroling()
        {
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet) { 
                EnemyMovement movePos = new EnemyMovement(enemyID, walkPoint);
                Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + walkPoint);
                ServerManager.Instance.SendToAll(Tags.EnemyMove, movePos);
                agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f)
                walkPointSet = false;
        }
        private void SearchWalkPoint()
        {
            //Calculate random point in range
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
                walkPointSet = true;
        }

        private void ChasePlayer()
        {
            EnemyMovement movePos = new EnemyMovement(enemyID, player.position);
            Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + player.position);
            ServerManager.Instance.SendToAll(Tags.EnemyMove, movePos);
            agent.SetDestination(player.position);
        }


    }
}