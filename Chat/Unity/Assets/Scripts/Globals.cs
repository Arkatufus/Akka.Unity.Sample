using UnityEngine;

namespace Akka.Chat.Client
{
    public class Globals:Singleton<Globals>
    {
        [HideInInspector]
        public string ServerAddress;
    }
}
