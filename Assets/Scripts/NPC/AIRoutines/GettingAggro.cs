using UnityEngine;
using System.Collections;

public class GettingAggro : AIRoutine
{
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
        // start aggroUpdating
        // StartCoroutine(UpdateAggro());

        // Find best inspecting method
        if(aiInfos.aggro < 0.5f)
        {
            // only interested
            StartSubroutine("Look");
        }
        else
        {
            // has some aggro
            StartSubroutine("Follow");
        }
    }

    IEnumerator Look()
    {
        // wait at current point
        nav.Stop();

        for (;;)
        {
            // do action
            // Create Vector from enemy to player
            Vector3 toTarget = attributes.target.transform.position - avatar.position;
            toTarget = Vector3.Normalize(toTarget);

            //Rotate enemy to player
            avatar.forward = toTarget;

            // wait some time
            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator Follow()
    {
        for (;;)
        {
            // do action
            // position near player
            if (Vector3.Distance(nav.destination, aiInfos.personalLastSighting.position) > 2f)
            {
                nav.SetDestination(aiInfos.personalLastSighting.position);
                nav.stoppingDistance = 5f;
                nav.Resume();
            }

            // wait some time
            yield return new WaitForSeconds(0.25f);

            // target lost?
            if (Vector3.Distance(avatar.position, aiInfos.personalLastSighting.position) < 1f)
            {
                StartSubroutine("Search");
                break;
            }
        }
    }

    IEnumerator Search()
    {
        // choose near points to search
        Vector3[] searchPoints = new Vector3[] { };
        // go through points and look for target
        for (int i = 0; i < searchPoints.Length; i++)
        {
            // go there

            // look around
            yield return null;
        }

        // all spots watched and target is still not in sight
        enemySight.TargetLost();
        // FinishAction();
    }
    
    IEnumerator UpdateAggro()
    {
        for(;;)
        {
            // target in sight?
            if (aiInfos.targetInSight)
            {
                // getting aggro?
                float newAggro = aiInfos.aggro + Time.deltaTime * 0.1f * aiInfos.aggressive;
                enemySight.SetAggro(newAggro);

                if (newAggro > 0.5f)
                {
                    // target search successfull or still only looking
                    if(curRoutine != "Follow")
                    {
                        StartSubroutine("Follow");
                    }

                    if(newAggro == 1)
                    {
                        // FinishAction();
                    }
                }
            }
            // target not in sight
            else
            {
                // only interested?
                if (aiInfos.aggro <= 0.5f)
                {
                    // lose aggro?
                    enemySight.SetAggro(aiInfos.aggro - Time.deltaTime * 0.1f * aiInfos.aggressive);
                    if (aiInfos.aggro == 0)
                    {
                        // FinishAction();
                    }
                }
                // player position lost
                else if (aiInfos.personalLastSighting.position == aiInfos.resetPosition)
                {
                    // follow to last known position
                    // look around
                    // lose aggro
                    enemySight.SetAggro(0);
                    break;
                }
            }
            yield return null;
        }
        /*
        // 2. has aggro
        while (aggro == 1)
        {
            // target in sight?
            if (targetInSight)
            {
                // update last known position
                personalLastSighting = curTarget.position;
            }
            else
            {
                // color aggrobar grey
                // follow to last known position
                // look around

                // player position lost
                if (personalLastSighting == lastPlayerSighting.resetPosition)
                {
                    // lose aggro
                    SetAggro(0);
                    break;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        */
    }

    public override void FinishAction()
    {
        // stop all inspecting routines
        StopAllCoroutines();
    }
}
