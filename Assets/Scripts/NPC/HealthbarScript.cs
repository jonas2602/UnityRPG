using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour {

    private Image healthbar;
    private GameObject barObject;

	// Use this for initialization
	void Awake ()
    { 
        healthbar = GetComponent<Image>();
        barObject = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void UpdateHealth(float fillAmount)
    {
        // update bar
        healthbar.fillAmount = fillAmount;
    }

    IEnumerator ShowBar()
    {
        barObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        // barObject.SetActive(false);
    }
}
