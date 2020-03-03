using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{
    public float Min { get; private set; }
    public float Max { get; private set; }
    public int Index { get; private set; }
    public List<float> elevation;
    public List<int> indexes;
    //public float[] elevation { get; private set; }
    //public int[] indexes { get; private set; }


    public MinMax() {
        Min = float.MaxValue;
        Max = float.MinValue;
        Index = 0;
        elevation = new List<float>();
        indexes = new List<int>();
    }

    public void AddValue(float v) {
        if (v > Max) {
            Max = v;
        }
        if (v < Min) {
            Min = v;
        }
    }
    public void AddValueToReturnIndex(float v, int index) {
        if (elevation != null) {
            //Debug.Log(v);
            elevation.Add(v);
            indexes.Add(index);
       }
        if (v > Max) {
            Max = v;
            Index = index;
        }
        if (v < Min) {
            Min = v;
        }
    }
}
