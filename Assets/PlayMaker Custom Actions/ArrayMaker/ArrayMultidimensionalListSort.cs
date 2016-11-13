// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Multi array, Multi dimensional,2DArray

using UnityEngine;
using System;
using System.Collections;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Multidimensional sorts (2d sort) of two array of same length from the sequence of elements in a PlayMaker ArrayList Proxy component. Includes a shuffle option")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11918.0")]
	public class ArrayMultidimensionalListSort : ArrayListActions
	{
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString keyArrayName;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString itemsArrayName;

		[ActionSection("Sort Option")]
		[Tooltip("Shuffle and can be used with 'useRange' to optional start Index for the shuffling.")]
		public FsmBool shuffle;
		public FsmBool useRange;
		[Tooltip("The starting index of the range to sort.")]
		public FsmInt startIndex;
		[Tooltip("The number of elements in the range to sort (the ending index of the range to sort).")]
		[TitleAttribute("Range")]
		public FsmInt length;
		[Tooltip("When On - sort by item and not by key")]
		[TitleAttribute("Sort by item")]
		public FsmBool flipOn;
		
		[ActionSection("Event")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doneEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;

		private int l1;
		private int l2;
		private int lengthTemp;
		private object element;

		public override void Reset()
		{
			gameObject = null;
			keyArrayName = null;
			itemsArrayName = null;
			startIndex = 0;
			length = 0;
			useRange= false;
			flipOn= false;
			failureEvent = null;
			shuffle = false;
			doneEvent= null;
		}

		
		public override void OnEnter()
		{
			if (shuffle.Value == true){
				
				DoArrayListShuffle();
				Fsm.Event(doneEvent);
				Finish();
			}

			else {
			DoArrayListSort();
			}

			Fsm.Event(doneEvent);
			Finish();
		}

		
		public void DoArrayListSort()
		{
			setProxyKey();

			if (! isProxyValid()) {
				Fsm.Event(failureEvent);			
				return;
			}


			l1 = proxy.arrayList.Count;

			if (useRange.Value == true){

				var testend = Mathf.Min(proxy.arrayList.Count-1,startIndex.Value + length.Value);
				if (testend > l1);
				{
				Debug.LogWarning("<color=#6B8E23ff>Your range is above array max length. Please review!</color>", this.Owner);
				Fsm.Event(failureEvent);			
				return;
				}
			}


			var key = new object [l1];

			for(int i=0; i<l1; i++)
			{
				element = null;
				
				try{
					element = proxy.arrayList[i];
				}catch(System.Exception e){
					Debug.Log(e.Message);
					string fullLabel = Fsm.GetFullFsmLabel(this.Fsm);
					string name = Fsm.ActiveStateName;
					Debug.Log("Fsm Path= "+fullLabel+" : "+name);
					Fsm.Event(failureEvent);
					return;
				}

				key[i] = element;
			}

			setProxyItem();
		
			if (! isProxyValid()) {
				Fsm.Event(failureEvent);			
				return;
			}

			l2 = proxy.arrayList.Count;
		
			
			var item = new object [l2];

			if (l1 != l2){
				
				Debug.LogWarning("<color=#6B8E23ff>The arrays are NOT equal length. They must be the same length. Please review!</color>", this.Owner);
				Fsm.Event(failureEvent);
				Finish ();
			};


			for(int i=0; i<l2; i++)
			{
				element = null;
				
				try{
					element = proxy.arrayList[i];
				}catch(System.Exception e){
					Debug.Log(e.Message);
					string fullLabel = Fsm.GetFullFsmLabel(this.Fsm);
					string name = Fsm.ActiveStateName;
					Debug.Log("Fsm Path= "+fullLabel+" : "+name);
					Fsm.Event(failureEvent);
					return;
				}
				
				item[i] = element;
			}



// ---> Sort if's

			if (flipOn.Value == false & useRange.Value == false){
				Array.Sort( key, item);
			}

			if (flipOn.Value == true & useRange.Value == false) {

				Array.Sort( item, key);

			}

			if (flipOn.Value == false & useRange.Value == true){
				Array.Sort( key, item,startIndex.Value, length.Value );
			}

			if (flipOn.Value == true & useRange.Value == true){
				Array.Sort( item, key,startIndex.Value, length.Value );
				}
// <---


			lengthTemp = l1;
			setProxyKey();
	
			proxy.arrayList.Clear();
			
			for(int i=0; i<lengthTemp; i++)
			{
				proxy.arrayList.Add(key[i]);
				
			}

			lengthTemp = l2;
			setProxyItem();

			proxy.arrayList.Clear();
			
			for(int i=0; i<lengthTemp; i++)
			{
				proxy.arrayList.Add(item[i]);
				
			}


		}


		public void setProxyKey(){

			SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),keyArrayName.Value);
			return;
		}

		public void setProxyItem(){

			SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),itemsArrayName.Value);
			return;
		}

		public void DoArrayListShuffle()
		{
			setProxyItem();
			int start = 0;
			int end = proxy.arrayList.Count-1;
			
			if (startIndex.Value>0)
			{
				start = Mathf.Min(startIndex.Value,end);
			}
			
			if (length.Value>0)
			{
				end = Mathf.Min(proxy.arrayList.Count-1,start + length.Value);
				
			}

			for (int i = end; i > start; i--)
			{
				setProxyItem();
				int swapWithPos = UnityEngine.Random.Range(start,i + 1);

				System.Object tmp = proxy.arrayList[i];
				proxy.arrayList[i] = proxy.arrayList[swapWithPos];
				proxy.arrayList[swapWithPos] = tmp;

				setProxyKey();
				tmp = proxy.arrayList[i];
				proxy.arrayList[i] = proxy.arrayList[swapWithPos];
				proxy.arrayList[swapWithPos] = tmp;
			}
	
			Fsm.Event(doneEvent);
		}
	}
}
