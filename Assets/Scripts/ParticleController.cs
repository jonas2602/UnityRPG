using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{

    private ParticleSystem partSystem;

    void Awake()
    {
        partSystem = GetComponentInChildren<ParticleSystem>();
    }

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Explode());
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("ParticleSystem is playing: " + partSystem.isPlaying);
	}

    IEnumerator Explode()
    {
        for (;;)
        {
            partSystem.Play();
            // partSystem.Stop();

            yield return new WaitForSeconds(20f);
        }
    }
}
