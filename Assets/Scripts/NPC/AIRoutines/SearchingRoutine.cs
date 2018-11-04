using UnityEngine;
using System.Collections;
using System;

public class SearchingRoutine: AIRoutine
{
    private float maxSearchTime = 10f;

    public override void FinishAction()
    {
        StartCoroutine(Search());
    }

    public override void StartAction()
    {
        StopAllCoroutines();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

        // first reaching person goes in last known target direction
        // rest searches at current position

        // beta
        yield return new WaitForSeconds(maxSearchTime);

        // all spots watched and target is still not in sight
        aiInfos.personalLastSighting.position = aiInfos.resetPosition;
    }
}
