using UnityEngine;
using System.Collections;

public class FightingRoutine : AIRoutine
{
    [SerializeField]
    private Tactic tactic;
    private CombatEvents combatEvents;

    // combat
    [SerializeField]
    private float curDistanceToTarget;
    private Vector3 optPos;

    [SerializeField]
    private string attackStatus;

    [SerializeField]
    private float attackRange = 3.5f;
    private float targetAttackRange;
    private int maxComboLength = 3;
    private int curComboLength = 0;
    private float parryChance = 0.7f;

    public enum Tactic
    {
        OffensiveMelee,
        DefensiveMelee,
        OffensiveRanged,
        DefensiveRanged,
    }

    protected override void Awake()
    {
        base.Awake();

        combatEvents = avatar.GetComponent<CombatEvents>();
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    public override void StartAction()
    {
        StartCoroutine(OffensiveCombat());
        StartCoroutine(DeffensiveCombat());
        StartCoroutine(UpdateTactic());
        StartCoroutine(CalculateOptPos());
        StartCoroutine(Positioning());
    }

    IEnumerator OffensiveCombat()
    {
        for (;;)
        {
            // Debug.Log("start attack");
            // reset comboLength
            int comboLength = 0;

            // wait for free time and correct distance
            attackStatus = "wait for attack";
            for (;;)
            {
                // has correct distance
                if (curDistanceToTarget < attackRange)
                {
                    // has free time
                    if (!animInfo.InCombatMove())
                    {
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            attackStatus = "attack target";
            // Calculate next Action
            if (Random.value < 0.6f)
            {
                // normal attack
                Debug.Log("Attack");
                // RotateToTarget(attributes.target.transform);
                anim.SetInteger("attack", 0);
                comboLength++;
            }
            else
            {
                // Heavy Attack
                Debug.Log("Heavy Attack");
                // RotateToTarget(attributes.target.transform);
                anim.SetInteger("attack", 1);
                comboLength++;
            }

            // may continue combo
            attackStatus = "continue combo";
            while (comboLength < maxComboLength && curDistanceToTarget < attackRange)
            {
                // will continue combo?
                if (Random.value > 0.75f)
                {
                    // wait for continue
                    while (anim.GetFloat("continueValue") < 0 || anim.GetBool("continue"))
                    {
                        yield return null;
                    }

                    // continue combo
                    Debug.Log("continue Combo");
                    anim.SetBool("continue", true);
                    curComboLength++;
                }
                // finish current attack
                else
                {
                    break;
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator Positioning()
    {
        // try max distance while moving arround target
        // calc way around target (cycle)

        for (;;)
        {
            // calculate new path needed?
            Vector3 difference = nav.destination - optPos;
            difference.y = 0;
            if (difference.magnitude > 1f)
            {
                // Debug.Log(optPos + ", " + nav.destination);
                nav.SetDestination(optPos);
                nav.Resume();
            }

            // calculate distance to target
            Vector3 toTarget = attributes.target.transform.position - avatar.position;
            curDistanceToTarget = toTarget.magnitude;

            yield return new WaitForSeconds(0.5f);
        }
    }

    void RotateToTarget(Transform enemy)
    {
        // current Enemy position
        Vector3 enemyPos = enemy.transform.position;

        // Vector from char to enemy
        Vector3 charToEnemy = enemyPos - this.transform.position;
        charToEnemy.y = 0;

        // rotates char to enemy
        avatar.forward = charToEnemy;
    }

    IEnumerator CalculateOptPos()
    {
        // try to position perfect for ...
        for (;;)
        {
            switch (tactic)
            {
                // ... offensive melee attacks in front of the target, in attack range of the target -> for many fast attacks
                case Tactic.OffensiveMelee:
                    {
                        Vector3 toTarget = Vector3.Normalize(attributes.target.transform.position - avatar.position);
                        optPos = attributes.target.transform.position - toTarget * attackRange;
                        break;
                    }
                // ... defensive melee attacks (behind the target, out of his attack radius -> some intensive hits without getting damage)
                case Tactic.DefensiveMelee:
                    {
                        optPos = attributes.target.transform.position - attributes.target.transform.forward * attackRange;
                        break;
                    }
                // ... offensive ranged attacks (out of enemy melee range but in range for ranged attacks)
                case Tactic.OffensiveRanged:
                    {
                        Vector3 toTarget = Vector3.Normalize(attributes.target.transform.position - avatar.position);
                        optPos = attributes.target.transform.position - Vector3.ClampMagnitude(toTarget * targetAttackRange, attackRange);
                        break;
                    }
                // ... defensive ranged attacks (out of target attack range or under cover -> geringe trefferfläche)
                case Tactic.DefensiveRanged:
                    {
                        Vector3 toTarget = Vector3.Normalize(attributes.target.transform.position - this.transform.position);
                        optPos = attributes.target.transform.position - toTarget * targetAttackRange;
                        break;
                    }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator UpdateTactic()
    {
        for (;;)
        {
            // do action
            // still same target?
            // balance enemycount, allycount
            // low targethp -> try to finish (burstDamage)
            // high hp -> offense -> higher attack frequence, less parry chance, near enemy, very less ranged attacks (berserk)
            // low hp -> deffence -> nearly no attacks, high parry chance, out of enemy range, dodge more often, ranged attacks as main damage
            /*if (health.curHp / attributes.maxHp < 0.4f)
            {
                tactic = Tactic.DefensiveRanged;
            }
            else*/
            {
                tactic = Tactic.OffensiveMelee;
            }


            // wait some time
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator DeffensiveCombat()
    {
        for (;;)
        {

            // no longer able to block only parry
            // parry
            GameObject attackingEnemy = combatEvents.EnemyIsParryable();

            // attacked by parryable enemy
            if (attackingEnemy)
            {
                // not in action
                if (animInfo.InCombatMove())
                {
                    // calculate random
                    if (Random.value <= parryChance)
                    {
                        //rotateToEnemy
                        Vector3 toEnemy = attackingEnemy.transform.position - this.transform.position;
                        this.transform.forward = toEnemy;

                        Debug.Log(this.name + " is parrying " + attackingEnemy.name);
                        anim.SetBool("parrying", true);
                        attackingEnemy.GetComponent<Animator>().SetBool("parryed", true);
                    }
                }
            }
            yield return null;
        }
    }

    public override void FinishAction()
    {
        StopAllCoroutines();
    }
}
