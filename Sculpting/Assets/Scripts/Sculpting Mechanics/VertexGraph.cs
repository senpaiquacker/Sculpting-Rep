using UnityEngine;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

public class Vertex3
{
    public int VertexId { get; private set; }
    public Vector3 LocalStartPosition { get; private set; }
    public Vector3 LocalPosition { get; set; }
    public Vector3 WorldPosition 
    {
        get => LocalMesh
            .ModelTransform
            .localToWorldMatrix
            .MultiplyPoint3x4(LocalPosition);
        set =>
            LocalPosition = LocalMesh
            .ModelTransform
            .worldToLocalMatrix
            .MultiplyPoint3x4(value);
    }
    public VertexGraph LocalMesh { get; set; }
    public Line[] EdgeExpressions 
    {
        get => edges.Select(a => a.EdgeLine).ToArray();
    }
    private List<Edge> edges;
    public int[] NeighbourIds
    {
        get => edges.Select(a => a.Neighbour(this).VertexId).ToArray();
    }
    public Vertex3[] Neighbours
    { 
        get => edges.Select(a => a.Neighbour(this)).ToArray(); 
    }
    public Dictionary<int, Edge> EdgesByVertId
    {
        get => edges.ToDictionary(a => a.Neighbour(this).VertexId);
    }
    public Vertex3(int vertId, Vector3 startPos, VertexGraph mesh)
    {
        VertexId = vertId;
        LocalStartPosition = startPos;
        LocalPosition = LocalStartPosition;
        LocalMesh = mesh;
        edges = new List<Edge>();
    }
    public static bool operator ==(Vertex3 vert1, Vertex3 vert2) => 
        (vert1 is null || vert2 is null) ? 
            (vert1 is null && vert2 is null ? true : false) : 
            vert1.Equals(vert2);
    public void ConnectNewVertices(params Vertex3[] vertices)
    {
        foreach(var v in vertices)
            new Edge(this, v);
    }
    public void ConnectNewVertices(params int[] vertexIds)
    {
        foreach(var v in vertexIds)
            new Edge(this, LocalMesh.Vertices[v]);
    }
    public void RemoveConnectionWith(Vertex3 vertex) => EdgesByVertId[vertex.VertexId].DestroyConnection();
    public void AddEdge(Edge edge) => edges.Add(edge);
    public void RemoveEdge(Edge edge) => edges.Remove(edge);
    public static bool operator !=(Vertex3 vert1, Vertex3 vert2) => !(vert1 == vert2);
    public bool Equals(Vertex3 vert1) =>  VertexId == vert1.VertexId;
}
public class Edge
{
    private Vertex3 first;
    private Vertex3 second;
    public Triangle[] Triangles;
    public Tuple<Edge,Edge>[] NeighbourEdgesByTriangles
    {
        get
        {
            var answ = new Tuple<Edge, Edge>[Triangles.Length];
            int i = 0;
            foreach(var trio in Triangles)
            {
                var last = trio.LastOne(this);
                answ[i] = new Tuple<Edge, Edge>
                    (first.EdgesByVertId[last], 
                    second.EdgesByVertId[last]);
                i++;
            }
            return answ;
        }
    }
    public VertexGraph Root { get => first.LocalMesh; }
    public (Vertex3 FirstV, Vertex3 SecondV) Link
    {
        get => (first, second);
    }
    private Line line;
    public Line EdgeLine
    {
        get
        {
            if (line == null)
                line = Line.CreateByTwoPoints(first.WorldPosition, second.WorldPosition);
            return line;
        }
    }
    public Edge(Vertex3 vert1, Vertex3 vert2)
    {
        first = vert1;
        second = vert2;
        vert1.AddEdge(this);
        vert2.AddEdge(this);
    }
    public Vertex3 Neighbour(Vertex3 init)
    {
        return Link.FirstV == init ? Link.FirstV : Link.SecondV;
    }
    public bool IsIntersects(PointsPlane plane)
    {
        Vector3 intersection;
        return MathBlock.FindPlaneLineIntersection(plane, line, out intersection);
    }
    public void DestroyConnection()
    {
        first.RemoveEdge(this);
        second.RemoveEdge(this);
    }
    public override bool Equals(object other)
    {
        if (other is Edge)
            return (first == ((Edge)other).first && second == ((Edge)other).second)
                || (first == ((Edge)other).second && second == ((Edge)other).first);
        else return false;
    }
    public static bool operator ==(Edge e1, Edge e2) => e1.Equals(e2);
    public static bool operator !=(Edge e1, Edge e2) => !(e1 == e2);
}
public class Triangle
{
    private int[] vertices = new int[3];
    public int this[int i] { get => vertices[i]; }
    public Triangle(int v1, int v2, int v3) => vertices = new[] { v1, v2, v3 };
    public Tuple<int, int> OtherTwo(int id)
    {
        var res = vertices.Where(a => a != id).ToArray();
        return new Tuple<int, int>(res[0], res[1]);
    }
    public int LastOne(Edge edge)
    {
        if (!vertices.Contains(edge.Link.FirstV.VertexId) || !vertices.Contains(edge.Link.SecondV.VertexId))
            throw new ArgumentException("Triangle doesn't contain this edge");
        return vertices.First(a => a != edge.Link.FirstV.VertexId && a != edge.Link.SecondV.VertexId);
    }
    public bool Contains(int vertId) => vertices.Contains(vertId);
    public bool Equals(Triangle tr)
    {
        for(int i = 0; i < 3; i++)
            if (this[i] != tr[i])
                return false;
        return true;
    }
}
public class VertexGraph : MonoBehaviour
{
    public Transform ModelTransform { get; private set; }
    private Vertex3[] vertices;
    public Vertex3[] Vertices { get => vertices; }
    private Edge[] edges;
    public Edge[] Edges { get => edges; }
    private Triangle[] triangles;
    public Triangle[] Triangles { get => triangles; }
    public Plane[] TriPlanes 
    {
        get => Triangles
            .Select(a => Plane
            .CreateByThreePoints
            (Vertices[a[0]].LocalPosition, 
            Vertices[a[1]].LocalPosition, 
            Vertices[a[2]].LocalPosition))
            .ToArray();
    }
    public VertexGraph(Vector3[] verts, int[] trios, Transform transform)
    {
        ModelTransform = transform;
        triangles = new Triangle[trios.Length / 3];
        for(int i = 0; i < trios.Length; i+=3)
            triangles[i / 3] = new Triangle(trios[i], trios[i + 1], trios[i + 2]);
        vertices = new Vertex3[verts.Length];
        for(int i = 0; i < verts.Length; i++)
            if (vertices[i] == default)
                vertices[i] = new Vertex3(i, verts[i], this);
    }
    public void AddEdge(int vertId1, int vertId2)
    {
        edges = edges.Append(new Edge(vertices[vertId1], vertices[vertId2])).ToArray();
        Vertices[vertId1].ConnectNewVertices(vertId2);
        Vertices[vertId2].ConnectNewVertices(vertId1);
    }
    public void AddEdge(Vertex3 vert1, Vertex3 vert2)
    {
        edges = edges.Append(new Edge(vert1, vert2)).ToArray();
        vert2.ConnectNewVertices(vert1);
    }
    
    public static Edge[] FindAllEdgesInScene() =>
        FindObjectsOfType<VertexMaterial>()
            .SelectMany(a => a.VertexSystem.Edges)
            .ToArray();
}
