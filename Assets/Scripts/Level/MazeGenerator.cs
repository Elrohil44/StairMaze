﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class MazeGenerator : MonoBehaviour
{
    public GameObject BatteryPrefab;
    public GameObject Stairs;
    public GameObject LevelTextPrefab;
    public GameObject ScaryTextPrefab;

    public static string[] ScaryTexts = new string[]
    {
        "You will be next",
        "See you downstairs",
        "You won't hide",
        "I'll wait for lights down",
        "There is no way out!",
        "Winter is coming",
        "The North remembers"
    };

    public MazeFloorGenerator floorGenerator;
    public MazeCeilingGenerator ceilingGenerator;
    public MazeWallGenerator wallGenerator;
    public int level;
    public int seed;
    public Vector3 exit;
    public PlacementLocation exitPlacement;

    private short[,] maze;

    private System.Random rand;
    // Start is called before the first frame update
    void Start()
    {
        if (rand == null)
        {
            rand = new System.Random(seed + level);
        }

        this.maze = GenerateRandomMazeArray(14, 14);
        SetPlayerStartPlaceInMaze(this.maze);
        PlaceExit(this.maze);
        PlaceBatteries();
        PlaceLevelText();
        PlaceScaryText();
        PrintMaze(this.maze);
        // generate geometry
        if (floorGenerator != null)
        {
            floorGenerator.SetMaze(this.maze);
        }
        if (wallGenerator != null)
        {
            wallGenerator.SetMaze(this.maze);
        }
        if (ceilingGenerator != null)
        {
            ceilingGenerator.SetMaze(this.maze);
        }
    }

    short[,] TransposePattern(short[,] pattern)
    {
        short[,] transposed = new short[pattern.GetLength(1), pattern.GetLength(0)];
        for (int i = 0; i < pattern.GetLength(0); i++)
        {
            for (int j = 0; j < pattern.GetLength(1); j++)
            {
                transposed[j, i] = pattern[i, j];
            }
        }
        return transposed;
    }

    void PlaceExit(short[,] maze)
    {
        short[,] exitPattern = new short[,] {
            { 1, 1, 1 },
            { 1, 0, 0 },
            { 1, 1, 1 }
        };

        List<PlacementLocation> placements = GetPlacementLocations(exitPattern);
        PlacementLocation selectedLocation;
        if (placements.Count != 0)
        {
            selectedLocation = placements[placements.Count > 1 ? rand.Next(0, placements.Count - 1) : 0];
        } else
        {
            if (level == 0)
            {
                selectedLocation = new PlacementLocation((int)Math.Floor(maze.GetLength(0) / 2.0), (int)Math.Floor(maze.GetLength(1) / 2.0), false, false);
            } else
            {
                Transform prevLevel = transform.parent.Find((level - 1).ToString());
                PlacementLocation prevExit = prevLevel.gameObject.GetComponent<MazeGenerator>().exitPlacement;
                selectedLocation = new PlacementLocation((int)Math.Floor(maze.GetLength(0) / 2.0), (int)Math.Floor(maze.GetLength(1) / 2.0), !prevExit.reversedHorizontally, false);
            }
        }
        this.exitPlacement = selectedLocation;
        Debug.Log("Exit is at: " + selectedLocation.ToString());

        maze[selectedLocation.x + 1, selectedLocation.y + 1] = 2;
        exit = GetLocation(selectedLocation.x + 1, selectedLocation.y + 1);
        if (!selectedLocation.reversedHorizontally)
        {
            maze[selectedLocation.x + 2, selectedLocation.y + 1] = 2;
        } else
        {
            maze[selectedLocation.x, selectedLocation.y + 1] = 2;
        }
        if (level > 0)
        {
            Transform prevLevel = transform.parent.Find((level - 1).ToString());
            Vector3 prevExit = prevLevel.gameObject.GetComponent<MazeGenerator>().exit;
            Vector3 currentPosition = prevLevel.localPosition;
            transform.localPosition = new Vector3(prevExit.x + currentPosition.x, transform.localPosition.y, prevExit.z + currentPosition.z);
        }

        Stairs.transform.localPosition = new Vector3(exit.x+(selectedLocation.reversedHorizontally ? -1f : 1f), -0.5f, exit.z);
        Stairs.transform.Rotate(0, selectedLocation.reversedHorizontally ? 180f: 0f, 0);
    }

    void PlaceBatteries()
    {
        short[,] cornerPattern = new short[,]
        {
            { 1, 1, 1 },
            { 0, 0, 1 },
            { 1, 0, 1 }
        };

        List<PlacementLocation> placements = GetPlacementLocations(cornerPattern);

        bool alternativeLocation = false;
        if (placements.Count == 0)
        {
            cornerPattern = new short[,]
            {
                { 0, 0 }
            };

            placements = GetPlacementLocations(cornerPattern);
            alternativeLocation = true;
        }
        if (placements.Count > 0)
        {
            PlacementLocation selectedLocation = placements.Count > 1
                ? placements[rand.Next(0, placements.Count - 1)]
                : placements[0];

            int x = alternativeLocation
                ? selectedLocation.x
                : selectedLocation.x + 1;
            int y = alternativeLocation
                ? selectedLocation.y
                : selectedLocation.y + 1;

            maze[x,y] = 4;

            Transform batteries = this.gameObject.transform.Find("Batteries");
            GameObject battery = Instantiate(BatteryPrefab, batteries);
            battery.transform.localPosition = GetLocation(x, y);
        }
    }

    void PlaceLevelText()
    {
        short[,] blindWayPattern = new short[,]
        {
            { 1, 1, 1 },
            { 1, 0, 1 },
            { 1, 0, 1 }
        };

        List<PlacementLocation> placements = GetPlacementLocations(blindWayPattern);

        Transform texts = this.gameObject.transform.Find("Texts");
        placements.ForEach(placement =>
        {
            if (placement.reversedHorizontally) return;
            GameObject text = Instantiate(LevelTextPrefab, texts);
            Vector3 position = GetLocation(placement.x + 1, placement.y + 1);
            text.GetComponent<TextMeshPro>().text = (level + 1).ToString();
            if (!placement.reversedVertically)
            {
                position.z -= 0.4999f;
                text.transform.Rotate(0, 180, 0, Space.Self);
            }
            else
            {
                position.z += 0.4999f;
            }
            text.transform.localPosition = position;
        });

        placements = GetPlacementLocations(TransposePattern(blindWayPattern));

        placements.ForEach(placement =>
        {

            if (placement.reversedVertically) return;
            GameObject text = Instantiate(LevelTextPrefab, texts);
            Vector3 position = GetLocation(placement.x + 1, placement.y + 1);
            text.GetComponent<TextMeshPro>().text = (level + 1).ToString();
            if (!placement.reversedHorizontally)
            {
                position.x -= 0.4999f;
                text.transform.Rotate(0, -90, 0, Space.Self);
            }
            else
            {
                position.x += 0.4999f;
                text.transform.Rotate(0, 90, 0, Space.Self);
            }
            text.transform.localPosition = position;
        });
    }

    void PlaceScaryText()
    {
        short[,] corridorPattern = new short[,]
        {
            { 1, 1, 1 },
            { 0, 0, 0 }
        };

        List<PlacementLocation> placements = GetPlacementLocations(corridorPattern);

        Transform texts = this.gameObject.transform.Find("Texts");
        if (placements.Count == 0) return;

        PlacementLocation selectedPlacement = placements.Count > 1
            ? placements[rand.Next(0, placements.Count - 1)]
            : placements[0];


        GameObject text = Instantiate(ScaryTextPrefab, texts);
        Vector3 position;
        text.GetComponent<TextMeshPro>().text = ScaryTexts[rand.Next(0, ScaryTexts.Length - 1)];
        if (selectedPlacement.reversedVertically)
        {
            position = GetLocation(selectedPlacement.x + 1, selectedPlacement.y);
            position.z += 0.4999f;
        }
        else
        {
            position = GetLocation(selectedPlacement.x + 1, selectedPlacement.y + 1);
            position.z -= 0.4999f;
            text.transform.Rotate(0, 180, 0);
        }
        position.y += .2f;
        text.transform.localPosition = position;
    }

    Vector3 GetLocation(int x, int y)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        float relPosX = -1 * (width / 2);
        float floorY = .0f;
        float relPosZ = -1 * (height / 2);

        return new Vector3(x + relPosX, floorY, y + relPosZ);
    }

    List<PlacementLocation> GetPlacementLocations(short[,] pattern)
    {
        short[,] transposedPattern = TransposePattern(pattern);
        List<PlacementLocation> placements = new List<PlacementLocation>();

        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        int patternWidth = transposedPattern.GetLength(0);
        int patternHeight = transposedPattern.GetLength(1);

        for (int seekX = 0; seekX < width - patternWidth; seekX++)
        {
            for (int seekY = 0; seekY < height - patternHeight; seekY++)
            {
                bool isMismatch = false;
                for (int i = 0; i < patternWidth; i++)
                {
                    for (int j = 0; j < patternHeight; j++)
                    {
                        isMismatch |= transposedPattern[i, j] != maze[seekX + i, seekY + j];
                        if (isMismatch)
                            break;
                    }
                    if (isMismatch)
                        break;
                }
                if (!isMismatch)
                {
                    placements.Add(new PlacementLocation(seekX, seekY, false, false));
                }
                isMismatch = false;

                // horizontally reversed
                for (int i = 0; i < patternWidth; i++)
                {
                    for (int j = 0; j < patternHeight; j++)
                    {
                        isMismatch |= transposedPattern[patternWidth - i - 1, j] != maze[seekX + i, seekY + j];
                        if (isMismatch)
                            break;
                    }
                    if (isMismatch)
                        break;
                }
                if (!isMismatch)
                {
                    placements.Add(new PlacementLocation(seekX, seekY, true, false));
                }
                isMismatch = false;

                //vertically reversed
                for (int i = 0; i < patternWidth; i++)
                {
                    for (int j = 0; j < patternHeight; j++)
                    {
                        isMismatch |= transposedPattern[i, patternHeight - j - 1] != maze[seekX + i, seekY + j];
                        if (isMismatch)
                            break;
                    }
                    if (isMismatch)
                        break;
                }
                if (!isMismatch)
                {
                    placements.Add(new PlacementLocation(seekX, seekY, false, true));
                }
                isMismatch = false;

                //horizontally and vertically reversed
                for (int i = 0; i < patternWidth; i++)
                {
                    for (int j = 0; j < patternHeight; j++)
                    {
                        isMismatch |= transposedPattern[patternWidth - i - 1, patternHeight - j - 1] != maze[seekX + i, seekY + j];
                        if (isMismatch)
                            break;
                    }
                    if (isMismatch)
                        break;
                }
                if (!isMismatch)
                {
                    placements.Add(new PlacementLocation(seekX, seekY, true, true));
                }
                isMismatch = false;
            }
        }

        return placements;
    }

    void SetPlayerStartPlaceInMaze(short[,] maze)
    {
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        int xCenter = width / 2;
        int yCenter = height / 2;
        maze[xCenter, yCenter] = 0;
        maze[xCenter, yCenter + 1] = 0;
        maze[xCenter, yCenter - 1] = 0;
        maze[xCenter + 1, yCenter] = 0;
        maze[xCenter + 1, yCenter + 1] = 0;
        maze[xCenter + 1, yCenter - 1] = 0;
        maze[xCenter - 1, yCenter] = 0;
        maze[xCenter - 1, yCenter + 1] = 0;
        maze[xCenter - 1, yCenter - 1] = 0;

        if (level > 0)
        {
            maze[xCenter, yCenter] = 3;

            Transform prevLevel = transform.parent.Find((level - 1).ToString());
            PlacementLocation prevExit = prevLevel.gameObject.GetComponent<MazeGenerator>().exitPlacement;
            if (!prevExit.reversedHorizontally)
            {
                maze[xCenter + 1, yCenter] = 3;
            }
            else
            {
                maze[xCenter - 1, yCenter] = 3;
            }
        }
    }

    short[,] GenerateRandomMazeArray(int width = 14, int height = 14,
        double complexity = 0.75, double density = 0.75)
    {
        int[] shape = new int[] {
            (int)Math.Floor((double)height / 2) * 2 + 1,
            (int)Math.Floor((double)width / 2) * 2 + 1 };
        complexity = complexity * (5 * (shape[1] + shape[0]));
        density = density * (Math.Floor((double)shape[1] / 2) * Math.Floor((double)shape[0] / 2));

        short[,] Z = new short[shape[0], shape[1]];
        for (int x = 0; x < shape[0]; x++) {
            for (int y = 0; y < shape[1]; y++)
            {
                Z[x, y] = 0;
            }
        }

        for (int x = 0; x < shape[0]; x++)
        {
            Z[x, 0] = 1;
            Z[x, shape[1] - 1] = 1;
        }
        for (int y = 0; y < shape[1]; y++)
        {
            Z[0, y] = 1;
            Z[shape[0] - 1, y] = 1;
        }

        int[,] neighbours = new int[4, 2];
        int neighboursCount = 0;
        for (int i = 0; i < density; i++)
        {
            int x = rand.Next(0, shape[0] / 2) * 2;
            int y = rand.Next(0, shape[1] / 2) * 2;
            Z[x, y] = 1;

            for (int j = 0; j < complexity; j++)
            {
                neighboursCount = 0;
                if (x > 1)
                {
                    neighbours[neighboursCount, 0] = x - 2;
                    neighbours[neighboursCount, 1] = y;
                    neighboursCount++;
                }

                if (x < shape[0] - 2)
                {
                    neighbours[neighboursCount, 0] = x + 2;
                    neighbours[neighboursCount, 1] = y;
                    neighboursCount++;
                }

                if (y > 1)
                {
                    neighbours[neighboursCount, 0] = x;
                    neighbours[neighboursCount, 1] = y - 2;
                    neighboursCount++;
                }

                if (y < shape[1] - 2)
                {
                    neighbours[neighboursCount, 0] = x;
                    neighbours[neighboursCount, 1] = y + 2;
                    neighboursCount++;
                }

                if (neighboursCount > 0)
                {
                    int randNeighbourNdx = rand.Next(0, neighboursCount);
                    int x_ = neighbours[randNeighbourNdx, 0];
                    int y_ = neighbours[randNeighbourNdx, 1];
                    if (Z[x_, y_] == 0)
                    {
                        Z[x_, y_] = 1;
                        Z[x_ + (int)Math.Floor((x - x_) / 2.0d), y_ + (int)Math.Floor((y - y_) / 2.0d)] = 1;
                        x = x_;
                        y = y_;
                    }
                }
            }
        }

        return Z;

    }

    

    void PrintMaze(short[,] maze)
    {
        short[,] transposedMaze = TransposePattern(maze);
        int rowLength = transposedMaze.GetLength(0);
        int colLength = transposedMaze.GetLength(1);
        string arrayString = "";
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                if (transposedMaze[i, j] == 2)
                {
                    arrayString += "D";
                }
                else if (transposedMaze[i, j] == 3)
                {
                    arrayString += "U";
                }
                else if (transposedMaze[i, j] == 4)
                {
                    arrayString += "B";
                } else
                {
                    arrayString += transposedMaze[i, j] == 1 ? "█ " : "░ ";
                }
            }
            arrayString += System.Environment.NewLine + System.Environment.NewLine;
        }

        Debug.Log(arrayString);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public short[,] GetMaze()
    {
        return this.maze;
    }
}

public struct PlacementLocation
{
    public int x;
    public int y;
    public bool reversedHorizontally;
    public bool reversedVertically;

    public PlacementLocation(int x, int y, bool reversedHorizontally, bool reversedVertically)
    {
        this.x = x;
        this.y = y;
        this.reversedHorizontally = reversedHorizontally;
        this.reversedVertically = reversedVertically;
    }

    public override string ToString()
    {
        return String.Format("{0}x{1} hr: {2} vr: {3}", this.x, this.y, this.reversedHorizontally, this.reversedVertically);
    }
}