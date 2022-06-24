using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KnifeInstrument : MonoBehaviour
{
    public MeshFilter TopPlane;
    public MeshFilter BotPlane;
    private PointsPlane topPlaneEquation;
    private PointsPlane botPlaneEquation;
    public PointsPlane TopPlaneEquation
    {
        get
        {
            if (topPlaneEquation == null)
                topPlaneEquation = PointsPlane
                    .CreateByCouplePoints(TopPlane.mesh.vertices
                        .Select(a =>
                        {
                            var l2w = TopPlane.transform.localToWorldMatrix;
                            return l2w.MultiplyPoint3x4(a);
                        })
                        .ToArray());
            return topPlaneEquation;
        }
    }
    public PointsPlane BotPlaneEquation
    {
        get
        {
            if (botPlaneEquation == null)
                botPlaneEquation = PointsPlane
                    .CreateByCouplePoints(BotPlane.mesh.vertices
                        .Select(a =>
                        {
                            var l2w = BotPlane.transform.localToWorldMatrix;
                            return l2w.MultiplyPoint3x4(a);
                        })
                        .ToArray());
            return botPlaneEquation;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var allEdges = VertexGraph.FindAllEdgesInScene();
        foreach(var edge in allEdges)
        {
            Vector3 inter;
            if(MathBlock.FindPlaneLineIntersection(BotPlaneEquation, edge.EdgeLine, out inter))
            {
                var triIds = edge.Root.Triangles
                    .Where(a => a.Contains(edge.Link.FirstV.VertexId) && a.Contains(edge.Link.SecondV.VertexId))
                    .Select(a => Array.IndexOf(edge.Root.Triangles, a));
                var newTris = edge.Root.ModelTransform.GetComponent<Mesh>().triangles.ToList();
                foreach(var tid in triIds)
                {
                    newTris.RemoveAt(tid * 3);
                    newTris.RemoveAt(tid * 3 + 1);
                    newTris.RemoveAt(tid * 3 + 2);
                }
                edge.Root.ModelTransform.GetComponent<Mesh>().triangles = newTris.ToArray();
            }
        }
    }
}
