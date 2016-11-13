// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Source http://hutonggames.com/playmakerforum/index.php?topic=10279

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Contains duplicates in the array ?")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=10279")]
	public class ArrayListContainDuplicates : ArrayListActions
	{
		
		[ActionSection("Array Setup")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;


		[ActionSection("Result")]
		[UIHint(UIHint.FsmBool)]
		[Tooltip("Set bool to True if found duplicate in array")]
		public FsmBool isTrue;


		[ActionSection("Event")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Send event if True")]
		public FsmEvent trueEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Send event if False")]
		public FsmEvent falseEvent;
		[ActionSection("")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;


		private int atIndex;
		private int atIndex_iplus;
		private int c;
		private bool restartbool;
		private bool duplicateFound;

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			atIndex = -1;
			trueEvent = null;
			falseEvent = null;
			failureEvent = null;
			atIndex_iplus = 0;
			c = 0;

		}
		
		
		public override void OnEnter()
		{
			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				doesArrayListContainsCount();
			
			Finish();
		}
		
		
		public void doesArrayListContainsCount()
		{

			if (! isProxyValid() ) {
				Fsm.Event(failureEvent);
				return;
			}

			c = proxy.arrayList.Count;

			if (c < 1) {
				Debug.Log(" Array is = 1 or below. Problem ? ");
				Fsm.Event(failureEvent);
				return;
			}
					

			Go();


		}

		public void Go(){
			
			duplicateFound = false;
			atIndex = -1;
			atIndex_iplus=0;

			var varTemp = proxy.preFillType.ToString();
			
			
			for(int a = 0; a<c;a++){
				
				atIndex++;
				atIndex_iplus = atIndex;
				
				for(int i = 0; i<c;i++){
IGNORE:				
				if (restartbool){
					atIndex_iplus = atIndex;
					restartbool = false;
				}
				
				
				atIndex_iplus++;
				
				
				if (atIndex_iplus == c){
					break;
				}
				
				bool elementContained = false;
				object element = null;
				object element_b = null;
				
				try{
					element = proxy.arrayList[atIndex];
					element_b = proxy.arrayList[atIndex_iplus];
				}catch(System.Exception e){
					Debug.Log(e.Message);
					string fullLabel = Fsm.GetFullFsmLabel(this.Fsm);
					string name = Fsm.ActiveStateName;
					Debug.Log("Fsm Path= "+fullLabel+" : "+name);
					Fsm.Event(failureEvent);
					return;
				}
				
				if (element == null){
					break;
				}
				
				if (element_b == null){
					goto IGNORE;
				}
				
					switch (varTemp) {
					case "Int":
						FsmInt fsmVarI = System.Convert.ToInt32(element);
						int tempInt = System.Convert.ToInt32(element_b);
						elementContained = fsmVarI.Value == tempInt;
						break;
						
						
					case "Float":
						FsmFloat fsmVarF = (float)element;
						float tempFloat = (float)(element_b); 
						elementContained = fsmVarF.Value == tempFloat;
						break;
						
					case "Bool":
						FsmBool fsmVarB = (bool)element;
						bool tempBool = (bool)(element_b); 
						elementContained = fsmVarB.Value == tempBool;
						break;
						
					case "Color":
						FsmColor fsmVarC = (Color)element;
						Color tempColor = (Color)(element_b); 
						elementContained = fsmVarC.Value == tempColor;
						break;
						
					case "Quaternion":
						FsmQuaternion fsmVarQ = (Quaternion)element;
						Quaternion tempQuaternion = (Quaternion)(element_b); 
						elementContained = fsmVarQ.Value == tempQuaternion;
						break;
						
					case "Rect":
						FsmRect fsmVarR = (Rect)element;
						Rect tempRect = (Rect)(element_b);
						elementContained = fsmVarR.Value == tempRect;
						break;
						
					case "Vector2":
						FsmVector2 fsmVarV2 = (Vector2)element;
						Vector2 tempV2 = (Vector2)(element_b);
						elementContained = fsmVarV2.Value == tempV2;
						break;
						
					case "Vector3":
						FsmVector3 fsmVarV3 = (Vector3)element;
						Vector3 tempV3 = (Vector3)(element_b);
						elementContained = fsmVarV3.Value == tempV3;
						break;
						
					case "String":
						FsmString fsmVarString = (string)element;
						string tempString = (string)(element_b);
						elementContained = fsmVarString.Value == tempString;
						break;
						
					case "GameObject":
						FsmGameObject fsmVarGameObject = (GameObject)element;
						GameObject tempGameObject = (GameObject)(element_b);
						elementContained = fsmVarGameObject.Value == tempGameObject;
						break;
						
					case "Material":
						Material fsmVarMaterial = (Material)element;
						Material tempMaterial = (Material)(element_b);
						elementContained = fsmVarMaterial == tempMaterial;
						break;
						
					case "Texture":
						Texture fsmVarTexture = (Texture)element;
						Texture tempTexture = (Texture)(element_b);
						elementContained = fsmVarTexture == tempTexture;
						break;
						
					case "AudioClip":
						var fsmVarUnknown = element;
						var tempUnknown = element_b;
						elementContained = fsmVarUnknown == tempUnknown;
						break;
						
					default:
						Debug.Log ("ERROR");
						break;
					}
				
				
				if (elementContained){

					duplicateFound = true;

					a = c;
					break;
					
				}
				
			}
			
		}

			if (duplicateFound){
			isTrue.Value = true;
			
			if (trueEvent != null){
				Fsm.Event(trueEvent);
			}
		}
		
			if (!duplicateFound){
			isTrue.Value = false;
			
			if (falseEvent != null){
				Fsm.Event(falseEvent);
			}
		}
			Finish();
	}


	}
}
