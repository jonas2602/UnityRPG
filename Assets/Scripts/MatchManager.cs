using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour {
    /*
    public struct ArenaFighter 
    {
        public bool healthy;
        public GameObject fighter;
        public bool entered;

        public ArenaFighter(GameObject fighter)
        {
            this.fighter = fighter;
            this.entered = false;
            this.healthy = true;
        }
    }

    public struct ArenaTeam
    {
        public bool lost;
        public ArenaFighter[] fighters;

        public ArenaTeam(ArenaFighter[] fighters)
        {
            this.fighters = fighters;
            this.lost = false;
        }
    }
    */

    public SphereCollider arenaSpace;
    public Questlog questlog;
    // public ArenaTeam[] teams = null;
    public Group[] groups = null;
    public ItemStack[] rewards = null;
    public Quest quest = null;
    public int partIndex;
    public MatchStatus status = MatchStatus.Finised;

    public enum MatchStatus
    {
        InProgress,
        Running,
        Finised
    }

	// Use this for initialization
	void Awake () 
    {
        arenaSpace = GetComponent<SphereCollider>();
    }
	
	// Update is called once per frame
    void Update()
    {
        if (groups.Length > 0)
        {
            // match is in progress
            if (status == MatchStatus.InProgress)
            {
                // check if all fighters have entered the arena
                int layerMask = 1 << 8;
                Collider[] hitColliders = Physics.OverlapSphere(arenaSpace.transform.position, arenaSpace.radius, layerMask);
                // Debug.DrawRay(arenaSpace.transform.position, new Vector3(arenaSpace.radius, 0, 0), Color.blue);
                // Debug.Log(arenaSpace.center + ", " + arenaSpace.radius);
                
                // count of fighters
                int fightersCount = 0;
                for(int i = 0; i< groups.Length;i++)
                {
                    fightersCount += groups[i].groupMember.Count;
                }

                // ... entered
                int entered = 0;
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    // Debug.Log(hitColliders[i].GetComponent<Attribute>().match + ", " + GetComponent<MatchManager>());
                    if (hitColliders[i].gameObject.GetComponent<GroupManager>().match == GetComponent<MatchManager>() && !hitColliders[i].isTrigger)
                    {
                        entered++;
                        // Debug.Log(hitColliders[i].name);
                    }
                }

                Debug.Log("fightersCount: " + fightersCount + " entered: " + entered);
  
                if(entered == fightersCount)
                {
                    // start match
                    StartMatch();
                    status = MatchStatus.Running;
                }
            }
            if (status == MatchStatus.Running)
            {
                // check if any fighter leaves the arena ...
                // if()
                {
                    // ... his team loses the fight
                }

                // check if only one team has healthy fighters ...
                int healthyTeamCount = 0;
                Group healthyTeam = null;

                for (int i = 0; i < groups.Length; i++)
                {
                    Health.Condition[] conditions = groups[i].GetConditions;
                    for (int j = 0; j < conditions.Length; j++)
                    {
                        if (conditions[j] == Health.Condition.Healthy)
                        {
                            healthyTeamCount++;
                            healthyTeam = groups[i];
                            break;
                        }
                    }
                }
                // only one team healthy?
                if(healthyTeamCount == 1)
                {
                    status = MatchStatus.Finised;
                    
                    // update quest
                    if(quest != null)
                    {
                        UpdateQuest(healthyTeam);
                    }

                    // distribute profits
                    if (rewards.Length > 0)
                    {
                        DistributeRewards(healthyTeam);
                    }
                }
            }
        }
    }


    // setup next match
    public void SetupMatch(Group[] groups, ItemStack[] rewards, Quest quest, int partIndex)
    {
        // there is no match running
        if (status == MatchStatus.Finised)
        {
            this.groups = groups;
            this.rewards = rewards;
            this.quest = quest;
            this.partIndex = partIndex;

            for(int i = 0; i < groups.Length;i++)
            {
                for(int j = 0; j < groups[i].groupMember.Count;j++)
                {
                    groups[i].groupMember[j].GetComponent<GroupManager>().match = GetComponent<MatchManager>();
                }
            }
            questlog = groups[0].groupMember[0].transform.Find("Interface").GetComponent<Questlog>();
            status = MatchStatus.InProgress;
        }
    }


    // setup next match
    public void SetupMatch(Group[] groups, ItemStack[] rewards)
    {
        // there is no match running
        if (status == MatchStatus.Finised)
        {
            this.groups = null;
            this.rewards = rewards;

            for (int i = 0; i < groups.Length; i++)
            {
                for (int j = 0; j < groups[i].groupMember.Count; j++)
                {
                    groups[i].groupMember[j].GetComponent<GroupManager>().match = GetComponent<MatchManager>();
                }
            }
        }

        status = MatchStatus.InProgress;
    }


    public void StartMatch()
    {
        status = MatchStatus.Running;
        for (int i = 0; i < groups.Length; i++)
        {
            for (int j = 0; j < groups[i].groupMember.Count; j++)
            {
                groups[i].groupMember[j].GetComponent<Health>().mortal = false;
            }
        }
    }


    /*
    // anyone enters the arena
    void OnTriggerEnter(Collider other)
    {
        // match is in progress
        if(status == MatchStatus.InProgress)
        {
            // check if fighter
            Vector2 personInfo = IsFighter(other.gameObject);
            if(personInfo.x != -1)
            {
                teams[(int)personInfo.x].fighters[(int)personInfo.y].entered = true;

                // check if all fighters have entered the arena
                if(AllFightersIn())
                {
                    // start match
                    StartMatch();
                }
            }
        }
    }

    
    // anyone leaves the arena
    void OnTriggerExit(Collider other)
    {
        // match is going
        if(status == MatchStatus.Running)
        {
            // check if any fighter leaves the arena ...
            Vector2 personInfo = IsFighter(other.gameObject);
            if(personInfo.x != -1)
            {
                // ... his team loses the match
                teams[(int)personInfo.x].lost = true;
            }
        }
    }
    */


    // check everyone in the arena
    void OnTriggerStay(Collider other)
    {
        // match is going
        if (status == MatchStatus.Running)
        {

        }
    }


    // is fighter?
    public bool IsFighter(GameObject person)
    {
        Group group = person.GetComponent<GroupManager>().group;

        for (int i = 0; i < groups.Length; i++)
        {
            if(groups[i] == group)
            {
                return true;
            }
        }

        return false;
        /*
        // go through teams
        for (int i = 0; i < teams.Length; i++)
        {
            // go through teamMembers
            for (int j = 0; j < teams[i].fighters.Length; j++)
            {
                if (teams[i].fighters[j].fighter == person)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return new Vector2(-1, -1);
        */
    }

    /*
    bool AllFightersIn()
    {
        // go through teams
        for (int i = 0; i < teams.Length; i++)
        {
            // go through teamMembers
            for (int j = 0; j < teams[i].fighters.Length; j++)
            {
                if (teams[i].fighters[j].entered == false)
                {
                    return false;
                }
            }
        }
        return true;
    }


    bool OneTeamHealthy(ref int healthyTeam)
    {
        int teamsHealthy = 0;
        for (int i = 0; i < teams.Length; i++)
        {
            int healthyMembers = 0;
            for (int j = 0; j < teams[i].fighters.Length; j++)
            {
                if (teams[i].fighters[j].fighter.GetComponent<Health>().condition == Health.Condition.Healthy)
                {
                    healthyMembers++;
                }
            }
            if (healthyMembers > 0)
            {
                teamsHealthy++;
                healthyTeam = i;
            }
        }
        if(teamsHealthy == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    */
    // check if finished

    // finish match


    void UpdateQuest(Group healthyTeam)
    {
        for (int i = 0; i < healthyTeam.groupMember.Count; i++)
        {
            if (healthyTeam.groupMember[i].tag == "Player")
            {
                questlog.SetPartProgress(quest, partIndex);
            }
        }
    }


    void DistributeRewards(Group winningTeam)
    {
        /*for(int i = 0; i< winningTeam.groupMember.Count;i++)
        {
            winningTeam.groupMember[i].GetComponent<>
            for(int j = 0; j< rewards.Length;j++)
            {

            }
        }*/
    }

}
