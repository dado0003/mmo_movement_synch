
using UnityEngine;
using UnityEngine.AI;


namespace DarkRiftRPG
{
    public class EnemyMovementClass : MonoBehaviour
    {
        public NavMeshAgent agent;

        public Transform player;

        public Animator animator;

        public Rigidbody rb;





        public void Awake()
        {

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();

        }
        
        public void moveToPos(Vector3 pos)
        {
            //Debug.Log("Moving");
            animator.SetBool("moving", true);

            agent.SetDestination(pos);
        }

        public void rotate(Quaternion rot)
        {
            //Debug.Log("Moving");
            rb.MoveRotation(rot);
            
        }
        public void attacking()
        {

            //Debug.Log("Attacking player");
            animator.SetBool("moving", false);
            
            
            
        }
    }
}