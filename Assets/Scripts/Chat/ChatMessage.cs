using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Chat
{
    public enum ChatChannel
    {
        Normal,
        Party,
        System
    }
    

    public class ChatMessage
    {
        public ChatChannel Channel;

        public string Sender;

        public string Message;
    }
}
