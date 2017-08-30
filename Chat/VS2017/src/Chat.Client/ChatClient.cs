using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Chat.Messages;
using UnityEngine;

namespace Chat.Client.Unity
{
    public class ChatClient : IChatBridge
    {
        public Action<string> OnChatMessage { get; set; }
        public Action<string> OnStatusMessage { get; set; }
        public Action<string> OnLogMessage { get; set; }

        private readonly ActorSystem _system;
        private readonly IActorRef _chatClient;

        public ChatClient(string userName, string serverAddress)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
		    port = 0
		    hostname = localhost
        }
    }
}"
);
            _system = ActorSystem.Create("MyClient", config);
            _chatClient = _system.ActorOf(ChatClientActor.Props(this, serverAddress), "ChatClient");

            _chatClient.Tell(new ConnectRequest()
            {
                Username = userName,
            });
        }

        public void SubmitChat(string input)
        {
            if (input.StartsWith("/"))
            {
                var parts = input.Split(' ');
                var cmd = parts[0].ToLowerInvariant();
                var rest = string.Join(" ", parts.Skip(1));

                if (cmd == "/nick")
                {
                    _chatClient.Tell(new NickRequest
                    {
                        NewUsername = rest
                    });
                }
                return;
            }

            _chatClient.Tell(new SayRequest()
            {
                Text = input,
            });
        }

        public void Shutdown()
        {
            _system.Terminate().Wait();
        }

    }

    class ChatClientActor : ReceiveActor, ILogReceive
    {
        private string _nick = "Roggan";

        public static Props Props(IChatBridge bridge, string serverAddress) =>
            Akka.Actor.Props.Create(() => new ChatClientActor(bridge, serverAddress));

        public ChatClientActor(IChatBridge bridge, string serverAddress)
        {
            var server = Context.ActorSelection($"akka.tcp://MyServer@{serverAddress}:8081/user/ChatServer");

            Receive<ConnectResponse>(message =>
            {
                bridge.OnStatusMessage("Connected!");
                bridge.OnStatusMessage(message.Message);
            });

            Receive<NickRequest>(message =>
            {
                message.OldUsername = _nick;
                bridge.OnStatusMessage($"Changing nick to {message.NewUsername}");
                _nick = message.NewUsername;
                server.Tell(message);
            });

            Receive<NickResponse>(message =>
                bridge.OnStatusMessage($"{message.OldUsername} is now known as {message.NewUsername}")
            );

            Receive<SayResponse>(message =>
                bridge.OnChatMessage($"{message.Username}: {message.Text}")
            );

            Receive<ConnectRequest>(message =>
            {
                bridge.OnStatusMessage("Connecting....");
                _nick = message.Username;
                server.Tell(message);
            });

            Receive<SayRequest>(message =>
            {
                message.Username = _nick;
                server.Tell(message);
            });

            Receive<Terminated>(message => bridge.OnStatusMessage("Server died"));

            ReceiveAny(message => bridge.OnLogMessage(message.ToString()));
        }
    }
}

