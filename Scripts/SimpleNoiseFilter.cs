using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
//#if (UNITY_EDITOR)
    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings) {
        this.settings = settings;
    }
//#endif
    public float Evaluate(Vector3 point, float mountainRadiu, int seed) {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++) {
            float v = noise.Evaluate(point * frequency + settings.centre);//Generate noise
            noiseValue += (v + 1) * .5f * amplitude;//Set layer value
            //Increase based when greater than 1 and decrease when less that 1 for each layer
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        if (point.y > -0.1f || point.y < -0.3f)
        {

            noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        }
        else {
            noiseValue = 0;
        }
        
        
        return noiseValue * settings.strength;
    }

}
