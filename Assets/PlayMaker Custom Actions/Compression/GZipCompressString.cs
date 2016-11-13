// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Compression using GZip library (.gz) for string compression
//require: https://github.com/Hitcents/Unity.IO.Compression

using UnityEngine;
using System;
using System.IO;
using Unity.IO.Compression;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("GZip Compression. - requires Unity.IO.Compression")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class GZipCompressString : FsmStateAction
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
				Debug.LogWarning("<b>[GZipCompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}
			 
			stringOutput.Value = Zip(stringInput.Value);

			Finish();
	
		}


		public static string Zip(string text)
		{
			byte[] buffer = System.Text.Encoding.Unicode.GetBytes(text);
			MemoryStream ms = new MemoryStream();
			using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
			{
				zip.Write(buffer, 0, buffer.Length);
			}
			
			ms.Position = 0;
			MemoryStream outStream = new MemoryStream();
			
			byte[] compressed = new byte[ms.Length];
			ms.Read(compressed, 0, compressed.Length);
			
			byte[] gzBuffer = new byte[compressed.Length + 4];
			System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
			System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

			return Convert.ToBase64String(gzBuffer);
		}
	}
}
