using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override IEnumerator Dead()
    {
        // show deathwindow

        // wait for despawn
        yield return new WaitForSeconds(despawnTime);

        // despawn
        Destroy(this.gameObject);
    }
}
