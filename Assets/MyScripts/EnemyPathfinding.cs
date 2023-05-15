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
    [SerializeField] List<GameObject> navPointList = new List<GameObject>();
    //mine: the next randomized node to follow
    GameObject navPointToFollow;

    [SerializeField] GameObject player;

    [SerializeField] float stoppingDistance, detectionDistance;


    //mine: get random item
    public GameObject GetRandomItem(List<GameObject>navPointListRandom)
    {
        int randomNum = Random.Range(0, navPointListRandom.Count);
        print(randomNum);
        GameObject printRandom = navPointListRandom[randomNum];
        print(printRandom);
        return printRandom;
    }


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

            //mine: get a random item
            instance.navPointToFollow = instance.GetRandomItem(instance.navPointList);

            //mine: play run animation
            instance.transform.GetChild(0).GetComponent<Animator>().Play("RunAnim");

        }

        public override void OnUpdate()
        {
            //updates the position the target object
            //move toward target
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.StateMachine.SetState(new ChaseState(instance));
            }
            else if (Vector3.Distance(instance.transform.position, instance.navPointToFollow.transform.position) > instance.stoppingDistance)
            {
                //instance.agent.SetDestination(instance.navPoint.transform.position);
                //mine: move to random navpoint
                instance.agent.SetDestination(instance.navPointToFollow.transform.position);
             
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

            //mine: play idle animation
            instance.transform.GetChild(0).GetComponent<Animator>().Play("IdleAnim");
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

            //mine: play run animation
            instance.transform.GetChild(0).GetComponent<Animator>().Play("RunAnim");
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
