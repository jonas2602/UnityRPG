using UnityEngine;
using System.Collections;
using System;

public class DialogRoutine : AIRoutine
{
    private Transform partner;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void StartAction()
    {
        partner = transform.root.GetComponent<DialogManager>().dialogPartner.transform;

        StartCoroutine(LookAtPartner());
    }

    IEnumerator LookAtPartner()
    {
        for(;;)
        {
            Vector3 viewForward = partner.position - transform.position;
            viewForward.y = 0.0f; // kill Y
            viewForward = Vector3.Normalize(viewForward);

            Vector3 axisSign = Vector3.Cross(viewForward, avatar.forward);
            // charDirection = -axisSign.y;

            yield return null;
        }
    }

    public override void FinishAction()
    {
        StopCoroutine(LookAtPartner());
    }
}
