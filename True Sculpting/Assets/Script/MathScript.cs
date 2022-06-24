using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathScript
{
    public static float GetSquareDistance(Vector3 pos1, Vector3 pos2)
    {
        return (pos1.x - pos2.x) * (pos1.x - pos2.x) +
               (pos1.y - pos2.y) * (pos1.y - pos2.y) +
               (pos1.z - pos2.z) * (pos1.z - pos2.z);
    }
    public static Vector3 TransformLocalToWorldPosition(Vector3 localPosition, Matrix4x4 l2w)
    {
        return l2w.MultiplyPoint(localPosition);
    }
}
