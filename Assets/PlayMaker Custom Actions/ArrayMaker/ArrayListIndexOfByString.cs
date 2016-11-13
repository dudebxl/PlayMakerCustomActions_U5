// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: Search array by string

using UnityEngine;
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the index of an item from a PlayMaker Array List Proxy component. Can search within a range")]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=12044.0")]
	public class ArrayListIndexOfByString : ArrayListActions
	{
		
		[ActionSection("Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;
		
		[Tooltip("Optional start index to search from: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt startIndex;
		
		[Tooltip("Optional amount of elements to search within: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt count;

		[ActionSection("Data")]
		
		[RequiredField]
		[Tooltip("The variable to get the index of.")]
		public FsmString name;
		
		[ActionSection("Result")]
		
		[Tooltip("The index of the item described below")]
		[UIHint(UIHint.Variable)]
		public FsmInt indexOf;
			
		[Tooltip("Optional Event sent if this arrayList contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemFound;	

		[Tooltip("Optional Event sent if this arrayList does not contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemNotFound;
		
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Optional Event to trigger if the action fails ( likely an out of range exception when using wrong values for index and/or count)")]
		public FsmEvent failureEvent;
		
		private char[] m_chars = null;
		private int m_charsCount = 0;
		private int m_charsCapacity = 0;
		private List<char> m_replacement = null;
		private bool m_isStringGenerated = false;
		private string m_stringGenerated = "";

		public override void Reset()
		{
			gameObject = null;
			reference = null;
			startIndex = null;
			count = null;
			itemFound = null;
			itemNotFound = null;
			
			name = null;
			
		}
		
		
		public override void OnEnter()
		{
			if ( SetUpArrayListProxyPointer( Fsm.GetOwnerDefaultTarget(gameObject),reference.Value) )
				DoArrayListIndexOf();
			
			Finish();
		}
		
		
		public void DoArrayListIndexOf()
		{
			if (! isProxyValid() ) 
				return;

			int i = 0;
			int indexOfResult = -1;
			int length = proxy.arrayList.Count;
			m_chars = new char[ m_charsCapacity = 32 ];

			for (i = startIndex.Value; i < length; i++){

			object element = null;
			
			try{
				element = proxy.arrayList[i];
			}catch(System.Exception e){
				Debug.LogWarning("<b>[ArrayListIndexOfByString]</b><color=#FF9900ff>  Please review!</color>", this.Owner);
				Fsm.Event(failureEvent);
				return;
			}
				Clear ();
				string result = element.ToString();

				result = result.Remove(result.Length-25);

				if (name.Value == result)
				{
					indexOf.Value = i;
					Fsm.Event(itemFound);
					Finish();
				}

			}
				
			

			if (i == length-1){
				indexOf.Value = -1;
				Fsm.Event(itemNotFound);
				Finish();
		}
	}


		public void Clear()
		{
			m_charsCount = 0;
			m_isStringGenerated = false;
			return;
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

}
}
