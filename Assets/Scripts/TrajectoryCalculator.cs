using UnityEngine;
// using UnityEditor;
using System.Collections;

public class TrajectoryCalculator : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform ball;
    [SerializeField]
    private float startVelocity;
    [SerializeField]
    private Vector3 velocity = Vector3.zero; 

    private Rigidbody sphereRb;

    void Awake()
    {
        sphereRb = ball.GetComponent<Rigidbody>();
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(velocity != Vector3.zero)
        {
            Debug.DrawRay(this.transform.position, velocity);
        }
	}

    public void ThrowBallAtTargetLocation()
    {
        if (!target)
        {
            Debug.LogError("Target missing");
        }

        velocity = CalculateTrajectory(this.transform.position, target.position, startVelocity, Physics.gravity.magnitude);

        // try it
        ResetBall();
        sphereRb.useGravity = true;
        sphereRb.AddForce(velocity, ForceMode.Impulse);
    }

    // Throws ball at location with regards to gravity (assuming no obstacles in path) and initialVelocity (how hard to throw the ball)
    public static Vector3 CalculateTrajectory(Vector3 startLocation, Vector3 targetLocation, float gravity)
    {
        // check if target is in range
        Vector3 direction = (targetLocation - startLocation).normalized;
        Vector3 groundedDirection = new Vector3(direction.x, 0, direction.z);
        float distance = Vector3.Distance(targetLocation, startLocation);

        // calculate min initial velocity
        float heightDifference = startLocation.y - targetLocation.y;
        float minVelocity = Mathf.Sqrt(gravity * Mathf.Sqrt(distance * distance + heightDifference * heightDifference) - gravity * heightDifference);

        // calculate angle
        float angle = 45f;
        if (heightDifference < 0)
        {
            angle = 0.5f * Mathf.Asin((gravity * distance) / (minVelocity * minVelocity)) * Mathf.Rad2Deg; // Mathf.Asin(Mathf.Sqrt(1 / 2 - heighDifference / (2 * Mathf.Sqrt(distance * distance + heighDifference * heighDifference)))) * Mathf.Rad2Deg;
        }
        
        // try it
        // ResetBall();
        // sphereRb.useGravity = true;
        return Quaternion.AngleAxis(-angle, Vector3.right) * direction * minVelocity;
        // sphereRb.AddForce(velocity, ForceMode.Impulse);
    }

    // allways shoot with max velocity
    public static Vector3 CalculateTrajectory(Vector3 startLocation, Vector3 targetLocation, float initialVelocity, float gravity)
    {
        Vector3 direction = (targetLocation - startLocation).normalized;
        float distance = Vector3.Distance(targetLocation, startLocation);
        float heightDifference = startLocation.y - targetLocation.y;

        // calculate angle
        float angle = 0.5f * Mathf.Asin((gravity * distance) / (initialVelocity * initialVelocity)) * Mathf.Rad2Deg;
        
        return Quaternion.AngleAxis(-angle, Vector3.right) * direction * initialVelocity;
    }

        
        /*
        Vector3 direction = (targetLocation - transform.position).normalized;
        Vector3 groundedDirection = new Vector3(targetLocation.x, 0, targetLocation.z) - new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(targetLocation, transform.position);
        float xDistance = groundedDirection.magnitude;

        float heighDifference = this.transform.position.y - target.position.y;
        float angle = Mathf.Asin(Mathf.Sqrt(1 / 2 - heighDifference / (2 * Mathf.Sqrt(xDistance * xDistance + heighDifference * heighDifference))));

        // try it
        ResetBall();
        sphereRb.useGravity = true;
        velocity = Quaternion.AngleAxis(-angle, groundedDirection) * groundedDirection.normalized * initialVelocity;
        sphereRb.AddForce(velocity, ForceMode.Impulse);
    }
    
        Vector3 direction = (targetLocation - transform.position).normalized;
        float distance = Vector3.Distance(targetLocation, transform.position);

        // check if target is in range
        float heighDifference = this.transform.position.y - target.position.y;
        float maxDistance = initialVelocity / gravity * Mathf.Sqrt(initialVelocity * initialVelocity + 2 * gravity * heighDifference);
        
        float firingElevationAngle = FiringElevationAngle(gravity, distance, initialVelocity);
        Vector3 elevation = Quaternion.AngleAxis(firingElevationAngle, transform.right) * transform.up;
        float directionAngle = AngleBetweenAboutAxis(transform.forward, direction, transform.up);
        Vector3 velocity = Quaternion.AngleAxis(directionAngle, transform.up) * elevation * initialVelocity;
        
        // ballGameObject is object to be thrown
        ResetBall();
        sphereRb.useGravity = true;
        sphereRb.AddForce(velocity, ForceMode.VelocityChange);
    }
    
    // Helper method to find angle between two points (v1 & v2) with respect to axis n
    public static float AngleBetweenAboutAxis(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    // Helper method to find angle of elevation (ballistic trajectory) required to reach distance with initialVelocity
    // Does not take wind resistance into consideration.
    private float FiringElevationAngle(float gravity, float distance, float initialVelocity)
    {
        return 0.5f * Mathf.Asin((gravity * distance) / (initialVelocity * initialVelocity)) * Mathf.Rad2Deg;
    }
    */
    public void ResetBall()
    {
        ball.position = this.transform.position;
        sphereRb.useGravity = false;
    }
}
/*
[CustomEditor(typeof(TrajectoryCalculator), true)]
public class TrajectoryCalculatorInspector : Editor // Window
{
    private static TrajectoryCalculator calculator;

    void OnEnable()
    {
        calculator = target as TrajectoryCalculator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Calculate Flight"))
        {
            calculator.ThrowBallAtTargetLocation();
        }

        if (GUILayout.Button("Reset Ball"))
        {
            calculator.ResetBall();
        }
    }
}*/
