using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // Start is called before the first frame update
    public InstrumentPicker InstrumentAssembly;
    private void Start()
    {
        LoadInstruments();
    }
    private void LoadInstruments()
    {
        var result = new List<Instrument>();
        result.Add(new Brush());
        result.Add(new Select());
        result.Add(new TriangleCut());
        InstrumentAssembly.Instruments = result;
    }
}
