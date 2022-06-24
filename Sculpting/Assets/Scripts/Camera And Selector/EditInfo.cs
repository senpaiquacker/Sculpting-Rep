using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditInfo : MonoBehaviour
{
    [SerializeField]
    private VertexMaterial s_obj;
    public VertexMaterial SelectedObject
    {
        get
        {
            return s_obj;
        }
        set
        {
            if(s_obj != null)
                s_obj.UnHighlight();
            s_obj = value;
            if(s_obj != null)
                s_obj.Highlight();
        }
    }
}
