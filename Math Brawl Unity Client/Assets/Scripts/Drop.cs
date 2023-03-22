using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drop : MonoBehaviour, IDropHandler
{
    public Image thisImage;
    public GameObject question;

    public Guid id;

    private void Awake()
    {
        id = Guid.NewGuid();
    }


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
                if (Connection.droppedItems.ContainsKey(id))
                {
                    Connection.droppedItems[id] = new KeyValuePair<int, LevelGenerator.Operation>(draggable.number, draggable.operation);
                    
                    FindObjectOfType<Connection>().CheckForAllDrops();
                }
                
                draggable.startPosition = transform.position;
                thisImage.enabled = false;
                question.SetActive(false);
            }
        }
    }
}
