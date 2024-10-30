using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UISlot : MonoBehaviour
{
    public Image slotBack;
    public Text placeLabel;
    
    public Vector2 positionForGrid;
    public bool isInitEnable;

    // 格式要做成name_xxx的形式，前面标记类型，后面加个唯一标识符
    private string placingItemId = "";

    public string PlacingItemId
    {
        get => placingItemId;
        set
        {
            placingItemId = value;
            placeLabel.text = placingItemId;
        }
    }

    // public bool ComparePlaceItem(string placeItemId)
    // {
    //     
    // }
    
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
