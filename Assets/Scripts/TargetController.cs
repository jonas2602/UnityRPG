using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour
{

    public Vector3 center;

    void Awake()
    {
        center = transform.GetChild(0).position;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDamage(DamageInfo info)
    {
        if (info.damageType == DamageType.Projectile)
        {
            // set Vector to LocalSpace
            Vector3 contactLocal = this.transform.InverseTransformPoint((info as ProjectileDamageInfo).collision.contacts[0].point);
            contactLocal.y = 0; // kill y
            float distance = Vector3.Distance(center, contactLocal);

            int points = 0;
            if (distance < 1.04)
            {
                points = 1;
                if (distance < 0.72)
                {
                    points = 2;
                    if (distance < 0.401)
                    {
                        points = 5;
                        if (distance < 0.2)
                        {
                            points = 10;
                        }
                    }
                }
            }

            Debug.Log(points);
        }
    }
}
