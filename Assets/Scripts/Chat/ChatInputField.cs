using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class ChatInputField : MonoBehaviour , IInputModeChangeable
    {
        [SerializeField]
        private GameObject inputPanel;

        [SerializeField]
        private TMP_Text channelText;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private ChatVisibilityController chatVisibleController;       

        public bool IsTyping { get; private set; }

        private static ChatChannel[] _chatChannelCycle =
        {
            ChatChannel.Normal,
            ChatChannel.Party
        };

        private int _channelMode = 0;
        private ChatChannel _chatChannel = _chatChannelCycle[0];

        private void NextChannel()
        {
            _channelMode = (_channelMode + 1) % _chatChannelCycle.Length;
            _chatChannel = _chatChannelCycle[_channelMode];
        }

        private void Update()
        {
            if (!IsTyping)
            {
                if (InputModeManager.Instance.CurrentMode != InputMode.Game)
                    return;

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    
                    if (!InputModeManager.Instance.Enter(InputMode.Chat, this))
                        return;


                    OpenInputField();
                    UpdateChannelText();
                    return;
                }                                                
            }


            if (InputModeManager.Instance.CurrentMode != InputMode.Chat)
            {
                return;
            }

           

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextChannel();
                UpdateChannelText();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInputField();
                InputModeManager.Instance.Release(this);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendChatMessage();
                InputModeManager.Instance.Release(this);
                return;
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

        private void UpdateChannelText()
        {
            switch (_chatChannel)
            {
                case ChatChannel.Normal:
                {
                    channelText.text = "[전체]";
                    channelText.color = Color.cyan;
                    inputField.textComponent.color = Color.cyan;
                }
                    break;
                case ChatChannel.Party:
                {
                    channelText.text = "[그룹]";
                    channelText.color = Color.green;
                    inputField.textComponent.color = Color.green;
                }
                    break;
            }
            
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
                //ChatManager.Instance.MessageTest(msg, _chatChannel);
                ChatManager.Instance.SendChatMessage(msg, _chatChannel);
            }
        }

        public InputModeChangeableInfo GetInputModeInfo()
        {
            InputModeChangeableInfo info;
            info.Name = "Chat";
            info.Description = "Chating UI";
            return info;
        }
    }
}
