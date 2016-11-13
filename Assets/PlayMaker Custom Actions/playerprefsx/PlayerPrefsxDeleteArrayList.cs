// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Source http://hutonggames.com/playmakerforum/index.php?topic=10072.0

// Requires: ArrayMaker + PlayerPrefs X

using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/Array PlayerPrefs X")]
	[Tooltip("Delete PlayerPrefs X Array data in PlayerPrefs")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=10072.0")]
	public class PlayerPrefsxDeleteArrayList: ArrayListActions
	{
		[ActionSection("Array Setup")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		[UIHint(UIHint.FsmBool)]
		[Tooltip("Also destroy array List?")]
		public FsmBool alsoDestroyArrayList;

		[ActionSection("Result")]
		
		[UIHint(UIHint.FsmBool)]
		[Tooltip("When delete is done bool flip to True")]
		public FsmBool deleteDone;

		[ActionSection("Events")]

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when saved is done")]
		public FsmEvent finishedEvent;
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails (index is out of range exception or something went wrong)")]
		public FsmEvent failureEvent;
	
		private string key;
		private int c;
		private FsmString startKey;
		private FsmInt x;
		private bool destroy1;
		private bool destroy2;

		public override void Reset()
		{

			gameObject = null;
			
			failureEvent = null;
			finishedEvent = null;

			key= null;
			c = 0;
			x=0;
			destroy1=false;
			destroy2=false;

		}
		
		
		
		public override void OnEnter()
		{

				Delete();


			Finish();
		}



		public void CleanData(){
	
			for(int i = 0; i<c;i++){
			key = reference.Value + ":" + Convert.ToString(i);
			
			PlayerPrefs.DeleteKey(key);
			
			}

			if (alsoDestroyArrayList.Value){
				destroy1 = true;
			}
			key = startKey.Value;
			PlayerPrefs.DeleteKey(key);
			return;
		}

		void DoDestroyArrayList()
		{
			PlayMakerArrayListProxy[] proxies = proxy.GetComponents<PlayMakerArrayListProxy>();
			foreach (PlayMakerArrayListProxy iProxy in proxies) {
				if (iProxy.referenceName == reference.Value){
					UnityEngine.Object.Destroy(iProxy) ;
					destroy2 = true;
					return;
				}
			}
			
			return;
		}

		public void Delete(){

			startKey = reference.Value+"_Count";

			if(!startKey.IsNone || !startKey.Value.Equals("")) {
				x.Value = PlayerPrefs.GetInt(startKey.Value, x.IsNone ? 0 : x.Value);}
			else {
			Fsm.Event(failureEvent);
			}

			if (x.Value <= 0){
				Debug.LogError("Array PlayerPrefsx ERROR - Something went wrong when saving array - key count is 0 ? Need to manually delete key(s)"+"   @   array: "+reference.Value);
				Fsm.Event(failureEvent);
			}

			c = x.Value;
		
			CleanData();

			if (alsoDestroyArrayList.Value){
			if (SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value))
			{
				DoDestroyArrayList();
			}else{
				deleteDone.Value = false;
				Debug.LogError("Array PlayerPrefsx ERROR - Missing Array list porxy from source ? "+"   @   array: "+reference.Value);
				Fsm.Event(failureEvent);
			}
			}

			if (alsoDestroyArrayList.Value){

				if ( destroy1 && destroy2){

			deleteDone.Value = true;	

				}

				else {
				deleteDone.Value = false;
			}
			}

			else {
				deleteDone.Value = true;	
			}


			Fsm.Event(finishedEvent);
			Finish();
		
		}
	}
}
