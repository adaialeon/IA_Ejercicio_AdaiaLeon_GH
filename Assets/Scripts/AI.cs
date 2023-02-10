using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    enum State
    {
        Patrolling,
        Chasing,
        Traveling, 
        Waiting,
        Attacking,
    }

    State currentState;

    NavMeshAgent agent;

    public Transform[] destinationPoints;
    //int destinationIndex = 0;
    public Transform player;
    [SerializeField] float visionRange;
    [SerializeField] float patrolRange = 10f; 
    [SerializeField] Transform patrolZone;
    [SerializeField] float visionAngle;
    [SerializeField] [Range(0, 360)]
    public LayerMask obstaclesMask;
    public Transform[] points;
    private int destPoint = 0;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    void Start()
    {
        currentState = State.Patrolling;
        
        //destinationIndex = Random.Range(0, destinationPoints.Length);

        //deberes
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        GotoNextPoint();
    }


    void Update()
    {
        switch(currentState)
        {
            case State.Patrolling:
                Patrol();
            break;

            case State.Chasing:
                Chase();
            break;
            default:
                Chase();
            break;

            case State.Traveling:
                    Travel();
            break;

            case State.Waiting:
                    Wait();
            break;

            case State.Attacking:
                    Attack();
            break; 
        }

        //deberes
        if (agent.remainingDistance < 0.5f)
        GotoNextPoint();

    }

    //deberes
    void GotoNextPoint() 
    {
        if (points.Length == 0)
        return;
        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }

    /*void Patrol() 
    {
        agent.destination = destinationPoints[destinationIndex].position;
        if(Vector3.Distance(transform.position, destinationPoints[destinationIndex].position) < 1)
        {
        destinationIndex = Random.Range(0, destinationPoints.Length);
        }

        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            currentState = State.Chasing;
        }
    }*/

    //posición de la IA
    void Patrol() 
    {
        /*Vector3 randomPosition;
        if(RandomPoint(patrolZone.position, patrolRange, out randomPosition))
        {
            agent.destination = randomPosition; 
            Debug.DrawRay(randomPosition, Vector3.up * 5, Color.blue, 5f);
        }*/

        //deberes
        Vector3 destPoint;
        if(Destination(patrolZone.position, patrolRange, out destPoint))
        {
            agent.destination = destPoint; 
            Debug.DrawRay(destPoint, Vector3.up * 5, Color.blue, 5f);
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }

        currentState = State.Traveling; 
    }

    /*bool RandomPoint(Vector3 center, float range, out Vector3 point)
    {
        Vector3 RandomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(RandomPoint, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true; 
        }

        point = Vector3.zero;
        return false; 
    }*/

    //deberes
    bool Destination(Vector3 center, float range, out Vector3 point)
    {
        Vector3 Destination = center * destPoint * range;
        NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(Destination, out hit, 4, NavMesh.AllAreas))
        {
            point = hit.position;
            return true; 
        }

        point = Vector3.zero;
        return false; 
    }
    

    void Travel()
    {
        if(agent.remainingDistance <= 0.2)
        {
            currentState = State.Patrolling;
        }

        if(FindTarget())
        {
            currentState = State.Chasing;
        }
    }

    void Chase()
    {
        agent.destination = player.position;

        if(!FindTarget())
        {
            currentState = State.Patrolling;
        }
    }

    void Wait()
    {
        if(FindTarget())
        {
            currentState = State.Waiting;
        }
    }

    void Attack()
    {
        if(FindTarget())
        {
            currentState = State.Attacking;
        }
    }

    bool FindTarget()
    {
        if(Vector3.Distance(transform.position, player.position) < visionRange)
        {
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, player.position);
                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstaclesMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos() 
    {
        foreach(Transform point in destinationPoints)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(point.position, 1);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(patrolZone.position, patrolRange);
        
    }
}
