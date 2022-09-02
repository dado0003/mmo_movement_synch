
using UnityEngine;
using UnityEngine.AI;


namespace DarkRiftRPG
{
    public class EnemyMovementClass : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player;

    

        public void Awake()
        {

            agent = GetComponent<NavMeshAgent>();
            
        }
        
        public void moveToPos(Vector3 pos)
        {
            
            agent.SetDestination(pos);
        }
    }
}