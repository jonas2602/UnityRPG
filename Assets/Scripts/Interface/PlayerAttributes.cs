using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerAttributes : NetworkBehaviour {

    public struct ArenaInfo
    {
        public enum ConflictSituation
        {
            Won,
            Remaining,
            Lost
        }

        public ConflictSituation status;
        public GameObject opponent;

        public ArenaInfo(ConflictSituation status, GameObject opponent)
        {
            this.status = status;
            this.opponent = opponent;
        }
    }


    public Actor parentActor;
    public Faction fraction;
    public ArenaInfo arenaHistory;
    public ControlMode controlMode;
    
    [SyncVar(hook = "TargetSync")]
    public GameObject target = null;
    public GameObject usedObject = null;

    public int aggro = 0;
    public List<GameObject> aggroEnemy;

    public float weaponRange;
    public float arrowSpeed;
    
    public int curLevel = 1;
    public int curExp = 0;
    public int neededExp = 100;
    public int freeSkillPoints;

    public int maxHp = 100;
    public int maxStamina = 100;

    
    public float GetWeaponRange
    {
        get
        {
            return this.weaponRange;
        }
    }

    public Transform GetTargetTransform
    {
        get
        {
            return target ? target.transform : null;
        }
    }

    public Transform SetTarget
    {
        // an die netId auf dem target denken

        set
        {
            target = value != null ? value.gameObject : null;
            // Debug.Log("new Target: " + value + ", " + target);
        }
    }

    public float SetSpeed
    {
        set
        {
            arrowSpeed = value;
        }
    }

    public int GetMaxHp
    {
        get
        {
            return this.maxHp;
        }
    }

    public int GetFreeSkillPoints
    {
        get
        {
            return this.freeSkillPoints;
        }
    }

    public Faction GetMembership
    {
        get
        {
            return this.fraction;
        }
    }

    // Network Sync
    [Client]
    void TargetSync(GameObject activeOpponent)
    {
        this.target = activeOpponent;
    }
}

public enum ControlMode
{
    Player,
    Ai
}
