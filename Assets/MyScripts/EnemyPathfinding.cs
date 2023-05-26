using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FiniteStateMachine;

/* Script created by Melvin Jr Sanggalan
 * Last updated 22/05/2023
 * Script for the Enemy AI's StateMachine
 */

public class EnemyPathfinding : MonoBehaviour
{
    //reference to the NavAgentMesh attached to this object
    private NavMeshAgent agent;

    //object the AI is trying to navigate towards
    //[SerializeField] GameObject navPoint;

    //a list of all navpoints
    [SerializeField] List<GameObject> navPointList = new List<GameObject>();
    //the next randomized node to follow
    GameObject navPointToFollow;

    //bool for enemy stunned
    private bool enemyIsStunned = false;

    //bool for detecting if player has been caught
    public bool playerIsCaught = false;

    //bool for detecting if enemy is chasing
    public bool enemyIsChasing = false;

    //reference to game over screen
    public GameObject gameOverScreen;
    //reference to lose game sfx
    public GameObject loseGameSFX;
    //reference to alert light sound effect so we can turn it off
    public GameObject alertLight;

    //all of enemy SFX
    public GameObject enemyWalkSFX;
    public GameObject enemyRunSFX;


    [SerializeField] GameObject player;

    public float stoppingDistance, detectionDistance;


    //get random item
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
            Debug.Log("This object needs an nav mesh agent attached to it");
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

            //enemyIsChasing to false
            instance.enemyIsChasing = false;

            //get a random item
            instance.navPointToFollow = instance.GetRandomItem(instance.navPointList);

            //play run animation
            instance.transform.GetChild(0).GetComponent<Animator>().Play("WalkAnimation");

            //change speed
            instance.agent.speed = 4;

            //play walk sfx
            instance.enemyWalkSFX.SetActive(true);
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
                //move to random navpoint
                instance.agent.SetDestination(instance.navPointToFollow.transform.position);
             
            }
            else
            {
                //set state to IdleState
                instance.StateMachine.SetState(new IdleState(instance));
            }
        }

        public override void OnExit()
        {
            //stop walk sfx
            instance.enemyWalkSFX.SetActive(false);
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

            //enemyIsChasing to false
            instance.enemyIsChasing = false;

            //play idletime coroutine
            instance.StartCoroutine(idleTime(instance.transform.GetChild(0).gameObject));
        }

        //couritine wait time for idle
        IEnumerator idleTime(GameObject idleTimeEnemy)
        {
            //put the player in an idle animation
            idleTimeEnemy.GetComponent<Animator>().Play("IdleAnimation");

            //idle for 5 seconds
            yield return new WaitForSeconds(5.5f);

            //check if enemyIsStunned bool is false so player doesnt change states while stunned.
            if(instance.enemyIsStunned == false)
            {
                //go to MoveState after time
                instance.StateMachine.SetState(new MoveState(instance));
            }
        }

        public override void OnUpdate()
        {
            //check if player is near, if not then go back to movestate
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.StateMachine.SetState(new ChaseState(instance));
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

            instance.enemyIsChasing = true;
            //play run animation
            instance.transform.GetChild(0).GetComponent<Animator>().Play("RunAnimation");
            //change speed
            instance.agent.speed = 6;

            //play run sfx
            instance.enemyRunSFX.SetActive(true);
        }

        public override void OnUpdate()
        {
            //check if player has already been caught
            if(instance.playerIsCaught == false)
            {
                if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
                {
                    instance.agent.SetDestination(instance.player.transform.position);

                }
                else
                {
                    //enemyIsChasing to false
                    instance.enemyIsChasing = false;

                    //set to MoveState
                    instance.StateMachine.SetState(new MoveState(instance));
                }
            }

            else
            {
                Debug.Log("Game Over.");
                instance.StateMachine.SetState(new EndState(instance));
            }



        }

        public override void OnExit()
        {
            //stop run sfx
            instance.enemyRunSFX.SetActive(false);
        }

    }




    //stunnedstate
    public class StunnedState : EnemyMoveState
    {
        public StunnedState(EnemyPathfinding _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entering StunnedState");
            instance.agent.isStopped = true;

            //make enemyIsStunned true
            instance.enemyIsStunned = true;

            //play stunned coroutine
            instance.StartCoroutine(stunnedTime(instance.transform.GetChild(0).gameObject));

            //enemyIsChasing to false
            instance.enemyIsChasing = false;

        }


        //couritine wait time for stun
        IEnumerator stunnedTime(GameObject stunnedEnemy)
        {
            //put the player in a "stunned" animation
            stunnedEnemy.GetComponent<Animator>().Play("StunnedAnimation");

            //stunned for 5 seconds
            yield return new WaitForSeconds(5.5f);

            //make enemyIsStunned false
            instance.enemyIsStunned = false;

            //check if player is near, if not then go back to movestate
            if (Vector3.Distance(instance.transform.position, instance.player.transform.position) < instance.detectionDistance)
            {
                instance.StateMachine.SetState(new ChaseState(instance));
            }
            else
            {
                //set state to MoveState
                instance.StateMachine.SetState(new MoveState(instance));
            }

        }

    }





    //endstate 
    public class EndState : EnemyMoveState
    {
        public EndState(EnemyPathfinding _instance) : base(_instance)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Entering EndState");
            instance.agent.isStopped = true;

            //make enemyIsStunned true
            instance.enemyIsStunned = true;

            instance.gameObject.tag = "Player";

        }

    }





    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && playerIsCaught == false)
        {
            if(enemyIsChasing == true)
            {
                playerIsCaught = true;
                Debug.Log("Player Touched whilst being Chased.");

                gameOverScreen.SetActive(true);
                loseGameSFX.SetActive(true);
                alertLight.SetActive(false);

                //make it so the player can see and move their mouse
                Cursor.lockState = CursorLockMode.None;


            }

        }
    }
}
