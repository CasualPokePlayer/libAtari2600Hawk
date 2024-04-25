// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

using System.Runtime.InteropServices;

using HawkCommon.BaseImplementations;

namespace CInterface;

public static unsafe class Atari2600MemoryDomainExports
{
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_Release")]
	public static void Release(nint memoryDomainHandle)
	{
		GCHandle.FromIntPtr(memoryDomainHandle).Free();
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_GetSize")]
	public static uint GetSize(nint memoryDomainHandle)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		return (uint)memoryDomain.Size;
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PeekByte")]
	public static byte PeekByte(nint memoryDomainHandle, uint addr)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		return memoryDomain.PeekByte(addr);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PeekUshort")]
	public static ushort PeekUshort(nint memoryDomainHandle, uint addr, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		return memoryDomain.PeekUshort(addr, bigEndian);
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PeekUint")]
	public static uint PeekUint(nint memoryDomainHandle, uint addr, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		return memoryDomain.PeekUint(addr, bigEndian);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PokeByte")]
	public static void PokeByte(nint memoryDomainHandle, uint addr, byte val)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		memoryDomain.PokeByte(addr, val);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PokeUshort")]
	public static void PokeUshort(nint memoryDomainHandle, uint addr, ushort val, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		memoryDomain.PokeUshort(addr, val, bigEndian);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_PokeUint")]
	public static void PokeUint(nint memoryDomainHandle, uint addr, uint val, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		memoryDomain.PokeUint(addr, val, bigEndian);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPeekByte")]
	public static void BulkPeekByte(nint memoryDomainHandle, uint startAddress, byte* values, nuint numValues)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			values[i] = memoryDomain.PeekByte(startAddress++);
		}
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPeekUshort")]
	public static void BulkPeekUshort(nint memoryDomainHandle, uint startAddress, bool bigEndian, ushort* values, nuint numValues)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			values[i] = memoryDomain.PeekUshort(startAddress, bigEndian);
			startAddress += 2;
		}
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPeekUInt")]
	public static void BulkPeekUInt(nint memoryDomainHandle, uint startAddress, bool bigEndian, uint* values, nuint numValues)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			values[i] = memoryDomain.PeekUint(startAddress, bigEndian);
			startAddress += 4;
		}
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPokeByte")]
	public static void BulkPokeByte(nint memoryDomainHandle, uint startAddress, byte* values, nuint numValues)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			memoryDomain.PokeByte(startAddress++, values[i]);
		}
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPokeUshort")]
	public static void BulkPokeUshort(nint memoryDomainHandle, uint startAddress, ushort* values, nuint numValues, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			memoryDomain.PokeUshort(startAddress, values[i], bigEndian);
			startAddress += 2;
		}
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600MemoryDomain_BulkPokeUint")]
	public static void BulkPokeUint(nint memoryDomainHandle, uint startAddress, uint* values, nuint numValues, bool bigEndian)
	{
		var memoryDomain = (MemoryDomain)GCHandle.FromIntPtr(memoryDomainHandle).Target!;
		for (nuint i = 0; i < numValues; i++)
		{
			memoryDomain.PokeUint(startAddress, values[i], bigEndian);
			startAddress += 4;
		}
	}
}
