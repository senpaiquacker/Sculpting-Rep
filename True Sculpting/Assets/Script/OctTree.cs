using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class OctTree : MonoBehaviour
{
    List<Vertex> Edges;
    public Queue<Vertex> EdgesInQueue;
    public GameObject DebugCubePrefab;
    public Transform TargetObject;
    public float StartRenderDistance = 64f;
    public OctTreeNode Root;
    public Vector3 MinEdge;
    public Vector3 MaxEdge;
    private int DetalizationLevel;
    GeometryGraph System;
    public void Start()
    {
        System = GetComponent<GeometryHandler>().GeomSystem;
        Edges = System.Vertices;

        EdgesInQueue = new Queue<Vertex>();
        foreach (var v in Edges)
            EdgesInQueue.Enqueue(v);
        Root = new OctTreeNode(MinEdge, MaxEdge, DebugCubePrefab);
        DetalizationLevel = 1;
        var touchedVertices = new bool[Edges.Count];
        while(EdgesInQueue.Count != 0)
            Root.DeployVertex(EdgesInQueue.Dequeue());
    }
    public void Update()
    {
        if(MathScript.GetSquareDistance((MaxEdge + MinEdge) / 2, TargetObject.position) < 
            (StartRenderDistance / DetalizationLevel) *
            (StartRenderDistance / DetalizationLevel))
        {
            DetalizationLevel++;
            Root.AddLevel();
        }
        Root.CheckVisibility();
    }
    public List<int> FindBlock(Vector3 pos)
    {
        if (!CheckConfinement(MinEdge, MaxEdge, pos))
            return null;
        else
        {
            var res = new List<int>();
            int i = 0;
            if (Root.Children[0] != null)
            {
                foreach (var child in Root.Children)
                {
                    if (CheckConfinement(child.MinEdge, child.MaxEdge, pos))
                    {
                        res.Add(i);
                        return FindBlock(pos, child, res);
                    }
                    i++;
                }
                throw new System.InvalidOperationException();
            }
            else
                return new List<int>();
        }
    }
    private List<int> FindBlock(Vector3 pos, OctTreeNode node, List<int> result)
    {
        var count = result.Count;
        int i = 0;
        if (node.Children[0] != null)
        {
            foreach (var child in node.Children)
            {
                if (CheckConfinement(child.MinEdge, child.MaxEdge, pos))
                {
                    result.Add(i);
                    return FindBlock(pos, child, result);
                }
                i++;
            }
            throw new System.InvalidOperationException();
        }
        else
            return result;
    }
    public static (Vector3 min, Vector3 max) GetEdgesByNumber(int number, Vector3 min, Vector3 max)
    {
        var res = (new Vector3(), new Vector3());
        var numberCode = new bool[3];
        for(int i = 0; i < 3; i++)
        {
            numberCode[i] = number % 2 != 0;
            number /= 2;
        }
        if(!numberCode[0])
        {
            res.Item1.x = min.x;
            res.Item2.x = (max.x + min.x) / 2;
        }
        else
        {
            res.Item1.x = (max.x + min.x) / 2;
            res.Item2.x = max.x;
        }
        if(!numberCode[1])
        {
            res.Item1.y = min.y;
            res.Item2.y = (max.y + min.y) / 2;
        }
        else
        {
            res.Item1.y = (max.y + min.y) / 2;
            res.Item2.y = max.y;
        }
        if(!numberCode[2])
        {
            res.Item1.z = min.z;
            res.Item2.z = (max.z + min.z) / 2;
        }
        else
        {
            res.Item1.z = (max.z + min.z) / 2;
            res.Item2.z = max.z;
        }
        return res;
    }
    public static bool CheckConfinement(Vector3 Min, Vector3 Max, Vector3 pos)
    {
        bool xcontained = pos.x >= Min.x && pos.x <= Max.x;
        bool ycontained = pos.y >= Min.y && pos.y <= Max.y;
        bool zcontained = pos.z >= Min.z && pos.z <= Max.z;
        return xcontained && ycontained && zcontained;
    }
}
public class OctTreeNode
{
    public GameObject DebugCube;
    public List<Vertex> Vertices;
    public OctTreeNode[] Children = new OctTreeNode[8];
    public OctTreeNode Parent;
    public Vector3 MinEdge;
    public Vector3 MaxEdge;
    public Vector3 Center
    {
        get => (MaxEdge + MinEdge) / 2;
    }
    public void AddLevel()
    {
        if(Children[0] == null)
        {
            if (Vertices.Count > 2 && MaxEdge.x - MinEdge.x >= 1)
            {
                for (int i = 0; i < 8; i++)
                    Children[i] = new OctTreeNode(this, i);
                var vqueue = new Queue<Vertex>(Vertices);
                while(vqueue.Count > 0)
                    Balance(vqueue.Dequeue());
                Transform.Destroy(DebugCube);
            }
        }
        else
        {
            for(int i = 0; i < 8; i++)
                Children[i].AddLevel();
        }
    }
    public OctTreeNode(Vector3 minEdge, Vector3 maxEdge, GameObject cube)
    {
        MinEdge = minEdge;
        MaxEdge = maxEdge;
        Vertices = new List<Vertex>();
        //DebugCode
        DebugCube = Transform.Instantiate(cube);
        DebugCube.transform.position = Center;
        DebugCube.GetComponent<DebugCubeScript>().Node = this;
        var mesh = DebugCube.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        var size = Vector3.Distance(MinEdge * 1.2f, Center);
        for(int i = 0; i < verts.Length; i++)
            verts[i] = verts[i].normalized * size;
        mesh.vertices = verts;
        //EndOfDebugCode
    }
    public OctTreeNode(OctTreeNode parent, int number)
    {
        Parent = parent;
        Vertices = new List<Vertex>();
        var coords = OctTree.GetEdgesByNumber(number, Parent.MinEdge, Parent.MaxEdge);
        MinEdge = coords.min;
        MaxEdge = coords.max;
        //DebugCode
        DebugCube = Transform.Instantiate(parent.DebugCube);
        DebugCube.transform.position = Center;
        DebugCube.GetComponent<DebugCubeScript>().Node = this;
        var mesh = DebugCube.GetComponent<MeshFilter>().mesh;
        var verts = mesh.vertices;
        var size = Vector3.Distance(MinEdge, Center);
        for (int i = 0; i < verts.Length; i++)
            verts[i] = verts[i].normalized * size;
        mesh.vertices = verts;
        //EndOfDebugCode
    }
    public void DeployVertex(Vertex v)
    {
        Vertices.Add(v);
        Balance(v);
    }
    private void Balance(Vertex v)
    {
        if (Children[0] != null)
        {
            foreach (var child in Children)
            {
                if (OctTree.CheckConfinement(child.MinEdge, child.MaxEdge, v.LocalPosition))
                    child.DeployVertex(v);
            }
        }
    }
    //DebugMethod
    public void CheckVisibility()
    {
        if(Children[0] == null)
        {
            if (Vertices.Count == 0)
                DebugCube.SetActive(false);
            else
                DebugCube.SetActive(true);
        }
        else
        {
            foreach (var child in Children)
                child.CheckVisibility();
        }
    }
}
