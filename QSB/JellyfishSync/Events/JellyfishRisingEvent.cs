﻿using QSB.Events;
using QSB.JellyfishSync.WorldObjects;
using QSB.WorldSync;
using QSB.WorldSync.Events;

namespace QSB.JellyfishSync.Events
{
	public class JellyfishRisingEvent : QSBEvent<BoolWorldObjectMessage>
	{
		public override void SetupListener()
			=> GlobalMessenger<QSBJellyfish>.AddListener(EventNames.QSBJellyfishRising, Handler);

		public override void CloseListener()
			=> GlobalMessenger<QSBJellyfish>.RemoveListener(EventNames.QSBJellyfishRising, Handler);

		private void Handler(QSBJellyfish qsbJellyfish) => SendEvent(CreateMessage(qsbJellyfish));

		private BoolWorldObjectMessage CreateMessage(QSBJellyfish qsbJellyfish) => new()
		{
			ObjectId = qsbJellyfish.ObjectId,
			State = qsbJellyfish.IsRising
		};

		public override void OnReceiveRemote(bool isHost, BoolWorldObjectMessage message)
		{
			if (!WorldObjectManager.AllReady)
			{
				return;
			}

			var qsbJellyfish = QSBWorldSync.GetWorldFromId<QSBJellyfish>(message.ObjectId);
			qsbJellyfish.IsRising = message.State;
		}
	}
}