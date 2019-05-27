using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCeilingGenerator : MonoBehaviour
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
        float floorY = 1f;
        float relPosZ = -1 * (height / 2) - 0.5f;

        var ceilingVertices = new List<Vector3>();
        var ceilingTriangles = new List<int>();
        var ceilingUvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1 || maze[x, y] == 3)
                {
                    continue;
                }
                ceilingVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y));
                ceilingVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y));
                ceilingVertices.Add(new Vector3(relPosX + x, floorY, relPosZ + y + 1));
                ceilingVertices.Add(new Vector3(relPosX + x + 1, floorY, relPosZ + y + 1));
                int vertNdx = ceilingVertices.Count - 4;
                ceilingTriangles.Add(vertNdx);
                ceilingTriangles.Add(vertNdx + 1);
                ceilingTriangles.Add(vertNdx + 2);
                ceilingTriangles.Add(vertNdx + 3);
                ceilingTriangles.Add(vertNdx + 2);
                ceilingTriangles.Add(vertNdx + 1);

                ceilingUvs.Add(new Vector2(0, 0));
                ceilingUvs.Add(new Vector2(1, 0));
                ceilingUvs.Add(new Vector2(0, 1));
                ceilingUvs.Add(new Vector2(1, 1));

            }
        }
        mesh.vertices = ceilingVertices.ToArray();
        mesh.triangles = ceilingTriangles.ToArray();
        mesh.uv = ceilingUvs.ToArray();
        mesh.RecalculateNormals();
    }
}
