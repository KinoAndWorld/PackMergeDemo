using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PackageMerge
{
	public partial class EquipRoot : ViewController, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		
		private bool _isDragging = false;
		public Image IconImage;
		
		void Start()
		{
			// Code Here
		}
		
		public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging == false)
            {
                _isDragging = true;

                var renderer = IconImage.transform.parent.gameObject.GetComponent<SpriteRenderer>();
                if (renderer)
                {
                    renderer.sortingOrder = 10;
                }
                DragingItem(Input.mousePosition);
            }
        }

        private void DragingItem(Vector3 newPos)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, newPos, null,
                    out Vector2 localPoint))
            {
                IconImage.LocalPosition(localPoint);
                
                // 判断localPoint与所有slotCell的距离，找出最近的那个
                var allSlotCells = SlotRoot.shared.transform.GetComponentsInChildren<UISlot>();
                // 中点距离
                List<UISlot> markCells = new List<UISlot>();
                
                // RectTransformUtility.ScreenPointToWorldPointInRectangle(IconImage.transform as RectTransform, IconImage.transform.position, null, out Vector3 iconWorldPt);
                var gridPlaces = IconImage.GetComponentsInChildren<EquipGridPlace>();
                List<Vector3> equipPlacePts = gridPlaces.Select(e =>
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(e.transform as RectTransform, e.transform.position, null, out Vector3 outPos);
                    return outPos;
                }).ToList();
                
                foreach (var equipPlacePt in equipPlacePts)
                {
                    UISlot markCell = null;
                    var minMagn = 10000000.0f;
                    foreach (var cell in allSlotCells)
                    {
                        if (markCells.Contains(cell))
                        {
                            continue;
                        }
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(cell.transform as RectTransform, cell.transform.position, null, out Vector3 cellWorldPt);
                    
                        var magn = (cellWorldPt - equipPlacePt).magnitude;
                        if (magn < minMagn)
                        {
                            // $"cell.transform.position = [{cellWorldPt.x}, {cellWorldPt.y}], equipPlacePt = [{equipPlacePt.x}, {equipPlacePt.y}]".LogInfo();
                            minMagn = magn;
                            markCell = cell;
                        }
                        cell.flagPlacing(false);
                    }
                    ("目前距离最近的是 " + minMagn).LogInfo();
                    if (markCell != null && minMagn < 150)
                    {
                        markCell.flagPlacing(true);
                        markCells.Add(markCell);
                    }
                }
                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                // // 检测鼠标位置
                // var uiSlots = transform.parent.GetComponentsInChildren<UISlot>();
                // var needThrow = true;
                // UISlot targetSlot = null;
                // foreach (var uiSlot in uiSlots)
                // {
                //     var rectTransform = uiSlot.transform as RectTransform;
                //     if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position))
                //     {
                //         needThrow = false;
                //         targetSlot = uiSlot;
                //         break;
                //     }
                // }
                //
                // if (needThrow)
                // {
                //     slotData.Item = null;
                //     slotData.Count = 0;
                //     itemNeedRefreshEvent?.Trigger();
                // }
                // else
                // {
                //     if (targetSlot != null)
                //     {
                //         // 交换背包物品
                //         (targetSlot.slotData.Item, slotData.Item) = (slotData.Item, targetSlot.slotData.Item);
                //         (targetSlot.slotData.Count, slotData.Count) = (slotData.Count, targetSlot.slotData.Count);
                //         itemNeedRefreshEvent?.Trigger();
                //     }
                // }
                //
                // var renderer = IconImage.GetComponent<SpriteRenderer>();
                // if (renderer)
                // {
                //     renderer.sortingOrder = 0;
                // }
                // _isDragging = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                DragingItem(eventData.position);
            }
        }
	}
}
