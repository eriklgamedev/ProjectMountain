using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgidNoiseFilter : INoiseFilter
{
//#if (UNITY_EDITOR)
    NoiseSettings.RidgidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
    {
        this.settings = settings;
    }
//#endif
    public float Evaluate(Vector3 point, float mountainRadiu,int seed)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        noise.Randomize(seed);
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));//Generate noise
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            noiseValue += v * amplitude;//Set layer value
            //Increase based when greater than 1 and decrease when less that 1 for each layer
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        /*
        if (point.y > -0.1f || point.y < -0.3f)
        {
            noiseValue = 0;
        }
        else
        {
            noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
           
        }*/

        float altitude = point.y;
        float multiplierPercentage = 1f / altitude;
        return noiseValue * settings.strength * Mathf.Clamp(Mathf.Abs(multiplierPercentage), 1f, 6f);
    }

}
