// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/

// v1 for Min. PM 1.8.1+
// Keywords: Camera Fade

using System;
using UnityEngine;
using System.Collections;


namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Fade between two camera")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11308.0")]
	public class CameraCrossFade : FsmStateAction
	{
		[ActionSection("Camera")]
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		[Tooltip("From this camera")]
		public FsmGameObject cameraFrom;
		[CheckForComponent(typeof(Camera))]
		[Tooltip("To this camera")]
		public FsmGameObject cameraTo;

		[ActionSection("General settings")]
		public FsmFloat fadeTime;
		public FsmAnimationCurve curve;


		[ActionSection("Output")]
		public FsmBool inProgress;

		private Texture tex;
		private Texture2D tex2D;
		private float alpha;
		private bool  reEnableListener;
		private Transform shape;
		private bool isWorking;

		private static ScreenWipe use;
		private Coroutine routine;

		public override void Reset()
		{
			cameraFrom = null;
			cameraTo = null;
			fadeTime = 2.0f;
			curve = null;
			inProgress = false;
			isWorking = false;
		}
		
		public override void OnEnter()
		{
			isWorking = false;
			inProgress.Value = true;
			Camera c1 = cameraFrom.Value.GetComponent<Camera>();
			Camera c2 = cameraTo.Value.GetComponent<Camera>();

			routine = StartCoroutine (CrossFade (c1, c2, fadeTime.Value));
		}
			
		public override void OnGUI ()
		{
			if (isWorking) {
				GUI.depth = -9999999;
				GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), tex, ScaleMode.ScaleToFit, false, 0.0F);
			}
		}
	
		public override void OnExit()
		{
			StopCoroutine(routine);
		}
			

		private IEnumerator AlphaTimer (float time)
		{
			float rate = 1.0f / time;
				float t = 1.0f;

				while (t > 0f) {
					alpha = curve.curve.Evaluate (t);
					t -= Time.deltaTime * rate;

					yield return 0;
				}
		}

		private void CameraSetup (Camera cam1, Camera cam2, bool cam1Active,bool enableThis)
		{

			cam1.gameObject.SetActive(cam1Active);
			cam2.gameObject.SetActive(true);
			AudioListener listener = cam2.GetComponent<AudioListener> ();

			if (listener) {
				reEnableListener = listener.enabled ? true : false;
				listener.enabled = false;
			}
		}

		void CameraCleanup (Camera cam1, Camera cam2)
		{
			AudioListener listener = cam2.GetComponent<AudioListener> ();
			if (listener && reEnableListener) {
				listener.enabled = true;
			}
			cam1.gameObject.SetActive(false);
		}

	
		private IEnumerator CrossFade (Camera cam1, Camera cam2, float time)
		{
			isWorking = true;

			if (!tex2D) {
				tex2D = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
			}
			yield return new WaitForEndOfFrame ();
			tex2D.ReadPixels (new Rect (0, 0, Screen.width, Screen.height), 0, 0, false);
			tex2D.Apply ();
			tex = tex2D;
			yield return 0;

			CameraSetup (cam1, cam2, false, true);

			StartCoroutine (AlphaTimer (time));
			yield return new WaitForSeconds (time);

			CameraCleanup (cam1, cam2);
			isWorking = false;

			if (isWorking == false){
				inProgress.Value = false;	
				Finish();
			}
		}


			
	}
}
