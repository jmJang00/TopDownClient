using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Chat
{
    public class ChatItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;

        public void Set(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Normal:
                    text.color = Color.cyan;
                    break;

                case ChatChannel.Party:
                    text.color = Color.green;
                    break;  

                case ChatChannel.System:
                    text.color = Color.yellow;
                    break;
            }

            text.text = $"[{message.Sender}] {message.Message}";
        }
    }
}
