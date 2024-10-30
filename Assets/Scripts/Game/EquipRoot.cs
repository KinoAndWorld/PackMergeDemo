using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PackageMerge
{

    public enum EquipItemType
    {
        Weapon,
        Extend
    }
    
    [Serializable]
    public class EquipItem
    {
        public string Name;
        public string ItemId;
        public EquipItemType Type;
        public int Level = 1;
    }
    
	public partial class EquipRoot : ViewController, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		public Image IconImage;
        public EquipItem BindItem;
       
        public List<Vector2> patternPlaces;
        public float MatchMinDistance = 100.0f;
        
        private bool _isDragging;
        private Vector3 _originalPosition;
        private List<UISlot> _lastValidSlots;
        private UISlot _lastMarkCell;
        private int _lastNeighborCount;


        private void Start()
        {
            
            GameCenterManager.Shared.ExtendMode.RegisterWithInitValue(newValue =>
            {
                if (BindItem.Type == EquipItemType.Weapon)
                {
                    // this.gameObject.SetActive(!newValue);
                }
                else
                {
                    if (newValue) return;
                    if (!CheckValidWhenEndDrag()) return;
                    // 结束编辑 且目前有合法的放置
                    // 消耗扩展块 修改slot状态
                    _lastValidSlots.ForEach(e =>
                    {
                        e.PlacingItemId = "";
                        e.isInitEnable = true;
                        e.ResetBackColor();
                    });
                    _lastValidSlots = new List<UISlot>();
                    _lastMarkCell = null;
                    _lastNeighborCount = 0;
                    Destroy(gameObject);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
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
            if (_isDragging) return;
            _isDragging = true;
            if (BindItem.Type == EquipItemType.Extend)
            {
                // 开启扩展模式
                GameCenterManager.Shared.ExtendMode.Value = true;
            }

            _originalPosition = IconImage.LocalPosition();
            DragingItem(Input.mousePosition);
        }

        private void DragingItem(Vector3 newPos)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, newPos, null,
                    out Vector2 localPoint))
            {
                IconImage.LocalPosition(localPoint);

                if (BindItem.Type == EquipItemType.Weapon)
                {
                    DragItemForWeapon();
                }
                else
                {
                    DragItemForExtend();
                }
            }
        }

        /// <summary>
        /// 扩展拖拽处理
        /// </summary>
        private void DragItemForExtend()
        {
            var gridPlace = FindEquipGridPlaceMinXMaxY();
            RectTransformUtility.ScreenPointToWorldPointInRectangle(gridPlace.transform as RectTransform, gridPlace.transform.position, null, out Vector3 equipPlacePt);
            
            UISlot markCell = null;
            var minDistance = 10000000.0f;
            var allSlotCells = GameCenterManager.Shared.GetAllUnableUISlots();
            foreach (var cell in allSlotCells)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(cell.transform as RectTransform, cell.transform.position, null, out Vector3 cellWorldPt);
                
                var distance = (cellWorldPt - equipPlacePt).magnitude;
                if (distance < minDistance)
                {
                    // $"cell.transform.position = [{cellWorldPt.x}, {cellWorldPt.y}], equipPlacePt = [{equipPlacePt.x}, {equipPlacePt.y}]".LogInfo();
                    minDistance = distance;
                    markCell = cell;
                }
                cell.flagPlacing(false);
            }
            var validSlots = new List<UISlot>();
            int neighborCount = 0;
            if (markCell != null && markCell.isCanExtend(BindItem.ItemId) && minDistance < MatchMinDistance)
            {
                markCell.flagPlacing(true);
                validSlots.Add(markCell);
                if (GameCenterManager.Shared.ExtendSlotIsNeighbor(markCell))
                {
                    neighborCount++;
                }
                // 判断是否全部合法放置 try full pattern
                foreach (var patternPlace in patternPlaces)
                {
                    var cell = GetExtendSlotByPosition(markCell.positionForGrid + patternPlace);
                    if (!cell || validSlots.Contains(cell)) continue;
                    cell.flagPlacing(true);
                    validSlots.Add(cell);
                    if (GameCenterManager.Shared.ExtendSlotIsNeighbor(cell))
                    {
                        neighborCount++;
                    }
                }

                // 全部可以放置，转成绿色
                if (validSlots.Count == patternPlaces.Count && neighborCount > 0)
                {
                    foreach (var slot in validSlots)
                    {
                        slot.slotBack.color = Color.green;
                    }
                }
                _lastValidSlots = validSlots;
                _lastMarkCell = markCell;
                _lastNeighborCount = neighborCount;
            }
        }

        /// <summary>
        /// 武器拖拽处理
        /// </summary>
        private void DragItemForWeapon()
        {
            // 判断localPoint与所有slotCell的距离，找出最近的那个
            // find 左上角第一个place
            var gridPlace = FindEquipGridPlaceMinXMaxY();
            // var debugImage = gridPlace.GetComponent<Image>();
            // debugImage.color = new Color(1, 0.3f, 1, 0.5f);
            
            RectTransformUtility.ScreenPointToWorldPointInRectangle(gridPlace.transform as RectTransform, gridPlace.transform.position, null, out Vector3 equipPlacePt);
                
            UISlot markCell = null;
            var minDistance = 10000000.0f;
            var allSlotCells = GameCenterManager.Shared.GetAllUISlots();
            foreach (var cell in allSlotCells)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    cell.transform as RectTransform,
                    cell.transform.position,
                    null,
                    out Vector3 cellWorldPt);
                
                var distance = (cellWorldPt - equipPlacePt).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    markCell = cell;
                }
                cell.flagPlacing(false);
            }
            markCell.gameObject.GetComponent<Image>().color = Color.yellow;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                markCell.transform as RectTransform,
                markCell.transform.position,
                null,
                out Vector3 debugWorldPt);
            $"目前距离最近: {minDistance}, cell.transform.position = [{debugWorldPt.x}, {debugWorldPt.y}], equipPlacePt = [{equipPlacePt.x}, {equipPlacePt.y}]".LogInfo();
            
            List<UISlot> validSlots = new List<UISlot>();


            if (!markCell.isCanPlace(BindItem.ItemId))
            {
                $"markCell enable: {markCell.isInitEnable}, markCell placeId: {markCell.PlacingItemId}, curItemId: {BindItem.ItemId}".LogInfo();
            }
            
            if (markCell != null && markCell.isCanPlace(BindItem.ItemId) && minDistance < MatchMinDistance)
            {
                "我来噜".LogInfo();
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
                    "全部可以放置，转成绿色".LogInfo();
                    foreach (var slot in validSlots)
                    {
                        slot.slotBack.color = Color.green;
                    }
                }
                else
                {
                    $"patternPlaces({patternPlaces.Count})和validSlots({validSlots.Count})不匹配".LogInfo();
                }
                _lastValidSlots = validSlots;
                _lastMarkCell = markCell;
            }
        }

        private UISlot GetSlotByPosition(Vector2 pos, bool checkValid = false)
        {
            var allSlotCells = GameCenterManager.Shared.GetAllUISlots();
            foreach (var cell in allSlotCells)
            {
                if (cell.positionForGrid != pos) continue;
                if (checkValid && !cell.isCanPlace(BindItem.ItemId))
                {
                    continue;
                }
                return cell;
            }

            return null;
        }

        private UISlot GetExtendSlotByPosition(Vector2 pos)
        {
            var allSlotCells = GameCenterManager.Shared.GetAllUnableUISlots();
            return allSlotCells.FirstOrDefault(cell => cell.positionForGrid == pos);
        }

        private bool CheckValidWhenEndDrag()
        {
            if (BindItem.Type == EquipItemType.Extend)
            {
                return _lastValidSlots != null && _lastValidSlots.Count == patternPlaces.Count &&
                       _lastNeighborCount > 0;
            }
            else
            {
                return _lastValidSlots != null && _lastValidSlots.Count == patternPlaces.Count;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            // 落点在背包区域，就是放回背包
            if (PointInEquipRoot(eventData.position))
            {
                "放回背包".LogInfo();
                // 还原初始位置
                IconImage.LocalIdentity();
                if (_lastValidSlots != null) {
                    foreach (var slot in _lastValidSlots)
                    {
                        slot.flagPlacing(false);
                        slot.PlacingItemId = "";
                    }
                }
                // 标记PlacingItemId
                GameCenterManager.Shared.GetAllUISlots()
                    .Where(e => e.PlacingItemId == BindItem.ItemId)
                    .ForEach(e => e.PlacingItemId = "");
                
                // 重启布局
                var layout = gameObject.GetComponent<LayoutElement>();
                if (layout)
                {
                    layout.ignoreLayout = false;
                }
                
                _isDragging = false;
                return;
            }
            
            
            // find 左上角第一个place
            var gridPlace = FindEquipGridPlaceMinXMaxY();
            if (CheckValidWhenEndDrag())
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
                // $"cellWorldPt = {cellWorldPt}, widthLeft = {widthLeft}, heightLeft = {heightLeft}"
                //     .LogInfo();
                IconImage.Position(cellWorldPt);
                // 要计算gridPlace跟iconImage中心的偏移 这里临时处理下
                var fixLocalPos = IconImage.LocalPosition();
                fixLocalPos += new Vector3(widthLeft * 0.5f, -heightLeft * 0.5f, 0);
                IconImage.LocalPosition(fixLocalPos);

                // 保存格子放置的武器item
                foreach (var slot in GameCenterManager.Shared.GetAllUISlots())
                {
                    if (_lastValidSlots.Contains(slot))
                    {
                        slot.PlacingItemId = BindItem.ItemId;
                    }
                    else
                    {
                        if (slot.PlacingItemId == BindItem.ItemId)
                        {
                            slot.PlacingItemId = "";
                        }
                    }
                }

                var layout = gameObject.GetComponent<LayoutElement>();
                if (layout)
                {
                    layout.ignoreLayout = true;
                }
            }
            else
            {
                // 还原初始位置
                IconImage.LocalPosition(_originalPosition);
                if (_lastValidSlots != null) {
                    foreach (var slot in _lastValidSlots)
                    {
                        slot.flagPlacing(false);
                        slot.PlacingItemId = "";
                    }
                }
                _lastMarkCell.flagPlacing(false);
                _lastMarkCell.PlacingItemId = "";
            }
            _isDragging = false;
        }

        bool PointInEquipRoot(Vector2 pos)
        {
            var equipRootRect = GameCenterManager.Shared.DeckRoot;
            $"pos = {pos}, equipRootRect = {equipRootRect.ToString()}".LogInfo();
            return RectTransformUtility.RectangleContainsScreenPoint(equipRootRect, pos, null);
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
