using UnityEngine;
using System.Collections;

public class GroupManager : MonoBehaviour
{
    public Group group;
    public MatchManager match;

    void Awake()
    {
        group = new Group(new GameObject[] { this.gameObject });
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
