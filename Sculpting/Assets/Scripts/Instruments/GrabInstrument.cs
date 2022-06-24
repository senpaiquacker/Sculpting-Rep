using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabInstrument : ArmHolder
{
    public Transform SnapPoint;
    [SerializeField]
    private Vector3 LocalSnapPoint;
    [SerializeField]
    private GameObject SnappedObject;
    public Quaternion sObjRotation;
    public void SnapToArm(GameObject snappingObject)
    {
        SnappedObject = snappingObject;
        SnappedObject.transform.parent = SnapPoint.transform;
    }
    public void Unsnap()
    {
        SnappedObject = null;
        SnappedObject.transform.parent = null;
    }
    private void Start()
    {
        //debug code
        SnappedObject.transform.parent = transform;
        //end of debug
    }
    private void Update()
    {
        sObjRotation = SnappedObject.transform.rotation;
        //debug code
        var x = transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X");
        var y = transform.rotation.eulerAngles.z + Input.GetAxis("Mouse Y");
        transform.rotation = Quaternion.Euler(new Vector3(0, x, y));
        SnapPoint.rotation = transform.rotation;
        //debug code end
    }
}
