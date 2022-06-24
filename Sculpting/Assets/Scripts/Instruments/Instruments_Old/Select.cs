using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : Instrument
{
    public Action<Vector3> Apply
    {
        get
        {
            return (a) =>
            {
                    Camera.main.GetComponent<EditInfo>()
                .SelectedObject = FocusedObject != null ? FocusedObject : null;
            };
        }
    }
    public VertexMaterial FocusedObject
    {
        get
        {
            return Camera.main.GetComponent<Shooter>().HitObject != null ? 
            Camera.main.GetComponent<Shooter>().HitObject.GetComponent<VertexMaterial>() : null;
        }
    }
}
