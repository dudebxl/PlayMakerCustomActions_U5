// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// source: https://gist.github.com/darktable/1411710
// Keywords: HashTable to Json

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Build JSON format String from a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
    [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11991.0")]
	public class HashTableToJson : HashTableActions
	{
		
		[ActionSection("Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
		
		[ActionSection("Output")]
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("Json formatted string")]
		public FsmString jsonSource;

		[ActionSection("Result")]
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when elements are added")]
		public FsmEvent successEvent;
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Failure event - The event to trigger when elements exists already or an error")]
		public FsmEvent failureEvent;
	
		private int count;
		

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			successEvent = null;
			failureEvent = null;
			jsonSource  = null;
		
		}
		
		
		public override void OnEnter()
		{
			if (SetUpHashTableProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value))
			{
				
				AddToHashTable();
				Fsm.Event(successEvent);
			}
			
			Finish();
		}
		
		
		public void AddToHashTable()
		{

			
			if (!isProxyValid()) {
				Fsm.Event(failureEvent);	
				Debug.LogWarning("<b>[HashTableAddFromJson]</b><color=#6B8E23ff>Please review!</color>", this.Owner);
				return;
				
			}

			Convert();

			if (count == null) {
				Fsm.Event(failureEvent);	
				Debug.LogWarning("<b>[HashTableAddFromJson]</b><color=#6B8E23ff>Please review!</color>", this.Owner);
				return;
				
			}
						

			Fsm.Event(successEvent);
			Finish();


		}

		public void Convert()
		{
		
			Hashtable d = new Hashtable();

			foreach(var key in proxy.hashTable.Keys){
		
				d.Add(key, proxy.hashTable[key]);
			}

			jsonSource.Value = JsonSimple.Serialize(d);

			return;
		
		}




			public class JsonSimple {

			char[] IntToChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			
			string m_stringGenerated = "";
			bool m_isStringGenerated = false;
			
			char[] m_chars = new char[32];
			int m_charsCount = 0;
			int m_charsCapacity = 0;


			public static string Serialize(object obj) {
				return Serializer.Serialize(obj);
			}
			
			sealed class Serializer {
				StringBuilder builder;
				
				Serializer() {
					builder = new StringBuilder();
				}
				
				public static string Serialize(object obj) {
					var instance = new Serializer();
					
					instance.SerializeValue(obj);
					
					return instance.builder.ToString();
				}
				
				void SerializeValue(object value) {
					IList asList;
					IDictionary asDict;
					string asStr;
					
					if (value == null) {
						builder.Append("null");
					} else if ((asStr = value as string) != null) {
						SerializeString(asStr);
					} else if (value is bool) {
						builder.Append((bool) value ? "true" : "false");
					} else if ((asList = value as IList) != null) {
						SerializeArray(asList);
					} else if ((asDict = value as IDictionary) != null) {
						SerializeObject(asDict);
					} else if (value is char) {
						SerializeString(new string((char) value, 1));
					} else {
						SerializeOther(value);
					}
				}
				
				void SerializeObject(IDictionary obj) {
					bool first = true;
					
					builder.Append('{');
					
					foreach (object e in obj.Keys) {
						if (!first) {
							builder.Append(',');
						}
						
						SerializeString(e.ToString());
						builder.Append(':');
						
						SerializeValue(obj[e]);
						
						first = false;
					}
					
					builder.Append('}');
				}
				
				void SerializeArray(IList anArray) {
					builder.Append('[');
					
					bool first = true;
					
					foreach (object obj in anArray) {
						if (!first) {
							builder.Append(',');
						}
						
						SerializeValue(obj);
						
						first = false;
					}
					
					builder.Append(']');
				}
				
				void SerializeString(string str) {
					builder.Append('\"');
					
					char[] charArray = str.ToCharArray();
					foreach (var c in charArray) {
						switch (c) {
						case '"':
							builder.Append("\\\"");
							break;
						case '\\':
							builder.Append("\\\\");
							break;
						case '\b':
							builder.Append("\\b");
							break;
						case '\f':
							builder.Append("\\f");
							break;
						case '\n':
							builder.Append("\\n");
							break;
						case '\r':
							builder.Append("\\r");
							break;
						case '\t':
							builder.Append("\\t");
							break;
						default:
							int codepoint = System.Convert.ToInt32(c);
							if ((codepoint >= 32) && (codepoint <= 126)) {
								builder.Append(c);
							} else {
								builder.Append("\\u");
								builder.Append(codepoint.ToString("x4"));
							}
							break;
						}
					}
					
					builder.Append('\"');
				}
				
				void SerializeOther(object value) {
					if (value is float) {
						builder.Append(((float) value).ToString("R"));
					} else if (value is int
					           || value is uint
					           || value is long
					           || value is sbyte
					           || value is byte
					           || value is short
					           || value is ushort
					           || value is ulong) {
						builder.Append(value);
					} else if (value is double
					           || value is decimal) {
						builder.Append(System.Convert.ToDouble(value).ToString("R"));
					} else {
						SerializeString(value.ToString());
					}
				}
			}

			
			
			public bool IsEmpty()
			{
				return (m_isStringGenerated ? (m_stringGenerated == null) : (m_charsCount == 0));
			}
			
			public override string ToString()
			{
				if( !m_isStringGenerated )
				{
					m_stringGenerated = new string( m_chars, 0, m_charsCount );
					m_isStringGenerated = true;
				}
				return m_stringGenerated;
			}
			
			
			public void Clear()
			{
				m_charsCount = 0;
				m_isStringGenerated = false;
			}
			
			
			public void Append( string value )
			{
				ReallocateIFN( value.Length );
				int n = value.Length;
				for( int i=0; i<n; i++ )
					m_chars[ m_charsCount + i ] = value[ i ];
				m_charsCount += n;
				m_isStringGenerated = false;
			}

			public void Append( object value )
			{
				string str = value.ToString();
				Append( str );
			}
			
			public void Append( int value )
			{
				
				ReallocateIFN( 16 );
				
				
				if( value < 0 )
				{
					value = -value;
					m_chars[ m_charsCount++ ] = '-';
				}
				
				
				int nbChars = 0;
				do
				{
					m_chars[ m_charsCount++ ] = IntToChar[ value%10 ];
					value /= 10;
					nbChars++;
				} while( value != 0 );
				
				
				for( int i=nbChars/2-1; i>=0; i-- )
				{
					char c = m_chars[ m_charsCount-i-1 ];
					m_chars[ m_charsCount-i-1 ] = m_chars[ m_charsCount-nbChars+i ];
					m_chars[ m_charsCount-nbChars+i ] = c;
				}
				m_isStringGenerated = false;
			}
			
			
			public void Append( float value )
			{
				
				ReallocateIFN( 16 );
				
				
				if( value < 0 )
				{
					value = -value;
					m_chars[ m_charsCount++ ] = '-';
				}
				
				
				int nbFloatDigits = 0;
				while( Mathf.Abs( value-Mathf.Round( value ) ) > 0.001f )
				{
					value *= 10;
					nbFloatDigits++;
				}
				int valueInt = Mathf.RoundToInt( value );
				
				
				int nbChars = 0;
				do
				{
					m_chars[ m_charsCount++ ] = IntToChar[ valueInt%10 ];
					valueInt /= 10;
					nbChars++;
					if( nbFloatDigits == nbChars ) 
					{
						m_chars[ m_charsCount++ ] = '.';
						nbChars++;
					}
				} while( valueInt != 0 || nbChars <= nbFloatDigits+1 );
				
				
				for( int i=nbChars/2-1; i>=0; i-- )
				{
					char c = m_chars[ m_charsCount-i-1 ];
					m_chars[ m_charsCount-i-1 ] = m_chars[ m_charsCount-nbChars+i ];
					m_chars[ m_charsCount-nbChars+i ] = c;
				}
				m_isStringGenerated = false;
			}
			
			
			private void ReallocateIFN( int nbCharsToAdd )
			{
				if( m_charsCount + nbCharsToAdd > m_charsCapacity )
				{
					Debug.Log( "- CString reallocation " + m_charsCapacity + "=>" + System.Math.Max( m_charsCapacity + nbCharsToAdd, m_charsCapacity * 2 ) );
					m_charsCapacity = System.Math.Max( m_charsCapacity + nbCharsToAdd, m_charsCapacity * 2 );
					char[] newChars = new char[ m_charsCapacity ];
					m_chars.CopyTo( newChars, 0 );
					m_chars = newChars;
				}
			}
			//
				
			}
		}

	}
