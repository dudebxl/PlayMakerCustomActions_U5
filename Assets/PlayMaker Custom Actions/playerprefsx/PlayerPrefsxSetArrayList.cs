// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Source http://hutonggames.com/playmakerforum/index.php?topic=10072.0

// Requires: ArrayMaker + PlayerPrefs X

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/Array PlayerPrefs X")]
	[Tooltip("Save Array using PlayerPrefs X")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=10072.0")]
	public class PlayerPrefsxSetArrayList: ArrayListActions
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

		[ActionSection("Index Setup")]
		[Tooltip("From where to start iteration, leave to 0 to start from the beginning")]
		public FsmInt startIndex;
		
		[Tooltip("When to end iteration (incl in save), leave to 0 to iterate until the end")]
		public FsmInt endIndex;

		[Tooltip("Deletes previous array keys in Playerprefs then saves - fresh save ** slower but safer, will not work if startIndex/endIndex are >0")]
		public FsmBool cleanPass;

		[ActionSection("Result")]
		
		[UIHint(UIHint.Variable)]
		[Tooltip("When save is done bool flip to True")]
		public FsmBool saveDone;

		[ActionSection("Events")]

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when saved is done")]
		public FsmEvent finishedEvent;
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails (index is out of range exception or something went wrong)")]
		public FsmEvent failureEvent;
	
		private int nextItemIndex = -1;
		private string key;
		private int c;
		private int x;

		public override void Reset()
		{
			startIndex = null;
			endIndex = null;
			gameObject = null;
			
			failureEvent = null;
			finishedEvent = null;

			cleanPass = true;
			saveDone = false;

			key= null;
			c = 0;
			x = 0;
		}
		
		
		
		public override void OnEnter()
		{

				if ( ! SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				{
					Fsm.Event(failureEvent);
					
					Finish();
				}
		

			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				GetItemAtIndex();


			Finish();
		}



		public void CleanData(){
	
			for(int i = 0; i<x;i++){
				key = reference.Value + ":" + Convert.ToString(i);
			
			PlayerPrefs.DeleteKey(key);

			}
	
			return;
		}

			public void GetItemAtIndex(){

			if (! isProxyValid())
				return;


			c = proxy.arrayList.Count;
			string startKey = reference.Value+"_Count";


			if (startIndex.Value > 0){
				nextItemIndex = startIndex.Value-1;
				cleanPass.Value = false;
			}
			
			if (endIndex.Value > 0){
				cleanPass.Value = false;
			}

			if (cleanPass.Value != false)
			{
				x = PlayerPrefs.GetInt(startKey, x);
				CleanData();
			}

			PlayerPrefs.SetInt(startKey, c);

			for(int i = 0; i<c;i++){

				if (startIndex.Value != 0 && endIndex.Value != 0){
				
					if (nextItemIndex >= c)
				{
					saveDone.Value = true;
					Fsm.Event(finishedEvent);
					return;
				}

				if (endIndex.Value>0 && nextItemIndex >= endIndex.Value)
				{
					saveDone.Value = true;
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
					FsmInt fsmVarI = System.Convert.ToInt32(element);
					PlayerPrefs.SetInt(key, fsmVarI.IsNone ? 0 : fsmVarI.Value);
					break;

					case VariableType.Float:
					FsmFloat fsmVarF = (float)element;
					PlayerPrefs.SetFloat(key, fsmVarF.IsNone ? 0f : fsmVarF.Value);
					break;

					case VariableType.Bool:
					FsmBool fsmVarB = (bool)element;
					PlayerPrefsX.SetBool(key, fsmVarB.IsNone ? false : fsmVarB.Value);
					break;

					case VariableType.Color:
					FsmColor fsmVarC = (Color)element;
					PlayerPrefsX.SetColor(key, fsmVarC.IsNone ? Color.black : fsmVarC.Value);
					break;

					case VariableType.Quaternion:
					FsmQuaternion fsmVarQ = (Quaternion)element;
					PlayerPrefsX.SetQuaternion(key, fsmVarQ.IsNone ? Quaternion.identity : fsmVarQ.Value);
					break;

					case VariableType.Rect:
					FsmRect fsmVarR = (Rect)element;
					PlayerPrefsX.SetRect(key, fsmVarR.IsNone ? new Rect(0f,0f,0f,0f) : fsmVarR.Value);
			        break;

					case VariableType.Vector2:
					FsmVector2 fsmVarV2 = (Vector2)element;
					PlayerPrefsX.SetVector2(key, fsmVarV2.IsNone ? Vector2.zero : fsmVarV2.Value);
					break;

					case VariableType.Vector3:
					FsmVector3 fsmVarV3 = (Vector3)element;
					PlayerPrefsX.SetVector3(key, fsmVarV3.IsNone ? Vector3.zero : fsmVarV3.Value);
					break;

					case VariableType.String:
					FsmString fsmVarString = (string)element;
					PlayerPrefs.SetString(key, fsmVarString.IsNone ? "" : fsmVarString.Value);
					break;

					default:
					string fullLabel = Fsm.GetFullFsmLabel(this.Fsm);
					string name = Fsm.ActiveStateName;
					Debug.LogError("Array PlayerPrefsx ERROR - check it out "+"   @    "+fullLabel+" : "+name);
					break;
					}
							
			}

			saveDone.Value = true;
			Fsm.Event(finishedEvent);
			Finish();
		}
	}
}
