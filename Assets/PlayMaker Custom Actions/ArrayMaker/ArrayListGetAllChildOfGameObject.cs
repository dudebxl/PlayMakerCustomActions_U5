// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Get All Child Of GameObject

using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store all (all levels) childs of GameObject (active and/or inactive) from a parent.")]
	public class ArrayListGetAllChildOfGameObject : ArrayListActions
	{
	
		[ActionSection("Array Setup")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		public FsmString reference;

		[ActionSection("Setup")]
		[Tooltip("The parent gameObject")]
		[RequiredField]
		public FsmGameObject parent;
	
		[ActionSection("Option")]
		public FsmBool includeInactive;

		private GameObject go;
		private Transform[] child;
		private GameObject[] childObjects;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			parent = null;
			includeInactive = true;
		}

		
		public override void OnEnter()
		{
			go = parent.Value;

			child = go.GetComponentsInChildren<Transform>(includeInactive.Value);
			childObjects = new GameObject[child.Length];

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				FindGOByTag();
			
			Finish();
		}

		
		public void FindGOByTag()
		{
			if (! isProxyValid()) 
				return;
			
			proxy.arrayList.Clear();

			int value = 0;

			foreach(Transform trans in child) {

				value++;
				childObjects.SetValue (trans.gameObject, value - 1);
			
			}
		

			proxy.arrayList.InsertRange(0,childObjects);
				
		
			Array.Clear(child,0,child.Length);


			return;
		}
	}
}
