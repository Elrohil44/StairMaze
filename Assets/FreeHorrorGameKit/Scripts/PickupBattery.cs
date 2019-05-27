using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBattery : MonoBehaviour
{
    [Header("Battery System Settings")]
    public float chargeValue;

    void OnTriggerEnter(Collider collider)
    {

    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You get this battery: " + collider.gameObject.name);
                
                // add battery value                
                AddBattery(collider.gameObject, chargeValue);

                // disable game object
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {

    }

    void AddBattery(GameObject player, float value)
    {
        Debug.LogFormat("Battery charge value: {0}", value);
        if(player.GetComponent<PlayerBehaviour>().battery < player.GetComponent<PlayerBehaviour>().batteryMax)
            player.GetComponent<PlayerBehaviour>().battery += value;
    }

}
