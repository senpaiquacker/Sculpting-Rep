using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex
{
    public Vector3 LocalPosition { get; set; }
    public int Id { get; private set; }
    public GeometryGraph GlobalObject { get; set; }
    private List<Edge> edges;
    public List<Edge> Edges
    {
        get
        {
            if (edges is null)
                edges = GlobalObject.Edges.Where(a => a[0].Equals(this) || a[1].Equals(this)).ToList();
            return edges;
        }
    }
    public Vertex(int id)
    {
        Id = id;
    }
    public override bool Equals(object obj)
    {
        if (!(obj is Vertex) || obj is null)
            return false;
        return Id == ((Vertex)obj).Id;
    }
}
public class Edge
{
    public GeometryGraph GlobalObject { get; set; }
    private Tuple<Vertex, Vertex> Vertices;
    public Vertex this[int i]
    {
        get
        {
            if (i == 0)
                return Vertices.Item1;
            else if (i == 1)
                return Vertices.Item2;
            else throw new ArgumentException("There are only Two Vertices");
        }
    }
    public Triangle[] Triangles
    {
        get
        {
            return GlobalObject.Triangles.Where(a => a.ContainsEdge(this)).ToArray();
        }
    }
    public Edge(Vertex first, Vertex second)
    {
        Vertices = new Tuple<Vertex, Vertex>(first, second);
    }
    public Vertex GetOtherVertex(Vertex first)
    {
        return Vertices.Item1 == first ?
                        Vertices.Item2 :
                        (Vertices.Item2 == first ? 
                                  Vertices.Item1 : 
                                  throw new ArgumentException());
    }
    public void Destroy()
    {
        Vertices.Item1.Edges.Remove(this);
        Vertices.Item2.Edges.Remove(this);
        GlobalObject.Edges.Remove(this);
        foreach(var trio in Triangles)
        {
            trio.Destroy();
        }
    }
    //TODO: Line equation
    public override bool Equals(object obj)
    {
        if (!(obj is Edge) || obj is null)
            return false;
        return (Vertices.Item1 == ((Edge)obj)[0] || Vertices.Item1 == ((Edge)obj)[1]) &&
               (Vertices.Item2 == ((Edge)obj)[0] || Vertices.Item2 == ((Edge)obj)[1]);
    }
}
public class Triangle
{
    public GeometryGraph GlobalObject { get; set; }
    private Edge[] Edges = new Edge[3];
    public Edge this[int i]
    {
        get
        {
            return Edges[i];
        }
        set
        {
            Edges[i] = value;
        }
    }
    private Vertex[] vertices;
    public Vertex[] Vertices
    {
        get
        {
            if (vertices == null)
            {
                vertices = new Vertex[3];
                vertices.Select(a =>
                {
                    foreach (var edge in Edges)
                    {
                        if (!vertices.Contains(edge[0]))
                            return edge[0];
                        else if (!vertices.Contains(edge[1]))
                            return edge[1];
                    }
                    throw new ArgumentException("Dumped Triangle");
                });
            }
            return vertices;
        }
    }
    public Triangle(Vertex[] verts)
    {
        vertices = verts;
        var edges = new[]
        {   new Edge(verts[0], verts[1]),
            new Edge(verts[1], verts[2]),
            new Edge(verts[2], verts[0]) };
        foreach (var edge in edges)
            edge.GlobalObject = verts[0].GlobalObject;
        Edges = edges;
        verts[0].GlobalObject.Edges.AddRange(edges);
        GlobalObject = verts[0].GlobalObject;
    }
    public void Destroy()
    {
        GlobalObject.Triangles.Remove(this);
        GlobalObject.Changed = true;
    }
    public bool ContainsEdge(Edge edge)
    {
        return Edges[0].Equals(edge) || Edges[1].Equals(edge) || Edges[2].Equals(edge);
    }
    //TODO: Plane Equation
}
public class GeometryGraph
{
    public List<Vertex> Vertices;
    public List<Edge> Edges;
    public List<Triangle> Triangles;
    private Matrix4x4 LocalToWorld;
    public bool Changed;
    //private Matrix4x4 WorldToLocal;
    private GeometryGraph()
    {

    }
    public static GeometryGraph CreateGraph(Vector3[] verts, int[][] trios)
    {
        var res = new GeometryGraph();
        int i = 0;
        res.Vertices = verts.Select(a => 
        {
            var newVert =  new Vertex(i++);
            newVert.GlobalObject = res;
            newVert.LocalPosition = a;
            return newVert;
        }).ToList();
        res.Edges = new List<Edge>();
        res.Triangles = new List<Triangle>();
        foreach (var trio in trios)
        {
            var newTrio = new Triangle(trio.Select(a => res.Vertices[a]).ToArray());
            res.Triangles.Add(newTrio);
        }
        return res;
    }
    public bool ExportGeometry(out Vector3[] verts, out int[] trios)
    {
        verts = Vertices.Select(a => a.LocalPosition).ToArray();
        var restrios = new List<int>();
        foreach(var trio in Triangles)
            restrios.AddRange(trio.Vertices.Select(a => a.Id));
        trios = restrios.ToArray();
        return true;
    }
}
