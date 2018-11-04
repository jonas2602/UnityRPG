using UnityEngine;
using System.Collections;

public abstract class AIRoutine : MonoBehaviour
{
    protected Transform avatar;

    protected NPCAttributes aiInfos;
    protected PlayerAttributes attributes;
    protected EnemySight enemySight;
    protected Health health;
    protected UnityEngine.AI.NavMeshAgent nav;
    protected Animator anim;
    protected AnimInfo animInfo;
    protected Schedule schedule;
    protected WorldDialogOutput worldDialogOutput;
    protected EnvironmentScanner scanner;

    protected string curRoutine;

    public Condition startCondition;
    public int priority;

	// Use this for initialization
	protected virtual void Awake ()
    {
        avatar = transform.root;

        aiInfos = GetComponent<NPCAttributes>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        enemySight = GetComponent<EnemySight>();
        health = avatar.GetComponent<Health>();
        nav = avatar.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = avatar.GetComponent<Animator>();
        animInfo = avatar.GetComponent<AnimInfo>();
        schedule = GetComponent<Schedule>();
        worldDialogOutput = avatar.GetComponentInChildren<WorldDialogOutput>();
        scanner = avatar.GetComponentInChildren<EnvironmentScanner>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract void StartAction();
    /*
    {
        start routines
        Set variables
    }
    */

    public abstract void FinishAction();
    /*
    {
        stop routines
        start next Routine
    }
    */

    protected void StartSubroutine(string routineName)
    {
        // stop current subroutine
        if(curRoutine != "")
        {
            StopCoroutine(curRoutine);
        }

        // start new subroutine
        StartCoroutine(routineName);
        curRoutine = routineName;
    }

    /*
    // any action (example)
    IEnumerator Action()
    {
        // start action
        string nextActionName;

        for (;;)
        {
            // do action

            // wait some time
            yield return null;

            // check if still correct
            nextActionName = GetCorrectActionName();
            if (nextActionName == curActionName)
            {
                continue;
            }
            else
            {
                break;
            }
        }

        // finish action

        // let next action start
        // Debug.Log("start from action");
        StartAction(nextActionName);
    }
    */
}
