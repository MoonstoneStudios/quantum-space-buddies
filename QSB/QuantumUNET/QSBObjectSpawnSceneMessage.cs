﻿using UnityEngine;
using UnityEngine.Networking;

namespace QSB.QuantumUNET
{
	internal class QSBObjectSpawnSceneMessage : QSBMessageBase
	{
		public NetworkInstanceId NetId;
		public NetworkSceneId SceneId;
		public Vector3 Position;
		public byte[] Payload;

		public override void Deserialize(QSBNetworkReader reader)
		{
			NetId = reader.ReadNetworkId();
			SceneId = reader.ReadSceneId();
			Position = reader.ReadVector3();
			Payload = reader.ReadBytesAndSize();
		}

		public override void Serialize(QSBNetworkWriter writer)
		{
			writer.Write(NetId);
			writer.Write(SceneId);
			writer.Write(Position);
			writer.WriteBytesFull(Payload);
		}
	}
}