using UnityEngine;
using System.Collections;
using System;

public class ObserveRoutine: AIRoutine
{
    [SerializeField]
    private float lookTime;
    public float maxLookTime = 3f;

    public override void StartAction()
    {
        // wait at current point
        nav.Stop();

        // start watching
        StartCoroutine(Look());
    }

    public override void FinishAction()
    {
        // stop looking
        StopAllCoroutines();

        // continue moving
        nav.Resume();
    }

    IEnumerator Look()
    {
        for (;;)
        {
            if(aiInfos.targetInSight)
            {
                lookTime = 0f;
                // Debug.Log("target in sight");
            }
            // target not in sight?
            else
            {
                // continue looking for a while
                if (lookTime >= maxLookTime)
                {
                    // looking finished
                    aiInfos.personalLastSighting.position = aiInfos.resetPosition;
                }

                // Debug.Log("target not in sight");

                lookTime += Time.deltaTime;
            }

            // wait some time
            yield return null;
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
