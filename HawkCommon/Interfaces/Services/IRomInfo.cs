﻿namespace HawkCommon.Interfaces.Services;

/// <summary>
/// Provides detailed information about the Rom such as hashes,
/// and decisions made such as which board configuration was used.
/// </summary>
public interface IRomInfo : ISpecializedEmulatorService
{
	/// <summary>
	/// All necessary information about the Rom.
	/// Expected to be formatted for user consumption
	/// </summary>
	string? RomDetails { get; }
}

public interface IRedumpDiscChecksumInfo : IRomInfo
{
	string CalculateDiscHashes();
}