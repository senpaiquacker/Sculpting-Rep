using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArmHolder : MonoBehaviour
{
    public Mesh UsedInstrumentForm;
    public void VisualizeMesh()
    {
        MeshFilter instrumentFilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        instrumentFilter.mesh = UsedInstrumentForm;
        MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
    }
}
