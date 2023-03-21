using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int number = 0;
    public LevelGenerator.Operation operation = LevelGenerator.Operation.NotSet;
    
    public Image thisImage;
    public Vector3 startPosition;
    
    public void Init()
    {
        thisImage = GetComponent<Image>();
        startPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        thisImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
        thisImage.raycastTarget = true;

        // TODO : Handle wrong drop here
        // bool correctDrop = false;
        // var hoveredList = eventData.hovered;
        // foreach (var hovered in hoveredList)
        // {
        //     if (hovered.GetComponent<Drop>() != null)
        //     {
        //         correctDrop = true;
        //     }
        // }
        // if (!correctDrop)
        // {
        //     transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        //     transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
        // }
    }
}
