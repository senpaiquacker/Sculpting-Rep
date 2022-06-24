using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CutTool : MonoBehaviour
{
    public Transform[] CutPoints;
    private OctTree tree;
    private void Start()
    {
        CutPoints = GetComponentsInChildren<Transform>();
        tree = FindObjectOfType<OctTree>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach(var point in CutPoints)
        {
            List<int> result = tree.FindBlock(point.position);
            if (result != null && result.Count != 0)
            {
                OctTreeNode node = tree.Root;
                foreach (var i in result)
                    node = node.Children[i];
                if (node.Vertices.Count == 0)
                    continue;
                var origin = node.Vertices[0].GlobalObject;
                for(int i = 0; i < node.Vertices.Count - 1; i++)
                {
                    for(int j = i + 1; j < node.Vertices.Count; j++)
                    {
                        var k = origin
                            .Edges
                            .FirstOrDefault(a => a
                            .Equals(new Edge(node.Vertices[i], node.Vertices[j])));
                        if(!(k is null))
                            k.Destroy();
                    }
                }
            }
        }
    }
}
