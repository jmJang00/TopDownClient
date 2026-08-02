using System;
using System.Collections;
using System.Linq;
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


        public int LastUpdateTick { get { return _lastUpdateTick; } }
        private int _lastUpdateTick;

        protected override void Awake()
        {
            base.Awake();
            SetItemList(DefaultItems);            
        }

        public void SetLastUpdateTick(int serverTick)
        {
            _lastUpdateTick = serverTick;
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
            SetItemList(items);
            //foreach (InventoryItem item in items)
            //{
            //    if (!this.IsFull)
            //    {
            //        AddItem(item, item.Quantity);
            //    }
            //}
        }        

        public virtual void SetItemList(InventoryItem[] items)
        {
            for(int i = 0; i < items.Count(); ++i)
            {
                Content[i] = items[i];
            }            

            MMInventoryEvent.Trigger(MMInventoryEventType.ContentChanged, null, this.name, null, 0, 0, PlayerID);
        }



    }
}