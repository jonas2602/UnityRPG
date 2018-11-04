using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSight : MonoBehaviour
{
    struct Target
    {
        public Transform avatar;
        public float angle;
        public TargetInformationType infoType;

        public Target(Transform avatar, float angle, TargetInformationType infoType)
        {
            this.avatar = avatar;
            this.angle = angle;
            this.infoType = infoType;
        }
    }

    private Transform avatar;
    private TargetInfoScript targetInfo;
    private CharakterControler characterController;
    private PlayerAttributes attributes;
    private Camera cam;
    private Animator anim;
    private AnimInfo animInfo;
    private Relations relations;

    [SerializeField]
    private float scanWaitTime = 2f;
    [SerializeField]
    private float updateTargetWaitTime = 0.5f;
    [SerializeField]
    private float scanRadius = 10f;
    [SerializeField]
    private float interactRadius = 5f;
    [SerializeField]
    private float maxInteractCharAngle = 100f;
    [SerializeField]
    private float maxInteractCamAngle = 20f;
    [SerializeField]
    public float targetRadius = 10f;

    // combat
    [SerializeField]
    private List<Transform> enemys = new List<Transform>();
    private Transform nextEnemy;
    // private Transform activeEnemy;

    private IEnumerator targetInfoRoutine;


    // interacting
    [SerializeField]
    private List<Transform> npcs = new List<Transform>();
    [SerializeField]
    private List<Transform> objects = new List<Transform>();
    // private GameObject bestObject;
    // private GameObject usedObject;

    public bool GetInteractable
    {
        get
        {
            return targetInfo.GetInteractable;
        }
    }

    void Awake()
    {
        avatar = transform.root;

        targetInfo = GameObject.Find("ObjectInfo").GetComponent<TargetInfoScript>();
        targetInfo.gameObject.SetActive(false);

        cam = GameObject.FindWithTag("MainCamera").GetComponentInChildren<Camera>();
        characterController = GetComponent<CharakterControler>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        anim = avatar.GetComponent<Animator>();
        animInfo = avatar.GetComponent<AnimInfo>();
        relations = GameObject.FindWithTag("Relations").GetComponent<Relations>();

        EnvironmentScanner scanner = avatar.GetComponentInChildren<EnvironmentScanner>();
        enemys = scanner.enemys;
        npcs = scanner.allies;
        objects = scanner.interactables;
    }


    // Use this for initialization
    void Start()
    {
        // StartCoroutine(ScanEnvironment());
        StartCoroutine(ChoseTarget());
    }

    // Update is called once per frame
    void Update()
    {
        // what is near me
        // sort out things behind me (only objects not creatures there show symbol)
        // what has highest priority (infight only show enemys else show everything)
        // interactable(show "e") or to far away(show symol when npc else nothing)

        // when npc out of interactionrange hide "e"

        Vector3 interestDirection = characterController.GetMoveDirection;
        Vector3 rootPosition = avatar.position;
        if (interestDirection == Vector3.zero)
        {
            interestDirection = cam.transform.forward;
            rootPosition = cam.transform.position;
        }

        Debug.DrawRay(avatar.position, interestDirection, Color.black);
    }

    /*
    IEnumerator ScanEnvironment()
    {
        int creatureLayer = LayerMask.NameToLayer("Creature");
        int environmentLayer = LayerMask.NameToLayer("Environment");
        // Debug.Log("CreatureLayer: " + creatureLayer + " EnvironmentLayer: " + environmentLayer);

        for (;;)
        {
            objects = new List<Transform>();
            npcs = new List<Transform>();
            enemys = new List<Transform>();

            Collider[] hitColliders = Physics.OverlapSphere(avatar.position, scanRadius, ~environmentLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Transform curTransform = hitColliders[i].transform.root;

                // ignore myself
                if (curTransform == avatar.transform)
                {
                    continue;
                }
                // Debug.Log(hitColliders[i].name);

                // check for interactable
                string curTag = curTransform.tag;
                if (curTag == "Usable" || curTag == "Loot" || curTag == "Interactable")
                {
                    // Debug.Log("is interactable");
                    if (!objects.Contains(curTransform))
                    {
                        objects.Add(curTransform);
                    }
                }

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
    IEnumerator ChoseTarget()
    {
        for (;;)
        {
            Transform oldTarget = attributes.GetTargetTransform;
            List<Target> finalList = new List<Target>();

            // sort by type specific conditions
            // only out of combat chose interactable / npc
            if (animInfo.FreeMoveing())
            {
                ChoseOptObject(ref finalList);
                ChoseOptNpc(ref finalList);
            }
            // always chose enemy
            ChoseOptEnemy(ref finalList);

            // Debug.Log(finalList.Count);

            // sort final list by camAngle
            Target finalTarget = new Target();
            float bestAngle = -1;

            for (int i = 0; i < finalList.Count; i++)
            {
                float curAngle = finalList[i].angle;
                if (bestAngle == -1 || curAngle < bestAngle)
                {
                    finalTarget = finalList[i];
                }
            }

            // set final target
            // target has changed?
            if (oldTarget != finalTarget.avatar)
            {
                // reset current target
                if (oldTarget)
                {
                    // Debug.Log("reset target from " + oldTarget.name);
                    targetInfo.ResetTarget();
                }

                // has new target?
                if (finalTarget.avatar)
                {
                    // set new target
                    // Debug.Log("set target to " + finalTarget.name);
                    targetInfo.SetTarget(avatar, finalTarget.avatar, finalTarget.infoType, interactRadius);

                    if(finalTarget.infoType == TargetInformationType.Enemy)
                    {
                        anim.SetBool("hasTarget", true);
                    }
                }
                else
                {
                    anim.SetBool("hasTarget", false);
                }

                attributes.SetTarget = finalTarget.avatar;
            }

            yield return new WaitForSeconds(updateTargetWaitTime);
        }
    }


    void ChoseOptObject(ref List<Target> finalList/*, List<Transform> targetsList, float maxCamAngle, float maxCharAngle, float maxCharDistance*/)
    {
        Transform bestObject = null;
        float bestCamAngle = -1;
        // float bestCharDistance = scanRadius;

        for (int i = 0; i < objects.Count; i++)
        {
            Vector3 camDir = cam.transform.forward;
            Vector3 charDir = avatar.forward;
            Vector3 camToObj = objects[i].position - cam.transform.position;
            Vector3 charToObj = objects[i].position - avatar.position;

            // ignore things out of interactionRange
            if (Vector3.Distance(avatar.position, objects[i].position) > interactRadius)
            {
                continue;
            }

            // ignore things behind character
            if (Vector3.Angle(charDir, charToObj) > maxInteractCharAngle)
            {
                continue;
            }

            // chose object with best angle
            float camAngle = Vector3.Angle(camDir, camToObj);
            if (bestCamAngle == -1 || camAngle < bestCamAngle)
            {
                if (camAngle < maxInteractCamAngle)
                {
                    bestObject = objects[i];
                    bestCamAngle = camAngle;
                }
            }
        }

        // target found?
        if (bestObject)
        {
            finalList.Add(new Target(bestObject, bestCamAngle, TargetInformationType.Interactable));
        }
    }

    void ChoseOptNpc(ref List<Target> finalList)
    {
        Transform bestObject = null;
        float bestCamAngle = -1;

        for (int i = 0; i < npcs.Count; i++)
        {
            Vector3 camDir = cam.transform.forward;
            Vector3 camToNpc = npcs[i].position - cam.transform.position;

            // chose object with best angle
            float camAngle = Vector3.Angle(camDir, camToNpc);
            if (bestCamAngle == -1 || camAngle < bestCamAngle)
            {
                if (camAngle < maxInteractCamAngle)
                {
                    bestObject = npcs[i];
                    bestCamAngle = camAngle;
                }
            }
        }

        // target found?
        if (bestObject)
        {
            finalList.Add(new Target(bestObject, bestCamAngle, TargetInformationType.Interactable));
        }
    }

    void ChoseOptEnemy(ref List<Target> finalList)
    {
        Transform bestObject = null;
        float bestAngle = -1;

        Vector3 interestDirection = characterController.GetMoveDirection;
        Vector3 rootPosition = avatar.position;
        if (interestDirection == Vector3.zero || characterController.combatMode == CombatMode.Focused)
        {
            interestDirection = cam.transform.forward;
            rootPosition = cam.transform.position;
        }
        
        // Debug.DrawRay(avatar.position, interestDirection, Color.black);

        for (int i = 0; i < enemys.Count; i++)
        {
            Vector3 charToEnemy = enemys[i].position - rootPosition;

            // chose object with best angle
            float interestAngle = Vector3.Angle(interestDirection, charToEnemy);
            if (bestAngle == -1 || interestAngle < bestAngle)
            {
                bestObject = enemys[i];
                bestAngle = interestAngle;
            }
        }

        // target found?
        if (bestObject)
        {
            finalList.Add(new Target(bestObject, bestAngle, TargetInformationType.Enemy));
        }
    }

    void ChoseActiveTarget()
    {/*
        // has target
        if (activeEnemy)
        {
            // activeEnemy is out of range 
            if (Vector3.Distance(activeEnemy.position, this.transform.position) > targetRadius)
            {
                ResetTarget(ref activeEnemy, activeEnemyRing);
            }
            // active enemy is dead ...
            else if (activeEnemy.GetComponent<Health>().condition != Health.Condition.Healthy)
            {
                // ... has more "aggros"
                if (aggroEnemy.Count > 0)
                {
                    // switch aggro
                    SetTarget(ref activeEnemy, nextEnemy, activeEnemyRing);
                }
                // ... has no more "aggros"
                else
                {
                    // reset aggro
                    ResetTarget(ref activeEnemy, activeEnemyRing);

                    // hide mainWeapon
                    if (anim.GetInteger("armedMainWeapon") != -1)
                    {
                        anim.SetBool("hideMain", true);
                        anim.SetBool("drawMain", false);
                    }
                    // hide offWeapon
                    int armedOffWeapon = anim.GetInteger("currentOffWeapon");
                    if (armedOffWeapon != -1 && armedOffWeapon <= 18)
                    {
                        anim.SetBool("hideOff", true);
                        anim.SetBool("drawOff", false);
                    }
                }
            }
        }
        // has no target
        else
        {
            // has aggro
            if (aggroEnemy.Count > 0)
            {
                // set aggro
                SetTarget(ref activeEnemy, aggroEnemy[0].transform, activeEnemyRing);

                // draw weapon
                if (anim.GetInteger("currentMainWeapon") != 4)
                {
                    // animChar.SetBool("draw", true);
                }
            }
        }

        // Press Target Button
        if (Input.GetButtonDown("Target"))
        {
            // chose active target
            if (activeEnemy == null)
            {
                if (nextEnemy != null)
                {
                    SetTarget(ref activeEnemy, nextEnemy, activeEnemyRing);
                }
            }
            // unset active target
            else
            {
                // ResetTarget(ref activeEnemy, activeEnemyRing);
            }
        }*/
    }

    void ChoseNextTarget()
    {/*
        float bestAngle = -1f;
        Transform bestCreature = null;

        for (int i = 0; i < enemys.Count; i++)
        {
            Vector3 toEnemy = enemys[i].transform.position - this.transform.position;
            // Debug.DrawRay(this.transform.position, toEnemy, Color.magenta);
            // Debug.Log(hitColliders[i].gameObject.name + Vector3.Magnitude(toEnemy));
            float angle = Vector3.Angle(cam.transform.forward, toEnemy);

            if (angle < bestAngle || bestAngle == -1f)
            {
                bestCreature = enemys[i];
                bestAngle = angle;
            }
        }

        // exists next target
        if (bestAngle < 40f && bestCreature != null)
        {
            // new target equals current target
            if (nextEnemy != bestCreature)
            {
                SetTarget(ref nextEnemy, bestCreature, nextEnemyRing);
            }
        }
        else
        {
            ResetTarget(ref nextEnemy, nextEnemyRing);
        }*/
    }

    /*
    //interacting
    public void DrawButton()
    {
        if (bestObject && Locomotion())
        {
            Vector3 buttonPos = bestObject.transform.position;
            buttonPos.y += 1f;
            Vector3 screenPos = cam.WorldToScreenPoint(buttonPos);
            Rect buttonRect = new Rect(screenPos.x - 12.5f, Screen.height - screenPos.y, 25f, 25f);

            GUI.Box(buttonRect, "E", skin.GetStyle("Tooltip"));

            if (buttonRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0 && Event.current.type == EventType.mouseDown)
                {
                    Interact();
                }
            }
            if (Event.current.keyCode == KeyCode.E && Event.current.type == EventType.keyDown)
            {
                Interact();
            }
        }
    }
    */

    // animation Events
    void SetLayerWeight(string layerName)
    {
        StartCoroutine("ActivateLayer", new Vector3(anim.GetLayerIndex(layerName), 0.1f, 0.01f));
    }


    void ResetLayerWeight(string layerName)
    {
        StartCoroutine("DeactivateLayer", new Vector3(anim.GetLayerIndex(layerName), 0.1f, 0.01f));

        if (attributes.usedObject)
        {
            attributes.usedObject = null;
        }
    }
}

