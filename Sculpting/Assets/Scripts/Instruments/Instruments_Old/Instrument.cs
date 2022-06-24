using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Instrument
{
    Action<Vector3> Apply { get; }
    VertexMaterial FocusedObject { get; }
}
