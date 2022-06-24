using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Brush : Instrument
{
    public float Intensity;
    public float Radius;
    // Start is called before the first frame update
    public Action<Vector3> Apply
    {
        get
        {
            return (a) => 
            {
                var obj = FocusedObject;
                obj.UpdateMesh(UpdateMatrixOnMove
                (
                    obj, 
                    MathBlock.GetClosestVertexId(a, obj.VerticesGlobal),
                    direction
                ));
            };
        }
    }
    private Vector3[] UpdateMatrixOnMove(VertexMaterial obj, int id, Vector3 dir)
    {
        var sculpt = obj.Sculpt;
        sculpt[id] += Camera.main.transform.rotation * dir;
        return sculpt;
    }
    private Vector3 direction
    {
        get
        {
            return Camera.main.GetComponent<Shooter>()
                .ShootedRay.direction;
        }
    }
    public VertexMaterial FocusedObject
    {
        get
        {
            return Camera.main.GetComponent<EditInfo>().SelectedObject != null ?
                Camera.main.GetComponent<EditInfo>().SelectedObject :
                Camera.main.GetComponent<Shooter>().HitObject.GetComponent<VertexMaterial>();
        }
    }

}
