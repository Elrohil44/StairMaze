using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadWallText : MonoBehaviour
{

    private bool triggered;

    void OnTriggerEnter(Collider collider)
    {

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player" && !triggered)
        {
            TriggerMotionBlur(collider.gameObject);
            triggered = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {

    }

    void TriggerMotionBlur(GameObject player)
    {
        player.GetComponent<PlayerBehaviour>().TriggerMotionBlur();
    }

}
