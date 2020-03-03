//#if (UNITY_EDITOR) 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType {Simple, Ridgid, Peak};
    public FilterType filterType;
#if (UNITY_EDITOR) 
    [ConditionalHide("filterType", 0)]
#endif
    public SimpleNoiseSettings simpleNoiseSettings;
#if (UNITY_EDITOR)
    [ConditionalHide("filterType", 1)]
#endif
    public RidgidNoiseSettings ridgidNoiseSettings;
#if (UNITY_EDITOR)
    [ConditionalHide("filterType", 2)]
#endif
    public PeakNoiseSettings peakNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings {
        public float strength = 1;
        [Range(1, 8)]
        public int numLayers = 1;
        public float baseRoughness = 1;
        public float roughness = 2;
        public float persistence = .5f;
        public Vector3 centre;
        public float minValue;
    }

    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings {
        public float weightMultiplier = .8f;
    }

    [System.Serializable]
    public class PeakNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = .8f;
    }

}
