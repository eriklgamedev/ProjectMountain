//#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float mountainRadius = 1;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useTheFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}
//#endif