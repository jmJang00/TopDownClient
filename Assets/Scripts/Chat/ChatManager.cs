using Assets.Scripts.Chat;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get { Init(); return _instance; } }
    private static ChatManager _instance;
    public event Action<ChatMessage> OnMessageReceived;

    int _maxHistory = 100;
    private readonly Queue<ChatMessage> _history = new();


    private void Awake()
    {
        Init();
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("ChatManager");
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<ChatManager>();
        }        
    }

    public void Register(ChatScrollView scrollView)
    {
        foreach (var msg in _history)
        {
            if (msg != null)
                scrollView.Add(msg);
        }

        OnMessageReceived += scrollView.Add;
    }

    public void Unregister(ChatScrollView scrollView)
    {
        OnMessageReceived -= scrollView.Add;
    }

    public void MessageTest(string message, ChatChannel channel)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Message = message;
        newMessage.Channel = channel;
        newMessage.Sender = "Me";

        ReceiveChatMessage(newMessage);
    }

    public bool SendChatMessage(string message, ChatChannel channel)
    {
        C_ChatMessage pkt = new C_ChatMessage();
        pkt.message = message;

        //NetworkManager.Instance.Send(pkt.Write());
        //TODO 이부분은 차후 채팅세션으로 보내도록 수정
        return true;
    }

    public void ReceiveChatMessage(ChatMessage msg)
    {
        if (_history.Count >= _maxHistory)
            _history.Dequeue();

        _history.Enqueue(msg);

        OnMessageReceived?.Invoke(msg);        
    }
}
