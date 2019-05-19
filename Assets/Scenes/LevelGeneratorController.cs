using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LevelGeneratorController : MonoBehaviour
{
    public GameObject Player;
    public GameObject LevelPrefab;

    private List<Level> levels = new List<Level>();
    private int currentLevel = 0;

    void Start()
    {
        UpdateLevelDeque();
    }

    void Update()
    {
        if (Player != null)
        {
            int playerLevel = (int)Math.Ceiling(-Player.transform.position.y / this.transform.localScale.y);
            if (playerLevel > currentLevel)
            {
                Debug.Log("Transition from level " + currentLevel.ToString() + " to level " + playerLevel);
                currentLevel = playerLevel;
                this.UpdateLevelDeque();
            }
        }
    }

    void UpdateLevelDeque()
    {
        var expectedLevelNumbers = new List<int>();
        if (currentLevel - 1 >= 0)
            expectedLevelNumbers.Add((currentLevel - 1));
        expectedLevelNumbers.Add(currentLevel);
        expectedLevelNumbers.Add(currentLevel + 1);
        
        // todo: remove old entries
        foreach (Level level in levels) {
            if (!expectedLevelNumbers.Contains(level.number))
            {
                if (level.gameObject != null)
                    Destroy(level.gameObject);
            } else
            {
                expectedLevelNumbers.Remove(level.number);
            }
        }

        foreach (int levelNo in expectedLevelNumbers)
        {
            Level newLevel = new Level();
            newLevel.number = levelNo;
            newLevel.gameObject = Instantiate(LevelPrefab, new Vector3(0, -1f * levelNo * transform.localScale.y, 0), Quaternion.identity, transform);
            Debug.Log("Created level " + levelNo);
            levels.Add(newLevel);
        }
    }
}

struct Level {
    public GameObject gameObject;
    public int number;
}
