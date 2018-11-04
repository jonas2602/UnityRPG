using UnityEngine;
using System.Collections;

public class CameraControllerNew : MonoBehaviour
{

    private Transform avatar;
    private AnimInfo animInfo;
    private UIManager uiManager;
    private Transform stomage;
    private Transform freePivot;

    private Transform cameraPivot;
    private Vector3 freePos = new Vector3(0f, 1.27f, 0f);
    private Vector3 aimPos = new Vector3(-1.62f, 1.21f, 0.83f);
    private float aimCamDistance = 3.5f;
    private CamStates currentCamState;

    private Transform camTransform;
    [SerializeField]
    private Quaternion pivotRotation;
    [SerializeField]
    private Vector3 pivotPosition;

    [SerializeField]
    private float wheelSensitivity;
    [SerializeField]
    private float camMinDistance;
    [SerializeField]
    private float camMaxDistance;
    [SerializeField]
    private float mouseSensitivity;

    [SerializeField]
    private float xAxisRot;
    [SerializeField]
    private float yAxisRot;
    [SerializeField]
    private float camDistance;

    // Smoothing and damping
    private Vector3 velocityPivotSmooth = Vector3.zero;
    private Vector3 velocityCamSmooth = Vector3.zero;
    [SerializeField]
    private float camSmoothDampTime = 0.1f;
    [SerializeField]
    private float pivotSmoothDampTime = 10f;

    public void SetupCamera(Transform avatar)
    {
        // get references
        this.avatar = avatar;
        animInfo = avatar.GetComponent<AnimInfo>();
        uiManager = GameObject.FindWithTag("Interface").GetComponent<UIManager>();
        stomage = avatar.Find("Bones/Bones|Steuerbone/Bones|Bauch");
        cameraPivot = avatar.Find("CameraPivot");

        // set camera
        camTransform.SetParent(cameraPivot);
        camTransform.localPosition = new Vector3(0, 0, camDistance);
        camTransform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    public enum CamStates
    {
        Free,
        Aim /*,
        FirstPerson,
        Dialog*/
    }

    void Awake()
    {
        camTransform = this.transform;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (!avatar)
        {
            return;
        }

        if (!uiManager.IsInMenu())
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

            // update camDistance
            if (mouseWheel != 0 && !animInfo.IsAiming())
            {
                camDistance = camDistance + mouseWheel * wheelSensitivity;
                camDistance = Mathf.Clamp(camDistance, camMinDistance, camMaxDistance);
                // camTransform.localPosition = new Vector3(0, 0, camDistance);
                // camPosition = new Vector3(0, 0, camDistance);
            }

            // move camPivot
            if (animInfo.IsAiming() && currentCamState == CamStates.Free)
            {
                // camTransform.localPosition = new Vector3(0, 0, aimCamDistance);
                camDistance = aimCamDistance;
                // camPosition = new Vector3(0, 0, aimCamDistance);
                currentCamState = CamStates.Aim;
            }
            else if (!animInfo.IsAiming() && currentCamState == CamStates.Aim)
            {
                cameraPivot.localPosition = freePos;
                // pivotPosition = freePos;
                currentCamState = CamStates.Free;
            }

            // update camRotation
            xAxisRot = Mathf.Clamp(xAxisRot + mouseY * mouseSensitivity, -89, 89);
            yAxisRot = (yAxisRot + mouseX * mouseSensitivity) % 360;
            cameraPivot.eulerAngles = new Vector3(xAxisRot, yAxisRot, 0);
            // pivotRotation = Quaternion.Euler(xAxisRot, yAxisRot, 0);

            // smooth positioning
            // cameraPivot.localPosition = SmoothPosition(cameraPivot.localPosition, pivotPosition);
            // cameraPivot.rotation = SmoothRotation(cameraPivot.rotation, pivotRotation);
            camTransform.localPosition = SmoothPosition(camTransform.localPosition, new Vector3(0, 0, camDistance));

            // update bones
            if (animInfo.IsAiming())
            {
                // rotates avatar around y-Axis
                Vector3 avatarForward = camTransform.forward;
                avatarForward.y = 0;
                avatar.forward = avatarForward;

                // up / down
                stomage.Rotate(cameraPivot.right, cameraPivot.eulerAngles.x, Space.World);

                // move camPivot
                Vector3 aimAdditionalPos = stomage.right * aimPos.x + stomage.up * aimPos.y + stomage.forward * aimPos.z;
                cameraPivot.position = stomage.position + aimAdditionalPos;

                MunitionController munContr = avatar.GetComponentInChildren<ArrowController>();
                if (munContr)
                {
                    munContr.SetForceDirection(camTransform.forward);
                }
            }
        }
    }

    private Quaternion SmoothRotation(Quaternion fromRot, Quaternion toRot)
    {
        return Quaternion.Slerp(fromRot, toRot, Time.deltaTime * pivotSmoothDampTime);
    }

    private Vector3 SmoothPosition(Vector3 fromPos, Vector3 toPos)
    {
        // Making a smooth transition between camera's current position and the position it wants to be in
        return Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
    }
}