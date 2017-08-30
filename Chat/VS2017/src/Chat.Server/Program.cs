//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Chat.Messages;

namespace Chat.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        dot-netty.tcp {
            port = 8081
            hostname = 192.168.100.2
        }
    }
}"
);

            using (var system = ActorSystem.Create("MyServer", config))
            {
                system.ActorOf<ChatServerActor>("ChatServer");

                Console.ReadLine();
            }
        }
    }

    class ChatServerActor : ReceiveActor, ILogReceive
    {
        private readonly HashSet<IActorRef> _clients = new HashSet<IActorRef>();

        public ChatServerActor()
        {
            Receive<SayRequest>(message =>
            {
                Console.WriteLine("User {0} said {1}",message.Username , message.Text);
                var response = new SayResponse
                {
                    Username = message.Username,
                    Text = message.Text,
                };
                foreach (var client in _clients) client.Tell(response, Self);
            });

            Receive<ConnectRequest>(message =>
            {
                Console.WriteLine("User {0} has connected", message.Username);
                _clients.Add(Sender);
                Sender.Tell(new ConnectResponse
                {
                    Message = "Hello and welcome to Akka .NET chat example",
                }, Self);
            });

            Receive<NickRequest>(message =>
            {
                var response = new NickResponse
                {
                    OldUsername = message.OldUsername,
                    NewUsername = message.NewUsername,
                };

                foreach (var client in _clients) client.Tell(response, Self);
            });

            Receive<Disconnect>(message =>
            {
            });

            Receive<ChannelsRequest>(message =>
            {
            });

            ReceiveAny(obj =>
            {
                Console.WriteLine(obj.ToString());
            });
        }
    }
}

