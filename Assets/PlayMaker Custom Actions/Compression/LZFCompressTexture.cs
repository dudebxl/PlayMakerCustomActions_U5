// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Original License and source: http://forum.unity3d.com/threads/lzf-compression-and-decompression-for-unity.152579/
// Keywords: Optimized compression using LZF library for string compression


using UnityEngine;
using System;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Compression")]
	[Tooltip("Convert Texture to Png (or Jpg) and then compress it into a string (doe not work windows phone 8.1+). ")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12031.msg56119;topicseen#msg56119")]
	public class LZFCompressTexture : FsmStateAction
	{
		[ActionSection("Setup")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Input")]
		public FsmTexture textureInput;
		 
		[ActionSection("Option")]
		public ImageType imageType;
		public enum ImageType{
			Jpg,
			Png,
		}


		[Tooltip("	JPG quality to encode with, 1..100 (default 75)")]
		public FsmInt jpgQuality;

		[ActionSection("Debug")]
		[Tooltip("Get final compression size")]
		public FsmBool sizeDebug;

		[ActionSection("Output")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Output string")]
		public FsmString stringOutput;
		 

		private byte[] jpgByte;
		private byte[] pngByte;
		private byte[] compressed;

		public override void Reset()
		{
			textureInput = null;
			stringOutput = null;
			imageType = ImageType.Jpg;
			jpgQuality = 75;
			sizeDebug = false;
		
		}

		public override void OnEnter()
		{
			if (textureInput.IsNone || textureInput.Value == null)
			{
				Debug.LogWarning("<b>[LZFCompressionTexture]</b><color=#FF9900ff> Empty Input - Please review!</color>", this.Owner);
				Finish ();
			}

			Texture2D texItem = textureInput.Value as Texture2D;
			
			switch (imageType)
			{
			case ImageType.Jpg:
				jpgByte = texItem.EncodeToJPG(jpgQuality.Value);
				compressed = LZF_CompressTexture.Compress(jpgByte);
				break;
			case ImageType.Png:
				pngByte =  texItem.EncodeToPNG();
				compressed = LZF_CompressTexture.Compress(pngByte);
				break;
			}

			stringOutput.Value = Convert.ToBase64String(compressed);
			compressed = null;

			texItem = null;

			if (sizeDebug.Value == true)	{
				string tempString = BytesToString((long)Encoding.ASCII.GetByteCount(stringOutput.Value));
				Debug.Log("<b>[LZFCompressionTexture]</b><color=#5E9DC8ff> String Size: </color>"+tempString);
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

public static class LZF_CompressTexture
	{
		private static readonly uint HLOGTEX = 14;
		private static readonly uint HSIZETEX = (1 << 14);
		private static readonly uint MAX_LITTEX = (1 << 5);
		private static readonly uint MAX_OFFTEX = (1 << 13);
		private static readonly uint MAX_REFTEX = ((1 << 8) + (1 << 3));
		
	
		private static readonly long[] HashTable = new long[HSIZETEX];

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
			
			Array.Clear(HashTable, 0, (int)HSIZETEX);
			
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
					hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOGTEX)) - hval * 5) & (HSIZETEX - 1));
					reference = HashTable[hslot];
					HashTable[hslot] = (long)iidx;
					
					
					if ((off = iidx - reference - 1) < MAX_OFFTEX
					    && iidx + 4 < inputLength
					    && reference > 0
					    && input[reference + 0] == input[iidx + 0]
					    && input[reference + 1] == input[iidx + 1]
					    && input[reference + 2] == input[iidx + 2]
					    )
					{
					
						uint len = 2;
						uint maxlen = (uint)inputLength - iidx - len;
						maxlen = maxlen > MAX_REFTEX ? MAX_REFTEX : maxlen;
						
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
						HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOGTEX)) - hval * 5) & (HSIZETEX - 1))] = iidx;
						iidx++;
						
						hval = (hval << 8) | input[iidx + 2];
						HashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOGTEX)) - hval * 5) & (HSIZETEX - 1))] = iidx;
						iidx++;
						continue;
					}
				}
				else if (iidx == inputLength)
					break;

				lit++;
				iidx++;
				
				if (lit == MAX_LITTEX)
				{
					if (oidx + 1 + MAX_LITTEX >= outputLength)
						return 0;
					
					output[oidx++] = (byte)(MAX_LITTEX - 1);
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

