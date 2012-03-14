using System.IO.Ports;
using System;

namespace MayhemSerial
{
	public class SerialSettings
	{
		public int BaudRate
		{
			get;
			private set;
		}

		public Parity Parity
		{
			get;
			private set;
		}

		public StopBits StopBits
		{
			get;
			private set;
		}

		public int DataBits
		{
			get;
			private set;
		}

		public SerialSettings(int baudRate, Parity parity, StopBits stopBits, int dataBits)
		{
			BaudRate = baudRate;
			Parity = parity;
			StopBits = stopBits;
			DataBits = dataBits;
		}

		#region Overriding Equals and related items
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			SerialSettings other = obj as SerialSettings;
			if (other == null)
				return false;

			return Equals(other);
		}

		public bool Equals(SerialSettings obj)
		{
			if (obj == null)
				return false;

			return BaudRate == obj.BaudRate &&
					Parity == obj.Parity &&
					StopBits == obj.StopBits &&
					DataBits == obj.DataBits;
		}

		public override int GetHashCode()
		{
			return BaudRate ^ Parity.GetHashCode() ^ StopBits.GetHashCode() ^ DataBits;
		}

		public static bool operator ==(SerialSettings a, SerialSettings b)
		{
			// If both are null, or both are same instance, return true.
			if (Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.BaudRate == b.BaudRate &&
					a.Parity == b.Parity &&
					a.StopBits == b.StopBits &&
					a.DataBits == b.DataBits;
		}

		public static bool operator !=(SerialSettings a, SerialSettings b)
		{
			return !(a == b);
		}
		#endregion
	}
}
