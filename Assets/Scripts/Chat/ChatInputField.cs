using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class ChatInputField : MonoBehaviour
    {
        [SerializeField]
        private GameObject inputPanel;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private ChatVisibilityController chatVisibleController;

        public bool IsTyping { get; private set; }

        private void Update()
        {
            if (!IsTyping)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    OpenInputField();
                }

                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInputField();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendChatMessage();
            }
        }


        private void OpenInputField()
        {
            inputPanel.SetActive(true);

            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();

            IsTyping = true;
            chatVisibleController.Show();
        }

        private void CloseInputField()
        {
            IsTyping = false;

            inputField.text = "";
            inputPanel.SetActive(false);
            chatVisibleController.HideAfter(3f);
        }

        private void SendChatMessage()
        {
            string msg = inputField.text.Trim();

            CloseInputField();

            if (msg.Length > 0)
            {
                ChatManager.Instance.MessageTest(msg);
                //ChatManager.Instance.SendChatMessage(msg);
            }
        }
    }
}
