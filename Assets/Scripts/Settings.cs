using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static int seed = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void setSeed(string newSeed)
    {
        int.TryParse(newSeed, out seed);
        Debug.Log(newSeed);
        Debug.Log("sadasdas");
    }
}
