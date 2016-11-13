// License: Attribution 4.0 International (CC BY 4.0)
/*--- __ECO__ __PLAYMAKER__ __ACTION__ ---*/
// Keywords: crayon


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Add 120 crayon colors to a PlayMaker HashTable Proxy component (PlayMakerHashTableProxy).")]
	[HelpUrl("http://www.colourlovers.com/web/blog/2008/04/22/all-120-crayon-names-color-codes-and-fun-facts")]
	public class HashTableSet120CrayonColors : HashTableActions
	{
		
		[ActionSection("Set up")]

		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;


		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent doneEvent;
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event if error")]
		public FsmEvent failureEvent;


		public override void Reset()
		{
			gameObject = null;
			reference = null;
		}
		
		
		public override void OnEnter()
		{
			if (SetUpHashTableProxyPointer(Fsm.GetOwnerDefaultTarget(gameObject),reference.Value))
			{
					AddToHashTable();

			}
			Finish();
		}
		
		
		public void AddToHashTable()
		{

			if (!isProxyValid()) {
				Fsm.Event(failureEvent);	
				Debug.LogWarning("<color=#6B8E23ff>Please review!</color>", this.Owner);
				return;

			}

			Dictionary <String, Color> dict = new Dictionary<string, Color>();

			dict.Add ("Mahogany",new Color(205,74,74));
			dict.Add ("Fuzzy Wuzzy Brown",new Color(204,102,102));
			dict.Add ("Chestnut",new Color(188,93,88));
			dict.Add ("Red Orange",new Color(255,83,73));
			dict.Add ("Sunset Orange",new Color(253,94,83));
			dict.Add ("Bittersweet",new Color(253,124,110));
			dict.Add ("Melon",new Color(253,188,180));
			dict.Add ("Outrageous Orange",new Color(255,110,74));
			dict.Add ("Vivid Tangerine",new Color(255,160,137));
			dict.Add ("Burnt Sienna",new Color(234,126,93));
			dict.Add ("Brown",new Color(180,103,77));
			dict.Add ("Sepia",new Color(165,105,79));
			dict.Add ("Orange",new Color(255,117,56));
			dict.Add ("Burnt Orange",new Color(255,127,73));
			dict.Add ("Copper",new Color(221,148,117));
			dict.Add ("Mango Tango",new Color(255,130,67));
			dict.Add ("Atomic Tangerine",new Color(255,164,116));
			dict.Add ("Beaver",new Color(159,129,112));
			dict.Add ("Antique Brass",new Color(205,149,117));
			dict.Add ("Desert Sand",new Color(239,205,184));
			dict.Add ("Raw Sienna",new Color(214,138,89));
			dict.Add ("Tumbleweed",new Color(222,170,136));
			dict.Add ("Tan",new Color(250,167,108));
			dict.Add ("Peach",new Color(255,207,171));
			dict.Add ("Macaroni and Cheese",new Color(255,189,136));
			dict.Add ("Apricot",new Color(253,217,181));
			dict.Add ("Neon Carrot",new Color(255,163,67));
			dict.Add ("Almond",new Color(239,219,197));
			dict.Add ("Yellow Orange",new Color(255,182,83));
			dict.Add ("Gold",new Color(231,198,151));
			dict.Add ("Shadow",new Color(138,121,93));
			dict.Add ("Banana Mania",new Color(250,231,181));
			dict.Add ("Sunglow",new Color(255,207,72));
			dict.Add ("Goldenrod",new Color(252,217,117));
			dict.Add ("Dandelion",new Color(253,219,109));
			dict.Add ("Yellow",new Color(252,232,131));
			dict.Add ("Green Yellow",new Color(240,232,145));
			dict.Add ("Spring Green",new Color(236,234,190));
			dict.Add ("Olive Green",new Color(186,184,108));
			dict.Add ("Laser Lemon",new Color(253,252,116));
			dict.Add ("Unmellow Yellow",new Color(253,252,116));
			dict.Add ("Canary",new Color(255,255,153));
			dict.Add ("Yellow Green",new Color(197,227,132));
			dict.Add ("Inch Worm",new Color(178,236,93));
			dict.Add ("Asparagus",new Color(135,169,107));
			dict.Add ("Granny Smith Apple",new Color(168,228,160));
			dict.Add ("Electric Lime",new Color(29,249,20));
			dict.Add ("Screamin Green",new Color(118,255,122));
			dict.Add ("Fern",new Color(113,188,120));
			dict.Add ("Forest Green",new Color(109,174,129));
			dict.Add ("Sea Green",new Color(159,226,191));
			dict.Add ("Green",new Color(28,172,120));
			dict.Add ("Mountain Meadow",new Color(48,186,143));
			dict.Add ("Shamrock",new Color(69,206,162));
			dict.Add ("Jungle Green",new Color(59,176,143));
			dict.Add ("Caribbean Green",new Color(28,211,162));
			dict.Add ("Tropical Rain Forest",new Color(23,128,109));
			dict.Add ("Pine Green",new Color(21,128,120));
			dict.Add ("Robin Egg Blue",new Color(31,206,203));
			dict.Add ("Aquamarine",new Color(120,219,226));
			dict.Add ("Turquoise Blue",new Color(119,221,231));
			dict.Add ("Sky Blue",new Color(128,218,235));
			dict.Add ("Outer Space",new Color(65,74,76));
			dict.Add ("Blue Green",new Color(25,158,189));
			dict.Add ("Pacific Blue",new Color(28,169,201));
			dict.Add ("Cerulean",new Color(29,172,214));
			dict.Add ("Cornflower",new Color(154,206,235));
			dict.Add ("Midnight Blue",new Color(26,72,118));
			dict.Add ("Navy Blue",new Color(25,116,210));
			dict.Add ("Denim",new Color(43,108,196));
			dict.Add ("Blue",new Color(31,117,254));
			dict.Add ("Periwinkle",new Color(197,208,230));
			dict.Add ("Cadet Blue",new Color(176,183,198));
			dict.Add ("Indigo",new Color(93,118,203));
			dict.Add ("Wild Blue Yonder",new Color(162,173,208));
			dict.Add ("Manatee",new Color(151,154,170));
			dict.Add ("Blue Bell",new Color(173,173,214));
			dict.Add ("Blue Violet",new Color(115,102,189));
			dict.Add ("Purple Heart",new Color(116,66,200));
			dict.Add ("Royal Purple",new Color(120,81,169));
			dict.Add ("Purple Mountains’ Majesty",new Color(157,129,186));
			dict.Add ("Violet (Purple)",new Color(146,110,174));
			dict.Add ("Wisteria",new Color(205,164,222));
			dict.Add ("Vivid Violet",new Color(143,80,157));
			dict.Add ("Fuchsia",new Color(195,100,197));
			dict.Add ("Shocking Pink",new Color(251,126,253));
			dict.Add ("Pink Flamingo",new Color(252,116,253));
			dict.Add ("Plum",new Color(142,69,133));
			dict.Add ("Hot Magenta",new Color(255,29,206));
			dict.Add ("Purple Pizzazz",new Color(255,29,206));
			dict.Add ("Razzle Dazzle Rose",new Color(255,72,208));
			dict.Add ("Orchid",new Color(230,168,215));
			dict.Add ("Red Violet",new Color(192,68,143));
			dict.Add ("Eggplant",new Color(110,81,96));
			dict.Add ("Cerise",new Color(221,68,146));
			dict.Add ("Wild Strawberry",new Color(255,67,164));
			dict.Add ("Magenta",new Color(246,100,175));
			dict.Add ("Lavender",new Color(252,180,213));
			dict.Add ("Cotton Candy",new Color(255,188,217));
			dict.Add ("Violet Red",new Color(247,83,148));
			dict.Add ("Carnation Pink",new Color(255,170,204));
			dict.Add ("Razzmatazz",new Color(227,37,107));
			dict.Add ("Piggy Pink",new Color(253,215,228));
			dict.Add ("Jazzberry Jam",new Color(202,55,103));
			dict.Add ("Blush",new Color(222,93,131));
			dict.Add ("Tickle Me Pink",new Color(252,137,172));
			dict.Add ("Pink Sherbet",new Color(247,128,161));
			dict.Add ("Maroon",new Color(200,56,90));
			dict.Add ("Red",new Color(238,32,77));
			dict.Add ("Radical Red",new Color(255,73,108));
			dict.Add ("Mauvelous",new Color(239,152,170));
			dict.Add ("Wild Watermelon",new Color(252,108,133));
			dict.Add ("Scarlet",new Color(252,40,71));
			dict.Add ("Salmon",new Color(255,155,170));
			dict.Add ("Brick Red",new Color(203,65,84));
			dict.Add ("White",new Color(237,237,237));
			dict.Add ("Timberwolf",new Color(219,215,210));
			dict.Add ("Silver",new Color(205,197,194));
			dict.Add ("Gray",new Color(149,145,140));
			dict.Add ("Black",new Color(35,35,35));

		
			proxy.hashTable.Clear();

			foreach(var key in dict.Keys)
			{
				var itemKey = key;
				Color item = dict[key];
				item = new Color (item.r/255,item.g/255,item.b/255);
				proxy.hashTable.Add(itemKey,item);
				
			}

			dict = new Dictionary<string, Color>();

			Fsm.Event(doneEvent);


			
		}
		
	}
}
