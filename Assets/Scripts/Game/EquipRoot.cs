using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PackageMerge
{
    [Serializable]
    public class EquipItem
    {
        public string Name;
        public string ItemId;
        
    }
    
	public partial class EquipRoot : ViewController, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		public Image IconImage;
        public EquipItem BindItem;
       
        public List<Vector2> patternPlaces;

        private bool _isDragging;
        private List<UISlot> _lastValidSlots;
        private UISlot _lastMarkCell;
        
		void Start()
		{
            
        }

        private EquipGridPlace FindEquipGridPlaceMinXMaxY()
        {
            var places = IconImage.GetComponentsInChildren<EquipGridPlace>();
            if (places == null || places.Length == 0)
            {
                throw new ArgumentException("数组不能为空");
            }
            return places
                .OrderBy(v => v.LocalPosition().x)
                .ThenByDescending(v => v.LocalPosition().y)
                .First();
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
                // find 左上角第一个place
                var gridPlace = FindEquipGridPlaceMinXMaxY();
                // var debugImage = gridPlace.GetComponent<Image>();
                // debugImage.color = new Color(1, 0.3f, 1, 0.5f);
                
                RectTransformUtility.ScreenPointToWorldPointInRectangle(gridPlace.transform as RectTransform, gridPlace.transform.position, null, out Vector3 equipPlacePt);
                
                UISlot markCell = null;
                var minMagn = 10000000.0f;
                var allSlotCells = SlotRoot.shared.transform.GetComponentsInChildren<UISlot>();
                foreach (var cell in allSlotCells)
                {
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(cell.transform as RectTransform, cell.transform.position, null, out Vector3 cellWorldPt);
                
                    var magn = (cellWorldPt - equipPlacePt).magnitude;
                    if (magn < minMagn)
                    {
                        // $"cell.transform.position = [{cellWorldPt.x}, {cellWorldPt.y}], equipPlacePt = [{equipPlacePt.x}, {equipPlacePt.y}]".LogInfo();
                        minMagn = magn;
                        markCell = cell;
                    }
                    cell.flagPlacing(false);
                    cell.placingItemId = null;
                }
                ("目前距离最近的是 " + minMagn).LogInfo();
                List<UISlot> validSlots = new List<UISlot>();
                
                if (markCell != null && markCell.isCanPlace() && minMagn < 60)
                {
                    markCell.flagPlacing(true);
                    validSlots.Add(markCell);
                    // 判断是否全部合法放置
                    
                    // try full pattern
                    foreach (var patternPlace in patternPlaces)
                    {
                        var cell = GetSlotByPosition(markCell.positionForGrid + patternPlace, true);
                        if (!cell || validSlots.Contains(cell)) continue;
                        cell.flagPlacing(true);
                        validSlots.Add(cell);
                    }

                    // 全部可以放置，转成绿色
                    if (validSlots.Count == patternPlaces.Count)
                    {
                        foreach (var slot in validSlots)
                        {
                            slot.slotBack.color = Color.green;
                        }
                    }
                    _lastValidSlots = validSlots;
                    _lastMarkCell = markCell;
                }
                
            }
        }

        private UISlot GetSlotByPosition(Vector2 pos, bool checkValid = false)
        {
            var allSlotCells = SlotRoot.shared.transform.GetComponentsInChildren<UISlot>();
            foreach (var cell in allSlotCells)
            {
                if (cell.positionForGrid != pos) continue;
                if (checkValid && !cell.isCanPlace())
                {
                    continue;
                }
                return cell;
            }

            return null;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            // find 左上角第一个place
            var gridPlace = FindEquipGridPlaceMinXMaxY();

            if (_lastValidSlots != null && _lastValidSlots.Count == patternPlaces.Count)
            {
                // 合法放置，定点
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    _lastMarkCell.transform as RectTransform, _lastMarkCell.transform.position,
                    null, out Vector3 cellWorldPt);
                // 对齐的是gridPlace
                // 宽度差和高度差
                var widthLeft = ((RectTransform)IconImage.transform).rect.width -
                                ((RectTransform)gridPlace.transform).rect.width;
                var heightLeft = ((RectTransform)IconImage.transform).rect.height -
                                 ((RectTransform)gridPlace.transform).rect.height;
                $"cellWorldPt = {cellWorldPt}, widthLeft = {widthLeft}, heightLeft = {heightLeft}"
                    .LogInfo();
                IconImage.Position(cellWorldPt);
                // 要计算gridPlace跟iconImage中心的偏移 这里临时处理下
                var fixLocalPos = IconImage.LocalPosition();
                fixLocalPos += new Vector3(widthLeft * 0.5f, -heightLeft * 0.5f, 0);
                IconImage.LocalPosition(fixLocalPos);

                // 保存格子放置的武器item
                foreach (var lastValidSlot in _lastValidSlots)
                {
                    lastValidSlot.placingItemId = BindItem.ItemId;
                }
            }
            else
            {
                // 还原现场
                IconImage.LocalIdentity();
                foreach (var slot in _lastValidSlots)
                {
                    slot.flagPlacing(false);
                }
            }
            _isDragging = false;
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
