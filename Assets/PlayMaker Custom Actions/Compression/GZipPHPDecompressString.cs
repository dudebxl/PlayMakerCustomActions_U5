// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Decompression PHP base64_encode(gzencode("hello world")) using GZip library for string decompression
//require: https://github.com/Hitcents/Unity.IO.Compression

using UnityEngine;
using System;
using Unity.IO.Compression;
using System.IO;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("GZip will decompress php base64_encode(gzencode(hello world)) - requires Unity.IO.Compression")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class GZipPHPDecompressString : FsmStateAction
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
			
			gzdecode,
			gzinflate,
			
		};
	
		public override void Reset()
		{
			stringInput = null;
			stringOutput = null;
			type = ControlsSelect.gzdecode;
		}

		public override void OnEnter()
		{
			if (stringInput.IsNone || stringInput.Value == "")
			{
				Debug.LogWarning("<b>[GZipDecompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}

			switch(type){
			case ControlsSelect.gzdecode:
			stringOutput.Value = UnZip(stringInput.Value);
			break;

			case ControlsSelect.gzinflate:
			stringOutput.Value = UnZip2(stringInput.Value);
			break;
			}

			Finish();
	
		}

		public static string UnZip(string compressedText)
		{

			byte[] gzBuffer = Convert.FromBase64String(compressedText);
			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(gzBuffer, 0, gzBuffer.Length);
				ms.Seek(0, 0);

				byte[] buffer = new byte[4096];
				
				ms.Position = 0;
				using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
				{
					zip.Read(buffer, 0, buffer.Length);
				}
					
				return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
			}
		}

		public static string UnZip2(string compressedText)
		{
			
			byte[] gzBuffer = Convert.FromBase64String(compressedText);
			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(gzBuffer, 0, gzBuffer.Length);
				ms.Seek(0, 0);
				
				byte[] buffer = new byte[4096];
				
				ms.Position = 0;
				using (DeflateStream zip = new DeflateStream(ms, CompressionMode.Decompress))
				{
					zip.Read(buffer, 0, buffer.Length);
				}
				
				return Encoding.ASCII.GetString(buffer, 0, buffer.Length);
			}
		}



	}
}
