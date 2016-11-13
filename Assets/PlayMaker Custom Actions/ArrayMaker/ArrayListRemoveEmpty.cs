// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Remove an empty (null) element of a PlayMaker Array List Proxy component")]
	public class ArrayListRemoveEmpty : ArrayListActions
	{
		
		[ActionSection("Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		private PlayMakerArrayListProxy proxy;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
		}
		
		
		public override void OnEnter()
		{
			proxy = GetArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value,true);
				
			if (proxy != null){
			doArrayListRemoveEmpty();
			}

			else {
				Debug.LogWarning("<b>[ArrayListRemoveEmpty]</b><color=#FF9900ff> Maybe wrong name - Please review!</color>", this.Owner);
			}

			
			Finish();
		}
		
		
		public void doArrayListRemoveEmpty()
		{

			int c = proxy.arrayList.Count;

			for (int i = 0;i<c;i++){
			
			object element = null;
			
			try{
				element = proxy.arrayList[i];
			}catch(System.Exception){
				Debug.LogWarning("<b>[ArrayListRemoveEmpty]</b><color=#FF9900ff> Please review!</color>", this.Owner);
				return;
			}

				if (element == null || element.Equals(null)){
					proxy.arrayList.RemoveAt(i);
					c = c-1;
					i = i-1;

					if (c == 0 && element != null){
						return;
					}
 
				}
			}

			return;
		}
		
		
	}
}
