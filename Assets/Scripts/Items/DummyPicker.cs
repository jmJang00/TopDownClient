using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
	/// <summary>
	/// A Stimpack / health bonus, that gives health back when picked
	/// </summary>
	[AddComponentMenu("TopDown Engine/Items/Stimpack")]
	public class DummyPicker : PickableItem
	{
		[Header("DummyPicker")]		
		/// if this is true, only player characters can pick this up
		[Tooltip("if this is true, only player characters can pick this up")]
		public bool OnlyForPlayerCharacter = true;


        public override void PickItem(GameObject picker)
        {
            Effects();
            Pick(picker);
        }

		/// <summary>
		/// Triggered when something collides with the stimpack
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker)
		{			
            //TODO :: Override To Each Picker			 
		}
	}
}