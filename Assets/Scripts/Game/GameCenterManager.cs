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
	}
}
