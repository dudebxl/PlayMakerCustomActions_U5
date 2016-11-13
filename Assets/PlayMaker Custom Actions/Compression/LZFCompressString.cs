// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Original License and source: http://forum.unity3d.com/threads/lzf-compression-and-decompression-for-unity.152579/
// Keywords: Optimized compression using LZF library for string compression


using UnityEngine;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("LZF Compressor. The compression algorithm is extremely fast and it is compatible with all commmon platforms/devices (except windows phone 8.1+). ")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class LZFCompressString : FsmStateAction
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

			byte[] temptxt =  System.Text.Encoding.ASCII.GetBytes(stringInput.Value);
			byte[] compressed = LZF_Compress.Compress(temptxt);
			stringOutput.Value = Convert.ToBase64String(compressed);

				for (int i = 0; i < loop.Value; i++)
			{
				byte[] compressedLoop = LZF_Compress.Compress(compressed);
				stringOutput.Value = Convert.ToBase64String(compressedLoop);
			}
			
			Finish();
	
		}
	}
}

public static class LZF_Compress
	{
		private static readonly uint HLOG = 14;
		private static readonly uint HSIZE = (1 << 14);
		private static readonly uint MAX_LIT = (1 << 5);
		private static readonly uint MAX_OFF = (1 << 13);
		private static readonly uint MAX_REF = ((1 << 8) + (1 << 3));
		
	
		private static readonly long[] HashTable = new long[HSIZE];

		public static byte[] Compress(byte[] inputBytes)
		{
	
			int outputByteCountGuess = inputBytes.Length * 2;
			byte[] tempBuffer = new byte[outputByteCountGuess];
			int byteCount = lzf_compress (inputBytes, ref tempBuffer);

			while (byteCount == 0)
			{
				outputByteCountGuess *=2;
				tempBuffer = new byte[outputByteCountGuess];
				byteCount = lzf_compress (inputBytes, ref tempBuffer);
			}
			
			byte[] outputBytes = new byte[byteCount];
			Buffer.BlockCopy(tempBuffer, 0, outputBytes, 0, byteCount);
			return outputBytes;
		}


		public static int lzf_compress(byte[] input, ref byte[] output)
		{
			int inputLength = input.Length;
			int outputLength = output.Length;
			
			Array.Clear(HashTable, 0, (int)HSIZE);
			
			long hslot;
			uint iidx = 0;
			uint oidx = 0;
			long reference;
			
			uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);
			long off;
			int lit = 0;
			
			for (; ;)
			{
				if (iidx < inputLength - 2)
				{
					hval = (hval << 8) | input[iidx + 2];
					hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
					reference = HashTable[hslot];
					HashTable[hslot] = (long)iidx;
					
					
					if ((off = iidx - reference - 1) < MAX_OFF
					    && iidx + 4 < inputLength
					    && reference > 0
					    && input[reference + 0] == input[iidx + 0]
					    && input[reference + 1] == input[iidx + 1]
					    && input[reference + 2] == input[iidx + 2]
					    )
					{
					
						uint len = 2;
						uint maxlen = (uint)inputLength - iidx - len;
						maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;
						
						if (oidx + lit + 1 + 3 >= outputLength)
							return 0;
						
						do
							len++;
						while (len < maxlen && input[reference + len] == input[iidx + len]);
						
						if (lit != 0)
						{
							output[oidx++] = (byte)(lit - 1);
							lit = -lit;
							do
								output[oidx++] = input[iidx + lit];
							while ((++lit) != 0);
						}
						
						len -= 2;
						iidx++;
						
						if (len < 7)
						{
							output[oidx++] = (byte)((off >> 8) + (len << 5));
						}
						else
						{
							output[oidx++] = (byte)((off >> 8) + (7 << 5));
							output[oidx++] = (byte)(len - 7);
						}
						
						output[oidx++] = (byte)off;
						
						iidx += len - 1;
						hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);
						
						hval = (hval << 8) | input[iidx + 2];
						HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;
						
						hval = (hval << 8) | input[iidx + 2];
						HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;
						continue;
					}
				}
				else if (iidx == inputLength)
					break;

				lit++;
				iidx++;
				
				if (lit == MAX_LIT)
				{
					if (oidx + 1 + MAX_LIT >= outputLength)
						return 0;
					
					output[oidx++] = (byte)(MAX_LIT - 1);
					lit = -lit;
					do
						output[oidx++] = input[iidx + lit];
					while ((++lit) != 0);
				}
			}
			
			if (lit != 0)
			{
				if (oidx + lit + 1 >= outputLength)
					return 0;
				
				output[oidx++] = (byte)(lit - 1);
				lit = -lit;
				do
					output[oidx++] = input[iidx + lit];
				while ((++lit) != 0);
			}
			
			return (int)oidx;
		}
	}

