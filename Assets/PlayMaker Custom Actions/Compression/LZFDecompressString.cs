// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Original License and source: http://forum.unity3d.com/threads/lzf-compression-and-decompression-for-unity.152579/
// Keywords: Optimized decompression using LZF library for string decompression


using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("LZF decompressor. The compression algorithm is extremely fast and it is compatible with all commmon platforms/devices (except windows phone 8.1+). ")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class LZFDecompressString : FsmStateAction
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
		 
		[ActionSection("Options")]
		[Tooltip("How many times you want to loop")]
		public FsmInt loop;

		public override void Reset()
		{
			stringInput = null;
			stringOutput = null;
			loop = 0;
		
		}

		public override void OnEnter()
		{
			if (stringInput.IsNone || stringInput.Value == "")
			{
				Debug.LogWarning("<b>[LZFCompressionString]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}
			 
			byte[] temptxt = Convert.FromBase64String(stringInput.Value);
			byte[] compressed = LZF_Decompress.Decompress(temptxt);
			stringOutput.Value = System.Text.ASCIIEncoding.ASCII.GetString(compressed);

				for (int i = 0; i < loop.Value; i++)
			{
				byte[] compressedLoop = LZF_Decompress.Decompress(compressed);
				stringOutput.Value = System.Text.ASCIIEncoding.ASCII.GetString(compressedLoop);
			}
			
			Finish();
	
		}
	}
}
public static class LZF_Decompress
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
