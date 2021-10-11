using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator
{
    public NoiseSettings settings;

    public NoiseFilter noise;

    public MinMax elevationMinMax;

    public TerrainGenerator(NoiseSettings settings, int seed)
    {
        this.settings = settings;
        noise = new NoiseFilter(seed);
        elevationMinMax = new MinMax();
    }

    public Vector3 getPointOnPlanet(Vector3 pointOnSphere, out float unscaledElevation)
    {
        float h = 0;

        for(int i = 0; i < settings.layers; ++i)
        {
            h += noise.evaluate(pointOnSphere * settings.speed * Mathf.Pow(settings.speedIncrease, i) + settings.offset) * settings.strength * Mathf.Pow(settings.amplitudePersistence, i);
        }

        unscaledElevation = h;

        elevationMinMax.register(unscaledElevation);

        h = Mathf.Max(settings.min, h);

        h = settings.radius * (h + 1);

        return pointOnSphere * h;
    }
}

public class NoiseFilter
{
    private Noise noise;

    public NoiseFilter(int seed)
    {
        noise = new Noise(seed);
    }

    public float evaluate(Vector3 point)
    {
        float value = (noise.Evaluate(point) + 1) / 2.0f;
        return value;
    }
}

public class MinMax
{
    public float min { get; private set; }
    public float max { get; private set; }

    public MinMax()
    {
        min = 500f;
        max = 0f;
    }

    public void register(float v)
    {
        if(v > max)
        {
            max = v;
        }

        if(v < min)
        {
            min = v;
        }
    }
}
