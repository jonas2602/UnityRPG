using UnityEngine;
using System.Collections;

public class DailyRoutine : AIRoutine
{
    
    public Activity curActivity;
    public GameObject curTaskObject;
    public GameObject usedObject;

    public float patrolSpeed = 2f;
    public float patrolWaitTime = 1f;

    public int taskIndex;
    public float taskTime = 15;

    public bool breakAfterCycle = false;
    

    // Use this for initialization
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public override void StartAction()
    {
        // start first routine
        StartCoroutine(ActivityManager());
    }

    public override void FinishAction()
    {
        // break all activitys
        StopAllCoroutines();
        // FinishTask();
        // hide all current items
    }

    IEnumerator ActivityManager()
    {
        for (;;)
        {
            yield return new WaitForSeconds(1f);

            // wait for routines
            if(schedule.currentActivities.Count == 0)
            {
                Debug.LogWarning("no routines available");
                yield return new WaitUntil(() => schedule.currentActivities.Count > 0);
                Debug.LogWarning("continue dailys");
            }


            ChoseActivity();

            // perform activity
            for (taskIndex = 0; taskIndex < curActivity.taskObjects.Count; taskIndex++)
            {
                curTaskObject = curActivity.taskObjects[taskIndex];

                /*// go to task
                MoveToTask();

                // only patroll or interact?
                // if(curActivity.type == Activity.Type.Interact)
                // {
                // wait 
                WaitForFree();

                // start task
                StartTask();

                // do task
                DoTask();

                // finish task
                FinishTask();
                // }
                // else
                // {
                //      wait
                // }*/
            }
        }
    }

    void ChoseActivity()
    {
        // calculate activity to perform
        int activityIndex = Mathf.RoundToInt(Random.value * (schedule.currentActivities.Count - 1));
        Activity nextActivity = schedule.currentActivities[activityIndex];

        if (curActivity != nextActivity)
        {
            // Debug.Log("switch activity");

            // replace activity
            curActivity = nextActivity;
        }
    }
    /*
    IEnumerator MoveToTask()
    {
        nav.destination = curTaskObject.transform.position;

        // target not reached?
        float distanceToTask = 10f;
        while (distanceToTask < 2f)
        {
            distanceToTask = Vector3.Distance(this.transform.position, curTaskObject.transform.position);
            if (distanceToTask < 2f)
            {
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WaitForFree()
    {
        // wait until object is free
        while (curTaskObject.GetComponent<Used>().isUsed)
        {
            // in warteschlange anstellen

            // ... wait
            yield return new WaitForSeconds(2f);
        }
    }

    void StartTask()
    {
        // Debug.Log("start task");
        
        Transform startTransform = curTaskObject.transform.Find("UserStartTransform");

        Animator animUsed = curTaskObject.GetComponent<Animator>();
        curTaskObject.GetComponent<Used>().isUsed = true;
        usedObject = curTaskObject;

        // set pos,rot
        this.transform.rotation = startTransform.rotation;
        this.transform.position = new Vector3(startTransform.position.x, this.transform.position.y, startTransform.position.z);

        //start interaction
        int layerIndex = anim.GetLayerIndex(curTaskObject.name);
        anim.SetLayerWeight(layerIndex, 1);
        anim.SetInteger("interact", layerIndex);

        // object interacts with user
        if (animUsed)
        {
            animUsed.SetBool("used", true);
        }
    }

    IEnumerator DoTask()
    {
        bool moreActivitys = schedule.currentActivities.Count > 1;
        bool moreTasks = curActivity.taskObjects.Count > 1;

        for (;;)
        {
            // wait for task finished
            yield return new WaitForSeconds(taskTime);

            // calculate next task
            ChoseActivity();

            // break task?
            if (breakAfterCycle || moreTasks || moreActivitys)
            {
                break;
            }
        }
    }
    
    void FinishTask()
    {
        // stop interacting
        Debug.Log("task finished");
        Animator animUsed = null;
        if (usedObject)
        {
            animUsed = usedObject.GetComponent<Animator>();
        }
        anim.SetInteger("interact", 0);

        // object interacts with user
        if (animUsed)
        {
            animUsed.SetBool("used", false);
        }
        if (usedObject)
        {
            usedObject.GetComponent<Used>().isUsed = false;
        }
    }

    void ResetLayerWeight(string layerName)
    {
        anim.SetLayerWeight(anim.GetLayerIndex(layerName), 0);
        usedObject.GetComponent<Used>().isUsed = false;
        usedObject = null;
    }*/
}
