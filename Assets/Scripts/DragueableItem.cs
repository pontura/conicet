﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragueableItem : MonoBehaviour
{
    public int id;
    public Image image;
    public states state;
    Vector3 originalPos;
    public List<DragueableItemDestination> destinations;
    Vector3  offset;
    public DragueableItemDestination itemDest;
    RandomRotation randomRotation;

    public enum states
    {
        IDLE,
        DRAGGING,
        NONE,
        DONE
    }
    public void SetDestiny(DragueableItemDestination itemDest)
    {
        destinations.Add(itemDest);
    }
    public void Init(int id, Sprite sprite)
    {
        randomRotation = GetComponent<RandomRotation>();
        this.id = id;
        this.image.sprite = sprite;
        originalPos = transform.position;
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(-15, 15));
    }
    public void StartDrag()
    {
        if (itemDest != null)
        {
            itemDest.Reset();
            
            itemDest = null;
        }
        offset = Input.mousePosition - transform.position;
        state = states.DRAGGING;
        randomRotation.enabled = false;
        transform.localEulerAngles = Vector3.zero;
    }
    public void EndDrag()
    {
        float distance = Mathf.Abs(Vector3.Distance(transform.position, originalPos));
        foreach (DragueableItemDestination idest in destinations)
        {
            float d = Mathf.Abs(Vector3.Distance(transform.position, idest.transform.position));
            if (d < distance && idest.state == DragueableItemDestination.states.IDLE)
            {   
                itemDest = idest;
                itemDest.dragueableItemID = id;
                distance = d;
            }
        }
        if (itemDest != null)
        {
            itemDest.SetDone();
            state = states.DONE;
        }
        else
        {
            state = states.NONE;
        }
    }
    void Update()
    {
        if (state == states.DRAGGING)
        {
            transform.position = Input.mousePosition - offset;
        }
        else if (state == states.DONE)
        {
            transform.position = Vector3.Lerp(transform.position, itemDest.transform.position, Time.deltaTime*10);
            if(Vector3.Distance(transform.position, itemDest.transform.position) <1)
            {
                state = states.IDLE;
                transform.position = itemDest.transform.position;
                transform.localEulerAngles = Vector3.zero;
            }
        }
        else if (state == states.NONE)
        {
            transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * 10);
            if (Vector3.Distance(transform.position, originalPos) < 1)
            {
                state = states.IDLE;
                transform.position = originalPos;
                randomRotation.enabled = true;
            }
        }       
    }
    public void Reset()
    {
        state = states.NONE;

        foreach (DragueableItemDestination idest in destinations)
            idest.state = DragueableItemDestination.states.IDLE;

        itemDest = null;
    }
}
