using System;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace MayhemSerial
{
	[DataContract]
	public class SerialSettings
	{
		[DataMember]
		public int BaudRate
		{
			get;
			private set;
		}

		[DataMember]
		public Parity Parity
		{
			get;
			private set;
		}

		[DataMember]
		public StopBits StopBits
		{
			get;
			private set;
		}

		[DataMember]
		public int DataBits
		{
			get;
			private set;
		}

		public SerialSettings(int baudRate, Parity parity, StopBits stopBits, int dataBits)
		{
			this.BaudRate = baudRate;
			this.Parity = parity;
			this.StopBits = stopBits;
			this.DataBits = dataBits;
		}

		#region Overriding Equals and related items
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			SerialSettings other = obj as SerialSettings;
			if (other == null)
			{
				return false;
			}

			return Equals(other);
		}

		public bool Equals(SerialSettings obj)
		{
			if (obj == null)
			{
				return false;
			}

			return this.BaudRate == obj.BaudRate &&
					this.Parity == obj.Parity &&
					this.StopBits == obj.StopBits &&
					this.DataBits == obj.DataBits;
		}

		public override int GetHashCode()
		{
			return this.BaudRate ^ this.Parity.GetHashCode() ^ this.StopBits.GetHashCode() ^ this.DataBits;
		}

		public static bool operator ==(SerialSettings a, SerialSettings b)
		{
			// If both are null, or both are same instance, return true.
			if (object.ReferenceEquals(a, b))
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
