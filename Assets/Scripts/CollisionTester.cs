using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionTester : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cubesInRange = new List<GameObject>();

    private GameObject me;

    
	// Use this for initialization
	void Start ()
    {
        StartCoroutine(ListController());
        me = transform.root.gameObject;

        cubesInRange.Add(me);
        cubesInRange.Add(me);
        cubesInRange.RemoveAll(cube => cube == me);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter: " + other.name);
        if (other.gameObject != me)
        {
            cubesInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("TriggerExit");
        cubesInRange.Remove(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CollisionEnter");
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("CollisionExit");
    }

    IEnumerator ListController()
    {
        for (;;)
        {
            for (int i = 0; i < cubesInRange.Count; i++)
            {
                // Debug.Log(cubesInRange[i]);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
