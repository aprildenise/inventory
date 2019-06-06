using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IMoveHandler, ICancelHandler
{

    RectTransform rt;
    ItemUI itemUI;


    private void Awake()
    {
        Transform parent = gameObject.transform.parent;
        rt = parent.GetComponent<RectTransform>();
        itemUI = parent.gameObject.GetComponent<ItemUI>();
    }


    public void OnMove(AxisEventData eventData)
    {
        //Debug.Log("moving ui....:" + eventData.moveVector);

        itemUI.MoveUI(eventData.moveVector);


        // Move the UI based on the given movement
    }

    public void OnCancel(BaseEventData eventData)
    {
        itemUI.ResetPosition();
    }
}
