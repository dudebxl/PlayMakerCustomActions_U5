// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Optimized StringBuilder ArrayList


using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Builds a String from Int from a PlayMaker ArrayList Proxy component with less memory allocations.")]
	[HelpUrl("")]
	public class ArrayListBuildIntToStringFast : ArrayListActions
	{
		[ActionSection("Set up")]
		
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		[UIHint(UIHint.FsmInt)]
		[Tooltip("The index to retrieve the item from")]
		public FsmInt startIndex;

		[Tooltip("The number of elements in the array.")]
		[TitleAttribute("Range")]
		public FsmInt length;

        [Tooltip("Separator to insert between each String. E.g. space character.")]
        public FsmString separator;

        [Tooltip("Add Separator to end of built string.")]
	    public FsmBool addToEnd;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the final String in a variable.")]
        public FsmString storeResult;


		[ActionSection("Event")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doneEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;

        private string result;
		private readonly char[] IntToChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		private char[] m_chars = null;
		private string m_stringGenerated = "";
		private bool m_isStringGenerated = false;
		private int m_charsCount = 0;
		private int m_charsCapacity = 0;
		private object element;
		private int range;
		private int intVariable;

		public override void Reset()
		{
			separator = null;
		    addToEnd = true;
			storeResult = null;
			startIndex = 0;
			length = 0;
			failureEvent = null;
			doneEvent= null;
		}

		public override void OnEnter()
		{
			if ( SetUpArrayListProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				DoBuildString();

			    Finish();
			
		}

		public override void OnUpdate()
		{
			DoBuildString();
		}
		
		void DoBuildString()
		{
			if (storeResult == null) return;
			
	
			if (! isProxyValid()) {
				Fsm.Event(failureEvent);			
				return;
			}

			element = null;
			
			try{
				element = proxy.arrayList[0];
			}catch(System.Exception){
				Debug.LogWarning("<color=#6B8E23ff>Please review!</color>", this.Owner);
				Fsm.Event(failureEvent);
				return;
			}

			if (element.GetType() != typeof(int) ){

				Debug.LogWarning("<color=#6B8E23ff>ArrayList is not a string array. Please review!</color>", this.Owner);
				Fsm.Event(failureEvent);			
				return;
			}

			System.Text.StringBuilder result = new System.Text.StringBuilder(32);
			int arrayLength = proxy.arrayList.Count;
			m_chars = new char[ m_charsCapacity = 32 ];
			Clear();
			intVariable= 0;

			range = proxy.arrayList.Count;

			if (length.Value > 0){


				int end = startIndex.Value + length.Value;

				if (end >= range)
				{
					Debug.LogWarning("<color=#6B8E23ff>Your range is above array max length. Please review!</color>", this.Owner);
					Fsm.Event(failureEvent);			
					return;
				}

				range = startIndex.Value + length.Value;

			}



			for (var i = startIndex.Value; i < range-1; i++)
		    {

				element = null;
				
				try{
					element = proxy.arrayList[i];
				}catch(System.Exception){
					Debug.LogWarning("<color=#6B8E23ff>Please review!</color>", this.Owner);
					Fsm.Event(failureEvent);
					return;
				}

				intVariable = System.Convert.ToInt32(element);
	
				Append(intVariable);
				Append(separator.Value);
		    }

			ToString();

			element = null;
			element = proxy.arrayList[range-1];
			intVariable = System.Convert.ToInt32(element);
			Append(intVariable);

		    if (addToEnd.Value)
		    {
				Append(separator.Value);
		    }

			

			storeResult.Value = ToString();
		
		}

		private void Append( int value )
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


		public void Append( string value )
		{
			ReallocateIFN( value.Length );
			int n = value.Length;
			for( int i=0; i<n; i++ )
				m_chars[ m_charsCount + i ] = value[ i ];
			m_charsCount += n;
			m_isStringGenerated = false;
		}

		private void ReallocateIFN( int nbCharsToAdd )
		{
			if( m_charsCount + nbCharsToAdd > m_charsCapacity )
			{
				m_charsCapacity = System.Math.Max( m_charsCapacity + nbCharsToAdd, m_charsCapacity * 2 );
				char[] newChars = new char[ m_charsCapacity ];
				m_chars.CopyTo( newChars, 0 );
				m_chars = newChars;
			}
		}


		public bool IsEmpty()
		{
			return (m_isStringGenerated ? (m_stringGenerated == null) : (m_charsCount == 0));
		}

		public void Clear()
		{
			m_charsCount = 0;
			m_isStringGenerated = false;
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

	}
}
