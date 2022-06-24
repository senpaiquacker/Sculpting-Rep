using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    // Start is called before the first frame update
    public Ray ShootedRay { get; private set; }
    public Collider HitObject { get; private set; }
    public bool IsOnScreen { get; set; }
    public Instrument CurrentInstrument;
    void Start()
    {
        CurrentInstrument = new Brush();
        IsOnScreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && IsOnScreen)
        {
            RaycastHit hit;
            ShootedRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ShootedRay, out hit);
            HitObject = hit.collider;
            CurrentInstrument.Apply(hit.point);
        }
    }
    
}