using System.Linq;
using UnityEngine;
using QFramework;

namespace PackageMerge
{
	public partial class GameCenterManager : ViewController
	{
		public static GameCenterManager Shared;
		
		// 扩展编辑模式
		public readonly BindableProperty<bool> ExtendMode = new BindableProperty<bool>(false);

		private void Awake()
		{
			Shared = this;
			
			ExtendMode.RegisterWithInitValue(newValue =>
			{
				$"update ExtendMode = {newValue}".LogInfo();
				ExitExtendButton.gameObject.SetActive(newValue);
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			ExitExtendButton.onClick.AddListener(() =>
			{
				ExtendMode.Value = false;
			});
		}
		
		[RuntimeInitializeOnLoadMethod]
		static void CommonInit()
		{
			Application.targetFrameRate = 60;
		}

		public UISlot[] GetAllUISlots()
		{
			return SlotRoot.transform.GetComponentsInChildren<UISlot>();
		}

		public UISlot[] GetAllUnableUISlots()
		{
			return SlotRoot.transform.GetComponentsInChildren<UISlot>()
				.Where(e => e.isInitEnable == false).ToArray();
		}

		public bool ExtendSlotIsNeighbor(UISlot slot)
		{
			var fourDirections = new []
			{
				new Vector2(-1, 0), // 上
				new Vector2(1, 0), // 下
				new Vector2(0, -1), // 左
				new Vector2(0, 1) // 右
			};
			foreach (var direction in fourDirections)
			{
				var detectPos = slot.positionForGrid + direction;
				var tryFindSlot = SlotRoot.GetSlotByPosition(detectPos, SlotSearchType.Enable);
				if (tryFindSlot)
				{
					return true;
				}
			}

			return false;
		}
	}
}
