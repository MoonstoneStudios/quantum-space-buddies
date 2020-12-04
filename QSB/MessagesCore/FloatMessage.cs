﻿using QSB.Messaging;
using QSB.QuantumUNET;
using UnityEngine.Networking;

namespace QSB.MessagesCore
{
	public class FloatMessage : PlayerMessage
	{
		public float Value;

		public override void Deserialize(QSBNetworkReader reader)
		{
			base.Deserialize(reader);
			Value = reader.ReadSingle();
		}

		public override void Serialize(QSBNetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(Value);
		}
	}
}