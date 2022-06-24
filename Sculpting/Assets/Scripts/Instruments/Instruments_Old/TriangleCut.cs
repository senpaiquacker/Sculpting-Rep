using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCut : Instrument
{
    // Start is called before the first frame update
    public Action<Vector3> Apply
    {
        get
        {
            return (a) =>
            {
                var obj = FocusedObject ;
                /*var vertid = MathBlock.GetClosestVertexId(a, obj.Sculpt);
                var trio = MathBlock
                .FindTrianglePointIn
                    (MathBlock
                    .GetAllAdjastentTriangles(obj.VertexTrios, vertid),
                    obj.Sculpt, a);
                MathBlock.GetClosestProjection(trio, obj.Sculpt, a);*/
                var i = MathBlock.GetClosestVertexId(a, obj.VerticesGlobal);
                //var trio = MathBlock.FindTrianglePointIn(obj.VertexSystem, a);
                //Debug.Log(trio);
            };
        }
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
