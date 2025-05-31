using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public Vector3 size;
    public int noiseLayers;
    public float zoom;
    public float zoomPowIndex_1 = 2.2f;
    public float heightPowIndex_2 = 2.5f;
    public int startPow;
    public float heightScale;

    MeshFilter meshFilter;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = GenerateMesh(GenerateFractalNoise(new float[(int)size.x, (int)size.z], noiseLayers, zoom, startPow), size.y / noiseLayers);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            meshFilter.mesh = GenerateMesh(GenerateFractalNoise(new float[(int)size.x, (int)size.z], noiseLayers, zoom, startPow), size.y / noiseLayers);
        }
    }

    float[,] GenerateFractalNoise(float[,] input, int layers, float zoom = 1f, int startPow = 0)
    {
        for (int i = startPow; i < layers; i++)
        {
            input = GenerateNoise(input, Mathf.Pow(zoomPowIndex_1, i) * zoom, Mathf.Pow(heightPowIndex_2, i));
        }

        for (int i = 0; i < input.GetLength(0); i++)
        {
            for (int j = 0; j < input.GetLength(1); j++)
            {
                input[i, j] = input[i, j];
            }
        }

        return input;
    }

    float[,] GenerateNoise(float[,] input, float zoom, float height)
    {
        float x = zoom;

        for (int i = 0; i < input.GetLength(0); i++)
        {
            for (int j = 0; j < input.GetLength(1); j++)
            {
                input[i, j] += Mathf.PerlinNoise((i + transform.position.x) / (2 * x) + StarSystem.singleton.seed, (j + transform.position.z) / (2 * x) + StarSystem.singleton.seed);
                input[i, j] = input[i, j] * height * heightScale;
            }
        }

        return input;
    }

    public Mesh GenerateMesh(float[,] heightMap, float heightMultiplier = 1f, float xSpacing = 1f, float zSpacing = 1f)
    {
        int width = heightMap.GetLength(0);
        int length = heightMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * length];
        int[] triangles = new int[(width - 1) * (length - 1) * 6];

        // Generate vertices
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float y = heightMap[x, z] * heightMultiplier;
                vertices[x * length + z] = new Vector3(x * xSpacing, y, z * zSpacing);
            }
        }

        // Generate triangles
        int t = 0;
        for (int x = 0; x < width - 1; x++)
        {
            for (int z = 0; z < length - 1; z++)
            {
                int topLeft = x * length + z;
                int topRight = topLeft + 1;
                int bottomLeft = (x + 1) * length + z;
                int bottomRight = bottomLeft + 1;

                // First triangle (reversed winding order)
                triangles[t++] = topLeft;
                triangles[t++] = topRight;
                triangles[t++] = bottomLeft;

                // Second triangle (reversed winding order)
                triangles[t++] = topRight;
                triangles[t++] = bottomRight;
                triangles[t++] = bottomLeft;
            }
        }

        // Create and assign mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }
}
