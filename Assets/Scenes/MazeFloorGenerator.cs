using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeFloorGenerator : MonoBehaviour
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
        int width = maze.GetLength(0);
        int height = maze.GetLength(1);
        float relPosX = -1 * (width / 2) - 0.5f;
        float floorY = 0f;
        float relPosZ = -1 * (height / 2) - 0.5f;

        var floorVertices = new List<Vector3>();
        var floorTriangles = new List<int>();
        var floorUvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] != 0)
                {
                    continue;
                }
                floorVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y));
                floorVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y));
                floorVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y + 1));
                floorVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y + 1));
                int vertNdx = floorVertices.Count - 4;
                floorTriangles.Add(vertNdx + 2);
                floorTriangles.Add(vertNdx + 1);
                floorTriangles.Add(vertNdx);
                floorTriangles.Add(vertNdx + 1);
                floorTriangles.Add(vertNdx + 2);
                floorTriangles.Add(vertNdx + 3);

                floorUvs.Add(new Vector2(0, 0));
                floorUvs.Add(new Vector2(1, 0));
                floorUvs.Add(new Vector2(0, 1));
                floorUvs.Add(new Vector2(1, 1));

            }
        }
        mesh.vertices = floorVertices.ToArray();
        mesh.triangles = floorTriangles.ToArray();
        mesh.uv = floorUvs.ToArray();
    }
}
