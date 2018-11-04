using UnityEngine;
using System.Collections;

public class NavMeshTester : MonoBehaviour
{

    private ParticleSystem partSystem;

    private UnityEngine.AI.NavMeshAgent nav;
    public Transform target;
    public bool movementControl;
    public float speedMultiplier = 0.02f;

    public bool moveSphere;

    void Awake()
    {
        partSystem = target.GetComponentInChildren<ParticleSystem>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Use this for initialization
    void Start()
    {
        // Making sure the rotation/position is controlled by Mecanim.
        // nav.updateRotation = false;
        nav.updatePosition = false;

        EventManager.OnClicked += TestFunction;
    }

    // Update is called once per frame
    void Update()
    {
        // target reached?
        if (Vector3.Distance(nav.transform.position, target.position) < 2f && moveSphere)
        {
            partSystem.Play();

            // move target
            float coordX = Random.value * 30f + 10;
            float coordZ = Random.value * 30f + 10;
            target.position = new Vector3(coordX, target.position.y, coordZ);
            
            // set new destination
            nav.SetDestination(target.position);

            // Debug.LogWarning("targetPos: " + target.position + " Destination: " + nav.destination);
        }

        // stop current path
        if (Input.GetKeyDown("p"))
        {
            nav.Stop();
        }

        // continue current path
        if (Input.GetKeyDown("r"))
        {
            nav.Resume();
        }

        // 
        if (Input.GetKeyDown("m"))
        {
            Vector3 currentPos = this.transform.position;
            nav.Move(new Vector3(currentPos.x + 2f, currentPos.y, currentPos.z));
        }

        if (Input.GetKeyDown("w"))
        {
            // teleport to next corner of the path
            nav.Warp(nav.steeringTarget);
            Debug.Log(nav.hasPath);
            // continue path
            nav.SetDestination(target.position);
        }

        // set max speed
        if (Input.GetKeyDown("2"))
        {
            // change speed
            nav.speed = 2f;
            // change acceleration(f'(speed))
            nav.acceleration = 5f; // ~2.5*speed
        }
        if (Input.GetKeyDown("0"))
        {
            nav.speed = 10f;
            nav.acceleration = 25f;
        }

        // Debug.Log("velocity(speed): " + Vector3.Magnitude(nav.velocity) + " DesiredVelocity: " + Vector3.Magnitude(nav.desiredVelocity) + " RemainingDistance: " + nav.remainingDistance);
        // smoothed moveDirection
        // Debug.DrawRay(this.transform.position, nav.velocity, Color.grey);
        // vector to next cornor of the path
        // Debug.DrawRay(this.transform.position + this.transform.up, nav.desiredVelocity, Color.blue);

        ControllCube();
    }

    void ControllCube()
    {
        // Debug.Log(Time.deltaTime);
        if (movementControl)
        {
            this.transform.position += nav.velocity * speedMultiplier; // Time.deltaTime;
        }
        nav.nextPosition = this.transform.position;
    }

    void TestFunction()
    {
        Debug.Log("event happend");
    }
}