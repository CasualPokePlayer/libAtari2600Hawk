﻿namespace HawkCommon.Interfaces.Services;

public interface ICycleTiming : ISpecializedEmulatorService
{
	/// <summary>
	/// Total elapsed emulation time relative to <see cref="ClockRate"/>
	/// </summary>
	long CycleCount { get; }
	
	/// <summary>
	/// Clock Rate in hz for <see cref="CycleCount"/>
	/// </summary>
	double ClockRate { get; }
}