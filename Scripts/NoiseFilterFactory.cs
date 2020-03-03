using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory 
{
    //#if (UNITY_EDITOR) 
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings) {
        switch (settings.filterType) {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Ridgid:
                return new RidgidNoiseFilter(settings.ridgidNoiseSettings);
            case NoiseSettings.FilterType.Peak:
                return new PeakNoiseFilter(settings.peakNoiseSettings);
        }
        return null;
    }
    //#endif
}
