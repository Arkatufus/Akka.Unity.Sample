using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Akka.Chat.Client
{
    public class MainMenuUi:MonoBehaviour
    {
        public InputField ServerAddress;
        public Button OkButton;

        private void Awake()
        {
            OkButton.interactable = false;

            ServerAddress.onValueChanged.AddListener(value =>
            {
                OkButton.interactable = false;
                if (value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4)
                {
                    IPAddress _;
                    OkButton.interactable = IPAddress.TryParse(value, out _);
                }
            });

            OkButton.onClick.AddListener(() =>
            {
                Globals.Instance.ServerAddress = ServerAddress.text;
                SceneManager.LoadScene(1, LoadSceneMode.Single);
            });
        }
    }
}
