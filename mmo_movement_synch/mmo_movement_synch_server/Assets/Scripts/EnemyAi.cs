
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
        public float timeBetweenAttacks;



        //States
        public float sightRange;
        public bool playerInSightRange;

        public float attackRange;
        public bool playerInAttackRange;
        public bool enemyAttacking;
        bool alreadyAttacked;

        private void Awake()
        {

            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (GameObject.Find("X Bot@Walking(Clone)") != null)
            {
                player = GameObject.Find("X Bot@Walking(Clone)").transform;
            }

            //Check for sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        private void AttackPlayer()
        {
            enemyAttacking = true;
            agent.SetDestination(transform.position);
            EnemyMovement movePos = new EnemyMovement(enemyID, walkPoint, enemyAttacking, transform.rotation);
            //Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + walkPoint);
            ServerManager.Instance.SendToAll(Tags.EnemyMove, movePos);
            transform.LookAt(player);
            if (!alreadyAttacked)
            {


                EnemyMovement movePosi = new EnemyMovement(enemyID, player.position, enemyAttacking, transform.rotation);
                //Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + player.position);
                ServerManager.Instance.SendToAll(Tags.EnemyMove, movePosi);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }

        }
        private void ResetAttack()
        {
            alreadyAttacked = false;
        }

        private void Patroling()
        {
            enemyAttacking = false;
            if (!walkPointSet) SearchWalkPoint();

            if (walkPointSet) { 
                EnemyMovement movePos = new EnemyMovement(enemyID, walkPoint, enemyAttacking, transform.rotation);
                //Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + walkPoint);
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
            enemyAttacking = false;
            EnemyMovement movePos = new EnemyMovement(enemyID, player.position, enemyAttacking, transform.rotation);
            //Debug.Log("Enemy ID: " + enemyID + "Walkpoint:" + player.position);
            ServerManager.Instance.SendToAll(Tags.EnemyMove, movePos);
            agent.SetDestination(player.position);
        }


    }
}