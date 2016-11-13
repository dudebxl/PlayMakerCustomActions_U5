// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Original License and source: http://forum.unity3d.com/threads/lzf-compression-and-decompression-for-unity.152579/
// Keywords: Optimized decompression using LZF library for string decompression


using UnityEngine;
using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("LZF decompressor. The compression algorithm is extremely fast and it is compatible with all commmon platforms/devices (except windows phone 8.1+). ")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class LZFDecompressTexture : FsmStateAction
	{
		[ActionSection("Setup")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Input")]
		public FsmString stringInput;
		 
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Output string")]
		public FsmTexture textureInput;

		[ActionSection("Debug")]
		[Tooltip("Get final compression size")]
		public FsmBool sizeDebug;
		 
		private Texture2D texItem;

		public override void Reset()
		{
			stringInput = null;
			textureInput = null;
			texItem = null;
		
		}

		public override void OnEnter()
		{
			if (stringInput.IsNone || stringInput.Value == "")
			{
				Debug.LogWarning("<b>[LZFCompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}
			 
			texItem = new Texture2D(2,2);

			byte[] temptxt = Convert.FromBase64String(stringInput.Value);
			byte[] compressed = LZF_DecompressTexture.Decompress(temptxt);

		
			texItem.LoadImage(compressed);

			textureInput.Value = texItem;
			texItem = null;

			if (sizeDebug.Value == true)	{
				string tempString = BytesToString(compressed.LongLength);
				Debug.Log("<b>[LZFDecompressionTexture]</b><color=#5E9DC8ff> String Size: </color>"+tempString);
			}

			Finish();
	
		}

		public String BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}
	}
}
public static class LZF_DecompressTexture
	{
		public static byte[] Decompress(byte[] inputBytes)
		{
		
			int outputByteCountGuess = inputBytes.Length * 2;
			byte[] tempBuffer = new byte[outputByteCountGuess];
			int byteCount = lzf_decompress (inputBytes, ref tempBuffer);
			
		
			while (byteCount == 0)
			{
				outputByteCountGuess *=2;
				tempBuffer = new byte[outputByteCountGuess];
				byteCount = lzf_decompress (inputBytes, ref tempBuffer);
			}
			
			byte[] outputBytes = new byte[byteCount];
			Buffer.BlockCopy(tempBuffer, 0, outputBytes, 0, byteCount);
			return outputBytes;
		}

		public static int lzf_decompress(byte[] input, ref byte[] output)
	{
		int inputLength = input.Length;
		int outputLength = output.Length;
		
		uint iidx = 0;
		uint oidx = 0;
		
		do
		{
			uint ctrl = input[iidx++];
			
			if (ctrl < (1 << 5))
			{
				ctrl++;
				
				if (oidx + ctrl > outputLength)
				{

					return 0;
				}
				
				do
					output[oidx++] = input[iidx++];
				while ((--ctrl) != 0);
			}
			else 
			{
				uint len = ctrl >> 5;
				
				int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);
				
				if (len == 7)
					len += input[iidx++];
				
				reference -= input[iidx++];
				
				if (oidx + len + 2 > outputLength)
				{

					return 0;
				}
				
				if (reference < 0)
				{

					return 0;
				}
				
				output[oidx++] = output[reference++];
				output[oidx++] = output[reference++];
				
				do
					output[oidx++] = output[reference++];
				while ((--len) != 0);
			}
		}
		while (iidx < inputLength);
		
		return (int)oidx;
	}
}
