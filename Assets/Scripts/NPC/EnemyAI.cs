using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public Vector3 externalDestination = Vector3.zero;

    public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
    public float damageWaitTime = 4f;
    public float saveDistanceToOpponent = 2;
    public float minHpToAttack = 0.3f;
    public float actionTimeDistance = 5f;
    public float actionTime = 0;
    public float stateChangeTime = 0.5f;

    public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.
    public Vector3[] positioningWayPoints;
    public Vector3 lastDestination = Vector3.zero;
    public Transform posToReach;
    public Vector3 damageDir = Vector3.zero;

    private Transform avatar;
    private Animator anim;
    private Health health;
    private DialogManager dialogManager;
    private Schedule schedule;
    [SerializeField]
    private PlayerAttributes attributes;
    private GroupManager groupManager;
    private NPCAttributes aiInfos;
    private UnityEngine.AI.NavMeshAgent nav;                               // Reference to the nav mesh agent.
    private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
    public float chaseTimer;                                // A timer for the chaseWaitTime.
    private float damageTimer;

    public string curActionName;
    public string optActionName;

    // Coroutines
    public List<AIRoutine> routineDatabase = new List<AIRoutine>();
    public Dictionary<string, AIRoutine> aiRoutines = new Dictionary<string, AIRoutine>();


    void Awake()
    {
        // Setting up the references.
        avatar = transform.root;
        dialogManager = avatar.GetComponent<DialogManager>();
        health = avatar.GetComponent<Health>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        groupManager = avatar.GetComponent<GroupManager>();
        aiInfos = GetComponent<NPCAttributes>();
        schedule = GetComponent<Schedule>();
        nav = avatar.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = avatar.GetComponent<Animator>();
        lastPlayerSighting = GameObject.FindWithTag("EnemyManager").GetComponent<LastPlayerSighting>();

        // Routines
        AIRoutine[] routines = GetComponents<AIRoutine>();
        for (int i = 0; i < routines.Length; i++)
        {
            aiRoutines.Add(routines[i].GetType().ToString(), routines[i]);
        }
    }


    void Start()
    {
        // activate first action
        StartCoroutine(Scheduler());
    }

    /** highest prioroty comes first
     * Fight        |
     * Chase        |
     * Searching
     * Following        |        
     * Observe          |
     * LookForDamage    |
     * Dialog
     * externalDestination
     * Group
     * (has extra work: quest, arena, ...)
     * Schedule
     **/

    IEnumerator Scheduler()
    {
        // get best action for the current situation
        string bestAction = "";

        for (;;)
        {
            // If has target ...
            if (attributes.target)
            {
                // ... and has interest
                if (aiInfos.aggro <= 0.5f)
                {
                    bestAction = "ObserveRoutine";
                }
                // ... and has aggro ...
                else
                {
                    // ... and target position unknown
                    if (aiInfos.targetPositionLost)// aiInfos.personalLastSighting == aiInfos.resetPosition)
                    {
                        bestAction = "SearchingRoutine";
                    }
                    // ... and target position known
                    else
                    {
                        // ... and not fighting
                        if (aiInfos.aggro < 1f)
                        {
                            bestAction = "ChasingRoutine"; // walk
                        }
                        // ... and fight
                        else
                        {
                            // ... target in attack range
                            if (aiInfos.targetInSight)
                            {
                                bestAction = "FightingRoutine";
                            }
                            // ... target out of range
                            else
                            {
                                bestAction = "ChasingRoutine"; // run
                            }
                        }
                    }
                }
            }
            // not infight but get damage
            else if (damageDir != Vector3.zero)
            {
                // Debug.Log(this.name + ": look for damage");
                bestAction = "LookForDamage";
            }
            // is in dialog
            else if (dialogManager.dialogPartner)
            {
                bestAction = "DialogRoutine";
            }
            // included in arenafight
            else if (groupManager.match)
            {
                nav.destination = groupManager.match.arenaSpace.transform.position;
            }

            // get external destination
            else if (externalDestination != Vector3.zero)
            {
                nav.destination = externalDestination;
            }
            // in group
            else if (groupManager.group.groupMember.Count > 1)
            {
                bestAction = "GroupRoutine";
            }
            /* else if (has extra work(quest, group, talk, arena, ...))
             * {
             *     
             * }
             */
            // Otherwise ...
            else
            {
                // ... Daily Routine.
                // Debug.Log(this.name + "Daily Routine");
                
                // not in dailyRoutine 
                if(curActionName != schedule.activeActivity.type.ToString())
                {
                    // update activity
                    schedule.ChangeActivity();
                }

                bestAction = schedule.activeActivity.type.ToString();
            }
            

            // store best actionName
            optActionName = bestAction;

            // check if optAction is running ...
            if (curActionName != optActionName)
            {
                // ... start opt action
                Dispatcher();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void Dispatcher()
    {
        AIRoutine curAiRoutine;

        // is action running?
        if (aiRoutines.ContainsKey(curActionName))
        {
            // get current action
            aiRoutines.TryGetValue(curActionName, out curAiRoutine);

            // stop it
            curAiRoutine.FinishAction();
            curActionName = "";
        }

        // get new routine
        if (aiRoutines.TryGetValue(optActionName, out curAiRoutine))
        {
            // start it
            curAiRoutine.StartAction();
            curActionName = optActionName;
        }
    }

    void GetDamage(DamageInfo info)
    {
        // Debug.Log("Ai react to damage");
        
        // look for damage cause

    }

    void Update()
    {

    }

    
    void LookForDamage()
    {
        if (attributes.target)
        {
            aiInfos.aggro = 0.9f;
            damageTimer = 0;
            damageDir = Vector3.zero;
        }
        else if (damageTimer >= damageWaitTime)
        {
            damageTimer = 0;
            damageDir = Vector3.zero;
        }
        else
        {
            nav.destination = this.transform.position;
            //this.transform.forward = damageDir;
            damageTimer += Time.deltaTime;
        }
    }
}
