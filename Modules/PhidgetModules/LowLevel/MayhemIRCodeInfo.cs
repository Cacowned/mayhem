using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;
using System.Runtime.Serialization;

namespace PhidgetModules.LowLevel
{
	[Serializable]
	public class MayhemIRCodeInfo
	{
		private int BitCount;
		private int CarrierFrequency;
		private int DutyCycle;
		private IRCodeInfo.IREncoding Encoding;
		private int Gap;
		private int[] Header;
		private IRCodeInfo.IRCodeLength Length;
		private int MinRepeat;
		private int[] One;
		private int[] Repeat;
		private MayhemIRCode ToggleMask;
		private int Trail;
		private int[] Zero;

		[NonSerialized]
		private IRCodeInfo irCodeInfo;
		public IRCodeInfo IRCodeInfo
		{
			get
			{
				return irCodeInfo;
			}
			set
			{
				SetIRCodeInfo(value);
			}
		}

		public MayhemIRCodeInfo(IRCodeInfo info)
		{
			SetIRCodeInfo(info);
		}

		private void SetIRCodeInfo(IRCodeInfo info)
		{
			irCodeInfo = info;

			BitCount = info.BitCount;
			CarrierFrequency = info.CarrierFrequency;
			DutyCycle = info.DutyCycle;
			Encoding = info.Encoding;
			Gap = info.Gap;
			Header = info.Header;
			Length = info.Length;
			MinRepeat = info.MinRepeat;
			One = info.One;
			Repeat = info.Repeat;
			ToggleMask = new MayhemIRCode(info.ToggleMask);
			Trail = info.Trail;
			Zero = info.Zero;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			irCodeInfo = new IRCodeInfo(Encoding, BitCount, Header, Zero, One, Trail, Gap, Repeat, MinRepeat, ToggleMask.Data, Length, CarrierFrequency, DutyCycle);
		}
	}
}
