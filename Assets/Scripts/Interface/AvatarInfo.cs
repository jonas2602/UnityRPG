using UnityEngine;
using System.Collections;

public class AvatarInfo : MonoBehaviour
{

    public GameObject player;
    public Health health;
    public Group group;
    public PlayerAttributes attribute;
    public CharakterControler charContr;
    public GUISkin skin;
    public Camera cam;

    // player GUI
    public Vector2 windowPos = new Vector2(20, 20);
    public Vector2 windowSize = new Vector2(275, 80);

    public float windowDistance = 0f;

    public Vector2 healthPos = new Vector2(10, 10);
    public Vector2 healthSize = new Vector2(220, 30);

    public Vector2 staminaPos = new Vector2(20, 45);
    public Vector2 staminaSize = new Vector2(180, 20);

    public Vector2 expPos = new Vector2(60, 3);
    public Vector2 expSize = new Vector2(160, 8);

    public Vector2 levelUpPos = new Vector2(245, 10);
    public float levelUpSize = 25;

    // group GUI
    public Vector2 groupNamePos = new Vector2(40, 10);
    public Vector2 groupNameSize = new Vector2(110, 30);

    public Vector2 groupHealthPos = new Vector2(30, 40);
    public Vector2 groupHealthSize = new Vector2(220, 30);

    public Vector2 groupStatusPos = new Vector2(0, 0);
    public Vector2 groupStatusSize = new Vector2(40, 40);

    public Vector2 groupLevelPos = new Vector2(150, 10);
    public Vector2 groupLevelSize = new Vector2(30, 30);

    // enemy GUI
    public Vector2 enemyWindowPos = new Vector2(0, 0);
    public Vector2 enemyWindowSize = new Vector2(0, 0);

    public Vector2 enemyAggroPos = new Vector2(0, 0);
    public Vector2 enemyAggroSize = new Vector2(0, 0);

    public Vector2 enemyHealthPos = new Vector2(0, 0);
    public Vector2 enemyHealthSize = new Vector2(0, 0);

    public Vector2 enemyLevelPos = new Vector2(0, 0);
    public float enemyLevelSize = 0f;


