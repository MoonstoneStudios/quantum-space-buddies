﻿using QSB.Messaging;

namespace QSB.SatelliteSync.Messages
{
	internal class SatelliteProjectorMessage : QSBMessage<bool>
	{
		public SatelliteProjectorMessage(bool usingProjector) => Data = usingProjector;

		public override void OnReceiveRemote()
		{
			if (Data)
			{
				SatelliteProjectorManager.Instance.RemoteEnter();
			}
			else
			{
				SatelliteProjectorManager.Instance.RemoteExit();
			}
		}
	}
}