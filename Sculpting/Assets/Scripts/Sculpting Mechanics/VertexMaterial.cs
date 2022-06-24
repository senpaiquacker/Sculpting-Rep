using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class VertexMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    private Mesh modelMesh;
    public Vector3[] VerticesGlobal
    {
        get
        {
            Matrix4x4 l2W = transform.localToWorldMatrix;
            return modelMesh.vertices.Select(a => l2W.MultiplyPoint3x4(a)).ToArray();
        }
    }

    public Vector3[] Sculpt { get; private set; }
    private Vector3[] origMeshVerts;
    private int[] origMeshTrios;
    private int[] backfaceTrios;
    public List<int[]> VertexTrios { get; private set; }
    public VertexGraph VertexSystem { get; private set; }

    void Start()
    {
        modelMesh = GetComponent<MeshFilter>().mesh;
        origMeshVerts = modelMesh.vertices;
        origMeshTrios = modelMesh.triangles;
        VertexTrios = new List<int[]>();
        Sculpt = origMeshVerts;
        UpdateMesh(origMeshVerts, origMeshTrios);
        VertexSystem = new VertexGraph(origMeshVerts, origMeshTrios, transform);
        UnHighlight();
    }
    public int GetCleanVertexTriosCount(int id)
    {
        return origMeshTrios.Count(a => a == id);
    }
    private void DisableCulling(int[] AddedTrios)
    {
        var triangles = AddedTrios.ToList();
        var arrLength = triangles.Count;
        for(int i = 0; i < arrLength; i+=3)
        {
            int[] trngl = { triangles[i], triangles[i + 1], triangles[i + 2] };
            triangles.AddRange(trngl.Reverse());
        }
        modelMesh.triangles = triangles.ToArray();
    }

    private void FillTrios(int[] trios)
    {
        var triangles = modelMesh.triangles.ToList();
        for(int i = 0; i < triangles.Count; i += 3)
        {
            VertexTrios.Add
            (
                new[]
                {
                    triangles[i],
                    triangles[i + 1],
                    triangles[i + 2]
                }
            );    
        }
    }

    public void UpdateMesh(Vector3[] updatedVertices)
    {
        modelMesh.vertices = updatedVertices;
        var hitbox = GetComponent<MeshCollider>();
        hitbox.sharedMesh = modelMesh;
    }

    public void UpdateMesh(Vector3[] updatedVertices, int[] addedTrios)
    {
        UpdateMesh(updatedVertices);
        FillTrios(origMeshTrios);
        DisableCulling(origMeshTrios);
    }

    public void Highlight()
    {
        GetComponent<MeshRenderer>().materials[0].SetColor("_WireColor", Color.green);
    }

    public void UnHighlight()
    {
        GetComponent<MeshRenderer>().materials[0].SetColor("_WireColor", Color.red);
    }
}
