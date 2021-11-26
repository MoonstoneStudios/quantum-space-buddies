﻿using QuantumUNET.Transport;

namespace QSB.Utility.VariableSync
{
	public class FloatVariableSyncer : BaseVariableSyncer
	{
		public VariableReference<float> FloatToSync;

		public override void WriteData(QNetworkWriter writer)
		{
			if (FloatToSync == null)
			{
				writer.Write(0f);
			}
			else
			{
				writer.Write(FloatToSync.Value);
			}
		}

		public override void ReadData(QNetworkReader writer)
		{
			if (FloatToSync == null)
			{
				writer.ReadSingle();
			}
			else
			{
				FloatToSync.Value = writer.ReadSingle();
			}
		}

		public override bool HasChanged()
		{
			// TODO - do this!!
			return true;
		}
	}
}
