using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Line
{
    public Vector3 CanonicalM { get; set; }
    public Vector3 CanonicalDirection { get; set; }
    public bool IsOnLine(Vector3 point)
    {
        return (point.x - CanonicalM.x) / CanonicalDirection.x ==
            (point.y - CanonicalM.y) / CanonicalDirection.y &&
            (point.y - CanonicalM.y) / CanonicalDirection.y ==
            (point.z - CanonicalM.z) / CanonicalDirection.z;
    }
    private Line(Vector3 M, Vector3 direction)
    {
        CanonicalM = M;
        CanonicalDirection = direction;
    }
    public static Line CreateByTwoPoints(Vector3 point1, Vector3 point2) => new Line(point1, point2 - point1);
    public static Line CreateCanonically(Vector3 M, Vector3 direction) => new Line(M, direction);
}
public class Plane
{
    private readonly float PlaneEps = Mathf.Pow(10, -6);
    public bool IsLimited { get => Points != default; }
    private (Vector3 Point1, Vector3 Point2, Vector3 Point3) points;
    public (Vector3 Point1, Vector3 Point2, Vector3 Point3) Points
    {
        get => points;
        set
        {
            points = value;
            AssureNormalParams();
        }
    }
    public Vector3 Normal { get; protected set; }
    public Vector3 MPoint { get; protected set; }
    public bool IsOnPlane(Vector3 point)
    {
        if (!IsOnPlane(point, true))
            return false;
        if (IsLimited)
        {
            var ABP = MathBlock.GetTriangleSquare(Points.Point1, Points.Point2, point);
            var ACP = MathBlock.GetTriangleSquare(Points.Point1, Points.Point3, point);
            var BCP = MathBlock.GetTriangleSquare(Points.Point2, Points.Point3, point);
            var ABC = MathBlock.GetTriangleSquare(Points.Point1, Points.Point2, Points.Point3);
            return Mathf.Abs(ABC - (ABP + ACP + BCP)) < PlaneEps;
        }
        else return IsOnPlane(point, true);   
    }
    public bool IsOnPlane(Vector3 point, bool unlimit) => Vector3.Dot(Normal, point - MPoint) == 0;
    private Plane(Vector3 normalVec, Vector3 point)
    {
        Normal = normalVec;
        MPoint = point;
    }
    private void AssureNormalParams()
    {
        Normal = new Vector3
            ((Points.Point2.y - Points.Point1.y) * (Points.Point3.z - Points.Point1.z)
          - (Points.Point3.y - Points.Point1.y) * (Points.Point2.z - Points.Point1.z),
            (Points.Point2.z - Points.Point1.z) * (Points.Point3.x - Points.Point1.x) 
          - (Points.Point2.x - Points.Point1.x) * (Points.Point3.z - Points.Point1.z),
            (Points.Point2.x - Points.Point1.x) * (Points.Point3.y - Points.Point1.y)
          - (Points.Point3.x - Points.Point1.x) * (Points.Point2.y - Points.Point1.y));
        MPoint = new Vector3(-1 * Points.Point1.x, -1 * Points.Point1.y, -1 * Points.Point1.z);
    }
    protected Plane()
    {

    }
    protected Plane(Vector3 A, Vector3 B, Vector3 C)
    {
        Points = (A, B, C);
        Normal = new Vector3
            ((B.y - A.y) * (C.z - A.z) - (C.y - A.y) * (B.z - A.z),
            (B.z - A.z) * (C.x - A.x) - (B.x - A.x) * (C.z - A.z),
            (B.x - A.x) * (C.y - A.y) - (C.x - A.x) * (B.y - A.y));
        MPoint = new Vector3(-1 * A.x, -1 * A.y, -1 * A.z);
    }
    public static Plane CreateByThreePoints(Vector3 A, Vector3 B, Vector3 C) => new Plane(A, B, C);
}
public class PointsPlane : Plane
{
    public new List<Vector3> Points { get; private set; }
    private PointsPlane() : base()
    {

    }
    private PointsPlane(params Vector3[] vecs)
    {
        Points = new List<Vector3>();
        Normal = new Vector3
            ((vecs[1].y - vecs[0].y) * (vecs[2].z - vecs[0].z)
          - (vecs[2].y - vecs[0].y) * (vecs[1].z - vecs[0].z),
            (vecs[1].z - vecs[0].z) * (vecs[2].x - vecs[0].x)
          - (vecs[1].x - vecs[0].x) * (vecs[2].z - vecs[0].z),
            (vecs[1].x - vecs[0].x) * (vecs[2].y - vecs[0].y)
          - (vecs[2].x - vecs[1].x) * (vecs[1].y - vecs[0].y));
        MPoint = new Vector3(-1 * vecs[0].x, -1 * vecs[0].y, -1 * vecs[0].z);
        foreach (var v in vecs)
            AddPointToPlane(v);
    }
    public static PointsPlane CreateByCouplePoints(params Vector3[] vecs) => new PointsPlane(vecs);
    public void AddPointToPlane(Vector3 point, Vector3 rayDirection)
    {
        var line = Line.CreateCanonically(point, rayDirection);
        var inter = new Vector3();
        if (!MathBlock.FindPlaneLineIntersection(this, line, out inter))
            throw new ArgumentException("Line is parallel to Plane");
        if (IsOnPlane(inter, true))
            Points.Add(inter);
    }
    public void AddPointToPlane(Vector3 point) => AddPointToPlane(point, Normal);
}
public static class MathBlock
{
    public static bool FindPlaneLineIntersection(Plane plane, Line line, out Vector3 intersection)
    {
        if (Vector3.Dot(plane.Normal, line.CanonicalDirection) == 0)
        {
            intersection = Vector3.negativeInfinity;
            return false;
        }
        var t = (Vector3.Dot(plane.Normal, line.CanonicalM)
                + Vector3.Dot(plane.Normal, plane.MPoint))
                / Vector3.Dot(plane.Normal, line.CanonicalDirection);
        intersection = line.CanonicalDirection * t + line.CanonicalM;
        return true;
    }
    public static float Determinant3x3(float[,] m3)
    {
        if (m3.Length != 3 || m3.LongLength / m3.Length != 3)
            throw new ArgumentException();
        return m3[0, 0] * m3[1, 1] * m3[2, 2]
             - m3[0, 0] * m3[1, 2] * m3[2, 1]
             - m3[0, 1] * m3[1, 0] * m3[2, 2]
             + m3[0, 1] * m3[1, 2] * m3[2, 0]
             + m3[0, 2] * m3[1, 0] * m3[2, 1]
             - m3[0, 2] * m3[1, 1] * m3[2, 0];
    }
    public static float GetTriangleSquare(Vector3 A, Vector3 B, Vector3 C)
    {
        var ab = Vector3.Distance(A, B);
        var bc = Vector3.Distance(B, C);
        var ca = Vector3.Distance(C, A);
        var p = (ab + bc + ca) / 2;
        return Mathf.Sqrt(p * (p - ab) * (p - bc) * (p - ca));
    }
    public static int GetClosestVertexId(Vector3 point, Vector3[] verticesGlobal)
    {
        int answ = 0;
        float dist = -1;
        for (int i = 0; i < verticesGlobal.Length; i++)
        {
            var d = Vector3.Distance(point, verticesGlobal[i]);
            if (dist == -1 || d < dist)
            {
                dist = d;
                answ = i;
            }
        }
        return answ;
    }
    public static int GetClosestVertexId(Vector3 point, VertexGraph mesh)
    {
        int answ = 0;
        float dist = -1;
        foreach(var i in mesh.Vertices)
        {
            var d = Vector3.Distance(point, i.WorldPosition);
            if(dist == -1 || d < dist)
            {
                dist = d;
                answ = i.VertexId;
            }
        }
        return answ;
    }
    public static List<int[]> GetAllAdjastentTriangles(List<int[]> trios, int id)
    {
        var result = new List<int[]>();
        foreach(var tr in trios)
            if (tr.Contains(id))
                result.Add(tr);
        return result;
    }
    /*public static List<Triangle> GetAllAdjastentTriangles(VertexGraph mesh, int id)
    {
        var result = new List<Triangle>();
        foreach(var tr in mesh.Triangles)
            if (tr.Contains(id))
                result.Add(tr);
        return result;
    }*/
    public static List<int[]> GetAllAdjastentTriangles(List<int[]> trios, Tuple<int, int> line)
    {
        var result = new List<int[]>();
        foreach (var tr in trios)
            if(tr.Contains(line.Item1) && tr.Contains(line.Item2))
                result.Add(tr);
        return result;
    }
    /*public static List<Triangle> GetAllAdjastentTriangles(VertexGraph mesh, Tuple<int, int> line)
    {
        var result = new List<Triangle>();
        foreach(var tr in mesh.Triangles)
            if (tr.Contains(line.Item1) && tr.Contains(line.Item2))
                result.Add(tr);
        return result;
    }
    public static Triangle FindTrianglePointIn(VertexGraph mesh, Vector3 point)
    {
        int vertid = GetClosestVertexId(point, mesh);
        Debug.Log(vertid);
        var adj = GetAllAdjastentTriangles(mesh, vertid);
        foreach(var tr in adj)
        {
            if (Plane.CreateByThreePoints
               (mesh.Vertices[tr[0]].WorldPosition,
                mesh.Vertices[tr[1]].WorldPosition,
                mesh.Vertices[tr[2]].WorldPosition).IsOnPlane(point))
                return tr;
        }
        throw new InvalidOperationException();
    }
    public static Vector3 GetClosestProjection(int[] triangle, Vector3[]verticesGlobal, Vector3 point)
    {
        var projList = new List<Vector3>();
        projList
            .Add(
            GetProjection(
                new Tuple<Vector3, Vector3>
                (verticesGlobal[triangle[0]],
                verticesGlobal[triangle[1]]), point));
        projList
            .Add(
            GetProjection(
                new Tuple<Vector3, Vector3>
                (verticesGlobal[triangle[1]],
                verticesGlobal[triangle[2]]), point));
        projList
            .Add(
            GetProjection(
                new Tuple<Vector3, Vector3>
                (verticesGlobal[triangle[2]],
                verticesGlobal[triangle[0]]), point));
        var result = projList[0];
        var minDist = -1.0f;
        for(int i = 0; i < projList.Count; i++)
        {
            var dist = Vector3.Distance(projList[i], result);
            if (dist < minDist || minDist == -1)
            {
                minDist = dist;
                result = projList[i];
            }
        }
        return result;
    }
    private static Vector3 GetProjection(Tuple<Vector3, Vector3> line, Vector3 point)
    {
        var lVec = line.Item2 - line.Item1;
        float lambda;
        var linePoint = line.Item1;
        lambda = -1 *
            (lVec.x * (point.x - linePoint.x) +
             lVec.y * (point.y - linePoint.y) +
             lVec.z * (point.z - linePoint.z)) /
             (lVec.x * lVec.x + lVec.y * lVec.y + lVec.z * lVec.z);
        var result =
            (new Vector3(linePoint.x + lVec.x * lambda,
            linePoint.y + lVec.y * lambda,
            linePoint.z + lVec.z * lambda));
        return
            Vector3.right * Mathf.Clamp(result.x, line.Item1.x, line.Item2.x) +
            Vector3.up * Mathf.Clamp(result.y, line.Item1.y, line.Item2.y) +
            Vector3.forward * Mathf.Clamp(result.z, line.Item1.z, line.Item2.z);
    }*/
}
