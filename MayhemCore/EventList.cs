﻿
namespace MayhemCore
{
	/// <summary>
	/// Is a list of all the available events that implement T if they have a configuration window
	/// </summary>
	/// <typeparam name="T">The ModuleType interface that every module must implement</typeparam>
	internal class EventList : ModuleList<EventBase>
	{
	}
}
