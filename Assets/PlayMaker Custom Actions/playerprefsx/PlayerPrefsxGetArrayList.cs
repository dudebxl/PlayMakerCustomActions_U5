// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Source http://hutonggames.com/playmakerforum/index.php?topic=10072.0

// Requires: ArrayMaker + PlayerPrefs X


using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/Array PlayerPrefs X")]
	[Tooltip("Get PlayerPrefs X Array and set into Array")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=10072.0")]
	public class PlayerPrefsxGetArrayList: ArrayListActions
	{
		[ActionSection("Array Setup")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		public VariableType variableType;

		[UIHint(UIHint.FsmBool)]
		[Tooltip("Prefill array on enter ** slower but safer, will reset values - delete any data in array for fresh start, will not work if startIndex/endIndex are >0")]
		public FsmBool arrayAutoPrefill;

		[ActionSection("Index Setup")]
		[Tooltip("From where to start iteration, leave to 0 to start from the beginning")]
		public FsmInt startIndex;
		
		[Tooltip("When to end iteration (incl in save), leave to 0 to iterate until the end")]
		public FsmInt endIndex;

		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("When save is done bool flip to True")]
		public FsmBool extractDone;

		[UIHint(UIHint.FsmInt)]
		[Tooltip("Get prefill count")]
		public FsmInt arrayPrefillCount;

		[ActionSection("Events")]

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when saved is done")]
		public FsmEvent finishedEvent;
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails (index is out of range exception or something went wrong)")]
		public FsmEvent failureEvent;
	
		private int nextItemIndex = -1;
		private string key;
		private string fullLabel;
		private string name;
		private int c;


		PlayMakerArrayListProxy addedComponent;

		public override void Reset()
		{
			startIndex = null;
			endIndex = null;
			gameObject = null;
			fullLabel = null;
			failureEvent = null;
			finishedEvent = null;
			name = null;
			extractDone = false;
			arrayPrefillCount = 0;

			key= null;
			c = 0;

		}
		
		
		
		public override void OnEnter()
		{

				if ( ! SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				{
					Fsm.Event(failureEvent);
					
					Finish();
				}
		

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				SetItemAtIndex();


			Finish();
		}


		public void ResetArrayList()
		{
			proxy.arrayList.Clear();

			for(int i=0; i<c; i++)
			{
				proxy.arrayList.Add(variableType);

		}
			return;
		}


		public void SetItemAtIndex(){

			if (! isProxyValid()){
				Debug.LogError("Array PlayerPrefsx ERROR - Missing Array ? "+"   @    "+fullLabel+" : "+name);
				return;
			}

			string startKey = reference.Value+"_Count";
			c = PlayerPrefs.GetInt(startKey, c);
			arrayPrefillCount.Value = c;

			if (c == 0){
				Debug.LogError("Array PlayerPrefsx ERROR - Something went wrong when saving array - key count is 0 ? "+"   @   array: "+reference.Value);
			return;
			}



			if (startIndex.Value > 0){
				nextItemIndex = startIndex.Value-1;
				arrayAutoPrefill.Value = false;
			}
			
			if (endIndex.Value > 0){
				arrayAutoPrefill.Value = false;
			}



			if (arrayAutoPrefill.Value){
				ResetArrayList();
			}


			for(int i = 0; i<c;i++){

				if (startIndex.Value != 0 && endIndex.Value != 0){
				
					if (nextItemIndex >= c)
				{
					extractDone.Value = true;
					Fsm.Event(finishedEvent);
					return;
				}

				if (endIndex.Value>0 && nextItemIndex >= endIndex.Value)
				{
					extractDone.Value = true;
					Fsm.Event(finishedEvent);
					return;
				}
				}

			nextItemIndex++;
			
			key = reference.Value + ":" + Convert.ToString(nextItemIndex);

			object element = null;
			
			try{
					element = proxy.arrayList[nextItemIndex];
			}catch(System.Exception e){
				Debug.Log(e.Message);
				Fsm.Event(failureEvent);
				return;
			}
			
				switch (variableType) {
					case VariableType.Int:
					FsmInt fsmVarI  =  PlayerPrefs.GetInt(key);
					element = fsmVarI.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Float:
					FsmFloat fsmVarF = PlayerPrefs.GetFloat(key);
					element = fsmVarF.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Bool:
					FsmBool fsmVarB = PlayerPrefsX.GetBool(key);
					element =fsmVarB.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Color:
					FsmColor fsmVarC = PlayerPrefsX.GetColor(key);
					element =fsmVarC.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Quaternion:
					FsmQuaternion fsmVarQ = PlayerPrefsX.GetQuaternion(key);
					element =fsmVarQ.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Rect:
					FsmRect fsmVarR = PlayerPrefsX.GetRect(key);
					element =fsmVarR.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
			        break;

					case VariableType.Vector2:
					FsmVector2 fsmVarV2 = PlayerPrefsX.GetVector2(key);
					element =fsmVarV2.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.Vector3:
					FsmVector3 fsmVarV3 = PlayerPrefsX.GetVector3(key);
					element =fsmVarV3.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					case VariableType.String:
					FsmString fsmVarString = PlayerPrefs.GetString(key);
					element =fsmVarString.Value;
					proxy.Set(nextItemIndex,element,variableType.ToString());
					break;

					default:
					fullLabel = Fsm.GetFullFsmLabel(this.Fsm);
					name = Fsm.ActiveStateName;
					Debug.LogError("Array PlayerPrefsx ERROR - check it out "+"   @    "+fullLabel+" : "+name);
					break;
					}
							
			}

			extractDone.Value = true;
			Fsm.Event(finishedEvent);
			Finish();
		}
	}
}
