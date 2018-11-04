using UnityEngine;
using System.Collections;

public class ArrowController : MunitionController
{
    [SerializeField]
    private Animator animAvatar;
    private bool inWorldspace;

    private PlayerAttributes attributes;
    
    protected override void Awake()
    {
        base.Awake();

        // attributes = new PlayerAttributes();
    }
    
    void FixedUpdate()
    {
        if (inWorldspace)
        {
            if (rb.velocity.normalized != Vector3.zero)
            {
                this.transform.forward = rb.velocity;
            }
        }
    }

    public override void Shoot()
    {
        rb.isKinematic = false;
        col.isTrigger = false;

        animAvatar = GetComponentInParent<Animator>();
        if(forceDirection == Vector3.zero)
        {
            forceDirection = this.transform.forward;
        }

        this.transform.SetParent(null, true);
        float strain = animAvatar.GetFloat("strain");
        rb.AddForce(forceDirection * 40f * strain,ForceMode.Impulse);

        inWorldspace = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.root == animAvatar.transform)
        {
            return;
        }
        
        this.transform.SetParent(collision.transform, true);
        rb.isKinematic = true;
        col.isTrigger = true;
        inWorldspace = false;

        Transform hitObject = collision.transform.root;
        
        ProjectileDamageInfo info = new ProjectileDamageInfo(new Item(), new Item(), this.transform.forward, collision, DamageType.Projectile, rb.velocity.magnitude);
        hitObject.BroadcastMessage("GetDamage", info, SendMessageOptions.DontRequireReceiver);
        
    }

    /*void ObjectHit()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position, fwd, out hit, 1.25f))
        {
            target = hit.collider.gameObject;
            if (hit.collider.isTrigger == false)
            {
                rb.isKinematic = true;
                col.enabled = false;
                this.tag = "Usable";
                isInWorldspace = false;
                speed = vel.magnitude;
                this.transform.parent = target.gameObject.transform;
                Debug.Log("Arrow hits: " + target.name);
                // Debug.Log(speed);
            }
            if(target.tag != "Untagged")
            {
                // Debug.Log(target.tag);
                if (target.layer == 8)
                {
                    Debug.Log(Mathf.RoundToInt(speed * 0.5f) + " Damage to " + target.name);

                    attribute.SetSpeed = Mathf.RoundToInt(speed * 0.5f);
                    DamageInfo info = new DamageInfo(attribute, this.transform.position, this.transform.forward, DamageType.Projectile);
                    target.SendMessage("GetDamage", info);
                }
                if(target.tag == "Target")
                {
                    target.SendMessage("CollisionCalculation", hit.point);
                }
            }

        }
    }

    void GetDirection()
    {
        vel = rb.velocity;
        // Debug.DrawLine(arrow.transform.position, arrow.transform.position + vel.normalized * 5);
        if (vel.normalized != Vector3.zero)
        {
            this.transform.forward = vel.normalized;
        }
    }*/
}
