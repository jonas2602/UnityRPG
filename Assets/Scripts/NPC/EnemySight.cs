using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemySight : MonoBehaviour
{
    private Transform avatar;
    private Transform head;
    private UnityEngine.AI.NavMeshAgent nav;                       // Reference to the NavMeshAgent component.
    private Relations relations;
    private Animator anim;                     // Reference to the Animator.
    private PlayerAttributes attributes;
    private EquipmentManager equipmentManager;
    private AggrobarScript aggroBarScript;
    private NPCAttributes aiInfos;
    private LastPlayerSighting lastPlayerSighting;  // Reference to last global sighting of the player.
    
    public List<Transform> enemys = new List<Transform>();
    public List<Transform> npcs = new List<Transform>();

    [SerializeField]
    private float scanWaitTime = 1f;
    [SerializeField]
    private float scanRadius = 10f;

    public void TellEnemySight(GameObject enemy, Vector3 position)
    {
        // has no target
        if(!attributes.target)
        {
            attributes.target = enemy;
            aiInfos.personalLastSighting.position = position;
        }
        // has enemy as target
        else if(enemy == attributes.target)
        {
            // don't know where target is 
            if(!aiInfos.targetInSight)
            {
                aiInfos.personalLastSighting.position = position;
            }
        }
    }

    public bool TellEnemySearch(GameObject enemy)
    {
        // searched enemy in sight?
        if(enemys.Contains(enemy.transform))
        {
            return true;

            // tell position
        }
        else
        {
            // searched enemy not in sight
            return false; 
        }
    }

    void Awake()
    {
        // Setting up the references.
        avatar = transform.root;

        head = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Hals/Bones|Kopf");
        nav = avatar.GetComponent<UnityEngine.AI.NavMeshAgent>();
        relations = GameObject.FindWithTag("Relations").GetComponent<Relations>();
        anim = avatar.GetComponent<Animator>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        equipmentManager = avatar.GetComponent<EquipmentManager>();
        aiInfos = GetComponent<NPCAttributes>();
        lastPlayerSighting = GameObject.FindWithTag("EnemyManager").GetComponent<LastPlayerSighting>();
        aggroBarScript = avatar.GetComponentInChildren<AggrobarScript>(true);

        EnvironmentScanner scanner = avatar.GetComponentInChildren<EnvironmentScanner>();
        enemys = scanner.enemys;
        npcs = scanner.allies;

        // Set the personal sighting and the previous sighting to the reset position.
        aiInfos.personalLastSighting.position = lastPlayerSighting.resetPosition;
        aiInfos.globalLastSighting.position = lastPlayerSighting.resetPosition;
    }

    void Start()
    {
        // check environment
        // StartCoroutine(ScanEnvironment());
        // calculate target
        StartCoroutine(CalculateTarget());
        // target in sight?
        StartCoroutine(UpdateSighting());
        // update aggro
        StartCoroutine(UpdateAggro());
    }
    
    public void SetAggro(float newAggro)
    {
        aiInfos.aggro = Mathf.Clamp01(newAggro);
        // Debug.Log(aiInfos.aggro);
        anim.SetInteger("hasAggro", (int)aiInfos.aggro);

        aggroBarScript.UpdateAggro(aiInfos.aggro);

        // tell target he has no longer aggro
        if (aiInfos.aggro == 0)
        {
            // weapon armed?
            if (equipmentManager.WeaponSetArmed())
            {
                Debug.Log("hide weapon");
                equipmentManager.DrawHideWeaponSet();
            }
        }

        // has aggro & weapon unarmed?
        if (aiInfos.aggro > 0.5f && !equipmentManager.WeaponSetArmed())
        {
            Debug.Log("draw weapon");
            equipmentManager.DrawHideWeaponSet();
        }

        // tell target he has aggro
        if (aiInfos.aggro == 1)
        {
            attributes.target.GetComponent<CombatEvents>().SetAggroEnemy(1, avatar.gameObject);
        }
    }

    public void TargetLost()
    {
        attributes.target = null;
        aiInfos.targetInSight = false;
        SetAggro(0);
    }
    
    /*IEnumerator ScanEnvironment()
    {
        int creatureLayer = LayerMask.NameToLayer("Creature");
        int environmentLayer = LayerMask.NameToLayer("Environment");
        // Debug.Log("CreatureLayer: " + creatureLayer + " EnvironmentLayer: " + environmentLayer);

        for (;;)
        {
            // objects = new List<Transform>();
            enemys = new List<Transform>();
            npcs = new List<Transform>();
            

            Collider[] hitColliders = Physics.OverlapSphere(avatar.position, scanRadius, ~environmentLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Transform curTransform = hitColliders[i].transform.root;

                // ignore myself
                if (curTransform == avatar)
                {
                    continue;
                }
                // Debug.Log(hitColliders[i].name);
                
                // check for creatures
                if (curTransform.gameObject.layer == creatureLayer)
                {


                    // is alive?
                    if (curTransform.GetComponent<Health>().condition == Health.Condition.Healthy)
                    {
                        // friend ...
                        if (!(relations.GetFractionRelation(avatar.gameObject, curTransform.gameObject) == Relation.Enemy))
                        {
                            npcs.Add(curTransform);
                        }
                        // ... or foe
                        else
                        {
                            enemys.Add(curTransform);
                        }
                    }
                }
            }

            // wait 0.5 sec
            yield return new WaitForSeconds(scanWaitTime);
        }
    }
    */

    float CalculatePathLength(Vector3[] allWayPoints)
    {
        // Create a float to store the path length that is by default 0.
        float pathLength = 0;

        // Increment the path length by an amount equal to the distance between each waypoint and the next.
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }
    
    public Vector3[] CalculatePath(Vector3 targetPosition)
    {
        // Create a path and set it based on a target position.
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        if (nav.enabled)
        {
            nav.CalculatePath(targetPosition, path);
        }
        // Create an array of points which is the length of the number of corners in the path + 2.
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        // The first point is the enemy's position.
        allWayPoints[0] = avatar.position;

        // The last point is the target position.
        allWayPoints[allWayPoints.Length - 1] = targetPosition;

        // The points inbetween are the corners of the path.
        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        return allWayPoints;
    }
    
    void HearSth(Vector3 position)
    {
        float volume = 0;
        float length = CalculatePathLength(CalculatePath(position));

        if (length <= 20)
        {
            volume = 1 - length * 0.05f;
        }

        aiInfos.aggro = volume * aiInfos.aggressive;
        Mathf.Clamp(aiInfos.aggro, 0, 1);
    }

    IEnumerator CalculateTarget()
    {
        for (;;)
        {
            // wait some time
            yield return new WaitForSeconds(2f);

            // has no target & no enemys in sight
            if (!attributes.target && enemys.Count == 0)
            {
                // break calculation
                continue;
            }

            // get target with highest priority
            Transform prioTarget = null;
            float bestAngle = 180f;
            for (int i = 0; i < enemys.Count; i++)
            {
                // target in sight?
                if(!TargetInSight(enemys[i]))
                {
                    continue;
                }

                Vector3 toOpponent = enemys[i].position - avatar.position;
                float angle = Vector3.Angle(avatar.forward, toOpponent);
                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    prioTarget = enemys[i];
                }
            }

            bool changeTarget = true;

            // has target
            if (attributes.target)
            {
                // is it usefull to change target?
                if (aiInfos.personalLastSighting.position != aiInfos.resetPosition)
                {
                    // target not lost
                    changeTarget = false;
                }
            }

            if (changeTarget)
            {
                // has old target?
                if (attributes.target)
                {
                    attributes.target.GetComponent<CombatEvents>().SetAggroEnemy(-1, avatar.gameObject);
                }

                // set target with highest priority
                // Debug.Log(prioTarget);
                attributes.target = prioTarget ? prioTarget.gameObject : null;
                anim.SetBool("hasTarget", prioTarget);

                // has new target?
            }
        }
    }
    
    IEnumerator UpdateSighting()
    {
        for (;;)
        {
            yield return new WaitForSeconds(0.25f);

            aiInfos.targetInSight = false;

            // has no target
            if (!attributes.target)
            {
                // Debug.Log("has no target");
                continue;
            }

            // target in sight?
            if (TargetInSight(attributes.target.transform))
            {
                aiInfos.targetInSight = true;

                // last known position?
                Transform target = attributes.target.transform;
                aiInfos.personalLastSighting.SetSighting(target.position, target.forward);
                aiInfos.targetPositionLost = false;
            }
        }
    }

    public bool TargetInSight(Transform target)
    {
        Vector3 toTarget = target.position - avatar.position;

        // is in fieldofview
        float angle = Vector3.Angle(avatar.forward, toTarget);
        if (angle > aiInfos.fieldOfViewAngle)
        {
            // Debug.Log("target out of viewAngle");
            return false;
        }
        // is an obstacle in the way
        RaycastHit hitInfo;
        Physics.Linecast(avatar.position, target.position, out hitInfo);
        if (hitInfo.transform.root != target)
        {
            // Debug.Log("target behind obstacle(" + hitInfo.transform.root + ")");
            return false;
        }
        // is in viewDistance
        if (Vector3.Magnitude(toTarget) > aiInfos.sightRadius)
        {
            // Debug.Log("target to far away");
            return false;
        }

        return true;
    }

    public void GetDamage(DamageInfo info)
    {
        // Debug.Log("enemySight react to damage");

        // update aggro
        if (aiInfos.aggro < 0.9f)
        {
            SetAggro(0.9f);
        }
    }

    IEnumerator UpdateAggro()
    {
        for (;;)
        {
            yield return null;
            
            // has target?
            if(!attributes.target && aiInfos.aggro == 0)
            {
                continue;
            }

            // target in sight?
            if (aiInfos.targetInSight)
            {
                // getting aggro?
                float newAggro = aiInfos.aggro + Time.deltaTime * 0.1f * aiInfos.aggressive;
                SetAggro(newAggro);
            }
            // target not in sight
            else
            {
                // player position lost
                if (aiInfos.personalLastSighting.position == aiInfos.resetPosition)
                {
                    // reset aggro
                    SetAggro(0);
                }
                // only interested?
                else if (aiInfos.aggro <= 0.5f)
                {
                    // lose aggro?
                    SetAggro(aiInfos.aggro - Time.deltaTime * 0.1f * aiInfos.aggressive);
                }
                // search for player
                else
                {
                    // dye aggroBar grey
                }
            }
        }
    }
}
