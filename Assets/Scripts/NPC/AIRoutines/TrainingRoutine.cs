using UnityEngine;
using System.Collections;
using System;

public class TrainingRoutine : AIRoutine
{
    private EquipmentManager equipmentManager;
    private Transform stomage;
    private Transform shootingPosition;
    private Activity curActivity;

    [SerializeField]
    private Vector3 shootVector = Vector3.zero;
    private float taskTime;

    [SerializeField]
    private string status;

    public override void StartAction()
    {
        StartCoroutine(ActivityController());
    }

    public override void FinishAction()
    {
        StopAllCoroutines();
        FinishTraining();
    }

    protected override void Awake()
    {
        base.Awake();

        equipmentManager = avatar.GetComponent<EquipmentManager>();
        stomage = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch");
    }

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void LateUpdate()
    {
        if(shootVector != Vector3.zero)
        {
            Debug.DrawRay(this.transform.position, shootVector * 2f, Color.cyan);

            // rotate player
            Vector3 shootForward = shootVector;
            shootForward.y = 0;
            avatar.forward = shootForward;
            float rotationAngle = Vector3.Angle(avatar.forward, shootVector);

            // rotate stomage
            stomage.Rotate(-avatar.right, rotationAngle, Space.World);
        }
    }

    IEnumerator ActivityController()
    {
        for (;;)
        {
            curActivity = schedule.activeActivity;

            shootingPosition = curActivity.taskObjects[0].transform;

            status = "MoveToShootPosition";
            yield return MoveToTraining();

            status = "Training";
            StartTraining();

            for (;;)
            {
                yield return Training();

                // break task?
                if (schedule.optActivity != curActivity)
                {
                    break;
                }
            }

            status = "Training Finished";
            FinishTraining();

            schedule.ChangeActivity();
        }
    }

    IEnumerator MoveToTraining()
    {
        // start moving
        nav.destination = shootingPosition.position;

        for (;;)
        {
            // calculate distance
            Vector3 toTask = avatar.position - shootingPosition.position;
            toTask.y = 0;

            float distanceToTask = toTask.magnitude;

            // target reached?
            if (distanceToTask < 1f)
            {
                break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void StartTraining()
    {
        nav.Stop();

        // chose bow
        equipmentManager.UpdateSelectedSlots(new int[] { 2, 3, 4 });

        // draw bow
        if (!equipmentManager.WeaponSetArmed())
        {
            // Debug.Log("draw bow");
            equipmentManager.DrawHideWeaponSet();
        }

        anim.SetLayerWeight(anim.GetLayerIndex("Legs"), 1);
    }

    IEnumerator Training()
    {
        // draw weapon
        while(!equipmentManager.GetArmedSet()[0].itemObject)
        {
            yield return new WaitForSeconds(1f);
        }

        Transform target = curActivity.taskObjects[1].transform;
        EquipedItem[] weaponSet = equipmentManager.GetArmedSet();
        Animator animWeapon = weaponSet[0].itemObject.GetComponent<Animator>();

        while (!animInfo.FreeMoveing())
        {
            yield return null;
        }

        // reload bow
        anim.SetBool("spannen", true);
        while(!animInfo.BowStraining())
        {
            yield return null;
        }

        // strain bow
        float strainLerp = 0;
        while (anim.GetFloat("strain") < 1)
        {
            strainLerp += Time.deltaTime * 1.5f;
            float strainValue = Mathf.Lerp(0, 1, strainLerp);
            anim.SetFloat("strain", 1, 0.01f, Time.deltaTime);
            animWeapon.SetFloat("strain", 1, 0.01f, Time.deltaTime);
        }

        // aim
        shootVector = TrajectoryCalculator.CalculateTrajectory(weaponSet[1].itemObject.transform.position, target.position, 40f, Physics.gravity.magnitude);
        avatar.GetComponentInChildren<MunitionController>().SetForceDirection(shootVector);
        yield return new WaitForSeconds(1f);

        // shoot
        anim.SetBool("spannen", false);

        // wait for shoot
        while (!animInfo.BowShooting())
        {
            yield return null;
        }

        while (animInfo.BowShooting())
        {
            strainLerp = 0;
            anim.SetFloat("strain", 0);
            animWeapon.SetFloat("strain", 0, 0.01f, Time.deltaTime);
            yield return null;
        }

        while (!animInfo.FreeMoveing())
        {
            yield return null;
        }
        shootVector = Vector3.zero;

        yield return null;
    }

    void FinishTraining()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Legs"), 0);

        // hide bow
        if (equipmentManager.WeaponSetArmed())
        {
            // Debug.Log("hide bow");
            equipmentManager.DrawHideWeaponSet();
        }

        nav.Resume();
    }
}
