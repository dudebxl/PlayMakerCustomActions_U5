// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// source: https://gist.github.com/darktable/1411710
// Keywords: HashTable from Json

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Build a Hashtable from JSON format String - Note: PlayMaker HashTable Proxy component (PlayMakerHashTableProxy) needs to be set to string")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=11991.0")]
    public class HashTableAddFromJson : HashTableActions
	{
		
		[ActionSection("Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;
		
		[ActionSection("Input")]
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
		private Type type;



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
				Debug.LogWarning("<b>[HashTableAddFromJson]</b><color=#FF9900ff>  Please review!</color>", this.Owner);
				return;
				
			}



			proxy.hashTable.Clear();
			Convert();

			if (count == null) {
				Fsm.Event(failureEvent);	
				Debug.LogWarning("<b>[HashTableAddFromJson]</b><color=#FF9900ff>  Please review!</color>", this.Owner);
				return;
				
			}
						

			Fsm.Event(successEvent);
			Finish();


		}

		public void Convert()
		{
		
			count = 0;
			ParseJSON( 0, JsonSimple.Deserialize( jsonSource.Value ) as Dictionary <string, object>, ref count );
					return;
		}


		private void ParseJSON( int level, Dictionary<string, object> currentDict, ref int count )
		{
					if( currentDict != null )
			{
						foreach( KeyValuePair < string, object > pair in currentDict )
				{
					if( pair.Value is Dictionary < string, object > ) {
						ParseJSON( level + 1, pair.Value as Dictionary < string, object >, ref count );
					} else {
						proxy.hashTable.Add(pair.Key,System.Convert.ToString(pair.Value));
						count++;
					}
				}
			}

		}

		public class JsonSimple {

			char[] IntToChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

			string m_stringGenerated = "";
			bool m_isStringGenerated = false;
			
			char[] m_chars = new char[32];
			int m_charsCount = 0;
			int m_charsCapacity = 0;

			public static object Deserialize(string json) {
		
				if (json == null) {
					return null;
				}
				
				return Parser.Parse(json);
			}
			
			sealed class Parser : IDisposable {
				const string WORD_BREAK = "{}[],:\"";
				
				public static bool IsWordBreak(char c) {
					return Char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
				}
				
				enum TOKEN {
					NONE,
					CURLY_OPEN,
					CURLY_CLOSE,
					SQUARED_OPEN,
					SQUARED_CLOSE,
					COLON,
					COMMA,
					STRING,
					NUMBER,
					TRUE,
					FALSE,
					NULL
				};
				
				StringReader json;
				
				Parser(string jsonString) {
					json = new StringReader(jsonString);
				}
				
				public static object Parse(string jsonString) {
					using (var instance = new Parser(jsonString)) {
						return instance.ParseValue();
					}
				}
				
				public void Dispose() {
					json.Dispose();
					json = null;
				}
				
				Dictionary<string, object> ParseObject() {
					Dictionary<string, object> table = new Dictionary<string, object>();
					
				
					json.Read();
					
			
					while (true) {
						switch (NextToken) {
						case TOKEN.NONE:
							return null;
						case TOKEN.COMMA:
							continue;
						case TOKEN.CURLY_CLOSE:
							return table;
						default:

							string name = ParseString();
							if (name == null) {
								return null;
							}
							

							if (NextToken != TOKEN.COLON) {
								return null;
							}

							json.Read();
							

							table[name] = ParseValue();
							break;
						}
					}
				}
				
				List<object> ParseArray() {
					List<object> array = new List<object>();
					

					json.Read();
					
				
					var parsing = true;
					while (parsing) {
						TOKEN nextToken = NextToken;
						
						switch (nextToken) {
						case TOKEN.NONE:
							return null;
						case TOKEN.COMMA:
							continue;
						case TOKEN.SQUARED_CLOSE:
							parsing = false;
							break;
						default:
							object value = ParseByToken(nextToken);
							
							array.Add(value);
							break;
						}
					}
					
					return array;
				}
				
				object ParseValue() {
					TOKEN nextToken = NextToken;
					return ParseByToken(nextToken);
				}
				
				object ParseByToken(TOKEN token) {
					switch (token) {
					case TOKEN.STRING:
						return ParseString();
					case TOKEN.NUMBER:
						return ParseNumber();
					case TOKEN.CURLY_OPEN:
						return ParseObject();
					case TOKEN.SQUARED_OPEN:
						return ParseArray();
					case TOKEN.TRUE:
						return true;
					case TOKEN.FALSE:
						return false;
					case TOKEN.NULL:
						return null;
					default:
						return null;
					}
				}
				
				string ParseString() {

					StringBuilder s = new StringBuilder();
					char c;
					
				
					json.Read();
					
					bool parsing = true;
					while (parsing) {
						
						if (json.Peek() == -1) {
							parsing = false;
							break;
						}
						
						c = NextChar;
						switch (c) {
						case '"':
							parsing = false;
							break;
						case '\\':
							if (json.Peek() == -1) {
								parsing = false;
								break;
							}


							c = NextChar;
							switch (c) {
							case '"':
							case '\\':
							case '/':
								s.Append(c);
								break;
							case 'b':
								s.Append('\b');
								break;
							case 'f':
								s.Append('\f');
								break;
							case 'n':
								s.Append('\n');
								break;
							case 'r':
								s.Append('\r');
								break;
							case 't':
								s.Append('\t');
								break;
							case 'u':
								var hex = new char[4];
								
								for (int i=0; i< 4; i++) {
									hex[i] = NextChar;
								}
								
								s.Append((char) System.Convert.ToInt32(new string(hex), 16));
								break;
							}
							break;
						default:
							s.Append(c);
							break;
						}
					}
					
					return s.ToString();
				}
				
				object ParseNumber() {
					string number = NextWord;
					
					if (number.IndexOf('.') == -1) {
						long parsedInt;
						Int64.TryParse(number, out parsedInt);
						return parsedInt;
					}
					
					double parsedDouble;
					Double.TryParse(number, out parsedDouble);
					return parsedDouble;
				}
				
				void EatWhitespace() {
					while (Char.IsWhiteSpace(PeekChar)) {
						json.Read();
						
						if (json.Peek() == -1) {
							break;
						}
					}
				}
				
				char PeekChar {
					get {
						return System.Convert.ToChar(json.Peek());
					}
				}
				
				char NextChar {
					get {
						return System.Convert.ToChar(json.Read());
					}
				}
				
				string NextWord {
					get {
						StringBuilder word = new StringBuilder();
						
						while (!IsWordBreak(PeekChar)) {
							word.Append(NextChar);
							
							if (json.Peek() == -1) {
								break;
							}
						}
						
						return word.ToString();
					}
				}
				
				TOKEN NextToken {
					get {
						EatWhitespace();
						
						if (json.Peek() == -1) {
							return TOKEN.NONE;
						}
						
						switch (PeekChar) {
						case '{':
							return TOKEN.CURLY_OPEN;
						case '}':
							json.Read();
							return TOKEN.CURLY_CLOSE;
						case '[':
							return TOKEN.SQUARED_OPEN;
						case ']':
							json.Read();
							return TOKEN.SQUARED_CLOSE;
						case ',':
							json.Read();
							return TOKEN.COMMA;
						case '"':
							return TOKEN.STRING;
						case ':':
							return TOKEN.COLON;
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						case '-':
							return TOKEN.NUMBER;
						}
						
						switch (NextWord) {
						case "false":
							return TOKEN.FALSE;
						case "true":
							return TOKEN.TRUE;
						case "null":
							return TOKEN.NULL;
						}
						
						return TOKEN.NONE;
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

