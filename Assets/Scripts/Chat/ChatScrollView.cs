using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class ChatScrollView : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private ChatPool pool;

        [SerializeField]
        private ChatVisibilityController chatVisibleController;

        private readonly Queue<ChatItem> visible = new();

        private const int MaxMessage = 100;

        public void Add(ChatMessage message)
        {
            ChatItem item = pool.Get();

            item.transform.SetParent(content, false);

            item.Set(message);

            visible.Enqueue(item);

            if (visible.Count > MaxMessage)
            {
                ChatItem old = visible.Dequeue();

                pool.Release(old);
            }

            Canvas.ForceUpdateCanvases();

            scrollRect.verticalNormalizedPosition = 0;

            chatVisibleController.ShowAndHideAfter(3f);
        }

        private void OnEnable()
        {
            ChatManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            ChatManager.Instance.Unregister(this);
        }
    }
}
