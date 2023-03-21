using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IDropHandler
{
    public Image thisImage;
    public GameObject question;
    
    void Start()
    {
        thisImage = GetComponent<Image>();
        question = transform.GetChild(0).gameObject;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //if (number == eventData.pointerDrag.transform.GetComponent<Drag>().number)
        {
            Drag draggable = eventData.pointerDrag.GetComponent<Drag>();
            if (draggable != null)
            {
                draggable.startPosition = transform.position;
                thisImage.enabled = false;
                question.SetActive(false);
            }
        }
    }
}
