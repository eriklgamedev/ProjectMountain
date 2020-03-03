using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeakNoiseFilter : INoiseFilter
{
//#if (UNITY_EDITOR)
    NoiseSettings.PeakNoiseSettings settings;
    Noise noise = new Noise();

    public PeakNoiseFilter(NoiseSettings.PeakNoiseSettings settings)
    {
        this.settings = settings;
    }
//#endif
    public float Evaluate(Vector3 point, float mountainRadius, int seed)
    {
        

        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float distancToCenter = 0;
        float maxDistancToCenter = 0;
        Vector2 centerPoint = new Vector2(0, 0);
        Vector2 currentPoint = new Vector2(point.x, point.z);

        noise.Randomize(seed);
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);//Generate noise
            noiseValue += (v + 1) * .5f * amplitude;//Set layer value
            //Increase based when greater than 1 and decrease when less that 1 for each layer
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }
        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);

        /*
        if (point.y > -0.1f || point.y < -0.3f)
        {

            noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        }
        else
        {
            noiseValue = 0;
        }
        */
        

        distancToCenter = Vector2.Distance(currentPoint, centerPoint);
        //x = c + r * (p - c) / (norm (p - c))
        maxDistancToCenter = Vector2.Distance(centerPoint,  new Vector2 (1f,1f));
        
       
        float multiplierPercentage = maxDistancToCenter / distancToCenter;
        //Debug.Log(multiplierPercentage);
        return noiseValue * (settings.strength * Mathf.Clamp(multiplierPercentage, 1f, 3f) * point.y);//Increase strength base on height
    }
}
