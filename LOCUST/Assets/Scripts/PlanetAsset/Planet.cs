using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NoiseSettings
{
    public float radius = 1;
    public float speed = 1;
    public float strength = .1f;
    public float amplitudePersistence = 0.5f;
    public float speedIncrease = 1.5f;
    [Range(1, 10)]
    public int layers = 1;
    public float min = 0.5f;
    public Vector3 offset = new Vector3(0, 0, 0);
}

public class Planet : MonoBehaviour
{
    public GameObject planetMarkerPrefab;

    [Range(2,256)]
    public int resolution = 20;

    public int texResolution = 50;

    [SerializeField, HideInInspector]
    private MeshFilter[] filters;
    private PlanetFace[] faces;

    private TerrainGenerator generator;

    public Material faceMaterial;

    public NoiseSettings settings;

    public Gradient elevationColors;
    public Gradient oceanColors;
    public Texture2D tex;

    public bool generate = false;
    public bool generateAndChangeSeed = false;

    public List<Vector3> surfacePoints = new List<Vector3>();

    public bool generated = false;

    private int seed = 0;

    private void Start()
    {
        seed = System.DateTime.Now.Millisecond;

        generatePlanet();

        surfacePoints.Clear();

        foreach (Vector3 p in getRandomPoints())
        {
            if(p.magnitude>0)
            {
                surfacePoints.Add(p);
                //Instantiate(planetMarkerPrefab, p, Quaternion.LookRotation(-p), transform);
            }
        }
        generated = true;
    }

    private void Update()
    {
        if(generate || generateAndChangeSeed)
        {
            if (generateAndChangeSeed) seed = System.DateTime.Now.Millisecond;

             generated = false;

            generatePlanet();
            generate = false;

            generated = true;
        }
    }

    public void generatePlanet()
    {
        init();
        generateMeshes();
        generateColors();

        generate = false;
    }

    //private void OnValidate()
    //{
    //    generatePlanet();
    //}

    void init()
    {        
        generator = new TerrainGenerator(settings, seed);//radius, layers, speed, strength, amplitudePersistence, speedIncrease, min, offset);

        if(tex==null)
        {
            tex = new Texture2D(texResolution, 2, TextureFormat.ARGB32, false);
        }

        if(filters == null || filters.Length == 0)
            filters = new MeshFilter[6];
        faces = new PlanetFace[6];

        Vector3[] directions = { Vector3.up, Vector3.left, Vector3.right, Vector3.down, Vector3.forward, Vector3.back };

        for(int i = 0; i < 6; ++i)
        {
            if(filters[i] == null)
            {
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.parent = transform;

                meshObject.AddComponent<MeshRenderer>();
                filters[i] = meshObject.AddComponent<MeshFilter>();
                filters[i].sharedMesh = new Mesh();
            }

            filters[i].GetComponent<MeshRenderer>().sharedMaterial = faceMaterial;

            faces[i] = new PlanetFace(generator, resolution, filters[i].sharedMesh, directions[i]);
        }
    }

    void generateMeshes()
    {
        foreach(PlanetFace f in faces)
        {
            f.constructMesh();
        }

        faceMaterial.SetVector("_elevationMinMax", new Vector4(generator.elevationMinMax.min, generator.elevationMinMax.max));
    }


    void generateColors()
    {
        Color[] colors = new Color[texResolution];
        Color[] waterColors = new Color[texResolution];

        Color oceanBaseColor = oceanColors.Evaluate(Random.value);
        Gradient selectedOcean = new Gradient();
        GradientColorKey[] oceanKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        oceanKeys[0].color = oceanBaseColor;
        oceanKeys[0].time = 0;
        alphaKeys[0].alpha = 1;
        alphaKeys[0].time = 0;        

        oceanKeys[1].color = Color.Lerp(oceanBaseColor, Color.white, 0.7f);
        oceanKeys[1].time = 5f;
        alphaKeys[1].alpha = 1;
        alphaKeys[1].time = 5f;

        selectedOcean.SetKeys(oceanKeys, alphaKeys);

        for (int i = 0; i < texResolution; ++i)
        {
            colors[i] = elevationColors.Evaluate(i / (texResolution - 1f));
            waterColors[i] = selectedOcean.Evaluate(i / (texResolution - 1f));
        }
        tex.SetPixels(0, 0, texResolution, 1, colors);
        tex.SetPixels(0, 1, texResolution, 1, waterColors);
        tex.Apply();

        faceMaterial.SetTexture("_planetTexture", tex);

        faceMaterial.SetFloat("_seaLevel", settings.min);
    }

    public Vector3[] getRandomPoints()
    {
        Vector3[] ps = new Vector3[faces.Length];

        for(int i = 0; i < faces.Length; ++i)
        {
            ps[i] = faces[i].getRandomOnLandPoint();

            //Debug.Log(ps[i]);
        }

        return ps;
    }
    
}
