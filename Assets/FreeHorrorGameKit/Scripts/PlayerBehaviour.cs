using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

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
    public float maxIntensity = 5.5f;

    [Header("Audio Settings")]
    public AudioClip slenderNoise;
    public AudioClip cameraNoise;

    public bool paused;

    private bool running;

	void Start ()
    {
        // set initial health values
        health = healthMax;
        battery = batteryMax;
        
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

        //animations
        if (Input.GetKey(KeyCode.LeftShift) && !transform.GetComponent<FirstPersonController>().IsWalking())
            this.gameObject.GetComponent<Animation>().CrossFade("Run", 1);
        else
            this.gameObject.GetComponent<Animation>().CrossFade("Idle", 1);
    }

    private void FixedUpdate()
    {
        if (battery > 0)
        {
            battery -= removeBatteryValue;
        }

        float batteryPercentage = battery / batteryMax;
        float color = batteryPercentage > 1
            ? 1
            : batteryPercentage;
        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = maxIntensity * batteryPercentage;
        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().color = new Color(1, color, color);
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

    public void DisableMotionBlur()
    {
        gameObject.transform.Find("Main Camera").GetComponent<MotionBlur>().enabled = false;
    }

    public void TriggerMotionBlur()
    {
        gameObject.transform.Find("Main Camera").GetComponent<MotionBlur>().enabled = true;
        Invoke("DisableMotionBlur", 10);
    }
}
