﻿using QSB.Messaging;
using QSB.TimeSync;

namespace QSB.Events
{
    class ServerTimeEvent : QSBEvent<ServerTimeMessage>
    {
        public override MessageType Type => MessageType.ServerTime;

        public override void SetupListener()
        {
            GlobalMessenger<float, int>.AddListener("QSBServerTime", (time, count) => SendEvent(
                new ServerTimeMessage { 
                    SenderId = PlayerRegistry.LocalPlayer.NetId,
                    ServerTime = time, 
                    LoopCount = count 
                }));
        }

        public override void OnReceiveRemote(ServerTimeMessage message)
        {
            WakeUpSync.LocalInstance.OnClientReceiveMessage(message);
        }
    }
}
