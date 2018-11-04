using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class CharakterControler : MonoBehaviour
{
    #region Variables

    //movement
    // Inspector serialized
    private Transform avatar;
    private Animator animChar;                              //  animator of char
    private AnimInfo animInfo;
    private CameraControllerNew camContr;                       //  cameraControler
    private Camera cam;
    private PlayerAttributes attributes;
    private EquipmentManager equipmentManager;
    private DialogManager dialogManager;
    private PlayerDialog playerDialog;
    private Health health;
    private ItemStorage itemStorage;
    private UIManager uiManager;
    private PlayerSight playerSight;
    private CombatEvents combatEvents;
    private CapsuleCollider capCollider;

    [SerializeField]
    private float directionSpeed = 5f;
    [SerializeField]
    private float directionDampTime = 0.15f;
    [SerializeField]
    private float speedDampTime = 0.2f;
    [SerializeField]
    private float distanceToGround;
    [SerializeField]
    private float heightOffset = 1.77f;
    [SerializeField]
    private float maxFallingTime = 5f;
    [SerializeField]
    private float fallingTimer = 0f;
    [SerializeField]
    private float checkDistance = 2;
    [SerializeField]
    private float rotationSpeed = 2f;


    // Private global only
    private float vertical = 0f;
    private float horizontal = 0f;
    public float charSpeed = 0f;
    private float charDirection = 0f;
    private float charAngle = 0f;
    private bool move = false;
    private Status status;

    private Vector3 charForward;
    private Vector3 stickForward;
    private Vector3 cameraForward;
    private Vector3 moveDirection;

    private AnimatorStateInfo animCharStateInfo;                //  Char AnimationStateInfo
    private AnimatorTransitionInfo animCharTransInfo;           //  Char AnimationTransitionInfo
    private const float SPRINT_SPEED = 2.0f;                    //max. SprintSpeed
    public MoveStates moveState = MoveStates.Free;

    // Hashes
    private int Move_ForwardId = 0;
    private int RunPivotLId = 0;
    private int RunPivotRId = 0;
    private int RunPivotLTransId = 0;
    private int RunPivotRTransId = 0;
    private int BowStrainId = 0;
    private int Move_ForwardStrainId = 0;
    private int LadenMove_ForwardId = 0;
    private int LadenStrainId = 0;
    private int SchussMoveForwardId = 0;
    private int DodgeRoleId = 0;
    private int DodgeStepId = 0;
    private int CombatMoveId = 0;
    private int SwordBlockId = 0;

    // layer
    int creatureLayer = -1;
    int interactableLayer = -1;

    //combat
    private ArrowController arrowContr;
    public float strainLerp = 0;
    public bool rotateBody = false;
    public CombatMode combatMode = CombatMode.Free;

    public int curStamina;

    /*       (MainWeapon, OffWeapon)       ->        (FunctionName, AnimatorLayerName) */
    public CombatTypeList combatTypes; 
    
    
    

    #endregion


    #region extern Functions

    public Vector3 GetMoveDirection
    {
        get
        {
            return this.moveDirection;
        }
    }

    public Animator Animator
    {
        get
        {
            return this.animChar;
        }
    }

    public ArrowController ArrowContr
    {
        set
        {
            arrowContr = value;
        }
    }

    public bool RotateBody
    {
        get
        {
            return rotateBody;
        }
    }

    public float Speed
    {
        get
        {
            return this.charSpeed;
        }
    }

    public float LocomotionThreshold
    {
        get
        {
            return 0.2f;
        }
    }

    public bool Armed
    {
        get
        {
            return animChar.GetInteger("armedMainWeapon") != -1;
        }
    }
    #endregion
    

    #region Locomotion Requests
    public bool IsInPivot()
    {
        return !(animCharStateInfo.fullPathHash == Move_ForwardId ||
                 animCharStateInfo.fullPathHash == CombatMoveId);
    }

    public bool IsInLocomotion()
    {
        return animCharStateInfo.fullPathHash == Move_ForwardId ||
               animCharStateInfo.fullPathHash == RunPivotLId ||
               animCharStateInfo.fullPathHash == RunPivotRId ||
               animCharTransInfo.fullPathHash == RunPivotLTransId ||
               animCharTransInfo.fullPathHash == RunPivotRTransId;
    }

    public bool InSwordPivot()
    {
        return !(animCharStateInfo.fullPathHash == CombatMoveId ||
                 animCharStateInfo.fullPathHash == SwordBlockId);
    }

    bool IsInCombat()
    {
        return animCharStateInfo.fullPathHash == CombatMoveId;
    }

    public bool IsStraining()
    {
        return animCharStateInfo.fullPathHash == BowStrainId;
    }
    
    /*public bool Shooting()
    {
        return animCharStateInfo.fullPathHash == ||
               animCharStateInfo.fullPathHash == ||
               animCharStateInfo.fullPathHash ==;
    }*/

    public bool Locomotion()
    {
        return animCharStateInfo.fullPathHash == Move_ForwardId;
    }

    public bool IsInscope()
    {
        return animCharStateInfo.fullPathHash == BowStrainId;
    }

    public bool GoesInscope()
    {
        return animCharTransInfo.fullPathHash == Move_ForwardStrainId ||
               animCharTransInfo.fullPathHash == LadenStrainId;
    }

    public bool LeaveScope()
    {
        return animCharTransInfo.fullPathHash == LadenMove_ForwardId ||
               animCharTransInfo.fullPathHash == SchussMoveForwardId;
    }

    bool IsDodging()
    {
        return animCharStateInfo.fullPathHash == DodgeRoleId ||
               animCharStateInfo.fullPathHash == DodgeStepId;
    }

    bool MayAttack()
    {
        return animCharStateInfo.fullPathHash == CombatMoveId ||
               animCharStateInfo.fullPathHash == Move_ForwardId;
    }

    bool MayBend()
    {
        return animCharStateInfo.fullPathHash == Move_ForwardId;
    }
    #endregion


    public enum MoveStates
    {
        RangedCombat,
        MeleeCombat,
        Free
    }

    public enum Status
    {
        Combat,
        Idle,
        Targeting,
        Interacting
    }


    // Use this for initialization
    void Awake()
    {
        avatar = transform.root;

        animChar = avatar.GetComponent<Animator>();
        animInfo = avatar.GetComponent<AnimInfo>();
        capCollider = avatar.GetComponent<CapsuleCollider>();
        health = avatar.GetComponent<Health>();
        dialogManager = avatar.GetComponent<DialogManager>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        equipmentManager = avatar.GetComponent<EquipmentManager>();
        itemStorage = avatar.GetComponent<ItemStorage>();
        uiManager = GameObject.FindWithTag("Interface").GetComponent<UIManager>();
        combatEvents = avatar.GetComponent<CombatEvents>();
        combatTypes = GameObject.FindWithTag("Database").GetComponent<CombatTypeList>();
        playerSight = GetComponent<PlayerSight>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        camContr = GameObject.FindWithTag("MainCamera").GetComponent<CameraControllerNew>();
        
        // Hash all animation names for performance
        Move_ForwardId = Animator.StringToHash("Fist.Move_Forward");
        RunPivotLId = Animator.StringToHash("Fist.RunPivotL");
        RunPivotRId = Animator.StringToHash("Fist.RunPivotR");
        RunPivotLTransId = Animator.StringToHash("Fist.Move_Forward -> Fist.RunPivotL");
        RunPivotRTransId = Animator.StringToHash("Fist.Move_Forward -> Fist.RunPivotR");
        BowStrainId = Animator.StringToHash("Bow.Move_Forward");
        SchussMoveForwardId = Animator.StringToHash("Bow.Bow.Bogen_schuss -> Bow.Move_Forward");
        Move_ForwardStrainId = Animator.StringToHash("Bow.Move_Forward -> Bow.Bow.Strain");
        LadenMove_ForwardId = Animator.StringToHash("Bow.Bow.Bogen_laden -> Bow.Move_Forward");
        LadenStrainId = Animator.StringToHash("Bow.Bow.Bogen_laden -> Bow.Bow.Strain");
        DodgeRoleId = Animator.StringToHash("Fist.Combat.Dodge_Role");
        DodgeStepId = Animator.StringToHash("Fist.Combat.Dodge_Step");
        CombatMoveId = Animator.StringToHash("Fist.Combat.Move_Sword");
        SwordBlockId = Animator.StringToHash("Fist.Combat.Move_Block");

        // setup layer
        creatureLayer = LayerMask.NameToLayer("Creature");
        interactableLayer = LayerMask.NameToLayer("Interactable");
    }


    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        // reset variables
        vertical = 0;
        horizontal = 0;

        // charAngle = 0f;
        charDirection = 0f;
        charSpeed = 0f;

        // interface open?
        if (uiManager.IsInMenu())
        {
            animChar.SetFloat("Speed", charSpeed, speedDampTime, Time.deltaTime);
            animChar.SetFloat("Direction", charDirection, directionDampTime, Time.deltaTime);
            animChar.SetFloat("Angle", charAngle);
            return;
        }

        animCharStateInfo = animChar.GetCurrentAnimatorStateInfo(0);
        animCharTransInfo = animChar.GetAnimatorTransitionInfo(0);

        // Landing
        if (GetGroundDistance() < 0.1f)
        {
            if (fallingTimer > maxFallingTime)
            {
                health.Damage(health.curHp);
            }
            fallingTimer = 0f;
            animChar.SetFloat("fallingTime", 0f);
        }
        // Falling
        else
        {
            fallingTimer += Time.deltaTime;
            animChar.SetFloat("fallingTime", fallingTimer);
        }

        Movement();

        // interact
        if (Input.GetButtonDown("Interact"))
        {
            // Debug.Log("try to interact with something");

            if (playerSight.GetInteractable)
            {
                // Debug.Log("there is something to interact with");
                Interact();
            }
        }

        Combat();
    }


    void Movement()
    {
        // get movement input
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        stickForward = new Vector3(horizontal, 0, vertical);

        // calculate movement Variables
        move = vertical != 0 || horizontal != 0;
        
        // aim for sth
        if (animInfo.IsAiming()) //IsInscope() )         // MoveStates.RangedCombat
        {
            if (horizontal != 0)
            {
                charDirection = horizontal > 0 ? 1f : -1f;
            }

            charSpeed = vertical;
        }
        else
        {
            // has aggro and is not running (infight)
            if (IsInCombat())                                                               // MoveStates.MeleeCombat
            {
                if (move)
                {
                    SmoothRotateToTarget();
                    
                    // get moveDirection in avatarspace
                    moveDirection = StickToAvatarspace(camContr.transform);

                    // convert to local space
                    Vector3 directionalMoveDirection = transform.InverseTransformDirection(moveDirection);

                    // update angle
                    charAngle = CalculateStickAngle();

                    //divide in animatorvariables
                    charSpeed = directionalMoveDirection.z;
                    charDirection = directionalMoveDirection.x;
                }
            }
            // no aggro or idle (out of fight) 
            else                                                                            // MoveStates.Free
            {
                // Translate controls stick coordinates into character space
                moveDirection = StickToAvatarspace(camContr.transform);
                // update speed
                float speedOut = stickForward.sqrMagnitude;
                charSpeed = (speedOut <= 1 ? speedOut : 1);
                
                // update angle
                charAngle = CalculateStickAngle();
                
                // update direction
                charDirection = charAngle / 180 * directionSpeed;
                
                // Press "Space" to Jump
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }

                // standing?
                if (charSpeed < LocomotionThreshold && Mathf.Abs(horizontal) < 0.05f)
                {
                    Idle();
                }
            }

            // Press "MouseR" to sprint
            if (Input.GetKey("left shift") && move)
            {
                // transform speed to SPRINT_SPEED
                charSpeed = SPRINT_SPEED;
            }
        }

        //Set Animator Variables
        // Debug.Log("Speed: " + charSpeed + " Direction: " + charDirection + " Angle: " + charAngle);
        animChar.SetFloat("Speed", charSpeed, speedDampTime, Time.deltaTime);
        animChar.SetFloat("Direction", charDirection, directionDampTime, Time.deltaTime);
        if (!IsInPivot())
        {
            animChar.SetFloat("Angle", charAngle);
        }
    }


    void Jump()
    {
        // check for walls
        float wallHeight = 0;
        // climb
        if (CheckForWall(ref wallHeight))
        {
            Debug.Log("climb");
            // animChar.SetFloat("climbHeight", wallHeight);
        }
        // jump
        else if (Locomotion())
        {
            Debug.Log("jump");
            animChar.SetBool("Jump", true);
        }

        /*
        // get character height to ground
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit))
        {
            distanceToGround = hit.distance - heightOffset;
        }*/
    }


    void Idle()
    {
        Vector3 viewForward;
        if (dialogManager.IsInDialog())
        {
            viewForward = dialogManager.dialogPartner.transform.position - transform.position;
            viewForward.y = 0.0f; // kill Y
            viewForward = Vector3.Normalize(viewForward);
        }
        else
        {
            viewForward = cam.transform.forward;
        }
        charForward = avatar.forward;

        //Cross Product of charDirection, viewDirection
        Vector3 axisSign = Vector3.Cross(viewForward, charForward);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), viewForward, Color.green);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), charForward, Color.red);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), axisSign, Color.blue);

        float viewAngle = Vector3.Angle(viewForward, charForward);
        if (viewAngle <= 100f)
        {
            charDirection = -axisSign.y;// Mathf.Clamp(viewAngle / 90, -1, 1);
        }
    }


    void Interact()
    {
        Transform target = attributes.target.transform;
        int targetLayer = target.gameObject.layer;
        // Debug.Log("Interact with: " + target);
        // is creature
        if (targetLayer == creatureLayer)
        {
            dialogManager.Address(target.gameObject);
        }
        // is interactable
        else
        {
            switch (target.tag)
            {
                case "Interactable":
                    {
                        // start interacting ...
                        if (animChar.GetInteger("interact") == 0)
                        {
                            // ... if object isn't still used
                            if (target.GetComponent<Used>().StartInteracting())
                            {
                                // move to interactingPosition
                                Transform startTransform = target.Find("UserStartTransform");
                                avatar.rotation = startTransform.rotation;
                                avatar.position = new Vector3(startTransform.position.x, avatar.position.y, startTransform.position.z);

                                // start interacting
                                int layerIndex = animChar.GetLayerIndex(target.name);
                                animChar.SetLayerWeight(layerIndex, 1);
                                animChar.SetInteger("interact", layerIndex);

                                attributes.usedObject = target.gameObject;
                            }
                        }
                        // stop interacting
                        else
                        {
                            // stop interacting
                            GameObject usedObject = attributes.usedObject;
                            animChar.SetInteger("interact", 0);

                            // tell to object
                            usedObject.GetComponent<Used>().StopInteracting();
                        }
                        break;
                    }
                case "Usable":
                    {
                        string meshName = target.GetComponent<MeshFilter>().sharedMesh.name;
                        itemStorage.AddItemToInventory(meshName);
                        Debug.Log(meshName + " successfull added to inventory");
                        Destroy(target);

                        break;
                    }
                case "Loot":
                    {
                        uiManager.OpenLootWindow();

                        // Destroy(target);

                        break;
                    }
            }
        }
    }


    public void StartDialog()
    {
        uiManager.OpenDialog(dialogManager);
    }


    void Combat()
    {
        // armedLeftItem
        // armedRightItem

        // currentMainWeapon
        // currentOffWeapon
        // currentTool 

        // one weaponWheelSet contains left and or right weapon
        // on press set current left and or right weapon

        // one other weaponWheelSlot contains tool for both hands
        // on press set current tool

        // press x -> draw: draw currentMainWeapon hide tool in the same hand if armed, if no tool armed draw currentOffWeapon
        //         -> hide: hide all weapons no tools 
        // press y -> draw: if no weapon equiped draw at right hand else search mainHand, hide offWeapon if available and draw tool in the other hand
        //         -> hide: hide tool, draw offWeapon if available

        // key down y -> draw bomb
        // key up y   -> through bomb

        // draw/hide main weapon
        if (Input.GetKeyDown("x"))
        {
            equipmentManager.DrawHideWeaponSet();
        }

        // draw/hide off weapon
        if (Input.GetKeyDown("y"))
        {
            equipmentManager.DrawHideTool();
        }

        if(Input.GetKeyUp("y"))
        {
            // throw bomb
        }

        // chose combatType
        int mainHand = animChar.GetInteger("armed" + equipmentManager.GetMainHand + "Weapon");
        int offHand = animChar.GetInteger("armed" + equipmentManager.GetOffHand + "Weapon");

        string methodName;
        if (combatTypes.GetCombatType(new Vector2(mainHand, offHand), out methodName))
        {
            MethodInfo combatFunction = this.GetType().GetMethod(methodName);
            combatFunction.Invoke(this, null);
        }
    }
    
    
    public Vector3 StickToAvatarspace(Transform camera)
    {
        if (stickForward == Vector3.zero)
        {
            return Vector3.zero;
        }

        charForward = avatar.forward;
        cameraForward = camera.forward;
        cameraForward.y = 0.0f; // kill cam Y
        
        // Debug.Log(stickForward + " , " + (stickForward == Vector3.zero.ToString());
        

        // Convert joystick input in Avatarspace coordinates
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(cameraForward));
        Vector3 inputInAvatarspace = referentialShift * stickForward;

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), inputInAvatarspace, Color.green);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), charForward, Color.magenta);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), stickForward, Color.blue);

        return inputInAvatarspace;
    }


    public float CalculateStickAngle()
    {
        //Cross Product of moveDirection, rootDirection
        Vector3 axisSign = Vector3.Cross(moveDirection, charForward);

        float angleRootToMove = 0;
        //angle between rootDirection, moveDirection
        if (move)
        {
            angleRootToMove = Vector3.Angle(charForward, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        }

        return angleRootToMove;
    }

    /*
    public void StickToWorldspace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
    {
        Vector3 charDirection = root.forward;
        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

        speedOut = stickDirection.sqrMagnitude;
        //bordered (selfmade)
        speedOut = (speedOut <= 1 ? speedOut : 1);


        // Get camera rotation
        Vector3 CameraDirection = camera.forward;
        CameraDirection.y = 0.0f; // kill Y
        //Creates Rotation from (0, 0, 1) to CameraDirection                       //Normalize: extrude length to 1.0f
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(CameraDirection));


        // Convert joystick input in Worldspace coordinates
        Vector3 moveDirection = referentialShift * stickDirection;
        //Cross Product of moveDirection, rootDirection
        Vector3 axisSign = Vector3.Cross(moveDirection, charDirection);

        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), charDirection, Color.magenta);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisSign, Color.red);

        //angle between rootDirection, moveDirection
        float angleRootToMove = Vector3.Angle(charDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);
        if (!isPivoting)
        {
            angleOut = angleRootToMove;
        }
        angleRootToMove /= 180f;
        directionOut = angleRootToMove * directionSpeed;
    }
    */

    float GetGroundDistance()
    {
        Vector3 capMax = capCollider.bounds.center;
        RaycastHit hit;
        Physics.Raycast(capMax, Vector3.down, out hit);
        Debug.DrawLine(capMax, hit.point, Color.blue);
        if (hit.collider)
        {
            distanceToGround = hit.distance - capCollider.height / 2 - animChar.GetFloat("colliderY") + 0.23f;
        }
        else
        {
            distanceToGround = 100;
        }
        // Debug.Log("distance: " + distanceToGround + ", Object: " + hit.collider.name);
        animChar.SetFloat("distanceToGround", distanceToGround);

        return distanceToGround;
    }


    //beta raycast von oben/unten direkt vorm character
    bool CheckForWall(ref float relativeHeight)
    {
        // calculate feet position
        Vector3 feetPos = this.transform.position;
        feetPos.y -= heightOffset - 0.2f;

        // check for barrier
        RaycastHit hit;
        if (Physics.Raycast(feetPos, this.transform.forward, out hit, checkDistance))
        {
            // check for climbable
            if (hit.collider.tag == "Untagged")
            {
                Renderer wall = hit.collider.GetComponent<Renderer>();
                Vector3 wallTop = wall.bounds.center + wall.bounds.extents;
                float playerYPos = this.transform.position.y - heightOffset - GetGroundDistance();
                relativeHeight = wallTop.y - playerYPos;

                Debug.Log(relativeHeight);
                return true;
            }
        }

        return false;
    }

    
    public void Bow()
    {
        EquipedItem[] weaponSet = equipmentManager.GetArmedSet();
        Animator animWeapon = weaponSet[0].itemObject.GetComponent<Animator>();

        // able to strain?
        if (Input.GetButtonDown("LeftKlick") && (equipmentManager.GetMunition().itemAmount > 0) && animInfo.FreeMoveing())
        {
            animChar.SetBool("spannen", true);
        }

        if (Input.GetButtonUp("LeftKlick"))
        {
            animChar.SetBool("spannen", false);
        }
        
        if (animInfo.BowStraining())
        {
            strainLerp += Time.deltaTime * 1.5f;
            float strainValue = Mathf.Lerp(0, 1, strainLerp);
            animChar.SetFloat("strain", strainValue);
            animWeapon.SetFloat("strain", strainValue);
        }

        if (animInfo.BowShooting())
        {
            strainLerp = 0;
            animChar.SetFloat("strain", 0);
            animWeapon.SetFloat("strain", 0, 0.01f, Time.deltaTime);
        }

        if (animInfo.IsAiming())
        {
            animChar.SetLayerWeight(animChar.GetLayerIndex("Legs"), 1);
        }
        else
        {
            animChar.SetLayerWeight(animChar.GetLayerIndex("Legs"), 0);
        }
    }

    /*
    void SetBodyRotation()
    {
        rotateBody = false;
    }
    */

    public void SingleSword()
    {
        // Attack
        if (Input.GetButtonDown("LeftKlick") && MayAttack())
        {
            combatEvents.ResetAttackList();

            // Strong
            if (Input.GetKey("left shift"))
            {
                if (animChar.GetFloat("continueValue") > 0)
                {
                    animChar.SetBool("continue", true);
                }
                else
                {
                    animChar.SetInteger("attack", 1);
                }
            }
            // Normal
            else
            {
                if (animChar.GetFloat("continueValue") > 0)
                {
                    animChar.SetBool("continue", true);
                }
                else
                {
                    animChar.SetInteger("attack", 0);
                }
            }

            // rotate to target
            if (attributes.target)
            {
                // RotateToTarget();
            }
        }

        // Block\Parry
        if (Input.GetButtonDown("RightKlick"))
        {
            GameObject attackingEnemy = combatEvents.EnemyIsParryable();
            // Parry
            if (attackingEnemy)
            {
                //rotateToEnemy
                Vector3 toEnemy = attackingEnemy.transform.position - this.transform.position;
                this.transform.forward = toEnemy;

                animChar.SetBool("parrying", true);
                attackingEnemy.GetComponent<Animator>().SetBool("parryed", true);
            }
            // Set Block
            else
            {
                animChar.SetBool("block", true);
            }
        }
        // Reset Block
        if (Input.GetButtonUp("RightKlick"))
        {
            animChar.SetBool("block", false);
        }

        // Dodge Role
        if (Input.GetButtonDown("MouseWheel"))
        {
            if (!IsDodging())
            {
                animChar.SetInteger("dodgeType", 0);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            // Unbalance
            if (Input.GetKey("left shift"))
            {
                animChar.SetInteger("attack", 2);
            }
            // Dodge Step
            else if (!IsDodging())
            {
                animChar.SetInteger("dodgeType", 1);
            }
        }

    }


    public void SwordShield()
    {
        SingleSword();
    }


    public void Fist()
    {
        SingleSword();
    }

    public void Claymore()
    {
        SingleSword();
    }


    void SmoothRotateToTarget()
    {
        Vector3 finalDirection;

        // target?
        if(attributes.target)
        {
            // rotate to target
            
            // current Enemy position
            Vector3 enemyPos = attributes.target.transform.position;

            // Vector from char to enemy
            finalDirection = enemyPos - avatar.position;
        }
        else
        {
            // rotate to cam forward;
            finalDirection = cam.transform.forward;
        }

        finalDirection.y = 0;
        
        // Debug.DrawRay(avatar.position, finalDirection, Color.red);

        // rotates char to enemy
        avatar.forward = Vector3.Lerp(avatar.forward, finalDirection, rotationSpeed * Time.deltaTime);
    }
}

public enum CombatMode
{
    Focused,
    Free
}