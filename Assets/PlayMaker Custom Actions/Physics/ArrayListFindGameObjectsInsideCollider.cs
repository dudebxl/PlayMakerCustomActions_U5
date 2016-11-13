// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store all active GameObjects from inside a collider with a specific tag and/or layer. Will filter first by tag then by layer. Tags and/or layers must be declared in the tag/layer manager before using them")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11754.0")]
	public class ArrayListFindGameObjectsInsideCollider : ArrayListActions
	{
	
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		public FsmObject colliderTarget;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
		
	
		[ActionSection("Filter")]
		[Tooltip("by tag")]
		public FsmString tag;
		[TitleAttribute("Incl Layer Filter")]
		[UIHint(UIHint.FsmBool)]
		[Tooltip("Also filter by layer?")]
		public FsmBool layerFilterOn;
		[UIHint(UIHint.Layer)]
		public int layer;

		private List<GameObject> tempList = new List<GameObject>();

		
		public override void Reset()
		{
			gameObject = null;
			reference = null;
			tag = null;
			layerFilterOn = false;
			layer = 0;
		}

		
		public override void OnEnter()
		{
			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				FindGOByTag();
			
			Finish();
		}

		
		public void FindGOByTag()
		{
			if (! isProxyValid()) 
				return;
			
			proxy.arrayList.Clear();
			
			GameObject[] objtag =  GameObject.FindGameObjectsWithTag (tag.Value);

			if (objtag.Length == 0) {
				Debug.LogWarning ("No object with tag:  "+tag.Value);
				return;
			}

	
			tempList.Clear();


			Collider temp =  colliderTarget.Value as Collider;
			Bounds colliderBounds = temp.bounds;

			if (layerFilterOn.Value == false)
			{
				
				for(int i = 0; i<objtag.Length;i++){

					Renderer tempRen = objtag[i].GetComponent<Renderer>();
					Bounds myObj = tempRen.bounds;

					bool insideCollider = colliderBounds.Intersects(myObj);

					if (insideCollider == true)
					{

						tempList.Add (objtag[i]);

					}
				}

			}

			if (layerFilterOn.Value == true)
			{
				
				for(int i = 0; i<objtag.Length;i++){
					
					Renderer tempRen = objtag[i].GetComponent<Renderer>();
					Bounds myObj = tempRen.bounds;
					
					bool insideCollider = colliderBounds.Intersects(myObj);
					
					if (insideCollider == true){
						
						if (objtag[i].gameObject.layer == layer)
						{
						
							tempList.Add (objtag[i]);

						}

					}
				}
				
			}

			proxy.arrayList.InsertRange(0,tempList);
				
		
		}
	}
}
