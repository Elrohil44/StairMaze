using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public MazeFloorGenerator floorGenerator;

    private short[,] maze;

    UnityEngine.Random rand = new UnityEngine.Random();
    // Start is called before the first frame update
    void Start()
    {
        this.maze = GenerateRandomMazeArray(24, 24);
        PrintMaze(this.maze);

        if (floorGenerator != null)
        {
            floorGenerator.SetMaze(this.maze);
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
            int x = UnityEngine.Random.Range(0, shape[0] / 2) * 2;
            int y = UnityEngine.Random.Range(0, shape[1] / 2) * 2;
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
                    int randNeighbourNdx = UnityEngine.Random.Range(0, neighboursCount);
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

        int xCenter = width / 2;
        int yCenter = height / 2;
        Z[xCenter, yCenter] = 0;
        Z[xCenter + 1, yCenter] = 0;
        return Z;

    }

    

    void PrintMaze(short[,] maze)
    {
        int rowLength = maze.GetLength(0);
        int colLength = maze.GetLength(1);
        string arrayString = "";
        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                arrayString += maze[i, j] == 1 ? "█ " : "░ ";
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
