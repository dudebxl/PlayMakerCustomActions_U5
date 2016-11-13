// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/


using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11276.0")]
	[Tooltip("Sets the angular drag that applies to rotational movement and is set up separately from the linear drag that affects positional movement.")]
	public class SetAngularDrag2d : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;
		
		[RequiredField]
		[HasFloatSlider(0.0f,10f)]
		public FsmFloat drag;
		
		[Tooltip("Repeat every frame. Typically this would be set to True.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			drag = 1;
		}

		public override void Awake()
		{
			Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{

			CacheRigidBody2d(Fsm.GetOwnerDefaultTarget(gameObject));

			DoSetDrag();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnFixedUpdate()
		{
			DoSetDrag();
            
            if (!everyFrame)
			{
				Finish();
			}
		}

		void DoSetDrag()
		{
			if (!rb2d)
			{
				return;
			}
		   
			rb2d.angularDrag = drag.Value;
		}
	}
}
