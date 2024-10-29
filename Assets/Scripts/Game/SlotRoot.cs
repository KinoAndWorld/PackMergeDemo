using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace PackageMerge
{

	public enum SlotSearchType
	{
		All,
		Enable,
		Disable
	}
	
	public partial class SlotRoot : ViewController
	{
		public int initColunm = 5;
		public int initRow = 7;

		public Vector2 initEnableYRange = new Vector2(2, 4);
		public Vector2 initEnableXRange = new Vector2(1, 3);

		private List<UISlot> _currentSlots = new List<UISlot>();
		 
		void Start()
		{
			ListenEvents();
			for (int i = 0; i < initRow; i++)
			{
				for (int j = 0; j < initColunm; j++)
				{
					var isInitEnable = i >= initEnableYRange.x && 
					                   i <= initEnableYRange.y && 
					                   j >= initEnableXRange.x &&
					                   j <= initEnableXRange.y; 
					SlotCell.InstantiateWithParent(this).Self(self =>
					{
						var slot = self.GetComponent<UISlot>();
						slot.positionForGrid = new Vector2(i, j);
						slot.isInitEnable = isInitEnable;
						slot.commonInit();
						_currentSlots.Add(slot);
					}).Show();
				}
			}
		}

		public UISlot GetSlotByPosition(Vector2 pos, SlotSearchType searchType)
		{
			var searchList = searchType switch
			{
				SlotSearchType.Enable => _currentSlots.FindAll(slot => slot.isInitEnable),
				SlotSearchType.Disable => _currentSlots.FindAll(slot => !slot.isInitEnable),
				_ => _currentSlots
			};

			return searchList.FirstOrDefault(e => e.positionForGrid == pos);
		}

		private void ListenEvents()
		{
			GameCenterManager.Shared.ExtendMode.Register(value =>
			{
				foreach (var currentSlot in _currentSlots.Where(currentSlot => !currentSlot.isInitEnable))
				{
					currentSlot.slotBack.enabled = value;
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
	}
}
