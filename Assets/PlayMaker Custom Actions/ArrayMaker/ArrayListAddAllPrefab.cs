// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=13784.0")]
	[Tooltip("Add all prefab from Resources folder to a PlayMaker Array List Proxy component")]
	public class ArrayListAddAllPrefab : ArrayListActions
	{

		[ActionSection("Resource Set up")]
		[RequiredField]
		[Tooltip("The path is relative to any Resources folder inside the Assets folder of your project, extensions must be omitted.")]
		public FsmString assetPath;


		[ActionSection("Array Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;
		
		[ActionSection("Output")]
		
		public FsmEvent successEvent;
		public FsmEvent failureEvent;

		private GameObject[] items;
		private int tempCount;

		public override void Reset()
		{
			gameObject = null;
			assetPath = null;
			reference = null;
			successEvent = null;
			failureEvent = null;
		}
		
		
		public override void OnEnter()
		{
			tempCount = 0;

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				loadRes();

			Fsm.Event(failureEvent);
			Finish();
		}

		public void loadRes()
		{
			items = Resources.LoadAll<GameObject>(assetPath.Value);
			tempCount = items.Length;

			if (tempCount > 0){
				AddToArrayList();
			}

			else	{
				Fsm.Event(failureEvent);
				Finish();
			}

		}

		public void AddToArrayList()
		{
			if (! isProxyValid() ) {
			Fsm.Event(failureEvent);
			Finish();
			}

			for (int x = 0; x < tempCount; x++){

				var _value = items[x];
				proxy.Add(_value,"GameObject");
			}


			Array.Clear(items,0,tempCount);

			Fsm.Event(successEvent);
			Finish();	
		}
		
		
	}
}
