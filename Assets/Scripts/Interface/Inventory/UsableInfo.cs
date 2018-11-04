using UnityEngine;
using System.Collections;

public class UsableInfo : MonoBehaviour
{
    [SerializeField]
    private UsableOnObject[] slots;

    public void UpdateUsables(Item[] equipedItems)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetupItem(equipedItems[i]);
        }
    }

    void Awake()
    {
        slots = GetComponentsInChildren<UsableOnObject>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
