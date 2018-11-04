using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour 
{
    public float deadZone = 2.5f;             // The number of degrees for which the rotation isn't controlled by Mecanim.

    public float speedDampTime = 0.1f;              // Damping time for the Speed parameter.
    public float directionDampTime = 0.7f;       // Damping time for the AngularSpeed parameter
    public float angleResponseTime = 0.6f;          // Response time for turning an angle into angularSpeed.
    public float rotationSpeed = 2;
    public float directionMultiplier = 2f;
    public float speedMultiplier = 1.5f;
    
    private Transform avatar;
    private PlayerAttributes attributes;
    private UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.
    private Animator anim;                  // Reference to the Animator.
    private AnimatorStateInfo animStateInfo;
    public MoveStates moveState;

    // Hashes
    private int CombatMoveId = 0;
    private int Move_ForwardId = 0;
    

    public MoveStates SetMoveState
    {
        set
        {
            this.moveState = value;
        }
    }

    public enum MoveStates
    {
        Combat,
        Free
    }


    void Awake ()
    {
        // Setting up the references.
        avatar = transform.root;
        nav = avatar.GetComponent<UnityEngine.AI.NavMeshAgent>();
        attributes = avatar.GetComponent<PlayerAttributes>();
        anim = avatar.GetComponent<Animator>();
        moveState = MoveStates.Free;

        // Hashes
        CombatMoveId = Animator.StringToHash("Fist.Combat.Move_Sword");
        Move_ForwardId = Animator.StringToHash("Fist.Move_Forward");

        // Making sure the rotation is controlled by Mecanim.
        nav.updateRotation = false;
        nav.updatePosition = false;

        // We need to convert the angle for the deadzone from degrees to radians.
        // deadZone *= Mathf.Deg2Rad;
    }
    
    
    void Update () 
    {
        animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        // Calculate the parameters that need to be passed to the animator component.
        NavAnimSetup();
    }
    
    bool IsInCombat()
    {
        return animStateInfo.fullPathHash == CombatMoveId;
    }

    public bool IsInPivot()
    {
        return !(animStateInfo.fullPathHash == Move_ForwardId);
    }

    /*
    void OnAnimatorMove ()
    {
        // Set the NavMeshAgent's velocity to the change in position since the last frame, by the time it took for the last frame.
        nav.velocity = anim.deltaPosition / Time.deltaTime;
        
        // The gameobject's rotation is driven by the animation's rotation.
        this.transform.rotation = anim.rootRotation;
    }
    */

    void NavAnimSetup ()
    {
        // Create the parameters to pass to the helper function.
        float speed = 0;
        float direction = 0;
        float angle = 0;
        bool move = true;

        // Debug.Log(avatar.name + ": " + nav.velocity.magnitude + ", " + nav.remainingDistance);
        if(nav.remainingDistance < 0.75f)
        {
            move = false;
        }

        if (IsInCombat())   // Combat
        {
            // Debug.Log("combat");
            // rotate to target
            if (move)
            {
                SmoothRotateToTarget();
            }
            // get move direction
            Vector3 navDirection = nav.velocity;
            navDirection.y = 0;
            // direction = Vector3.ClampMagnitude(direction, 1);

            Debug.DrawRay(new Vector3(avatar.position.x, avatar.position.y + 3f, avatar.position.z), navDirection, Color.white);

            // divide to speed and angle
            navDirection = transform.InverseTransformDirection(navDirection);
            speed = navDirection.z;
            direction = navDirection.x;

            // Debug.Log("Length: " + nav.desiredVelocity.magnitude + ", Speed: " + speed + ", Direction: " + direction);

        }
        else    // Free
        {
            // movement
            if (move)
            {
                // Debug.Log("move");

                // angle
                angle = FindAngle(avatar.forward, nav.desiredVelocity, avatar.up);

                // If the angle is within the deadZone...
                if (Mathf.Abs(angle) < deadZone)
                {
                    // ... set the direction to be along the desired direction and set the angle to be zero.
                    avatar.LookAt(avatar.position + nav.desiredVelocity);
                    angle = 0f;
                }

                // direction
                direction = angle / 180 * directionMultiplier;

                // speed
                speed = Vector3.Project(nav.velocity, avatar.forward).magnitude * speedMultiplier;
                speed = Mathf.Clamp(speed, 0f, nav.speed);
            }
            // idle, look at target
            else if (attributes.target)
            {
                // Debug.Log("idle");

                // angle
                Vector3 targetDirection = attributes.target.transform.position - avatar.position;
                float newAngle = FindAngle(avatar.forward, targetDirection, avatar.up);
                if (Mathf.Abs(newAngle) > 80)
                {
                    angle = newAngle;
                }

                // direction
                direction = newAngle / 90;

            }
        }

        // Call the Setup function of the helper class with the given parameters.
        SetupAnimator(speed, direction, angle);

        // move navMeshAgent back to gameobject 
        nav.nextPosition = this.transform.position;
    }
    
    
    float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        // Debug.Log(fromVector + ", " + toVector + ", " + upVector);

        // If the vector the angle is being calculated to is 0...
        if(toVector == Vector3.zero)
        {
            // ... the angle between them is 0.
            return 0f;
        }
            
        // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
        float angle = Vector3.Angle(fromVector, toVector);
        
        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        
        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
        
        // We need to convert the angle we've found from degrees to radians.
        // angle *= Mathf.Deg2Rad;

        return angle;
    }

    void SmoothRotateToTarget()
    {
        Vector3 finalDirection;

        // target?
        if (attributes.target)
        {
            // rotate to target

            // current Enemy position
            Vector3 enemyPos = attributes.target.transform.position;

            // Vector from char to enemy
            finalDirection = enemyPos - avatar.position;
        }
        else
        {
            Debug.LogWarning("no target to rotate to");
            // rotate to cam forward;
            finalDirection = avatar.forward;
        }

        finalDirection.y = 0;

        // rotates char to enemy
        avatar.forward = Vector3.Lerp(avatar.forward, finalDirection, rotationSpeed * Time.deltaTime);
    }

    public void SetupAnimator(float speed, float direction, float angle)
    {
        // Angular speed is the number of degrees per second.
        // float direction = angle / angleResponseTime;

        // Set the mecanim parameters and apply the appropriate damping to them.
        anim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
        anim.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);
        if (!IsInPivot())
        {
            anim.SetFloat("Angle", angle);
        }
    }
}
