using UnityEngine;
using System.Collections;

public class CameraFrustumTester : MonoBehaviour {

    public Transform target;
    private Camera cam;
    

	void Awake ()
    {
        cam = GetComponent<Camera>();
	}

    // Update is called once per frame
    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<MeshRenderer>().bounds))
        {
            Debug.Log(true);
        }
        else
        {
            Debug.Log(false);
        }
    }
}
