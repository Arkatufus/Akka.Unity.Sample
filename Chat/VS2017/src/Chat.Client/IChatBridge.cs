using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Client.Unity
{
    public interface IChatBridge
    {
        Action<string> OnChatMessage { get; set; }
        Action<string> OnStatusMessage { get; set; }
        Action<string> OnLogMessage { get; set; }
    }
}
