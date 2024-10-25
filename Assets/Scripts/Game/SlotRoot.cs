using System;
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
		
		void Start()
		{
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
						if (isInitEnable)
						{
							ColorUtility.TryParseHtmlString("#E5CDB1", out var newCol);
							self.color = newCol;
						}
						else
						{
							self.color = Color.grey;
						}
					}).Show();
				}
			}
		}
	}
}
