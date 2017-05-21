// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: vibration android
using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Android - causes the device to vibrate with custom settings using array list for the pattern. Please read instruction by clicking on the action url link. If set incorrectly, you may get odd behavior as a result!")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11816.0")]
	public class AndroidVibrateAdvanceArray2 : FsmStateAction
	{
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
		#else
		public static AndroidJavaClass unityPlayer;
		public static AndroidJavaObject currentActivity;
		public static AndroidJavaObject vibrator;
		#endif
	

		[ActionSection("Array Pattern Setup")]
		// The first value indicates the number of milliseconds to wait before turning the vibrator on.
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		[Tooltip("-1 means do not repeat")]
		public FsmInt repeat;


		[ActionSection("Option")]
		[Tooltip("Cancel the vibration by 're-entering' action with bool set to True. Do *not* do this immediately after calling vibrate. It may not have time to even begin vibrating!")]
		public FsmBool cancelVibrator;
		[Tooltip("The screen will never sleep if set to true as when the screen sleeps the vibration stops")]
		public FsmBool disableSleepTimeout;

		[ActionSection("Output")]
		public FsmBool notAndroid;
		[Tooltip("The event to send if it has No vibrator")]
		public FsmEvent notAndroidEvent;
		[Tooltip("If you cancel")]
		public FsmEvent vibratorCancelled;


		public override void Reset()
		{


			cancelVibrator = false;
			notAndroid = false;
			notAndroidEvent = null;
			repeat = -1;
			array = null;
			disableSleepTimeout = true;

		}


		public override void OnEnter()
		{
			if (cancelVibrator.Value == true)
			{
				Cancel();
				Fsm.Event(vibratorCancelled);
				Finish();

			}

			if (disableSleepTimeout.Value == true)
			{

				Screen.sleepTimeout = SleepTimeout.NeverSleep;

			} 

			notAndroid.Value = isAndroid();

			if (notAndroid.Value == false)
			{
				notAndroid.Value = true;
				Fsm.Event(notAndroidEvent);
				Finish();
				
			} 

			else
			{

				if (array.Length == 0)
			{
						return;
			} 

				else
				{
					int length = array.Length;
					long[] patternTemp = new long[length];

					for (int i = 0; i < length; i++)
				{
						patternTemp[i] = Convert.ToInt64(array.Get(i));

				} 

				Vibrate(patternTemp, repeat.Value);

				/* 
						Debug.Log ("<color=#5F9EA0>Repeat:"+repeat.Value+"</color>");
						
						for (int i = 0; i < length; i++)
						{
							
							Debug.Log ("<color=#5F9EA0>Pattern debug index:"+i+" Milliseconds: </color>"+patternTemp[i], this.Owner);
						} 

				*/
		

			}
			 
			}
		}

		public override void OnUpdate()
		{

			if (cancelVibrator.Value == true)
			{
				Cancel();
				Fsm.Event(vibratorCancelled);
				Finish();
				
			}

		}
		
		
		public static void Vibrate()
		{
			if (isAndroid())
				vibrator.Call("vibrate");
			else
				Handheld.Vibrate();
		}

		
		public static void Vibrate(long[] pattern, int repeat)
		{
			if (isAndroid())
				vibrator.Call("vibrate", pattern, repeat);

		}
		
		public static bool HasVibrator()
		{
			return isAndroid();
		}
		
		public static void Cancel()
		{
			if (isAndroid())
				vibrator.Call("cancel");
			return;
		}
		
		private static bool isAndroid()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			return true;
			#else
			return false;
			#endif
		}
	}
}
