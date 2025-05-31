using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_InputField bodyIndexField;
    [SerializeField] Transform camera;

    public void StartGeneration()
    {
        SceneManager.LoadScene("Sandbox");
    }
    public void LoadSandboxMenu()
    {
        SceneManager.LoadScene("Sandbox Menu");
    }
    public void LoadStarSystem()
    {
        SceneManager.LoadScene("System");
    }
    public void LoadOurStarSystem()
    {
        SceneManager.LoadScene("Our System");
    }
    public void LoadTerrain()
    {
        SetTerrain(StarSystem.singleton.terrainTypes[int.Parse(inputField.text)]);
        PlayerPrefs.SetFloat("R", StarSystem.singleton.colors[int.Parse(inputField.text)].r);
        PlayerPrefs.SetFloat("G", StarSystem.singleton.colors[int.Parse(inputField.text)].g);
        PlayerPrefs.SetFloat("B", StarSystem.singleton.colors[int.Parse(inputField.text)].b);
        SceneManager.LoadScene("Terrain");
    }
    public void GoToIndex()
    {
        camera.position = camera.forward * -5 + StarSystem.singleton.objects[int.Parse(inputField.text)].transform.position;
    }

    public void SaveSeed()
    {
        PlayerPrefs.SetInt("Seed", int.Parse(inputField.text));
    }
    public void SaveTime()
    {
        PlayerPrefs.SetFloat("Time", float.Parse(inputField.text));
    }
    public void SaveOpacity()
    {
        PlayerPrefs.SetFloat("Opacity", float.Parse(inputField.text));
    }
    public void SaveStarMass()
    {
        PlayerPrefs.SetFloat("StarMass", float.Parse(inputField.text));
    }
    public void SaveStarScale()
    {
        PlayerPrefs.SetFloat("StarScale", float.Parse(inputField.text));
    }
    public void SaveStarBrightness()
    {
        PlayerPrefs.SetFloat("StarBrightness", float.Parse(inputField.text));
    }
    public void SaveBodyCount()
    {
        PlayerPrefs.SetInt("BodyCount", int.Parse(inputField.text));
    }
    public void SaveBodySOI()
    {
        PlayerPrefs.SetInt("BodySOI" + bodyIndexField.text, int.Parse(inputField.text));
    }
    public void SaveBodyMass()
    {
        PlayerPrefs.SetFloat("BodyMass" + bodyIndexField.text, float.Parse(inputField.text));
    }
    public void SaveBodyRadius()
    {
        PlayerPrefs.SetFloat("BodyRadius" + bodyIndexField.text, float.Parse(inputField.text));
    }
    public void SaveBodyMa()
    {
        PlayerPrefs.SetFloat("BodyMa" + bodyIndexField.text, float.Parse(inputField.text));
    }
    public void SaveBodyMi()
    {
        PlayerPrefs.SetFloat("BodyMi" + bodyIndexField.text, float.Parse(inputField.text));
    }
    public void SaveBodyType()
    {
        PlayerPrefs.SetInt("BodyType" + bodyIndexField.text, int.Parse(inputField.text));
    }

    void SetTerrain(int input)
    {
        //Classic (2,3;2,2), Spiky (2,3; 2,4), Rocky (2; 2,2), Best Zoom = 3, (2,6;2,4)
        switch (input)
        {
            case 1:
                PlayerPrefs.SetFloat("TerrainZoom", 1.5f);
                PlayerPrefs.SetFloat("TerrainZoomPow", 2.3f);
                PlayerPrefs.SetFloat("TerrainHeightPow", 2.2f);
                break;
            case 2:
                PlayerPrefs.SetFloat("TerrainZoom", 1.5f);
                PlayerPrefs.SetFloat("TerrainZoomPow", 2.3f);
                PlayerPrefs.SetFloat("TerrainHeightPow", 2.4f);
                break;
            case 3:
                PlayerPrefs.SetFloat("TerrainZoom", 1.5f);
                PlayerPrefs.SetFloat("TerrainZoomPow", 2f);
                PlayerPrefs.SetFloat("TerrainHeightPow", 2.2f);
                break;
            case 4:
                PlayerPrefs.SetFloat("TerrainZoom", 3f);
                PlayerPrefs.SetFloat("TerrainZoomPow", 2.6f);
                PlayerPrefs.SetFloat("TerrainHeightPow", 2.4f);
                break;
            default:
                break;
        }
    }
}
