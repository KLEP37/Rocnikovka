using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Classic (2,3;2,2), Spiky (2,3; 2,4), Rocky (2; 2,2), Best Zoom = 3, (2,6;2,4)
public class TerrainMain : MonoBehaviour
{
    [SerializeField] Material material;

    [SerializeField] int noiseLayers;
    [SerializeField] float zoom;
    [SerializeField] float zoomPowIndex_1 = 2.2f;
    [SerializeField] float heightPowIndex_2 = 2.5f;
    [SerializeField] int startPow;
    [SerializeField] float heightScale;

    GameObject[,] terrain;
    [SerializeField] Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        Color color = new Color(PlayerPrefs.GetFloat("R"), PlayerPrefs.GetFloat("G"), PlayerPrefs.GetFloat("B"));
        material.SetColor("_Color_1", color);
        zoom = PlayerPrefs.GetFloat("TerrainZoom");
        zoomPowIndex_1 = PlayerPrefs.GetFloat("TerrainZoomPow");
        heightPowIndex_2 = PlayerPrefs.GetFloat("TerrainHeightPow");
        LoadTerrain();
    }

    void LoadTerrain()
    {
        if (terrain != null)
        {
            foreach (var item in terrain)
            {
                GameObject.Destroy(item);
            }
        }

        terrain = new GameObject[(int)size.x, (int)size.y];
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var mat = new Material(material);

                terrain[x, y] = new GameObject();
                terrain[x, y].transform.position = new Vector3(x * 99, 0, y * 99);
                var tg = terrain[x, y].AddComponent<TerrainGeneration>();
                terrain[x, y].AddComponent<MeshFilter>();
                terrain[x, y].AddComponent<MeshRenderer>().material = mat;
                tg.size = new Vector3(100, 10, 100);
                tg.noiseLayers = noiseLayers;
                tg.zoom = zoom;
                tg.zoomPowIndex_1 = zoomPowIndex_1;
                tg.heightPowIndex_2 = heightPowIndex_2;
                tg.startPow = startPow;
                tg.heightScale = heightScale;
            }
        }
    }
}
