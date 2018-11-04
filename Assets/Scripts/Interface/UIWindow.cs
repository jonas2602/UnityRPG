using UnityEngine;
using System.Collections;

public abstract class UIWindow: MonoBehaviour
{
    public virtual void OpenWindow()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void CloseWindow()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void SetupPlayer(GameObject player)
    {
        
    }
}
