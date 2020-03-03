using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    /// <summary>
    /// This class handles noise generation
    /// /// </summary>
    /// 
//#if (UNITY_EDITOR)
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;
    //Contructor
    public void UpdateSettings(ShapeSettings settings) {
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        
        for (int i = 0; i < noiseFilters.Length; i++) {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        elevationMinMax = new MinMax();
    }
//#endif

    //Noise function
    public Vector3 CalculatePointOnMountain(Vector3 pointOnUnitSphere, int seed, int index) {
        float firstLayerValue = 0;
       
        float elevation = 0;

        if (noiseFilters.Length > 0) {//Have filter
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere,settings.mountainRadius, seed);
            if (settings.noiseLayers[0].enabled) {//Layer enabled
                elevation = firstLayerValue;
            }
        }

        for (int i = 0; i < noiseFilters.Length; i++) {
            if (settings.noiseLayers[i].enabled) {
                float mask = (settings.noiseLayers[i].useTheFirstLayerAsMask) ? firstLayerValue : 1;//Set the first layer as mask?
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere, settings.mountainRadius, seed) * mask;//Apply each noise filter to vertex 
            }
        }
        //pointOnUnitSphere * settings.mountainRadius * (1+elevation);
        elevation = settings.mountainRadius * (1 + elevation);
        elevationMinMax.AddValueToReturnIndex(elevation, index);
        return pointOnUnitSphere * elevation;//return vector 3 after applying the noise
    }
}
