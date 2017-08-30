using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chat.Client.Unity;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Akka.Chat.Client
{
    public class ChatUi : MonoBehaviour
    {
        public string Username = "Roggan";
        public Text ChatText;
        public InputField TextInput;
        public Button SendButton;
        public ScrollRect ScrollRect;

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private ChatClient _client;

        #region Unity life cycle methods
        private void Awake()
        {
            SendButton.onClick.AddListener(SubmitChat);

            _client = new ChatClient(Username, Globals.Instance.ServerAddress)
            {
                OnChatMessage = OnChat,
                OnStatusMessage = OnStatus,
                OnLogMessage = OnLog
            };
        }

        private void OnDestroy()
        {
            _client.Shutdown();
        }

        private void Update()
        {
            if (_stringBuilder.Length == 0)
                return;

            ChatText.text += _stringBuilder.ToString();
            _stringBuilder.Clear();

            ScrollRect.verticalNormalizedPosition = 0;
            ScrollRect.CalculateLayoutInputVertical();
        }
        #endregion

        private void OnChat(string msg)
        {
            Debug.Log($"OnChat: {msg}");
            _stringBuilder.AppendLine(msg);
        }

        private void OnStatus(string msg)
        {
            Debug.Log($"OnStatus: {msg}");
            _stringBuilder.AppendLine($"<color=green>{msg}</color>");
        }

        private void OnLog(string msg)
        {
            Debug.Log(msg);
        }

        private void SubmitChat()
        {
            if (string.IsNullOrWhiteSpace(TextInput.text))
                return;

            _client.SubmitChat(TextInput.text);
            TextInput.text = "";
        }
    }
}
