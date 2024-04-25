﻿#nullable disable

using System.IO;

namespace HawkCommon.Interfaces.Services;

/// <summary>
/// This service manages the logic of sending and receiving savestates from the core
/// If this service is available, client apps will expose features for making savestates and that utilize savestates (such as rewind))
/// If unavailable these options will not be exposed
/// Additionally many tools depend on savestates such as TAStudio, these will only be available if this service is implemented
/// </summary>
public interface IStatable : IEmulatorService
{
	bool AvoidRewind { get; }
	void SaveStateBinary(BinaryWriter writer);
	void LoadStateBinary(BinaryReader reader);
}

public static class StatableExtensions
{
	/// <summary>
	/// Loads a state directly from a byte array
	/// </summary>
	public static void LoadStateBinary(this IStatable core, byte[] state)
	{
		using var ms = new MemoryStream(state, false);
		using var br = new BinaryReader(ms);
		core.LoadStateBinary(br);
	}

	/// <summary>
	/// Creates a byte array copy of the core's current state
	/// This creates a new buffer, and should not be used in performance sensitive situations
	/// </summary>
	public static byte[] CloneSavestate(this IStatable core)
	{
		using var ms = new MemoryStream();
		using var bw = new BinaryWriter(ms);
		core.SaveStateBinary(bw);
		bw.Flush();
		var stateBuffer = ms.ToArray();
		bw.Close();
		return stateBuffer;
	}
}
