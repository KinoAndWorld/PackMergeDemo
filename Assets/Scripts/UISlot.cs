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

    public bool isCanPlace()
    {
        return isInitEnable;
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

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Equip"))
    //     {
    //         slotBack.color = Color.red;
    //     }
    //     
    // }
    //
    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Equip"))
    //     {
    //         slotBack.color = Color.white;
    //     }
    // }
}
