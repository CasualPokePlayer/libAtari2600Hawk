using System;
using System.Collections.Generic;

using HawkCommon;
using HawkCommon.BaseImplementations;
using HawkCommon.Interfaces;
using HawkCommon.Interfaces.Services;

namespace Atari2600Hawk;

public partial class Atari2600 : IDebuggable
{
	public IDictionary<string, RegisterValue> GetCpuFlagsAndRegisters() => Cpu.GetCpuFlagsAndRegisters();
	
	public void SetCpuRegister(string register, int value) => Cpu.SetCpuRegister(register, value);
	
	public IMemoryCallbackSystem MemoryCallbacks { get; } = new MemoryCallbackSystem(new[] { "System Bus" });
	
	public bool CanStep(StepType type) => false;
	
	[FeatureNotImplemented]
	public void Step(StepType type) => throw new NotImplementedException();
	
	public long TotalExecutedCycles => Cpu.TotalExecutedCycles;
}