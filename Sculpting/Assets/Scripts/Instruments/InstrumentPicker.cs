using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class InstrumentPicker : MonoBehaviour
{
    public Shooter instrumentSlot;
    public List<Instrument> Instruments;
    void Start()
    {
        AssignButtons();
        Instruments = new List<Instrument>();
        Instruments.Add(new Select());
        Instruments.Add(new Brush());
        Instruments.Add(new TriangleCut());
    }
    private void AssignButtons()
    {
        var buttons = GetComponentsInChildren<Button>();
        foreach(var btn in buttons)
        {
            switch(btn.GetComponentInChildren<Text>().text)
            {
                case "Select":
                    btn
                        .onClick
                        .AddListener(
                        () => 
                        { 
                            instrumentSlot.CurrentInstrument = Instruments
                            .First(a => a is Select); 
                        });
                    break;
                case "Brush":
                    btn
                        .onClick
                        .AddListener(
                        () =>
                        {
                            instrumentSlot.CurrentInstrument = Instruments
                            .First(a => a is Brush);
                        });
                    break;
                case "Cut":
                    btn.onClick
                        .AddListener(
                        () =>
                        {
                            instrumentSlot.CurrentInstrument = Instruments.First(a => a is TriangleCut);
                        });
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
