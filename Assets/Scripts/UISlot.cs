using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class UISlot : MonoBehaviour
{
    public Image slotBack;

    public Vector2 positionForGrid;
    public bool isInitEnable;

    public string placingItemId = "";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void commonInit()
    {
        ResetBackColor();
    }

    public void ResetBackColor()
    {
        if (isInitEnable)
        {
            ColorUtility.TryParseHtmlString("#E5CDB1", out var newCol);
            slotBack.color = newCol;
        }
        else
        {
            slotBack.color = Color.grey;
            slotBack.enabled = false;
        }
    }

    public bool isCanPlace(string willPlaceId)
    {
        return isInitEnable && (placingItemId == "" || willPlaceId == placingItemId);
    }

    public void flagPlacing(bool flag)
    {
        if (flag)
        {
            slotBack.color = Color.red;
        }
        else
        {
            if (isInitEnable)
            {
                ColorUtility.TryParseHtmlString("#E5CDB1", out var newCol);
                slotBack.color = newCol;
            }
            else
            {
                slotBack.color = Color.grey;
            }
        }
    }

    public bool isCanExtend(string itemId)
    {
        return (placingItemId == "" || itemId == placingItemId);
    }
}
