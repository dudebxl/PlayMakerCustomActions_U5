// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Decompression using GZip library (.gz) for string decompression
//require: https://github.com/Hitcents/Unity.IO.Compression

using UnityEngine;
using System;
using System.IO;
using Unity.IO.Compression;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("GZip Decompression. - requires Unity.IO.Compression")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class GZipDecompressString : FsmStateAction
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
		 
	
		public override void Reset()
		{
			stringInput = null;
			stringOutput = null;
		
		}

		public override void OnEnter()
		{
			if (stringInput.IsNone || stringInput.Value == "")
			{
				Debug.LogWarning("<b>[GZipDecompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}
			 
			stringOutput.Value = UnZip(stringInput.Value);
		
			Finish();
	
		}

		public static string UnZip(string compressedText)
		{
			byte[] gzBuffer = Convert.FromBase64String(compressedText);
			using (MemoryStream ms = new MemoryStream())
			{
				int msgLength = BitConverter.ToInt32(gzBuffer, 0);
				ms.Write(gzBuffer, 4, gzBuffer.Length - 4);
				
				byte[] buffer = new byte[msgLength];
				
				ms.Position = 0;
				using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
				{
					zip.Read(buffer, 0, buffer.Length);
				}
				
				return System.Text.Encoding.Unicode.GetString(buffer, 0, buffer.Length);
			}

			
			
		}
	}
}
