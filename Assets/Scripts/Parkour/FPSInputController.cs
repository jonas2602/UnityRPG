using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterMotor))]
[AddComponentMenu("Character/FPS Input Controller")]

public class FPSInputController : MonoBehaviour
{
    private CharacterMotor motor;
    private CharacterController controller;
    public Animator anim;
    public Transform cam;
    public bool grounded = false;
    bool locked = false;
    public float timer;
    public float moveTimer = 0;
    float vaultTimer;
    float wallRunTimer = 2.0f;
    float wallRunVertTimer = 2.0f;
    float grabTime = 0.0f;
    // public MouseLook mouseLookXCam;
    public GameObject debug;
    GameObject edge;
    // Use this for initialization
    void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = true;
        moveTimer -= Time.deltaTime;
        vaultTimer -= Time.deltaTime;
        grabTime -= Time.deltaTime;
        if (locked)
            return;
        //
        motor.movement.maxGroundAcceleration = 80.0f;
        // Get the input vector from kayboard or analog stick
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 velocity = motor.movement.velocity;
        grounded = motor.grounded;
        if (directionVector != Vector3.zero)
        {
            // Get the length of the directon vector and then normalize it
            // Dividing by the length is cheaper than normalizing when we already have the length anyway
            float directionLength = directionVector.magnitude;
            directionVector = directionVector / directionLength;

            // Make sure the length is no bigger than 1
            directionLength = Mathf.Min(1.0f, directionLength);

            // Make the input vector more sensitive towards the extremes and less sensitive in the middle
            // This makes it easier to control slow speeds when using analog sticks
            directionLength = directionLength * directionLength;

            // Multiply the normalized direction vector by the modified length
            directionVector = directionVector * directionLength;
        }
        //all this is in effect when the player is on the ground
        if (moveTimer < 0)
        {
            RaycastHit hit = new RaycastHit();
            if (grounded)
            {
                wallRunTimer = 2.0f;
                wallRunVertTimer = 2.0f;
                if (Input.GetButton("Sprint") || Input.GetButton("Crouch"))
                {
                    //slow the player when he's springing or running
                    directionVector /= 4.0f;
                }
                if (Input.GetAxisRaw("Vertical") != 0)
                {
                    motor.movement.maxForwardSpeed += Time.deltaTime * 3.0f;
                }
                else
                {
                    motor.movement.maxForwardSpeed -= Time.deltaTime * 10.0f;
                }
                motor.movement.maxForwardSpeed = Mathf.Clamp(motor.movement.maxForwardSpeed, 8.0f, 15.0f);
                if (Input.GetButton("Crouch"))
                {
                    if (controller.height == 2.0f)
                    {
                        cam.localPosition = new Vector3(0, 0.5f, 0);
                        controller.height = 1.0f;
                        transform.Translate(0, -0.5f, 0);
                    }
                    if (velocity.magnitude > 5.0f && Input.GetAxisRaw("Vertical") == 1)
                    {
                        motor.movement.maxGroundAcceleration = 10.1f;
                    }

                }
                else
                {

                    if (controller.height == 1.0f)
                    {
                        if (!Physics.Raycast(transform.position, Vector3.up, 2.2f))
                        {
                            cam.localPosition = new Vector3(0, 0.9f, 0);
                            controller.height = 2.0f;
                            transform.Translate(0, 0.6f, 0);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetButton("Grab") && grabTime < 0)
                {
                    CheckForGrab();
                }
                WallRun();
                WallRunVertical();
                CheckForVaulting();
                if (Input.GetButton("Crouch") && Physics.Raycast(transform.position, -transform.up, out hit))
                {
                    if (hit.distance < 2.2f && velocity.y < -10.0f)
                    {
                        StartCoroutine("Roll");
                    }
                }
                if (Input.GetButtonDown("Jump"))
                {
                    CheckJumpMoves(velocity);
                }
            }
            motor.inputJump = Input.GetButton("Jump");
        }
        motor.inputMoveDirection = transform.rotation * directionVector;

        //print (velocity.y);
    }
    IEnumerator Roll()
    {
        //this coroutine allows the player to roll after long falls. It is triggered above.
        //disable mouse look so we can rotate the camera
        // cam.GetComponent<MouseLook>().enabled = false;
        //lock the controller
        locked = true;
        //original amount was for making angles easier
        float originalAmount = 0.4f;
        timer = originalAmount;
        //make the controller short again
        if (controller.height == 2.0f)
        {
            cam.localPosition = new Vector3(0, 0.6f, 0);
            controller.height = 1.0f;
            transform.Translate(0, -0.5f, 0);
        }
        //now start the timer
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //move us forward so we can run on exit
            motor.inputMoveDirection = transform.rotation * new Vector3(0, 0, 0.2f);
            //rotate the camera to simulate roll
            cam.RotateAround(transform.position, transform.right, (360.0f / originalAmount) * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        locked = false;
        // cam.GetComponent<MouseLook>().enabled = true;
    }
    IEnumerator TicTac180(float angle)
    {
        //
        float originalAmount = 0.3f;
        moveTimer = 0.1f;
        timer = originalAmount;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //move us forward so we can run on exit
            motor.inputMoveDirection = transform.rotation * new Vector3(0, 0, 0.2f);
            //rotate the camera to simulate roll
            transform.Rotate(0, (angle / originalAmount) * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }
    void CheckJumpMoves(Vector3 velocity)
    {
        //this section controls both types of tic-tacs, that is, jumps to do while in air.
        //45deg tic-tac, or backtac
        //to the left
        Vector3 leftVector = Vector3.Normalize((-transform.right * 2) + -transform.forward);
        Vector3 rightVector = Vector3.Normalize((transform.right * 2) + -transform.forward);
        Vector3 footPos = transform.position - new Vector3(0, -1, 0);
        float pushDistance = 0.7f;
        bool hitRightBack = Physics.Raycast(footPos, rightVector, pushDistance);
        bool hitLeftBack = Physics.Raycast(footPos, leftVector, pushDistance);
        bool hitRightFront = Physics.Raycast(footPos, -leftVector, pushDistance);
        bool hitLeftFront = Physics.Raycast(footPos, -rightVector, pushDistance);
        bool hitForward = Physics.Raycast(footPos, transform.forward, pushDistance * 1.3f);
        if (hitLeftFront && velocity.y > -1.0f)
        {
            motor.movement.velocity += (rightVector) * 8.0f + Vector3.up * 7.0f;
            StartCoroutine("TicTac180", 45.0f);
            moveTimer = 0.5f;
            return;
        }
        if (hitRightFront && velocity.y > -1.0f)
        {
            motor.movement.velocity += (leftVector) * 8.0f + Vector3.up * 7.0f;
            StartCoroutine("TicTac180", -45.0f);
            moveTimer = 0.5f;
            return;
        }
        if (hitRightBack && velocity.y > -1.0f)
        {
            motor.movement.velocity = (-rightVector) * 8.0f + Vector3.up * 7.0f;
            moveTimer = 0.5f;
            return;
        }
        if (hitLeftBack && velocity.y > -1.0f)
        {
            motor.movement.velocity = (-leftVector) * 8.0f + Vector3.up * 7.0f;
            moveTimer = 0.5f;
            return;
        }
        if (hitForward && velocity.y > -1.0f)
        {
            motor.movement.velocity = (-transform.forward) * 5.0f + Vector3.up * 7.0f;
            StartCoroutine("TicTac180", 180.0f);
            moveTimer = 0.5f;
            return;
            //
        }
    }
    void CheckForVaulting()
    {
        //
        Vector3 velocity = motor.movement.velocity;
        float groundSpeed = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z);
        if (velocity.y < -1.0f || !Input.GetButton("Vertical") || groundSpeed < 2.0f || vaultTimer > 0)
        {
            return;
        }
        Vector3 footPos = transform.position - new Vector3(0, -1.0f, 0);
        Vector3 headPos = transform.position - new Vector3(0, 0.8f, 0);
        float pushDistance = 1.7f;
        bool hitForwardFeet = Physics.Raycast(footPos, transform.forward, pushDistance);
        bool hitForwardHead = Physics.Raycast(headPos, transform.forward, pushDistance);
        if (hitForwardHead && !hitForwardFeet)
        {
            motor.movement.velocity += new Vector3(0, 4.0f, 0);
            vaultTimer = 0.3f;

        }

    }
    void WallRun()
    {
        Vector3 velocity = motor.movement.velocity;
        float groundSpeed = Mathf.Abs(velocity.x) + Mathf.Abs(velocity.z);
        if (velocity.y < -1.0f || !Input.GetButton("Vertical") || groundSpeed < 10.0f || wallRunTimer <= 0)
        {
            return;
        }
        Vector3 footPos = transform.position - new Vector3(0, -1.0f, 0);
        float pushDistance = 1.7f;
        bool wallLeft = Physics.Raycast(footPos, -transform.right, pushDistance);
        bool wallRight = Physics.Raycast(footPos, transform.right, pushDistance);
        wallRunTimer -= Time.deltaTime;
        //
        if (wallLeft || wallRight)
        {
            Vector3 camVect = new Vector3(cam.forward.x, Mathf.Clamp(velocity.y, 0.0f, 500.0f) / motor.movement.maxForwardSpeed, cam.forward.z);
            motor.movement.velocity = camVect * motor.movement.maxForwardSpeed;
        }
    }
    void WallRunVertical()
    {
        Vector3 velocity = motor.movement.velocity;
        //float groundSpeed = Mathf.Abs (velocity.x) + Mathf.Abs (velocity.z);
        if (velocity.y < 3.0f || !Input.GetButton("Vertical") || wallRunVertTimer <= 0)
        {
            return;
        }
        Vector3 footPos = transform.position - new Vector3(0, -1.0f, 0);
        float pushDistance = 1.7f;
        bool wallRun = Physics.Raycast(footPos, transform.forward, pushDistance);
        wallRunVertTimer -= Time.deltaTime * 1.0f;
        if (wallRun)
        {
            //Vector3 camVect = new Vector3(cam.forward.x, cam.forward.y*0.1f, cam.forward.z);
            motor.movement.velocity = new Vector3(velocity.x, 3.0f * wallRunVertTimer, velocity.z);
        }
    }
    void CheckForGrab()
    {

        /* old method that used forward then down
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitA))
        {
            RaycastHit hitB;
            float distanceForward = hitA.distance;
            if(distanceForward < 1.0f && Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0) + transform.forward+(transform.up*2), -Vector3.up, out hitB))
            {
                float distanceDown = hitB.distance;
                if(distanceDown > 0 && distanceDown < 1.0f)
                {
                    Vector3 grabPoint = transform.position  + new Vector3(0, 0.5f, 0)+ (transform.forward*distanceForward) + (transform.up *(2.0f-hitB.distance));
                    GameObject newEdge = (GameObject)Instantiate(debug, grabPoint, Quaternion.identity);
                    edge = newEdge;
                    edge.transform.up = hitB.normal;
                    edge.transform.forward = hitA.normal;
                    StartCoroutine("EdgeGrab");
                }
            }
        }
        */
        RaycastHit hitA;
        RaycastHit hitB;
        //
        if (Physics.Raycast(transform.position + (transform.forward * 0.5f) + (transform.up * 2), -Vector3.up, out hitB))
        {
            print(hitB.distance);
            if (hitB.distance < 1.0f && Physics.Raycast(hitB.point - new Vector3(0, 0.1f, 0) - (transform.forward * 0.5f), transform.forward, out hitA))
            {
                float distanceDown = hitB.distance;
                if (distanceDown > 0 && distanceDown < 1.5f)
                {
                    Vector3 grabPoint = new Vector3(hitA.point.x, hitB.point.y, hitA.point.z);
                    ////transform.position  + new Vector3(0, 0.5f, 0)+ (transform.forward*distanceForward) + (transform.up *(2.0f-hitB.distance));
                    GameObject newEdge = (GameObject)Instantiate(debug, grabPoint, Quaternion.identity);
                    edge = newEdge;
                    edge.transform.up = hitB.normal;
                    edge.transform.forward = hitA.normal;
                    StartCoroutine("EdgeGrab");
                }
                if (distanceDown < 1.5f && distanceDown < 2.0f)
                {
                    //
                    motor.movement.velocity += new Vector3(0, 4.0f, 0);
                }
            }
        }
    }
    IEnumerator EdgeGrab()
    {
        grabTime = 0.3f;
        motor.movement.velocity = Vector3.zero;
        motor.enabled = false;
        transform.position = edge.transform.position - (transform.forward * 0.5f) - (transform.up * 1.7f);
        //this coroutine is for grabbing the edges of things to hold onto
        //disable mouse look so we can rotate the camera
        // GetComponent<MouseLook>().enabled = false;
        // mouseLookXCam.sensitivityX = 15;
        //lock the controller
        locked = true;


        while (true)
        {
            float pullUpTime = 0.8f;
            //print ("looping");
            if (grabTime < 0)
            {
                if (Input.GetButton("Grab"))
                {
                    //print ("End loop");
                    while (pullUpTime > 0)
                    {
                        pullUpTime -= Time.deltaTime;
                        transform.Translate(0, 4.0f * Time.deltaTime, 1.0f * Time.deltaTime);
                        yield return new WaitForEndOfFrame();
                    }
                    //pull ourself up
                    break;
                }
                if (Input.GetButton("Crouch"))
                {
                    //print ("End loop");
                    //fall straight down
                    break;
                }
                if (Input.GetButton("Jump"))
                {
                    //print ("End loop");
                    //motor.movement.velocity = cam.forward*5.0f;
                    motor.grounded = true;
                    motor.inputJump = true;
                    //leap in direction of camera
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
        locked = false;
        // GetComponent<MouseLook>().enabled = true;
        // mouseLookXCam.sensitivityX = 0;
        transform.rotation = Quaternion.Euler(0.0f, cam.eulerAngles.y, 0.0f);
        cam.rotation = Quaternion.Euler(cam.eulerAngles.x, transform.eulerAngles.y, cam.eulerAngles.z);
        motor.enabled = true;
    }
}