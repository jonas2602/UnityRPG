using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    public PlayerAttributes attributes;
    public GroupManager groupManager;
    public Animator anim;                               // Reference to the animator component.

    public List<HealthbarScript> healthBars = new List<HealthbarScript>();

    [SyncVar(hook = "OnHealthChanged")]
    public int curHp;                                   // How much health the player has left.
    public Condition condition = Condition.Healthy;     // A bool to show if the player is dead or not.
    public bool mortal = true;                          // able to die

    public int despawnTime = 30;                     // A timer for counting to despawn the creature once he is dead.
    public int wakeupTime = 15;                      // A timer for counting to wake up the creature once he is knocked out.
    public float wakeupHp = 0.25f;// [%]
    public int resetAfterDeathTime = 5;              // How much time from the creature dying to the level reseting.

    public enum Condition
    {
        Healthy,
        Dead,
        KnockedOut
    }

    public int GetCurHp
    {
        get
        {
            return this.curHp;
        }
    }

    public void AddHealthbarScript(HealthbarScript healthbarScript)
    {
        healthBars.Add(healthbarScript);
        healthbarScript.UpdateHealth((float)curHp / attributes.maxHp);
    }

    public void RemoveHealthbarScript(HealthbarScript healthbarScript)
    {
        healthBars.Remove(healthbarScript);
    }

    void Awake()
    {
        // Setting up the references.
        Transform avatar = transform.root;

        anim = GetComponent<Animator>();
        attributes = GetComponent<PlayerAttributes>();
        groupManager = GetComponent<GroupManager>();
        HealthbarScript[] bars = GetComponentsInChildren<HealthbarScript>(true);
        for (int i = 0; i < bars.Length; i++)
        {
            healthBars.Add(bars[i]);
        }
        // get healthbar
    }

    void Start()
    {
        curHp = attributes.GetMaxHp;
    }

    void Update()
    {

    }

    public void GetDamage(DamageInfo info)
    {
        // Debug.Log("Health react to damage");
        Damage(DamageCalculator.Calculate(info, attributes));
    }

    public void Damage(int amount)
    {
        int newHp;
        // Debug.Log("Get " + amount + " Damage");
        if (curHp - amount <= 0)
        {
            newHp = 0;

            // dying / knockout
            Dying();
        }
        else
        {
            newHp = curHp - amount;
        }

        SetHealth(newHp);
    }


    public void Heal(int amount)
    {
        int newHp;
        // Debug.Log("Get " + amount + " Heal");
        if (curHp + amount >= attributes.maxHp)
        {
            newHp = attributes.maxHp;
        }
        else
        {
            newHp = curHp + amount;
        }

        SetHealth(newHp);
    }


    void SetHealth(int newHp)
    {
        curHp = newHp;

        float fillAmount = (float)newHp / attributes.GetMaxHp;

        for (int i = 0; i < healthBars.Count;i++)
        {
            healthBars[i].UpdateHealth(fillAmount);
        }
    }


    void Dying()
    {
        anim.SetBool("dying", true);
        groupManager.group.deathCount++;

        // dying
        if (mortal)
        {
            condition = Condition.Dead;

            StartCoroutine("Dead");
        }
        // knock out
        else
        {
            anim.SetBool("knockedOut", true);
            condition = Condition.KnockedOut;

            StartCoroutine("KnockedOut");
        }
        
        // make character static
        // this.gameObject.SendMessage("SetAggro", 0, SendMessageOptions.DontRequireReceiver);

        // create drops
        // GameObject.Instantiate(Resources.Load<GameObject>("Consumable/Lootbag")).AddComponent<Loot>().Init("Loot", this.transform.position, inventory.GetLootPool());
    }


    protected virtual IEnumerator Dead()
    {
        // wait for despawn
        yield return new WaitForSeconds(despawnTime);

        // despawn
        Destroy(this.gameObject);        
    }


    IEnumerator KnockedOut()
    {
        float ticktime = 0.25f;
        float regenPerTick = 1 / wakeupTime / ticktime * wakeupHp;

        float startTime = Time.time;

        // regenerate
        while (Time.time - startTime < wakeupTime)
        {
            int newHp = Mathf.RoundToInt((Time.time - startTime) / wakeupTime * wakeupHp * attributes.maxHp); // [%]
            SetHealth(newHp);
            yield return new WaitForSeconds(ticktime);
        }

        // revive
        anim.SetBool("revive", true);
        curHp = Mathf.RoundToInt(attributes.maxHp * wakeupHp);
        condition = Condition.Healthy;
        groupManager.group.deathCount--;
    }

    
    void OnHealthChanged(int newHp)
    {
        curHp = newHp;
    }
}
