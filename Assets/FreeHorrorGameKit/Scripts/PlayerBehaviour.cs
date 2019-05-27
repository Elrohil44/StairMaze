using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Health Settings")]
    public float health = 100;
    public float healthMax = 100;
    public float healValue = 5;
    public float secondToHeal = 10;

    [Header("Flashlight Battery Settings")]
    public GameObject Flashlight;
    public float battery = 100;
    public float batteryMax = 100;
    public float removeBatteryValue = 0.05f;
    public float secondToRemoveBaterry = 5f;

    [Header("Audio Settings")]
    public AudioClip slenderNoise;
    public AudioClip cameraNoise;

    public bool paused;

	void Start ()
    {
        // set initial health values
        health = healthMax;
        battery = batteryMax;
        
        
        // start consume flashlight battery
        StartCoroutine(RemoveBaterryCharge(removeBatteryValue, secondToRemoveBaterry));
    }
	
	void Update ()
    {
        
        // if health is low than 20%
        if(health / healthMax * 100 <= 20 && health / healthMax * 100 != 0)
        {
            Debug.Log("You are dying.");
            this.GetComponent<AudioSource>().PlayOneShot(slenderNoise);
        }

        // if health is low than 0
        if (health / healthMax * 100 <= 0)
        {
            Debug.Log("You are dead.");
            health = 0.0f;
        }

        // if battery is low 50%
        if (battery / batteryMax * 100 <= 50)
        {
            Debug.Log("Flashlight is running out of battery.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.85f;
        }

        // if battery is low 25%
        if (battery / batteryMax * 100 <= 25)
        {
            Debug.Log("Flashlight is almost without battery.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.0f;
        }

        // if battery is low 10%
        if (battery / batteryMax * 100 <= 10)
        {
            Debug.Log("You will be out of light.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 1.35f;           
        }

        // if battery out%
        if (battery / batteryMax * 100 <= 0)
        {
            battery = 0.00f;
            Debug.Log("The flashlight battery is out and you are out of the light.");
            Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
        }
        
        //animations
        if (Input.GetKey(KeyCode.LeftShift))
            this.gameObject.GetComponent<Animation>().CrossFade("Run", 1);
        else
            this.gameObject.GetComponent<Animation>().CrossFade("Idle", 1);
        
    }

    public IEnumerator RemoveBaterryCharge(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Removing baterry value: " + value);

            if (battery > 0)
                battery -= value;
            else
                Debug.Log("The flashlight battery is out");
        }
    }

    public IEnumerator RemovePlayerHealth(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Removing player health value: " + value);

            if (health > 0)
                health -= value;
            else
            {
                Debug.Log("You're dead");
                paused = true;
            }
        }
    }

    // function to heal player
    public IEnumerator StartHealPlayer(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Healling player value: " + value);

            if (health > 0 && health < healthMax)
                health += value;
            else
                health = healthMax;
        }
    }

    // page system - show UI
    private void OnTriggerEnter(Collider collider)
    {
        // start noise when reach slender
        if (collider.gameObject.transform.tag == "Slender")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().PlayOneShot(cameraNoise);
                this.GetComponent<AudioSource>().loop = true;
            }            
        }
    }

    // page system - pickup system
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.tag == "Page")
        {       
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You get this page: " + collider.gameObject.name);

                // disable game object
                collider.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // remove noise sound
        if (collider.gameObject.transform.tag == "Slender")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().clip = null;
                this.GetComponent<AudioSource>().loop = false;
            }          
        }
    }
}
