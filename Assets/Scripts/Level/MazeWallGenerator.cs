using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWallGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMaze(short[,] maze)
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();

        GenerateGeometryForMesh(mesh, maze);
        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    void GenerateGeometryForMesh(Mesh mesh, short[,] maze)
    {
        const int wallHeight = 1;
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        float relPosX = -1 * (width / 2) - 0.5f;
        float floorY = 0f;
        float relPosZ = -1 * (height / 2) - 0.5f;

        var wallVertices = new List<Vector3>();
        var wallTriangles = new List<int>();
        var wallUvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    continue;
                }

                // left wall
                if (x > 0 && maze[x-1, y] == 1)
                {
                    wallVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y));
                    wallVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x, floorY + wallHeight, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x, floorY + wallHeight, relPosZ + y));

                    int vertNdx = wallVertices.Count - 4;
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx + 1);
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 3);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx);

                    wallUvs.Add(new Vector2(1, 1));
                    wallUvs.Add(new Vector2(0, 1));
                    wallUvs.Add(new Vector2(0, 0));
                    wallUvs.Add(new Vector2(1, 0));
                }

                // top wall
                if (y > 0 && maze[x, y-1] == 1)
                {
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y));
                    wallVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y));
                    wallVertices.Add(new Vector3(relPosX + x, floorY + wallHeight, relPosZ + y));
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY + wallHeight, relPosZ + y));

                    int vertNdx = wallVertices.Count - 4;
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx + 1);
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 3);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx);

                    wallUvs.Add(new Vector2(1, 1));
                    wallUvs.Add(new Vector2(0, 1));
                    wallUvs.Add(new Vector2(0, 0));
                    wallUvs.Add(new Vector2(1, 0));
                }

                // right wall

                if (x < width && maze[x + 1, y] == 1)
                {
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y));
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY + wallHeight, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY + wallHeight, relPosZ + y));

                    int vertNdx = wallVertices.Count - 4;
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 1);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx + 3);

                    wallUvs.Add(new Vector2(0, 1));
                    wallUvs.Add(new Vector2(1, 1));
                    wallUvs.Add(new Vector2(1, 0));
                    wallUvs.Add(new Vector2(0, 0));
                }

                // bottom wall
                if (y < height && maze[x, y + 1] == 1)
                {
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x, floorY + wallHeight, relPosZ + y + 1));
                    wallVertices.Add(new Vector3(relPosX + x + 1, floorY + wallHeight, relPosZ + y + 1));

                    int vertNdx = wallVertices.Count - 4;
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 1);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx);
                    wallTriangles.Add(vertNdx + 2);
                    wallTriangles.Add(vertNdx + 3);

                    wallUvs.Add(new Vector2(1, 1));
                    wallUvs.Add(new Vector2(0, 1));
                    wallUvs.Add(new Vector2(0, 0));
                    wallUvs.Add(new Vector2(1, 0));
                }
            }
        }
        mesh.vertices = wallVertices.ToArray();
        mesh.triangles = wallTriangles.ToArray();
        mesh.uv = wallUvs.ToArray();
        mesh.RecalculateNormals();
    }
}
