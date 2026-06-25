using System;
using System.Collections;
using UnityEngine;

namespace MoreMountains.InventoryEngine
{
    [Serializable]
    public class MyInventory : Inventory
    {
        [SerializeField]
        [Tooltip("This is default items list.")]
        public InventoryItem[] DefaultItems;
        public uint Index;
        public bool isPlayer = false;

        protected override void Awake()
        {
            base.Awake();
            foreach (InventoryItem item in DefaultItems)
            {
                AddItem(item, item.Quantity);
            }
        }

        public virtual void RemoveItemAll()
        {
            for(int i = 0; i < Content.Length; ++i)
            {
                if (Content[i] != null)
                {
                    DestroyItem(i);
                }
            }            
        }

        public virtual void SetInventoryFromItemArray(InventoryItem[] items)
        {
            RemoveItemAll();
            foreach(InventoryItem item in items)
            {
                if (!this.IsFull)
                {
                    AddItem(item, item.Quantity);
                }
            }
        }
    }
}