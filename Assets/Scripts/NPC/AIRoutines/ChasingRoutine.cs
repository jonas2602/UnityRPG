using UnityEngine;
using System.Collections;
using System;

public class ChasingRoutine : AIRoutine
{
    private float chasingSpeed;
    private float followStoppingDistance = 5f;

    public override void FinishAction()
    {
        StopAllCoroutines();

        nav.Resume();
    }

    public override void StartAction()
    {
        StartCoroutine(Follow());
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Follow()
    {
        for (;;)
        {
            // wait some time
            yield return new WaitForSeconds(0.25f);

            // position of player known
            if (aiInfos.personalLastSighting.position != aiInfos.resetPosition)
            {
                // move to position near player
                nav.SetDestination(aiInfos.personalLastSighting.position);

                // next edge of path is target?
                if (nav.steeringTarget == nav.destination)
                {
                    // near target
                    if (nav.remainingDistance <= followStoppingDistance)
                    {
                        // Debug.Log("target last known reached");

                        // target in sight?
                        if (aiInfos.targetInSight)
                        {
                            nav.Stop();
                        }
                        // target position lost
                        else
                        {
                            aiInfos.targetPositionLost = true;
                        }

                    }
                }

                // continue following
                else
                {
                    nav.Resume();
                    // Debug.Log("following target");
                }
            }
        }
    }
}
