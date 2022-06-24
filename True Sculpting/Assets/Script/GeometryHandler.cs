using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeometryHandler : MonoBehaviour
{
    [SerializeField]
    private Mesh objMesh;
    public Mesh ObjectMesh
    {
        get => objMesh;
        set => objMesh = value;
    }
    private bool MeshChanged;
    public GeometryGraph GeomSystem;
    
    public void Start()
    {
        objMesh = GetComponent<MeshFilter>().mesh;
        int[][] tris = new int[objMesh.triangles.Length / 3][];
        for(int i = 0; i < objMesh.triangles.Length; i++)
        {
            if (tris[i / 3] == null)
                tris[i / 3] = new int[3];
            tris[i / 3][i % 3] = objMesh.triangles[i];
        }
        GeomSystem = GeometryGraph.CreateGraph(ObjectMesh.vertices, tris);
        DisableCulling();
    }
    public void Update()
    {
        MeshChanged = GeomSystem.Changed;
        if(MeshChanged)
        {
            UpdateMesh();
            DisableCulling();
            MeshChanged = false;
        }
    }
    private void DisableCulling()
    {
        var CullingTrios = objMesh.triangles.ToList();
        for (int i = 0; i < objMesh.triangles.Length; i += 3)
        {
            CullingTrios.Add(objMesh.triangles[i + 2]);
            CullingTrios.Add(objMesh.triangles[i + 1]);
            CullingTrios.Add(objMesh.triangles[i]);
        }
        objMesh.triangles = CullingTrios.ToArray();
    }
    private void UpdateMesh()
    {
        Vector3[] newverts;
        int[] newtrios;
        if(GeomSystem.ExportGeometry(out newverts, out newtrios))
        {
            objMesh.vertices = newverts;
            objMesh.triangles = newtrios;
        }
        GeomSystem.Changed = false;
    }
    private void OnRenderObject()
    {
    }
}
