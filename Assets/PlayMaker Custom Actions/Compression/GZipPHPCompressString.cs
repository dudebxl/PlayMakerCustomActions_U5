// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Compression PHP base64_decode(gzencode("hello world")) using GZip library for string compression
//require: https://github.com/Hitcents/Unity.IO.Compression

using UnityEngine;
using System;
using Unity.IO.Compression;
using System.IO;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("GZip compress string for php gzdecode. - requires Unity.IO.Compression")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class GZipPHPCompressString : FsmStateAction
	{
		[ActionSection("Setup")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Input")]
		public FsmString stringInput;
		 
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Output string")]
		public FsmString stringOutput;

		[Tooltip("Options")]
		public ControlsSelect type;
		
		public enum ControlsSelect {
			
			gzencode, 
			gzdeflate,

		};
	
		public override void Reset()
		{
			stringInput = null;
			stringOutput = null;
			type = ControlsSelect.gzencode;
		}

		public override void OnEnter()
		{
			if (stringInput.IsNone || stringInput.Value == "")
			{
				Debug.LogWarning("<b>[GZipCompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}

			switch(type){
			case ControlsSelect.gzencode:
			stringOutput.Value = Zip(stringInput.Value);
			break;
			
			case ControlsSelect.gzdeflate:
			stringOutput.Value = Zip2(stringInput.Value);
			break;
			}

			Finish();
	
		}


		public static string Zip(string text)
		{
			byte[] buffer = Encoding.Unicode.GetBytes(text);
			
			MemoryStream rawDataStream = new MemoryStream();

				GZipStream gzipOut = new GZipStream(rawDataStream, CompressionMode.Compress);
				gzipOut.Write(buffer, 0, buffer.Length);
				gzipOut.Close();

			
			byte[] compressed = rawDataStream.ToArray();

			return Convert.ToBase64String(compressed);
		}

	public static string Zip2(string text)
	{
		byte[] buffer = Encoding.Unicode.GetBytes(text);
		
		MemoryStream rawDataStream = new MemoryStream();
		
		DeflateStream gzipdeflateOut = new DeflateStream(rawDataStream, CompressionMode.Compress);
		gzipdeflateOut.Write(buffer, 0, buffer.Length);
		gzipdeflateOut.Close();
		
	
	byte[] compressed = rawDataStream.ToArray();
	
	return Convert.ToBase64String(compressed);
}



	}
}
