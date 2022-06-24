using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugCubeScript : MonoBehaviour
{
    public int[] VertIds;
    public OctTreeNode Node;
    public void Update()
    {
        if (Node != null)
            VertIds = Node.Vertices.Select(a => a.Id).ToArray();
    }
}
