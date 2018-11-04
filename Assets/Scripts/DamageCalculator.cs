using UnityEngine;
using System.Collections;

public class DamageCalculator : MonoBehaviour 
{
    public static int Calculate(DamageInfo offender, PlayerAttributes affected)
    {
        int damage = 0;
        switch(offender.damageType)
        {
            case DamageType.Projectile:
                {
                    damage = Mathf.RoundToInt(ProjectileDamage(offender as ProjectileDamageInfo, affected));
                    break;
                }
            case DamageType.Strike:
                {
                    damage = Mathf.RoundToInt(StrikeDamage());
                    break;
                }
            case DamageType.Strike_Heavy:
                {
                    damage = Mathf.RoundToInt(StrikeDamage() * 2.5f);
                    break;
                }
        }
        
        return damage;
    }

    static float StrikeDamage()
    {
        return 10;
    }

    static float ProjectileDamage(ProjectileDamageInfo offender, PlayerAttributes affected)
    {
        return offender.speed * 0.5f;
    }
}