    // Use this for initialization
    void Awake()
    {
        player = this.transform.root.gameObject;
        health = player.GetComponent<Health>();
        group = player.GetComponent<GroupManager>().group;
        attribute = player.GetComponent<PlayerAttributes>();
        charContr = player.GetComponent<CharakterControler>();
        cam = player.transform.Find("Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void DrawPlayerInfo()
    {
        // Draw Window
        Rect windowRect = new Rect(windowPos.x, windowPos.y, windowSize.x, windowSize.y);
        GUI.Box(windowRect, "", skin.GetStyle("Inventory"));

        // Draw Healthbar
        Rect maxHealthRect = new Rect(windowRect.xMin + healthPos.x, windowRect.yMin + healthPos.y, healthSize.x, healthSize.y);
        GUI.Box(maxHealthRect, "", skin.GetStyle("Inventory"));
        float curHp = (float)health.curHp / attribute.maxHp;
        if (curHp > 0)
        {
            Rect curHealthRect = new Rect(windowRect.xMin + healthPos.x, windowRect.yMin + healthPos.y, healthSize.x * curHp, healthSize.y);
            GUI.Box(curHealthRect, "", skin.GetStyle("Bar"));
        }

        // Draw Staminabar
        Rect maxStaminaRect = new Rect(windowRect.xMin + staminaPos.x, windowRect.yMin + staminaPos.y, staminaSize.x, staminaSize.y);
        GUI.Box(maxStaminaRect, "", skin.GetStyle("Inventory"));
        float curStamina = (float)charContr.curStamina / attribute.maxStamina;
        if (curStamina > 0)
        {
            Rect curStaminaRect = new Rect(windowRect.xMin + staminaPos.x, windowRect.yMin + staminaPos.y, staminaSize.x * curStamina, staminaSize.y);
            GUI.Box(curStaminaRect, "", skin.GetStyle("Bar"));
        }

        // Draw Expbar
        Rect maxExpRect = new Rect(windowRect.xMin + expPos.x, windowRect.yMin + expPos.y, expSize.x, expSize.y);
        GUI.Box(maxExpRect, "", skin.GetStyle("Inventory"));
        float curExp = (float)attribute.curExp / attribute.neededExp;
        if (curExp > 0)
        {
            Rect curExpRect = new Rect(windowRect.xMin + expPos.x, windowRect.yMin + expPos.y, expSize.x * curExp, expSize.y);
            GUI.Box(curExpRect, "", skin.GetStyle("Bar"));
        }
        if (attribute.GetFreeSkillPoints > 0)
        {
            Rect levelUpRect = new Rect(windowRect.xMin + levelUpPos.x, windowRect.yMin + levelUpPos.y, levelUpSize, levelUpSize);
            GUI.Box(levelUpRect, "", skin.GetStyle("Inventory"));
        }
    }


    public void DrawGroupInfo()
    {
        int memberCount = group.groupMember.Count;
        if (memberCount > 1)
        {
            for (int i = 1; i < memberCount; i++)
            {
                GameObject curMember = group.groupMember[i];
                Health memberHealth = curMember.GetComponent<Health>();
                PlayerAttributes memberAttribute = curMember.GetComponent<PlayerAttributes>();

                // Draw Window
                Rect windowRect = new Rect(windowPos.x, windowPos.y + i * (windowSize.y + windowDistance), windowSize.x, windowSize.y);
                GUI.Box(windowRect, "", skin.GetStyle("Inventory"));

                // Draw Name
                Rect nameRect = new Rect(windowRect.xMin + groupNamePos.x, windowRect.yMin + groupNamePos.y, groupNameSize.x, groupNameSize.y);
                GUI.Box(nameRect, curMember.name, skin.GetStyle("Inventory"));

                // Draw Status
                Rect conditionRect = new Rect(windowRect.xMin + groupStatusPos.x, windowRect.yMin + groupStatusPos.y, groupStatusSize.x, groupStatusSize.y);
                GUI.Box(conditionRect, "", skin.GetStyle("Inventory"));
                // GUI.DrawTexture(conditionRect, GetStatus(curMember));

                // Draw Healthbar
                Rect maxHealthRect = new Rect(windowRect.xMin + groupHealthPos.x, windowRect.yMin + groupHealthPos.y, groupHealthSize.x, groupHealthSize.y);
                GUI.Box(maxHealthRect, "", skin.GetStyle("Inventory"));
                float curHp = (float)memberHealth.curHp / memberAttribute.maxHp;
                if (curHp > 0)
                {
                    Rect curHealthRect = new Rect(windowRect.xMin + groupHealthPos.x, windowRect.yMin + groupHealthPos.y, groupHealthSize.x * curHp, groupHealthSize.y);
                    GUI.Box(curHealthRect, "", skin.GetStyle("Bar"));
                }

                // Draw Level
                Rect levelRect = new Rect(windowRect.xMin + groupLevelPos.x, windowRect.yMin + groupLevelPos.y, groupLevelSize.x, groupLevelSize.y);
                GUI.Box(levelRect, "" + memberAttribute.curLevel, skin.GetStyle("Inventory"));
            }
        }
    }


    Texture2D GetStatus(GameObject curMember)
    {
        Texture2D status;

        // not connected?
        /*if (curMember.tag == "Player" && not connected false)
        {
            status = Resources.Load<Texture2D>("MarkerIcons/Disconnected");
        }*/
        // connected
        // else
        {
            // alive + infight?
            if (curMember.GetComponent<Health>().condition == Health.Condition.Healthy && curMember.GetComponent<Animator>().GetInteger("armedWeapon") != -1)
            {
                status = Resources.Load<Texture2D>("MarkerIcons/Infight");
            }
            // out of fight
            else
            {
                status = Resources.Load<Texture2D>("MarkerIcons/" + curMember.GetComponent<Health>().condition);
            }
        }
        return status;
    }


    public void DrawEnemyInfo()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, 20f, 1 << 8);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            // is creature
            if (!hitColliders[i].isTrigger && hitColliders[i].gameObject != player)
            {
                // is in sight
                if (Vector3.Angle(cam.transform.forward, hitColliders[i].transform.position - cam.transform.position) < 90)
                {
                    Vector3 infoPos = hitColliders[i].bounds.center + hitColliders[i].bounds.extents;
                    Vector3 screenPos = cam.WorldToScreenPoint(infoPos);

                    // Draw Window
                    Rect windowRect = new Rect(screenPos.x - enemyWindowSize.x / 2, Screen.height - screenPos.y + enemyWindowPos.y, enemyWindowSize.x, enemyWindowSize.y);
                    GUI.Box(windowRect, "", skin.GetStyle("Inventory"));

                    Health enemyHealth = hitColliders[i].GetComponent<Health>();
                    PlayerAttributes enemyAttribute = hitColliders[i].GetComponent<PlayerAttributes>();
                    // EnemySight enemySight = hitColliders[i].GetComponent<EnemySight>();

                    //Draw Name
                    /*
                    // Draw Aggrobar
                    if (enemySight.aggro < 1)
                    {
                        Rect aggroRect = new Rect(windowRect.xMin + enemyAggroPos.x, windowRect.yMin + enemyAggroPos.y, enemyAggroSize.x * enemySight.aggro, enemyAggroSize.y);
                        GUI.Box(aggroRect, "", skin.GetStyle("Inventory"));
                    }*/
                    // Draw Healthbar
                    Rect maxHealthRect = new Rect(windowRect.xMin + enemyHealthPos.x, windowRect.yMin + enemyHealthPos.y, enemyHealthSize.x, enemyHealthSize.y);
                    GUI.Box(maxHealthRect, "", skin.GetStyle("Inventory"));
                    
                    float curHp = (float)enemyHealth.curHp / enemyAttribute.maxHp;
                    if (curHp > 0)
                    {
                        Rect curHealthRect = new Rect(windowRect.xMin + enemyHealthPos.x, windowRect.yMin + enemyHealthPos.y, enemyHealthSize.x * curHp, enemyHealthSize.y);
                        GUI.Box(curHealthRect, "", skin.GetStyle("Bar"));
                    }
                    
                    // Draw Level
                    Rect levelRect = new Rect(windowRect.xMin + enemyLevelPos.x, windowRect.yMin + enemyLevelPos.y, enemyLevelSize, enemyLevelSize);
                    GUI.Box(levelRect, "" + enemyAttribute.curLevel, skin.GetStyle("Inventory"));
                }
            }
        }
    }
}
