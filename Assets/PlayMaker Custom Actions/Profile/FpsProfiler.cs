using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Profiling;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("Profile")]
	[Tooltip("Get the fps")]
	public class FpsProfiler : FsmStateAction
	{
		
		[ActionSection("Fps Setup")]
		public FsmInt setRecommendedFps;
		public FsmInt setWarningFpsMin;
		public FsmInt setWarningFpsMax;
		public FsmInt setCriticalFpsMin;
		public FsmInt setCriticalFpsMax;

		[ActionSection("Color Setup")]
		public FsmColor textColor;
		public FsmColor normalFpsColor;
		public FsmColor warningFpsColor;
		public FsmColor criticalFpsColor;

		[ActionSection("FpsOutput")]
		public FsmInt Fps;
		public FsmFloat FpsMs;
		public FsmInt averageFps;
		public FsmInt minFps;
		public FsmInt maxFps;
		public FsmColor FpsColor;

		[ActionSection("Memory Output Mb")]
		public FsmInt memoryAllocation;
		public FsmInt totalMemory;
		public FsmInt reservedMemory;

		[ActionSection("Option")]
		public FsmBool inclMemory;
		public FsmBool forceQuit;

		//private
		private float updateInterval = 1.0f;
		private float lastInterval = 1.0f;
		private float frames = 0f; 

		private float fpsavrate = 0;
		private float fpsav = 0.0f;

		private int tempMin = 0;
		private int tempMax = 0;

		public override void Reset()
		{
			setRecommendedFps = 60;
			setWarningFpsMin = 30;
			 setWarningFpsMax = 59;
			 setCriticalFpsMin = 0;
			 setCriticalFpsMax = 29;

			textColor = Color.white;
			normalFpsColor = new Color(0,205,0,255);
			warningFpsColor = Color.magenta;
			criticalFpsColor = new Color(255,0,0,255);

			forceQuit = false;
			inclMemory = true;

	
		}
	
		public override void OnEnter()
		{
			lastInterval = Time.realtimeSinceStartup;
			frames = 0;
			tempMin = -1;
			tempMax = -1;

		}

		public override void OnUpdate()
		{
				
			if (forceQuit.Value == true)
			{
				Finish();
			}

			++frames;

			var timeNow = Time.realtimeSinceStartup;

			if (timeNow > lastInterval + updateInterval)
			{

				float temp = frames / (timeNow - lastInterval);
				Fps.Value = Convert.ToInt32(temp);
				FpsMs.Value = 1000.0f / temp;
				FpsMs.Value = Mathf.Round(FpsMs.Value * 100f) / 100f;

				++fpsavrate;
				fpsav += Fps.Value;
				averageFps.Value = Convert.ToInt32(fpsav/fpsavrate);

				if ( setCriticalFpsMin.Value <= Fps.Value && Fps.Value <= setCriticalFpsMax.Value)
				{
					FpsColor.Value = criticalFpsColor.Value;
				}

				else if ( setWarningFpsMin.Value <= Fps.Value && Fps.Value <= setWarningFpsMax.Value)
				{
					FpsColor.Value = warningFpsColor.Value;
				}

				else 
				{
					FpsColor.Value = normalFpsColor.Value;
				}
					
				if (tempMin == -1)
				{
					tempMin = Fps.Value;
				}

				if (tempMax == -1)
				{
					tempMax = Fps.Value;
				}

				if (tempMin >= Fps.Value)
				{
					tempMin = Fps.Value;
					minFps.Value = Fps.Value;
				}

				if (tempMax <= Fps.Value)
				{
					tempMax = Fps.Value;
					maxFps.Value = Fps.Value;
				}

				frames = 0;
				lastInterval = timeNow;

				if (inclMemory.Value = true)
				{
				memoryAllocation.Value = Convert.ToInt32( Profiler.GetTotalAllocatedMemory()/1048576);
				totalMemory.Value = Convert.ToInt32(Profiler.GetTotalReservedMemory()/1048576);
				reservedMemory.Value = Convert.ToInt32(Profiler.GetTotalUnusedReservedMemory()/1048576);
				}


		}


	}

}
}
