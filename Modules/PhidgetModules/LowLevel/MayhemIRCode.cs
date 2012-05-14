using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Phidgets;

namespace PhidgetModules.LowLevel
{
	[Serializable]
	public class MayhemIRCode
	{
		private int bitCount;
		private byte[] data;

		[NonSerialized]
		private IRCode irCode;
		public IRCode IRCode
		{
			get
			{
				return irCode;
			}
			set
			{
				SetIRCode(value);
			}
		}

		public int BitCount
		{
			get
			{
				return irCode.BitCount;
			}
		}

		public byte[] Data
		{
			get
			{
				return irCode.Data;
			}
		}


		public MayhemIRCode()
		{

		}

		public MayhemIRCode(IRCode code) {
			SetIRCode(code);
		}

		private void SetIRCode(IRCode code)
		{
			irCode = code;
			bitCount = code.BitCount;
			data = code.Data;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			IRCode = new IRCode(data, bitCount);
		}

		public override string ToString()
		{
			string hex = BitConverter.ToString(data);
			return hex.Replace("-", "");

		}
	}
}
