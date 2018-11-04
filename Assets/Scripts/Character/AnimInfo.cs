using UnityEngine;
using System.Collections;

public class AnimInfo : MonoBehaviour {

    private Animator anim;

    private AnimatorStateInfo mainLayerStateInfo;
    private AnimatorStateInfo bowLayerStateInfo;

    // layerindices
    private int BowLayerIndex;

    // Hashes
    private int SwordMoveState = 0;
    private int Move_ForwardState = 0;
    private int BowStrainState = 0;
    private int BowShootState = 0;
    private int BowLoadState = 0;

    void Awake()
    {
        anim = GetComponent<Animator>();

        // layer indices
        BowLayerIndex = anim.GetLayerIndex("Bow");

        // Hashes
        SwordMoveState = Animator.StringToHash("Fist.Combat.Sword_Move");
        Move_ForwardState = Animator.StringToHash("Fist.Move_Forward");
        BowStrainState = Animator.StringToHash("Bow.BowCombat.Strain");
        BowShootState = Animator.StringToHash("Bow.BowCombat.Bogen_schuss");
        BowLoadState = Animator.StringToHash("Bow.BowCombat.Bogen_laden");
    }

	// Use this for initialization
	void Start ()
    {
	
	}

    public bool InCombatMove()
    {
        return mainLayerStateInfo.fullPathHash == SwordMoveState;
    }

    public bool FreeMoveing()
    {
        return mainLayerStateInfo.fullPathHash == Move_ForwardState;
    }

    public bool BowStraining()
    {
        return bowLayerStateInfo.fullPathHash == BowStrainState;
    }

    public bool BowShooting()
    {
        return bowLayerStateInfo.fullPathHash == BowShootState;
    }

    public bool BowLoading()
    {
        return bowLayerStateInfo.fullPathHash == BowLoadState;
    }

    public bool IsAiming()
    {
        return bowLayerStateInfo.fullPathHash == BowStrainState ||
               bowLayerStateInfo.fullPathHash == BowShootState  ||
               bowLayerStateInfo.fullPathHash == BowLoadState;
    }
	
	// Update is called once per frame
	void Update ()
    {
        mainLayerStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bowLayerStateInfo = anim.GetCurrentAnimatorStateInfo(BowLayerIndex);
    }
}
