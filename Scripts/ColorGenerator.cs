﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator 
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;

    public void UpdateSettings(ColorSettings settings) {
        this.settings = settings;
        if (texture == null) {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UndateElevation(MinMax elevationMinMax) {
        settings.mountainMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public void UpdateColors() {
        Color[] colors = new Color[textureResolution];
        for (int i = 0; i < textureResolution; i++) {
            colors[i] = settings.gradient.Evaluate(i/(textureResolution - 1f));
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.mountainMaterial.SetTexture("_texture", texture);
    }
}
