﻿using System.Runtime.Serialization;

namespace MayhemCore.ModuleTypes
{
	/// <summary>
	/// Every module that has configuration
	/// that wants to be accessible from a WPF
	/// application needs to implement this interface
	/// </summary>
	public interface IWpf : ISerializable
	{
		void WpfConfig();
	}
}
