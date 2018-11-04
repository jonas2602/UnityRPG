using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AggrobarScript : MonoBehaviour {
    
    private Image interestBar;
    private Image aggroBar;
    private GameObject barObject;

    [SerializeField]
    private float aggro;

    // Use this for initialization
    void Awake()
    {
        interestBar = GetComponent<Image>();
        aggroBar = transform.GetChild(0).GetComponent<Image>();
        barObject = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateAggro(float newAggro)
    {
        float oldAggro = this.aggro;

        // update bar
        this.aggro = newAggro;
        interestBar.fillAmount = newAggro / 0.5f;
        aggroBar.fillAmount = (newAggro - 0.5f) / 0.5f;
        // Debug.Log("Aggro: " + aggroBar.fillAmount + "Interest: " + interestBar.fillAmount);

        // show bar
        if (oldAggro == 0)
        {
            barObject.SetActive(true);
        }
        // hide bar
        else if(newAggro == 0)
        {
            barObject.SetActive(false);
        }
    }
}
