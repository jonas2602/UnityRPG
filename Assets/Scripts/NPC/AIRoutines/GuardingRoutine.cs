using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardingRoutine : AIRoutine, AllyEnteredInfo
{
    [SerializeField]
    private float taskTime = 60;
    [SerializeField]
    private string status;

    private Transform curTaskObject;

    public override void StartAction()
    {
        StartCoroutine(ActivityController());
    }

    IEnumerator ActivityController()
    {
        for(;;)
        {
            curTaskObject = schedule.activeActivity.taskObjects[0].transform;
 
            status = "MoveToTask";
            yield return MoveToTask();

            status = "Guarding";
            StartTask();

            // yield return Guard();
            yield return new WaitForSeconds(taskTime);

            status = "Guarding Finished";
            FinishTask();
        }
    }

    public override void FinishAction()
    {
        StopAllCoroutines();
        FinishTask();
    }
    
    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    IEnumerator MoveToTask()
    {
        // start moving
        nav.destination = curTaskObject.position;

        for (;;)
        {
            // calculate distance
            Vector3 toTask = avatar.position - curTaskObject.position;
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

    void StartTask()
    {
        // Debug.Log("start task");
        nav.Stop();

        // set pos,rot
        avatar.rotation = curTaskObject.rotation;
        avatar.position = new Vector3(curTaskObject.position.x, avatar.position.y, curTaskObject.position.z);

        scanner.GetAlliesInfo(this);
    }


    /*IEnumerator Guard()
    {
        for (;;)
        {
            yield return new WaitForSeconds(1f);

            // break task?
            if (schedule.activeActivity != schedule.optActivity)
            {
                break;
            }
        }
    }*/

    void AllyEnteredInfo.AllyEntered(Transform ally, Relation relation)
    {
        // interested?
        if (status != "Guarding")
        {
            return;
        }

        // ally in front of me?
        Vector3 toAlly = ally.position - avatar.position;
        if (Vector3.Dot(toAlly, avatar.forward) > 0.5f)
        {
            // friend
            if (relation == Relation.Friendly)
            {
                // welcome ally
                // Line line = new Line(new List<LinePart>() { new LinePart(false, "Hello Sir") });
                // worldDialogOutput.SpeakText(line);
            }
            // neutral
            else
            {
                // start dialog
                avatar.GetComponent<DialogManager>().Address(ally.gameObject);
            }
        }
    }

    void FinishTask()
    {
        // Debug.Log("task finished");
        scanner.LeaveAlliesInfo(this);

        nav.Resume();
    }
}



public interface AllyEnteredInfo
{
    void AllyEntered(Transform ally, Relation relation);
}
