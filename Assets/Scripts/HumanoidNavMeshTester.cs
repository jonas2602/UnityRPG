using UnityEngine;
using System.Collections;

public class HumanoidNavMeshTester : MonoBehaviour
{
    private Transform avatar;
    private Animator anim;
    private UnityEngine.AI.NavMeshAgent nav;

    private AnimatorStateInfo animCharStateInfo;
    private int Move_ForwardId = 0;

    [SerializeField]
    private float speedDampTime = 0.1f;
    [SerializeField]
    private float directionDampTime = 0.7f;
    [SerializeField]
    private float directionMultiplier;

    public bool IsInPivot()
    {
        return !(animCharStateInfo.fullPathHash == Move_ForwardId);
    }

    void Awake()
    {
        avatar = this.transform;
        anim = GetComponent<Animator>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        Move_ForwardId = Animator.StringToHash("Fist.Move_Forward");
    }

	// Use this for initialization
	void Start ()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animCharStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!IsInPivot())
        {
            NavAnimSetup();
        }
    }

    public void NavAnimSetup()
    {
        float speed = 0;
        float direction = 0;
        float angle = 0;
        
        nav.nextPosition = this.transform.position;

        // get move direction from nav
        Debug.DrawRay(this.transform.position, nav.desiredVelocity, Color.green);   // path direction
        Debug.DrawRay(this.transform.position, nav.velocity, Color.black);          // smooth move direction

        // calculate animator variables

        // angle
        // get angle between the facing of the npc and the direction it's travelling
        angle = Vector3.Angle(avatar.forward, nav.desiredVelocity);
        
        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(avatar.forward, nav.desiredVelocity);

        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, avatar.up));

        // direction
        direction = angle / 180 * directionMultiplier;

        // speed
        speed = Vector3.Project(nav.velocity, avatar.forward).magnitude;

        SetupAnimator(speed, direction, angle);
    }

    
    public void SetupAnimator(float speed, float direction, float angle)
    {
        // Angular speed is the number of degrees per second.
        // float direction = angle / angleResponseTime;

        // Set the mecanim parameters and apply the appropriate damping to them.
        anim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
        anim.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
        anim.SetFloat("Angle", angle);

    }
}
