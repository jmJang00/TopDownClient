using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Chat
{
    public class ChatPool : MonoBehaviour
    {
        [SerializeField]
        private ChatItem prefab;

        private Queue<ChatItem> pool = new();

        public void Initialize(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ChatItem item = Instantiate(prefab);

                item.gameObject.SetActive(false);

                pool.Enqueue(item);
            }
        }

        public ChatItem Get()
        {
            if (pool.Count > 0)
            {
                ChatItem item = pool.Dequeue();

                item.gameObject.SetActive(true);

                return item;
            }

            return Instantiate(prefab);
        }

        public void Release(ChatItem item)
        {
            item.gameObject.SetActive(false);

            pool.Enqueue(item);
        }
    }
}
