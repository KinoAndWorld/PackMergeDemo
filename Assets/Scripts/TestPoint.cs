using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class TestPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, transform.position, null, out Vector3 point);
        $"point = {point.x}, {point.y}".LogInfo();
    }
}
