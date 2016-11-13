// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Source http://hutonggames.com/playmakerforum/index.php?topic=10482.0


using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Send event by event name to all the GameObjects within an arrayList.")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=10482.0")]
	public class ArrayListSendEventByNameToGameObjects : ArrayListActions
	{
		[ActionSection("Setup")]
		
		private FsmEventTarget eventTarget;
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
		
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;
		
		public FsmBool sendToChildren;
		
		
		
		public override void Reset()
		{
			eventTarget = new FsmEventTarget();
			
			eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			
			gameObject = null;
			reference = null;
			sendEvent = null;
			sendToChildren = false;
		}
		
		
		public override void OnEnter()
		{
			
			if (SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
			{
				DoSendEvent();
			}
			
			Finish ();
		}
		
		void DoSendEvent()
		{
			
			if (! isProxyValid())
			{
				return;
			}
			
			int c = proxy.arrayList.Count;
			if (c == 0) return;


			foreach(GameObject _go in proxy.arrayList)
			{
				sendEventToGO(_go);
			}
			
		}
		
		void sendEventToGO(GameObject _go)
		{
			FsmEventTarget _eventTarget = new FsmEventTarget();
			FsmOwnerDefault owner = new FsmOwnerDefault();
			owner.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
			owner.GameObject = new FsmGameObject();
			owner.GameObject.Value = _go;
			_eventTarget.gameObject = owner;
			_eventTarget.target = FsmEventTarget.EventTarget.GameObject;	
			
			_eventTarget.sendToChildren = sendToChildren.Value;
			
			Fsm.Event(_eventTarget,sendEvent.Value);
		}
		
	}
}
