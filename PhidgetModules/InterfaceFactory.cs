﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phidgets;

namespace PhidgetModules
{
	public static class InterfaceFactory
	{
		private static InterfaceKit ifKit;

		public static InterfaceKit GetInterface() {
			if (ifKit == null) {
				ifKit = new InterfaceKit();
				ifKit.open();
			}

			return ifKit;
		}
	}
}
