// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Create random colors for Array")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11927.0")]

	public class ArrayListSetColorRandom : ArrayListActions
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

		[ActionSection("Option")]
		public colortype colorType;
		public enum colortype  {
			
			RandomColorSimple,
			RandomRange,
			RandomRange2,
			
		};


		
		[ActionSection("Random Color Basic Option")]
		[Tooltip("Fill the array with basic random alpha")]
		public FsmBool includeAlpha;

		[ActionSection("Random Color General Option")]
		[Tooltip("Use the base color for random range")]
		public FsmBool usebaseColor;

		[ActionSection("Random Color Range Option")]
		[Tooltip("Mix base color + mix color 1 & 2")]
		public FsmBool useMixColor;
		public FsmBool useRandomOffset;
		[Tooltip("Offset color by pos or neg number")]
		[HasFloatSlider(-2, 2)]
		public FsmFloat offset;

		[ActionSection("Random Color Range2 Option")]
		[Tooltip("Offset color by pos or neg number")]
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat min;
		[Tooltip("Offset color by pos or neg number")]
		[HasFloatSlider(-0.5f, 0.5f)]
		public FsmFloat max;

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
		private Color newColor;

	

		public override void Reset()
		{
			gameObject = null;
			length = 1;
			reference = null;
			failureEvent = null;
			doneEvent= null;
			useRandomOffset =false;
			colorType = colortype.RandomColorSimple;
			useMixColor = false;
			usebaseColor = false;
			colorWeight1 = 1;
			colorWeight2 = 1;
			colorWeight3 =1;
			offset= 0.1f;
			min = 0f;
			max = 0f;
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



//<-- Random color
			if (colorType == colortype.RandomColorSimple ){

				for(int i=0; i<length.Value; i++)
				{
				

					colorSet = RandomColors();
					colorArray[i] = colorSet;

				}

			}
//--> Random color

			
//<-- Random range
			if (colorType == colortype.RandomRange ){

			
				List<Color> listColor = RandomRange();

				for(int i=0; i<length.Value; i++)
				{
			
					listColor[i] = Normalized(listColor[i]);
					colorArray[i] = listColor[i];

				}
				
			}
//--> Random range


//<-- Random range2
			if (colorType == colortype.RandomRange2 ){
				
				
				List<Color> listColor = RandomWalk();
				
				for(int i=0; i<length.Value; i++)
				{
					
					listColor[i] = Normalized(listColor[i]);
					colorArray[i] = listColor[i];
					
				}
				
			}
//--> Random range2
		

			proxy.arrayList.Clear();
			
			for(int i=0; i<length.Value; i++)
			{
				proxy.arrayList.Add(colorArray[i]);
				
			}

			Fsm.Event(doneEvent);
		
		}


		Color RandomColors(){

			colorTemp = new Color(UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f));

			
			if (includeAlpha.Value == true)
				colorTemp.a = UnityEngine.Random.Range(0.0f,1.0f);

			return colorTemp;
		}

		List<Color> RandomRange(){

			List<Color> colors = new List<Color>();
			newColor = new Color (0,0,0,1);

			if (useRandomOffset.Value == true)
				offset.Value = UnityEngine.Random.Range(0.0f,0.99f);

			if (usebaseColor.Value == true & useMixColor.Value == false){
				colorSet = baseColor.Value;
			}

			if (usebaseColor.Value == false & useMixColor.Value == false){
				colorSet = new Color(UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f));
			}

			else if (useMixColor.Value == true){
				colorSet = RandomMix(baseColor.Value, mixColor1.Value, mixColor2.Value);
			}

			for (int i = 0; i < length.Value; i++)
			{

				newColor.r = colorSet.r + UnityEngine.Random.Range(0.0f,1.0f) * offset.Value - offset.Value;
				newColor.g = colorSet.g + UnityEngine.Random.Range(0.0f,1.0f) * offset.Value - offset.Value;
				newColor.b = colorSet.b + UnityEngine.Random.Range(0.0f,1.0f) * offset.Value - offset.Value;
	
				if (includeAlpha.Value == true)
					newColor.a = UnityEngine.Random.Range(0.0f,1.0f);

		
				colors.Add(newColor);


			}

			
			return colors;
		}

		Color RandomMix(Color color1, Color color2, Color color3)
		{
			int randomIndex = UnityEngine.Random.Range(0,4);
			float mixRatio1 = (randomIndex == 0) ? UnityEngine.Random.Range(0.0f,1.0f) * colorWeight1.Value : UnityEngine.Random.Range(0.0f,1.0f);
			float mixRatio2 = (randomIndex == 1) ? UnityEngine.Random.Range(0.0f,1.0f) * colorWeight2.Value : UnityEngine.Random.Range(0.0f,1.0f);
			float mixRatio3 = (randomIndex == 2) ? UnityEngine.Random.Range(0.0f,1.0f) * colorWeight3.Value : UnityEngine.Random.Range(0.0f,1.0f);
			
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



		List<Color> RandomWalk()
		{
			List<Color> colors = new List<Color>();
			newColor = new Color (0,0,0,1);

			if (usebaseColor.Value == false){
				newColor = new Color(UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f), UnityEngine.Random.Range(0.0f,1.0f));
			}
			else  {
				newColor = baseColor.Value;
			}

			float range = max.Value - min.Value;

			for (int i = 0; i < length.Value; i++)
			{

				int rSign = UnityEngine.Random.Range(-1,2);
				if (rSign == 0)
					rSign = -1;
				int gSign = UnityEngine.Random.Range(-1,2);
				if (gSign == 0)
					gSign = -1;
				int bSign = UnityEngine.Random.Range(-1,2);
				if (bSign == 0)
					bSign = -1;


				float mixRatio1 = newColor.r + rSign * (min.Value + UnityEngine.Random.Range(0.0f,0.98f) * range);
				float mixRatio2 = newColor.g + gSign * (min.Value + UnityEngine.Random.Range(0.0f,0.98f) * range);
				float mixRatio3 = newColor.b + bSign * (min.Value + UnityEngine.Random.Range(0.0f,0.98f) * range);
				
				newColor = new Color (mixRatio1, mixRatio2, mixRatio3);

				if (includeAlpha.Value == true)
					newColor.a = UnityEngine.Random.Range(0.0f,1.0f);

				colors.Add(newColor);
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





//
	}
}
