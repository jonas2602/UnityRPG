using UnityEngine;
using System.Collections;
using System;

public class GroupRoutine : AIRoutine
{
    private GroupManager groupManager;

    [SerializeField]
    private GroupStatus activeGroupStatus;
    [SerializeField]
    private Transform groupLeader;
    [SerializeField]
    private float positioningDistance = 2f;

    protected override void Awake()
    {
        base.Awake();

        groupManager = avatar.GetComponent<GroupManager>();
    }

    public override void StartAction()
    {
        
        groupLeader = groupManager.group.groupMember[0].transform;
        StartCoroutine(ActivityController());
    }

    public override void FinishAction()
    {
        StopAllCoroutines();
        nav.Resume();
    }
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ActivityController()
    {
        for (;;)
        {
            if(aiInfos.groupStatus != activeGroupStatus)
            {
                SwitchGroupStatus(aiInfos.groupStatus);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void SwitchGroupStatus(GroupStatus newStatus)
    {
        StopCoroutine(activeGroupStatus.ToString());

        // Debug.Log("Stop: " + activeGroupStatus + " , Start: " + newStatus);

        StartCoroutine(newStatus.ToString());
        activeGroupStatus = newStatus;
    }

    IEnumerator Follow()
    {
        // follow groupLeader
        nav.Resume();

        for (;;)
        {
            // get position near leader
            Vector3 newDestination = groupLeader.position + groupLeader.right * positioningDistance;

            // go to position
            nav.SetDestination(newDestination); 
            yield return null;
        }
    }

    IEnumerator Wait()
    {
        // wait at current point
        nav.Stop();

        for (;;)
        {
            yield return new WaitForSeconds(60f);
        }
    }

}

public enum GroupStatus
{
    Wait,
    Follow
}
