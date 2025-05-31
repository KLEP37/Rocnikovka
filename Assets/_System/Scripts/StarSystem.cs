using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Best: 4210, 
public class StarSystem : MonoBehaviour
{
    public static StarSystem singleton;
    public float AuToUnityUnits;
    public float G = 0.001f;
    public float time;
    public float scale;
    public float starScale;
    public int seed;

    [SerializeField] bool generate;
    [SerializeField] bool sandbox;

    Star mainBody;

    public List<Material> materials;
    /*
    0 = Star
    1 = Gas Giant
    2 = Terrestrial01
    3 = Terrestrial02
    4 = TrailMat
    */
    public List<GameObject> objects;
    public List<int> terrainTypes;
    public List<Color> colors;

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        seed = PlayerPrefs.GetInt("Seed");
        time = PlayerPrefs.GetFloat("Time");
        time *= 0.2029125f;
        Random.InitState(seed);

        terrainTypes.Add(0);

        if (generate)
        {
            GenerateStarSystem();
            GeneratePlanets(0, new List<AU>());
        }
        if (sandbox)
        {
            LoadPremade(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        }
    }

    void LoadPremade(GameObject star)
    {
        objects.Add(star);
        star.name = "Star";
        Star starScript = star.AddComponent<Star>();
        mainBody = starScript;
        starScript.mass = 333000 * PlayerPrefs.GetFloat("StarMass");
        starScript.radius = 109 * Mathf.Pow(starScript.mass / 333000, 0.8f);
        var meshRenderer = star.GetComponent<MeshRenderer>();
        Material material = new Material(materials[0]);
        material.SetFloat("_Scale", PlayerPrefs.GetFloat("StarScale"));
        material.SetFloat("_RotationSpeed", 0.02f + 0.1f * Random.value);
        material.SetFloat("_Brightness", PlayerPrefs.GetFloat("StarBrightness"));
        float temperature = Mathf.Sqrt(Mathf.Sqrt(Mathf.Pow(starScript.mass / 333000, 2.5f) * 983449600000000));
        Color color = Mathf.CorrelatedColorTemperatureToRGB(temperature);
        colors.Add(color);
        material.SetColor("_Color", color);
        Color emissionColor = color;
        emissionColor.a = .5f;
        meshRenderer.material.SetColor("_EmissionColor", emissionColor);
        meshRenderer.material = material;

        for (int n = 1; n < PlayerPrefs.GetInt("BodyCount") + 1; n++)
        {
            AU distance = new AU();
            distance.SetAU(PlayerPrefs.GetFloat("BodyMa" + n));

            GameObject planet = new GameObject("Planet");
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objects.Add(visual);
            terrainTypes.Add(Random.Range(1, 5));
            visual.transform.parent = planet.transform;
            Object planetObject = planet.AddComponent<Object>();
            PACB physics = planet.AddComponent<PACB>();
            physics.Ma = distance.GetAU();
            physics.Mi = PlayerPrefs.GetFloat("BodyMa" + n);
            physics.SOI = mainBody;
            physics.travelDirection = 1;
            physics.orbitPoints = 6900;
            physics.angle1 = Random.value * 360;
            physics.angle2 = Random.value * 20;
            physics.angle3 = Random.value * 20;

            Color mainColor = Random.ColorHSV();
            colors.Add(mainColor);
            mainColor.a = .5f;
            physics.orbitColor = mainColor;

            int type = PlayerPrefs.GetInt("BodyType" + n);
            if (type == 1)
            {
                //Gas giant
                planetObject.mass = PlayerPrefs.GetFloat("BodyMass" + n);
                planetObject.radius = PlayerPrefs.GetFloat("BodyRadius" + n);
                meshRenderer = visual.GetComponent<MeshRenderer>();
                meshRenderer.material = materials[1];
                var ggv = visual.AddComponent<GasGiantVisual>();
                ggv.stripeCount = 3 + (int)Mathf.Pow(4 * Random.value, 2) + (int)(16 * Random.value);
                ggv.blurIndex = 3 + (int)(12f * Random.value);
                ggv.colorSimilarityIndex = .1f + .4f * Random.value;
                ggv.seed = seed;
                ggv.speedMultiplier = (.05f + Random.value * .5f);
                if (Random.value > .5f)
                {
                    ggv.fragmentZoom = 400 + 2000 * Random.value;
                }
                else
                {
                    ggv.fragmentZoom = 50 + 200 * Random.value;
                }
                ggv.brightness = 1f + Random.value;
                ggv.baseColor = mainColor;
            }
            else
            {
                //Terrestrial
                planetObject.mass = PlayerPrefs.GetFloat("BodyMass" + n);
                planetObject.radius = PlayerPrefs.GetFloat("BodyRadius" + n);
                meshRenderer = visual.GetComponent<MeshRenderer>();
                if (type == 2)
                {
                    material = new Material(materials[2]);
                }
                else
                {
                    material = new Material(materials[3]);
                    material.SetColor("_Color1", mainColor);
                    material.SetColor("_Color2", Random.ColorHSV());
                    material.SetColor("_Color3", Random.ColorHSV());
                    material.SetFloat("_Seed", seed);
                    material.SetFloat("_Clouds", Mathf.Clamp(Random.value * .5f - .1f, 0, 0.4f));
                    material.SetFloat("_Range", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.2f));
                    material.SetFloat("_Speed", Random.value * .4f);
                }
                meshRenderer.material = material;
            }

            float SOIInfluence = mainBody.mass / Mathf.Pow(distance.GetAU(), 2);
            planetObject.SOIdistance = Mathf.Sqrt(planetObject.mass / SOIInfluence);
        }
    }

    void GenerateStarSystem()
    {
        GenerateStar(GameObject.CreatePrimitive(PrimitiveType.Sphere));
    }
    void GenerateStar(GameObject star)
    {
        objects.Add(star);
        //Main sequence star
        star.name = "Star";
        Star starScript = star.AddComponent<Star>();
        mainBody = starScript;
        starScript.mass = 333000 * (0.3f + Random.value * 2);
        starScript.radius = 109 * Mathf.Pow(starScript.mass / 333000, 0.8f);
        var meshRenderer = star.GetComponent<MeshRenderer>();
        Material material = new Material(materials[0]);
        material.SetFloat("_Scale", 75 + 150 * Random.value);
        material.SetFloat("_RotationSpeed", 0.02f + 0.1f * Random.value);
        material.SetFloat("_Brightness", 1.3f + 0.5f * Random.value);
        float temperature = Mathf.Sqrt(Mathf.Sqrt(Mathf.Pow(starScript.mass / 333000, 2.5f) * 983449600000000));
        Color color = Mathf.CorrelatedColorTemperatureToRGB(temperature);
        colors.Add(color);
        material.SetColor("_Color", color);
        Color emissionColor = color;
        emissionColor.a = .5f;
        meshRenderer.material.SetColor("_EmissionColor", emissionColor);
        meshRenderer.material = material;
    }
    void GeneratePlanets(int generatedPlanetsCount, List<AU> orbitDistances)
    {
        float generationTreshold = 1 / (0.05f * generatedPlanetsCount + 1.05f);
        if (Random.value > generationTreshold)
        {
            return;
        }

        AU distance = new AU();
        distance.SetAU(Mathf.Clamp(1 / (1 - Mathf.Pow(Random.value, 2)) - 0.99f, 0.1f, 50));
        for (int i = 0; i < generatedPlanetsCount; i++)
        {
            if (Mathf.Abs(distance.GetAU() - orbitDistances[i].GetAU()) < 0.1f)
            {
                GeneratePlanets(generatedPlanetsCount, orbitDistances);
                return;
            }
        }

        orbitDistances.Add(distance);
        GameObject planet = new GameObject("Planet");
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        objects.Add(visual);
        terrainTypes.Add(Random.Range(1, 5));
        visual.transform.parent = planet.transform;
        Object planetObject = planet.AddComponent<Object>();
        PACB physics = planet.AddComponent<PACB>();
        physics.Ma = distance.GetAU();
        physics.Mi = physics.Ma * (1 - 0.1f * Random.value);
        physics.SOI = mainBody;
        physics.travelDirection = 1;
        physics.orbitPoints = 6900;
        physics.angle1 = Random.value * 360;
        physics.angle2 = Random.value * 20;
        physics.angle3 = Random.value * 20;

        Color mainColor = Random.ColorHSV();
        colors.Add(mainColor);
        mainColor.a = .75f;
        physics.orbitColor = mainColor;

        if (Random.value <= .25)
        {
            //Gas giant
            planetObject.mass = 10 + 500 * Random.value;
            planetObject.radius = 10f;
            var meshRenderer = visual.GetComponent<MeshRenderer>();
            meshRenderer.material = materials[1];
            var ggv = visual.AddComponent<GasGiantVisual>();
            ggv.stripeCount = 3 + (int) Mathf.Pow(4 * Random.value, 2) + (int) (16 * Random.value);
            ggv.blurIndex = 3 + (int) (12f * Random.value);
            ggv.colorSimilarityIndex = .1f + .4f * Random.value;
            ggv.seed = seed;
            ggv.speedMultiplier = (.05f + Random.value * .5f);
            if (Random.value > .5f)
            {
                ggv.fragmentZoom = 400 + 2000 * Random.value;
            }
            else
            {
                ggv.fragmentZoom = 50 + 200 * Random.value;
            }
            ggv.brightness = 1f + Random.value;
            ggv.baseColor = mainColor;
        }
        else
        {
            //Terrestrial
            planetObject.mass = 0.1f + Mathf.Pow(2.5f * Random.value, 2);
            planetObject.radius = 2f;
            var meshRenderer = visual.GetComponent<MeshRenderer>();
            Material material;
            if (Random.value > .5f)
            {
                material = new Material(materials[2]);
                material.SetColor("_Color", Random.ColorHSV());
                material.SetColor("_Color_1", Random.ColorHSV());
                material.SetColor("_Color_2", Random.ColorHSV());
                material.SetFloat("_Range", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.2f));
                material.SetFloat("_Clouds", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.4f));
                material.SetFloat("_Speed", .1f + Random.value * .5f);
                material.SetFloat("_Noise", .6f + Random.value * .2f);
            }
            else
            {
                material = new Material(materials[3]);
                material.SetColor("_Color1", mainColor);
                material.SetColor("_Color2", Random.ColorHSV());
                material.SetColor("_Color3", Random.ColorHSV());
                material.SetFloat("_Seed", seed);
                material.SetFloat("_Clouds", Mathf.Clamp(Random.value * .3f - .1f,0, 0.4f));
                material.SetFloat("_Range", Mathf.Clamp(Random.value * .3f - .1f,0, 0.2f));
                material.SetFloat("_Speed", Random.value * .4f);
            }
            meshRenderer.material = material;
        }

        float SOIInfluence = mainBody.mass / Mathf.Pow(distance.GetAU(), 2);
        planetObject.SOIdistance = Mathf.Sqrt(planetObject.mass / SOIInfluence);


        List<float> moonDistances = new List<float>();
    moonGen:
        if (planetObject.SOIdistance > .001f && Random.value > .4f)
        {
            float moonDist = Random.value * planetObject.SOIdistance;

            foreach (var item in moonDistances)
            {
                if (Mathf.Abs(moonDist - item) < .01f)
                {
                    goto endMoonGen;
                }
            }
            GameObject moon = new GameObject("Moon");
            GameObject moonVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            moonVisual.transform.parent = moon.transform;
            Object moonObject = moon.AddComponent<Object>();
            PACB moonPhysics = moon.AddComponent<PACB>();
            moonPhysics.Ma = moonDist;
            moonPhysics.Mi = moonPhysics.Ma * (1 - 0.1f * Random.value);
            moonPhysics.Ma += (planetObject.radius / 23454.8f) * StarSystem.singleton.scale; //Temp?
            moonPhysics.Mi += (planetObject.radius / 23454.8f) * StarSystem.singleton.scale; //Temp?
            moonPhysics.SOI = planetObject;
            moonPhysics.travelDirection = 1;
            moonPhysics.orbitPoints = 690;
            moonPhysics.angle1 = Random.value * 360;
            moonPhysics.angle2 = Random.value * 360;
            moonPhysics.angle3 = Random.value * 360;
            moonPhysics.update = true;

            moonObject.mass = 0.1f * planetObject.mass * (.1f + Random.value);
            moonObject.radius = .2f;
            var meshRenderer = moonVisual.GetComponent<MeshRenderer>();
            Material material;
            if (Random.value > .5f)
            {
                material = new Material(materials[2]);
                material.SetColor("_Color", Random.ColorHSV());
                material.SetColor("_Color_1", Random.ColorHSV());
                material.SetColor("_Color_2", Random.ColorHSV());
                material.SetFloat("_Range", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.2f));
                material.SetFloat("_Clouds", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.4f));
                material.SetFloat("_Speed",Random.value * .4f);
                material.SetFloat("_Noise", .6f + Random.value * .2f);
            }
            else
            {
                material = new Material(materials[3]);
                material.SetColor("_Color1", mainColor);
                material.SetColor("_Color2", Random.ColorHSV());
                material.SetColor("_Color3", Random.ColorHSV());
                material.SetFloat("_Seed", seed);
                material.SetFloat("_Clouds", Mathf.Clamp(Random.value * .5f - .1f, 0, 0.4f));
                material.SetFloat("_Range", Mathf.Clamp(Random.value * .3f - .1f, 0, 0.2f));
                material.SetFloat("_Speed", Random.value * .4f);
            }
            meshRenderer.material = material;

            SOIInfluence = mainBody.mass / Mathf.Pow(moonDist, 2);
            moonObject.SOIdistance = Mathf.Sqrt(moonObject.mass / SOIInfluence);

            Color moonMainColor = Random.ColorHSV();
            moonMainColor.a = .75f;
            moonPhysics.orbitColor = mainColor;

            moonDistances.Add(moonDist);
            goto moonGen;

        }
        endMoonGen:

        GeneratePlanets(generatedPlanetsCount + 1, orbitDistances);
    }

    public struct AU
    {
        //Astronomical unit
        //Sizes of bodies are their radii

        float value;

        public float GetAU()
        {
            return value;
        }
        public float GetUnityUnits()
        {
            return value * StarSystem.singleton.AuToUnityUnits;
        }
        public float GetSunSizes()
        {
            return value * 215.032f;
        }
        public float GetEarthSizes()
        {
            return value * 23454.8f;
        }

        public void SetAU(float value)
        {
            this.value = value;
        }
        public void SetUnityUnits(float value)
        {
            this.value = value / StarSystem.singleton.AuToUnityUnits;
        }
        public void SetSunSizes(float value)
        {
            this.value = value / 215.032f;
        }
        public void SetEarthSizes(float value)
        {
            this.value = value / 23454.8f;
        }
    }
}
