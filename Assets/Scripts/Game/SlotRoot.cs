using System;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;

namespace PackageMerge
{
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

		private void ListenEvents()
		{
			GameCenterManager.Shared.ExtendMode.Register(value =>
			{
				foreach (var currentSlot in _currentSlots)
				{
					if (!currentSlot.isInitEnable)
					{
						currentSlot.slotBack.enabled = value;
					}
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
	}
}
