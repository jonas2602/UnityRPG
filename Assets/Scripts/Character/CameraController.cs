using UnityEngine;
using System.Collections;



// Struct to hold data for aligning camera
struct CameraPosition
{
    // Position to align camera to, probably somewhere behind the character
    // or position to point camera at, probably somewhere along character's axis
    private Vector3 position;
    // Transform used for any rotation
    private Transform xForm;

    public Vector3 Position 
    { 
        get 
        { 
            return position; 
        } 

        set 
        { 
            position = value; 
        } 
    }
    public Transform XForm 
    { 
        get
        { 
            return xForm; 
        }
 
        set 
        { 
            xForm = value; 
        } 
    }

    public void Init(string camName, Vector3 pos, Transform transform, Transform parent)
    {
        position = pos;
        xForm = transform;
        xForm.name = camName;
        xForm.parent = parent;
        xForm.localPosition = Vector3.zero;
        xForm.localPosition = position;
    }
}



public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float up = 0.69f;
    [SerializeField]
    private float right = 1.59f;
    [SerializeField]
    private float forward = -1.2f;

    // Inspector serialized	
    [SerializeField]
    private Transform cameraXform;
    [SerializeField]
    private float distanceAway;
    [SerializeField]
    private float distanceRight;
    [SerializeField]
    private float distanceUp;
    [SerializeField]
    private CharakterControler characterControler;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private Transform cameraPivot;
    [SerializeField]
    private Transform avatar;
    [SerializeField]
    private AnimInfo animInfo;
    [SerializeField]
    private float firstPersonLookSpeed = 3.0f;
    /*[SerializeField]
    private Vector2 firstPersonXAxisClamp = new Vector2(-70.0f, 90.0f);
    [SerializeField]
    private float fPSRotationDegreePerSecond = 120f;*/
    [SerializeField]
    private float mouseWheelSensitivity = 3.0f;
    [SerializeField]
    private float compensationOffset = 0.2f;
    [SerializeField]
    private CamStates startingState = CamStates.Free;


    // Smoothing and damping
    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;


    // Private global only
    private Vector3 curLookDir;
    public Transform arrowPivot;
    public CamStates camState;
    public float xAxisRot = 0.0f;
    public float yAxisRot = 0.0f;
    private CameraPosition firstPersonCamPos;
    private Vector3 nearClipDimensions = Vector3.zero; // width, height, radius
    private Vector3[] viewFrustum;
    private Vector3 characterHeightOffset;
    private Vector3 camPosition;
    // private Transform head;
    private Transform chest;

    private float mouseX;
    private float mouseY;
    private float mouseWheel;
    private Vector3 lookAt;

    public Transform CameraXform
    {
        get
        {
            return this.cameraXform;
        }
    }

    public Vector3 LookDir
    {
        get
        {
            return this.curLookDir;
        }
    }
  
    public CamStates CamState
    {
        get
        {
            return this.camState;
        }
    }

    public CamStates SetCamState
    {
        set
        {
            camState = value;

            if(camState == CamStates.Free)
            {
                distanceAway = 5f;
                distanceRight = 0f;
                distanceUp = 1f;
            }
            else if (camState == CamStates.Target)
            {
                distanceAway = -1.86f;
                distanceRight = 0.01f;
                distanceUp = 0.77f;
            }
        }
    }

    public enum CamStates
    {
        Behind,
        Free,
        Target /*,
        FirstPerson,
        Dialog*/
    }

    public Vector3 CamToCharDirection
    {
        get
        {
            // Move height and distance from character in separate parentRig transform since RotateAround has control of both position and rotation
            Vector3 camToCharDirection = Vector3.Normalize(characterHeightOffset - this.transform.position);
            
            // Can't calculate distanceAway from a vector with Y axis rotation in it; zero it out
            // camToCharDirection.y = 0f;

            return camToCharDirection;
        }
    }


    public void SetupCamera(Transform avatar)
    {
        this.avatar = avatar;
        animInfo = avatar.GetComponent<AnimInfo>();
        uiManager = GameObject.FindWithTag("Interface").GetComponent<UIManager>();
        characterControler = avatar.GetComponentInChildren<CharakterControler>(true);
        arrowPivot = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/ArrowPivot");
        // head = character.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb/Bones|Hals/Bones|Kopf");
        chest = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch/Bones|Brustkorb");

        cameraPivot = avatar.Find("CameraPivot").transform;
    }


    void Start()
    {
        cameraXform = this.transform;
        if (cameraXform == null)
        {
            Debug.LogError("Parent camera to empty GameObject.", this);
        }

        curLookDir = avatar.forward;


        // Position and parent a GameObject where first person view should be
        firstPersonCamPos = new CameraPosition();
        firstPersonCamPos.Init
            (
                "First Person Camera",
                new Vector3(0.0f, 1.75f, 0.2f),
                new GameObject().transform,
                characterControler.transform
            );

        camState = startingState;

        // Intialize values to avoid having 0s
        characterHeightOffset = avatar.position + new Vector3(0f, distanceUp, 0f);
        
    }

    void Update()
    {
        // calculate new camera position

        // move camera towards target position
    }

    // Debugging information should be put here.
    void OnDrawGizmos()
    {

    }

    void LateUpdate()
    {
        viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);

        // Pull values from controller/keyboard
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        
        characterHeightOffset = avatar.position + (distanceUp * avatar.up);
        lookAt = cameraPivot.transform.position;
        camPosition = Vector3.zero;



        // Determine camera state
        // * Targeting *
        if (animInfo.IsAiming())
        {
            Aiming();
        }
        // * Free *
        else
        {
            Free();
        }
        
        //CompensateForWalls(characterOffset, ref targetPosition);
        SmoothPosition(cameraXform.position, camPosition);
        transform.LookAt(lookAt);

        if(animInfo.IsAiming())
        {
            // rotates avatar around y-Axis
            Vector3 avatarForward = cameraXform.forward;
            avatarForward.y = 0;

            avatar.forward = avatarForward;

            // up / down
            Vector3 camHorizontal = cameraXform.forward;
            camHorizontal.y = 0;
            Vector3 axisSign = Vector3.Cross(cameraXform.forward, camHorizontal);
            Debug.Log(axisSign);
            float angle = Vector3.Angle(camHorizontal, cameraXform.forward) * (axisSign.y >= 0 ? -1f : 1f);

            chest.Rotate(cameraXform.right, angle);
        }
    }


    private void Free()
    {
        //Zooming
        if (mouseWheel != 0)
        {
            distanceAway -= mouseWheel * mouseWheelSensitivity;
            distanceAway = Mathf.Clamp(distanceAway, 2, 10);
        }

        if (!uiManager.IsInMenu())
        {
            // capture mouse movement
            xAxisRot += (-mouseY * firstPersonLookSpeed);
            yAxisRot += (mouseX * firstPersonLookSpeed);

            // Clamp x-rotation
            xAxisRot = Mathf.Clamp(xAxisRot, -55, 80);
        }

        //rotates Gameobjects
        if (yAxisRot != 0 || xAxisRot != 0)
        {
            // Camera
            cameraXform.transform.localRotation = Quaternion.Euler(xAxisRot, yAxisRot, 0);
        }

        // Still need to track camera behind player even if they aren't using the right stick; achieve this by saving distanceAwayFree every frame
        if (camPosition == Vector3.zero)
        {
            camPosition = characterHeightOffset - cameraXform.forward * distanceAway;
        }
    }
    
    /*private void FirstPerson()
    {
        // Looking up and down
        // Calculate the amount of rotation and apply to the firstPersonCamPos GameObject
        xAxisRot += (-mouseY * firstPersonLookSpeed);
        // Clamp x-rotation
        xAxisRot = Mathf.Clamp(xAxisRot, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
        //rotates FPGameObject
        firstPersonCamPos.XForm.localRotation = Quaternion.Euler(xAxisRot, 0, 0);

        // creates rotation from lastframe rotation to current rotation
        Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCamPos.XForm.forward);
        // Superimpose firstPersonCamPos GameObject's rotation on camera(this)
        this.transform.rotation = rotationShift * this.transform.rotation;

        // Set Head rotation
        head.rotation = this.transform.rotation;
        // Move character model's head
        head.rotation = (head.rotation * rotationShift);


        // Looking left and right
        // Similarly to how character is rotated while in locomotion, use Quaternion * to add rotation to character
        Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, fPSRotationDegreePerSecond * (mouseX < 0f ? -1f : 1f), 0f), Mathf.Abs(mouseX));
        Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
        characterControler.transform.rotation = (characterControler.transform.rotation * deltaRotation);

        // Move camera to firstPersonCamPos
        camPosition = firstPersonCamPos.XForm.position;

        // Smoothly transition look direction towards firstPersonCamPos when entering first person mode
        lookAt = Vector3.Lerp(camPosition + character.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);

        // Choose lookAt target based on distance
        lookAt = (Vector3.Lerp(this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position, firstPersonCamPos.XForm.position)));

    }
    */

    private void Aiming()
    {
        ResetCamera();
        curLookDir = avatar.forward;

        // capture mouse movement
        xAxisRot += (-mouseY * firstPersonLookSpeed);
        yAxisRot += (mouseX * firstPersonLookSpeed);

        xAxisRot = Mathf.Clamp(xAxisRot, -55, 80);

        //rotates Gameobjects
        if (yAxisRot != 0 || xAxisRot != 0)
        {
            // Camera
            cameraXform.transform.localRotation = Quaternion.Euler(xAxisRot, yAxisRot, 0);
        }

        // FPGameobject
        firstPersonCamPos.XForm.localRotation = Quaternion.Euler(0, yAxisRot, 0);

        cameraPivot.transform.rotation = Quaternion.Euler(xAxisRot, yAxisRot, 0);
        lookAt = chest.position + chest.forward * forward + chest.right * right + chest.up * up;
        arrowPivot.position = lookAt;

        if (camPosition == Vector3.zero)
        {
            camPosition = cameraPivot.position + cameraPivot.forward * distanceAway + cameraPivot.right * distanceRight + cameraPivot.up * distanceUp;
        }
    }

    private void SmoothPosition(Vector3 fromPos, Vector3 toPos)
    {
        // Making a smooth transition between camera's current position and the position it wants to be in
        cameraXform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
    }
    
    private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
    {
        // Compensate for walls between camera
        RaycastHit wallHit = new RaycastHit();
        if (Physics.Linecast(fromObject, toTarget, out wallHit))
        {
            Debug.DrawRay(wallHit.point, wallHit.normal, Color.red);
            toTarget = wallHit.point;
        }

        // Compensate for geometry intersecting with near clip plane
        Vector3 camPosCache = GetComponent<Camera>().transform.position;
        GetComponent<Camera>().transform.position = toTarget;
        viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);

        for (int i = 0; i < (viewFrustum.Length / 2); i++)
        {
            RaycastHit cWHit = new RaycastHit();
            RaycastHit cCWHit = new RaycastHit();

            // Cast lines in both directions around near clipping plane bounds
            while (Physics.Linecast(viewFrustum[i], viewFrustum[(i + 1) % (viewFrustum.Length / 2)], out cWHit) ||
                   Physics.Linecast(viewFrustum[(i + 1) % (viewFrustum.Length / 2)], viewFrustum[i], out cCWHit))
            {
                Vector3 normal = wallHit.normal;
                if (wallHit.normal == Vector3.zero)
                {
                    // If there's no available wallHit, use normal of geometry intersected by LineCasts instead
                    if (cWHit.normal == Vector3.zero)
                    {
                        if (cCWHit.normal == Vector3.zero)
                        {
                            Debug.LogError("No available geometry normal from near clip plane LineCasts. Something must be amuck.", this);
                        }
                        else
                        {
                            normal = cCWHit.normal;
                        }
                    }
                    else
                    {
                        normal = cWHit.normal;
                    }
                }

                toTarget += (compensationOffset * normal);
                GetComponent<Camera>().transform.position += toTarget;

                // Recalculate positions of near clip plane
                viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);
            }
        }

        GetComponent<Camera>().transform.position = camPosCache;
        viewFrustum = DebugDraw.CalculateViewFrustum(GetComponent<Camera>(), ref nearClipDimensions);
    }
    
    private void ResetCamera()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime);
    }

}

