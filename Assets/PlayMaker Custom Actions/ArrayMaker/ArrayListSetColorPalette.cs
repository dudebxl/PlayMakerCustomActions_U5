// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Create color palette for Array")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11927.0")]

	public class ArrayListSetColorPalette : ArrayListActions
	{
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;


		[ActionSection("Array Option")]
		[Tooltip("The number of colors in the range to build (array length).")]
		[TitleAttribute("Array Size")]
		public FsmInt length;

		
		[ActionSection("Basic Option")]
		[Tooltip("set alpha")]
		[HasFloatSlider(0, 1f)]
		public FsmFloat setAlpha;

		[ActionSection("Palette Color Option")]
		[Tooltip("Mix base color + mix color 1 & 2")]
		public FsmBool useMixColor;
		[Tooltip("Offset color by pos or neg number")]
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat offsetGeneral;
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat offsetR;
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat offsetG;
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat offsetB;


		[ActionSection("Base Color setup")]
		public FsmColor baseColor;

		[ActionSection("Mix Color setup")]
		public FsmColor mixColor1;
		public FsmColor mixColor2;
		[Tooltip("Weight system.")]
		[HasFloatSlider(0, 1)]
		public FsmFloat colorWeight1;
		[HasFloatSlider(0, 1)]
		public FsmFloat colorWeight2;
		[HasFloatSlider(0, 1)]
		public FsmFloat colorWeight3;


		[ActionSection("Event")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doneEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;


		private Color colorSet;
		private Color colorTemp;


		public override void Reset()
		{
			gameObject = null;
			length = 1;
			reference = null;
			failureEvent = null;
			doneEvent= null;
			useMixColor = false;
			colorWeight1 = 1;
			colorWeight2 = 1;
			colorWeight3 =1;
			offsetGeneral= 0.1f;
			setAlpha = 1f;
			baseColor = null;
			mixColor1 = null;
			mixColor2 = null;
		}

		public override void OnEnter()
		{
			if (length.Value <= 0 )
				Debug.LogWarning("<color=#6B8E23ff>Length must be above 0. Please review!</color>", this.Owner);

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
			DoArraySetColor();

			Fsm.Event(doneEvent);
			Finish();
		}

		
		public void DoArraySetColor()
		{

			if (! isProxyValid()) {
				Fsm.Event(failureEvent);			
				return;
			}

			Color[] colorArray = new Color[length.Value];


			
//<-- Random palette

			
				List<Color> listColor = RandomRange();

				for(int i=0; i<length.Value; i++)
				{
			
					listColor[i] = Normalized(listColor[i]);
					colorArray[i] = listColor[i];

				}
				

//--> Random palette

	
		

			proxy.arrayList.Clear();
			
			for(int i=0; i<length.Value; i++)
			{
				proxy.arrayList.Add(colorArray[i]);
				
			}

			Fsm.Event(doneEvent);
		
		}



		List<Color> RandomRange(){

			List<Color> colors = new List<Color>();
			Color newColor = new Color (0,0,0,1);


			colorSet = baseColor.Value;


			if (useMixColor.Value == true){
				colorSet = RandomMix(baseColor.Value, mixColor1.Value, mixColor2.Value);
			}

			colorSet.a = setAlpha.Value;
			colors.Add(colorSet);


			for (int i = 1; i < length.Value; i++)
			{

		
				colorSet.r = colorSet.r + (offsetR.Value * offsetGeneral.Value);
				colorSet.g = colorSet.g + (offsetG.Value * offsetGeneral.Value);
				colorSet.b = colorSet.b + (offsetB.Value * offsetGeneral.Value);
	


				colorSet.a = setAlpha.Value;

		
				colors.Add(colorSet);


			}

			
			return colors;
		}



		Color Normalized(Color tempColor){

		tempColor.r = Mathf.Clamp(tempColor.r, 0, 1);
		tempColor.b = Mathf.Clamp(tempColor.b, 0, 1);
		tempColor.g = Mathf.Clamp(tempColor.g, 0, 1);
		float alpha = (float) tempColor.a;
		tempColor.a = alpha;

			return tempColor;
		}

		Color RandomMix(Color color1, Color color2, Color color3)
		{

			float mixRatio1 = offsetGeneral.Value * colorWeight1.Value;
			float mixRatio2 = offsetGeneral.Value * colorWeight2.Value;
			float mixRatio3 = offsetGeneral.Value * colorWeight3.Value;
			
			float sum = mixRatio1 + mixRatio2 + mixRatio3;
			
			mixRatio1 /= sum;
			mixRatio2 /= sum;
			mixRatio3 /= sum;
			
			colorTemp.r = mixRatio1 * color1.r + mixRatio2 * color2.r + mixRatio3 * color3.r;
			colorTemp.g = mixRatio1 * color1.g + mixRatio2 * color2.g + mixRatio3 * color3.g;
			colorTemp.b = mixRatio1 * color1.b + mixRatio2 * color2.b + mixRatio3 * color3.b;
			colorTemp.a = 1;
			
			return colorTemp;
		}


//
	}
}
