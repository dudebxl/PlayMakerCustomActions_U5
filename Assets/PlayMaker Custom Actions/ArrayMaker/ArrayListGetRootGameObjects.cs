// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: scenemanager,scene manager,ArrayList Get Root GameObjects
// require minimum 5.3

#if (UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define UNITY_PRE_5_3
#endif

using UnityEngine;
#if !UNITY_PRE_5_3
using UnityEngine.SceneManagement;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Get all the root GameObjects in the scene. Wait two frames for scene to load all objects for accurate results.")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12649.0")]
	public class ArrayListGetRootGameObjects : ArrayListActions
	{
		[ActionSection("ArrayList Setup")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;


		[ActionSection("Scene Setup")]

		[RequiredField]
		[Tooltip("Scene Name")]
		public FsmString sceneName;

		//bool[] _activeStates;

		private Scene target;
		
		public override void Reset()
		{
			gameObject = null;
			sceneName = null;
		}
		
		
		public override void OnEnter()
		{

			#if UNITY_PRE_5_3
			Debug.LogWarning("<b>[ArrayListGetRootGameObjects]</b><color=#FF9900ff> Need minimum unity5.3 !</color>", this.Owner);
			Finish ();
			#endif

			target = SceneManager.GetSceneByName(sceneName.Value);

			if (! SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
			{
				Finish();
			}
			
			DoAction();

			Finish();

		}

		
		void DoAction()
		{
			
			if (! isProxyValid())
			{
				return;
			}


			GameObject[] rootGameObjects = target.GetRootGameObjects();

			int length = rootGameObjects.Length;
			length = length-1;


			for(int x = 0; x <=length; x++)
				{
					proxy._arrayList.Add(rootGameObjects[x]);
				}
	
			return;

		}
			
		
	}
}
