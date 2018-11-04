using UnityEngine;
using System.Collections;

public class LastPlayerSighting : MonoBehaviour
{
    public Vector3 position = new Vector3(10000f, 10000f, 10000f);         // The last global sighting of the player.
    public Vector3 resetPosition = new Vector3(10000f, 10000f, 10000f);    // The default position if the player is not in sight.
}
