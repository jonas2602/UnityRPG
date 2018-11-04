using UnityEngine;
using System.Collections;

public class InteractingRoutine : AIRoutine
{
    public float taskTime = 15;
    public Activity curActivity;
    public Transform curTaskObject;
    
    public string status;

    public override void StartAction()
    {
        StartCoroutine(ActivityController());
    }

    public override void FinishAction()
    {
        StopAllCoroutines();
        FinishTask();
    }
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator ActivityController()
    {
        for(;;)
        {
            curActivity = schedule.activeActivity;
            bool moreTasks = (curActivity.taskObjects.Count > 0);
            
            // do one round
            for (; schedule.taskIndex < curActivity.taskObjects.Count; schedule.taskIndex++)
            {
                curTaskObject = curActivity.taskObjects[schedule.taskIndex].transform;

                status = "MoveToTask";
                yield return MoveToTask();

                status = "WaitForTask";
                yield return WaitForFree();

                status = "DoingTask";
                StartTask();

                for (;;)
                {
                    yield return new WaitForSeconds(taskTime);

                    // break task?
                    if(moreTasks || (schedule.optActivity != curActivity))
                    {
                        break;
                    }
                }

                status = "Task Finished";
                FinishTask();
            }

            schedule.ChangeActivity();
        }
    }

    IEnumerator MoveToTask()
    {
        // get target
        Transform startTransform = curTaskObject.Find("UserStartTransform");

        // start moving
        nav.destination = startTransform.position;

        for (;;)
        {
            // calculate distance
            Vector3 toTask = avatar.position - startTransform.position;
            toTask.y = 0;

            float distanceToTask = toTask.magnitude;

            // target reached?
            if (distanceToTask < 1f)
            {
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator WaitForFree()
    {
        // join queue
        while (!curTaskObject.GetComponent<Used>().StartInteracting())
        {
            // wait until object is free
            yield return new WaitForSeconds(0.5f);
        }
    }

    void StartTask()
    {
        // Debug.Log("start task");
        nav.Stop();
        Transform startTransform = curTaskObject.Find("UserStartTransform");

        // set pos,rot
        avatar.rotation = startTransform.rotation;
        avatar.position = new Vector3(startTransform.position.x, avatar.position.y, startTransform.position.z);

        // start interaction
        int layerIndex = anim.GetLayerIndex(curTaskObject.name);
        anim.SetLayerWeight(layerIndex, 1);
        anim.SetInteger("interact", layerIndex);
    }

    /*
    IEnumerator DoTask()
    {
        bool moreTasks = schedule.activeActivity.taskObjects.Count > 1;

        for (;;)
        {
            // wait for task finished
            yield return new WaitForSeconds(taskTime);

            // break task?
            if (schedule.activeActivity != schedule.optActivity || moreTasks)
            {
                break;
            }
        }
    }*/

    void FinishTask()
    {
        // stop interacting
        // Debug.Log("task finished");
        anim.SetInteger("interact", 0);
        
        if (curTaskObject)
        {
            curTaskObject.GetComponent<Used>().StopInteracting();
        }

        nav.Resume();
    }
}
