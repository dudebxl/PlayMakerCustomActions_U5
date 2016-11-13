// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Create color palette from texture for Array. Texture has to be read/write enabled.")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11929.0")]

	public class ArrayListSetColorFromTexture : ArrayListActions
	{
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		
		[ActionSection("Option")]
		[Tooltip("set alpha")]
		[HasFloatSlider(0, 1f)]
		public FsmBool disableAlpha;

		[ActionSection("Input")]
		public FsmTexture texture;


		[ActionSection("Event")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doneEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;


		private Color colorSet;
		private Color colorTemp;
		private Color[] pix;
		private List<Color> finalPix;
		private Texture2D targetmaskedTexture;
		private Color colorAtSource;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			failureEvent = null;
			doneEvent= null;
			texture = null;
			disableAlpha = false;
		}

		public override void OnEnter()
		{

			bool noError = false;

			if (texture.IsNone){
				Debug.LogWarning("<color=#6B8E23ff>No texture. Please review!</color>", this.Owner);
				noError = true;
			}

			targetmaskedTexture = null;
			targetmaskedTexture = (Texture2D) texture.Value;
		

			try
			{
				targetmaskedTexture.GetPixel(0, 0);
			}
			catch(UnityException)
			{

					Debug.LogWarning("<color=#6B8E23ff>Please enable read/write on texture  </color>"+"[" + texture.Value.name + "]", this.Owner);
					noError = true;		
			}
				
			if (noError == true){
				Fsm.Event(failureEvent);
				Finish();
			}

			else {
			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
			DoArraySetColor();
			}

			Fsm.Event(doneEvent);
			Finish();
		}

		
		public void DoArraySetColor()
		{

			if (! isProxyValid()) {
				Fsm.Event(failureEvent);			
				return;
			}

		
			pix = targetmaskedTexture.GetPixels(0);

			if (disableAlpha.Value == true){
				
			for (int x = 0; x < pix.Length; x++) {
				
				pix [x].a = 1f;
				
			}
			
		}

		
			finalPix = new List<Color>(); 
			finalPix.Add (pix[0]);


			for (int i = 0; i < pix.Length; i++) {
				if (disableAlpha.Value == false){
				 colorAtSource = ClearRGBIfNoAlpha (pix [i]);
				}

				else {

					colorAtSource = pix [i];
				}

				if (!ContainsColor (colorAtSource)) {
					finalPix.Add (colorAtSource);
				}
			}

			
			proxy.arrayList.Clear();
			
			for(int i=0; i<finalPix.Count; i++)
			{
				proxy.arrayList.Add(finalPix[i]);
				
			}

			finalPix = new List<Color>(); 
			pix = new Color[0];

			Fsm.Event(doneEvent);
		
		}

		static Color ClearRGBIfNoAlpha (Color colorToClear)
		{
			Color clearedColor = colorToClear;
			if (Mathf.Approximately (clearedColor.a, 0.0f)) {
				clearedColor = Color.clear;
			}
			return clearedColor;
		}

		public bool ContainsColor (Color colorToFind)
		{
			return IndexOf (colorToFind) >= 0;
		}

		public int IndexOf (Color colorToFind)
		{
			int index = -1;
			for (int i = 0; i < finalPix.Count; i++) {
				bool colorToFindIsZeroAlpha = Mathf.Approximately (colorToFind.a, 0.0f);
				bool currentColorIsZeroAlpha = Mathf.Approximately (finalPix [i].a, 0.0f);
				if ((colorToFindIsZeroAlpha && currentColorIsZeroAlpha) || finalPix [i] == colorToFind) {
					index = i;
					break;
				}
			}
			
			return index;
		}

		
		//
	}
}
