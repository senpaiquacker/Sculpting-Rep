using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoverBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Camera.main.GetComponent<Shooter>().IsOnScreen = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Camera.main.GetComponent<Shooter>().IsOnScreen = true;
    }
}
