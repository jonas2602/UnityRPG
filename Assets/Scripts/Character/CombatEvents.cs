using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatEvents : MonoBehaviour {

    private Animator anim;
    private CombatTypeList combatTypes;
    private PlayerAttributes attributes;
    private GroupManager groupManager;
    private EquipmentManager equipmentManager;
    private Relations relations;
    private Health health;

    private List<GameObject> attackList = new List<GameObject>();

    void Awake ()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        attributes = GetComponent<PlayerAttributes>();
        groupManager = GetComponent<GroupManager>();
        equipmentManager = GetComponent<EquipmentManager>();
        combatTypes = GameObject.FindWithTag("Database").GetComponent<CombatTypeList>();
        relations = GameObject.FindWithTag("Relations").GetComponent<Relations>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetAggroEnemy(int value, GameObject enemy)
    { 
        if (value == 1)
        {
            if (!attributes.aggroEnemy.Contains(enemy))
            {
                attributes.aggroEnemy.Add(enemy);
            }
        }
        else
        {
            attributes.aggroEnemy.Remove(enemy);
        }

        attributes.aggro = attributes.aggroEnemy.Count;
        anim.SetInteger("hasAggro", attributes.aggro);

    }

    public void ResetAttackList()
    {
        // Debug.Log("reset");
        attackList.Clear();
    }

    // offensive
    public void Attack(GameObject hitObject)
    {
        // remove doubles
        if(attackList.Contains(hitObject))
        {
            return;
        }

        // create list
        attackList.Add(hitObject);

        // attack
        int creatureLayer = LayerMask.NameToLayer("Creature");
        if (hitObject.layer == creatureLayer)
        {
            Animator animEnemy = hitObject.GetComponent<Animator>();
            DamageType damageMode = DamageType.Strike;
            bool damageTarget = false;

            switch (anim.GetInteger("attack"))
            {
                case 0:
                    {
                        if (animEnemy.GetBool("block"))
                        {
                            anim.SetBool("blocked", true);
                            animEnemy.SetBool("blocking", true);
                        }
                        else
                        {
                            damageMode = DamageType.Strike;
                            damageTarget = true;
                        }
                        break;
                    }
                case 1:
                    {
                        damageMode = DamageType.Strike_Heavy;
                        damageTarget = true;
                        break;
                    }
            }

            if (damageTarget && IsOpponent(hitObject))
            {
                Vector3 toTarget = hitObject.transform.position - this.transform.position;

                // Debug.Log("Damage to " + hitCollider.name + " Distance: " + Vector3.Magnitude(toTarget));
                DamageInfo info = new DamageInfo(new Item(), new Item(), toTarget, damageMode);

                // CmdTellServerWhoWasDamaged(hitColliders[i].name, info);
                Debug.Log(name + " deal damage to " + hitObject.name);
                hitObject.BroadcastMessage("GetDamage", info);
            }
        }
    }
    
    public bool IsOpponent(GameObject root)
    {
        // ... and if the raycast hits an enemy ...
        Relation hitRelation = relations.GetFractionRelation(this.gameObject, root);
        if (hitRelation == Relation.Enemy)
        {
            return true;
        }

        // ... or if the raycast hits an arenaopponment ...
        // has active match     
        if (groupManager.match)
        {
            // Debug.Log("has match");
            // match is running
            if (groupManager.match.status == MatchManager.MatchStatus.Running)
            {
                // Debug.Log("is match");
                GroupManager targetGroup = root.GetComponent<GroupManager>();

                // hit person is participant
                if (targetGroup.match)
                {
                    // Debug.Log("is fighter");
                    // is in opposing team
                    if (targetGroup.group != groupManager.group)
                    {
                        return true;
                    }
                }
            }
        }
        // is friendly
        return false;
    }

    public GameObject EnemyIsParryable()
    {
        int blockRange = 10;
        int creatureLayer = LayerMask.NameToLayer("Creature");

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, blockRange, creatureLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            // Is Parryable?
            if (hitColliders[i].gameObject.GetComponent<Animator>().GetFloat("parryable") > 0f)
            {
                // Is In Enemy Attackrange?
                if (Vector3.Distance(hitColliders[i].transform.position, this.transform.position) <= hitColliders[i].gameObject.GetComponent<PlayerAttributes>().GetWeaponRange)
                {
                    // Is In Attackangle?
                    if (Vector3.Angle(this.transform.position - hitColliders[i].transform.position, hitColliders[i].transform.forward) > 157.5f)
                    {
                        return hitColliders[i].gameObject;
                    }
                }
            }
        }
        return null;
    }

    // defensive
    public void GetDamage(DamageInfo info)
    {
        // Setback to animator
        Vector3 axisSign = Vector3.Cross(this.transform.forward, info.direction);
        float damageAngle = Vector3.Angle(this.transform.forward, info.direction) * (axisSign.y >= 0 ? -1f : 1f);
        // Debug.Log("damageAngle: " + damageAngle);
        anim.SetFloat("damageAngle", damageAngle);
        anim.SetInteger("SetbackMode", (int)info.damageType);
    }

    void RotateToEnemy()
    {
        if (attributes.target != null)
        {
            this.transform.forward = attributes.target.transform.position - this.transform.position;
            // Debug.Log("Rotate to Enemy");
        }
    }
    /*
    // set combat animator layer
    public void SetWeaponLayer()
    {
        int mainHand = anim.GetInteger("armed" + equipmentManager.GetMainHand + "Weapon");
        int offHand = anim.GetInteger("armed" + equipmentManager.GetOffHand + "Weapon");

        string layerName;
        if (combatTypes.GetCombatType(new Vector2(mainHand, offHand), out layerName))
        {
            // Debug.Log("activate layer: " + layerName);
            StartCoroutine(ActivateLayer(new Vector2(anim.GetLayerIndex(layerName), 2f)));
        }
    }
    
    public void ResetWeaponLayer()
    {
        int mainHand = anim.GetInteger("armed" + equipmentManager.GetMainHand + "Weapon");
        int offHand = anim.GetInteger("armed" + equipmentManager.GetOffHand + "Weapon");

        string layerName;
        if (combatTypes.GetCombatType(new Vector2(mainHand, offHand), out layerName))
        {
            // Debug.Log("deactivate layer: " + layerName);
            StartCoroutine(DeactivateLayer(new Vector2(anim.GetLayerIndex(layerName), 0.1f)));
        }
    }

    IEnumerator ActivateLayer(Vector2 info)
    {
        int layerIndex = (int)info.x;
        float multiplier = info.y;

        float weight = 0;
        while (weight <= 1)
        {
            weight += Time.deltaTime * multiplier;
            anim.SetLayerWeight(layerIndex, weight);

            yield return null;
        }
    }

    IEnumerator DeactivateLayer(Vector2 info)
    {
        int layerIndex = (int)info.x;
        float multiplier = info.y;

        float weight = 1;
        while (weight >= 0)
        {
            weight -= Time.deltaTime * multiplier;
            anim.SetLayerWeight(layerIndex, weight);

            yield return null;
        }
    }
    */
    // Animation call events
    void ResetDying()
    {
        anim.SetBool("dying", false);
    }

    void ResetDodge()
    {
        anim.SetInteger("dodgeType", -1);
    }
    
    void ResetAttackValues()
    {
        anim.SetInteger("attack", -1);
        anim.SetBool("continue", false);
    }
    
    void ResetSetbackMode()
    {
        anim.SetInteger("SetbackMode", -1);
    }

    void ResetJump()
    {
        anim.SetBool("Jump", false);
    }
}
