using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

public class EnemyPathfinding : MonoBehaviour
{
    //reference to the NavAgentMesh attached to this object
    private NavMeshAgent agent;

    //object the AI is trying to navigate towards
    [SerializeField] GameObject navPoint;

    //mine: a list of all navpoints
    public List<GameObject> navPointList = new List<GameObject>();

    [SerializeField] GameObject player;

    [SerializeField] float stoppingDistance, detectionDistance;


    public StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = new StateMachine();

        if(!TryGetComponent<NavMeshAgent>(out agent))
        {
            Debug.LogError("This object needs an nav mesh agent attached to it");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StateMachine.SetState(new IdleState(this));

        agent.isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.OnUpdate();
    }


    public abstract class EnemyMoveState : IState
    {
        protected EnemyPathfinding instance;

        public EnemyMoveState(EnemyPathfinding _instance)
        {
            instance = _instance;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }

    public class MoveState : EnemyMoveState
    {
        public MoveState(EnemyPathfinding _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entering MoveState");
            //set the agent to stopped.
            instance.agent.isStopped = false;
        }

        public override void OnUpdate()
        {
            //updates the position the target object
            //move toward target
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.StateMachine.SetState(new ChaseState(instance));
            }
            else if (Vector3.Distance(instance.transform.position, instance.navPoint.transform.position) > instance.stoppingDistance)
            {
                instance.agent.SetDestination(instance.navPoint.transform.position);
            }
            else
            {
                //set state to IdleState
                instance.StateMachine.SetState(new IdleState(instance));
            }
        }
    }

    public class IdleState: EnemyMoveState
    {
        public IdleState(EnemyPathfinding _instance): base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entering IdleState");
            instance.agent.isStopped = true;

            //chase animation test
            instance.transform.GetChild(0).GetComponent<Animator>().Play("IdleAnim");
            //chase animation test
        }

        public override void OnUpdate()
        {
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.StateMachine.SetState(new ChaseState(instance));
            }
            else if (Vector3.Distance(instance.transform.position, instance.navPoint.transform.position) > instance.stoppingDistance)
            {
                //set state to MoveState
                instance.StateMachine.SetState(new MoveState(instance));
            }

        }
    }

    public class ChaseState : EnemyMoveState
    {
        public ChaseState(EnemyPathfinding _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entering ChaseState");
            instance.agent.isStopped = false;

            //chase animation test
            instance.transform.GetChild(0).GetComponent<Animator>().Play("RunAnim");
            //chase animation test
        }

        public override void OnUpdate()
        {
            if(Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.agent.SetDestination(instance.player.transform.position);
            }
            else
            {
                //set to IdleState
                instance.StateMachine.SetState(new IdleState(instance));
            }

        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
