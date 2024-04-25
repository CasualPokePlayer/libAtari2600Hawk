// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using HawkCommon;
using HawkCommon.Database;
using HawkCommon.BaseImplementations;
using HawkCommon.Interfaces;

using Atari2600Hawk;

[assembly: DisableRuntimeMarshalling]

namespace CInterface;

public static unsafe class Atari2600HawkExports
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Atari2600Settings
	{
		public bool ShowBG;
		public bool ShowPlayer1;
		public bool ShowPlayer2;
		public bool ShowMissle1;
		public bool ShowMissle2;
		public bool ShowBall;
		public bool ShowPlayfield;
		public bool SECAMColors;
		public int NTSCTopLine;
		public int NTSCBottomLine;
		public int PALTopLine;
		public int PALBottomLine;
		public uint BackgroundColor; // 32-bit ARGB
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Atari2600SyncSettings
	{
		public Atari2600ControllerTypes Port1;
		public Atari2600ControllerTypes Port2;
		public bool BW;
		public bool LeftDifficulty;
		public bool RightDifficulty;
		public bool FastScBios;
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_Create")]
	public static nint Create(byte* rom, nuint romSize, Atari2600Settings* settings, Atari2600SyncSettings* syncSettings)
	{
		var romBuffer = new byte[romSize];
		fixed (byte* romBufferPtr = romBuffer)
		{
			NativeMemory.Copy(rom, romBufferPtr, romSize);
		}

		var gi = Database.GetGameInfo(romBuffer, "game.a26");

		Atari2600.A2600Settings s = null;
		Atari2600.A2600SyncSettings ss = null;
		
		if (settings != null)
		{
			s = new()
			{
				ShowBG = settings->ShowBG,
				ShowPlayer1 = settings->ShowPlayer1,
				ShowPlayer2 = settings->ShowPlayer2,
				ShowMissle1 = settings->ShowMissle1,
				ShowMissle2 = settings->ShowMissle2,
				ShowBall = settings->ShowBall,
				ShowPlayfield = settings->ShowPlayfield,
				SECAMColors = settings->SECAMColors,
				NTSCTopLine = settings->NTSCTopLine,
				NTSCBottomLine = settings->NTSCBottomLine,
				PALTopLine = settings->PALTopLine,
				PALBottomLine = settings->PALBottomLine,
				BackgroundColor = Color.FromArgb((int)settings->BackgroundColor)
			};
		}

		if (syncSettings != null)
		{
			ss = new()
			{
				Port1 = syncSettings->Port1,
				Port2 = syncSettings->Port2,
				BW = syncSettings->BW,
				LeftDifficulty = syncSettings->LeftDifficulty,
				RightDifficulty = syncSettings->RightDifficulty,
				FastScBios = syncSettings->FastScBios
			};
		}

		var ret = new Atari2600(gi, romBuffer, s, ss);
		var handle = GCHandle.Alloc(ret, GCHandleType.Normal);
		return GCHandle.ToIntPtr(handle);
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_Destroy")]
	public static void Destroy(nint a2600Handle)
	{
		var handle = GCHandle.FromIntPtr(a2600Handle);
		var a2600 = (Atari2600)handle.Target!;
		a2600.Dispose();
		handle.Free();
	}

	// IEmulator exports

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_FrameAdvance")]
	public static void FrameAdvance(nint a2600Handle, nint controllerHandle, bool render, bool renderSound)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		IController controller = controllerHandle != 0
			? (ExportedAtari2600Controller)GCHandle.FromIntPtr(controllerHandle).Target!
			: NullController.Instance;
		a2600.FrameAdvance(controller, render, renderSound);
	}

	// IVideoProvider exports

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_GetVideoBuffer")]
	public static void GetVideoBuffer(nint a2600Handle, uint* videoBuffer)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		var vp = a2600.AsVideoProvider();
		var vb = vp.GetVideoBuffer();
		fixed (int* vbPtr = vb)
		{
			NativeMemory.Copy(vbPtr, videoBuffer, (uint)(vp.BufferWidth * vp.BufferHeight * sizeof(int)));
		}
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_GetBufferWidth")]
	public static uint GetBufferWidth(nint a2600Handle)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		return (uint)a2600.AsVideoProvider().BufferWidth;
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_GetBufferHeight")]
	public static uint GetBufferHeight(nint a2600Handle)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		return (uint)a2600.AsVideoProvider().BufferHeight;
	}

	// ISoundProvider exports

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_GetSamplesSync")]
	public static void GetSamplesSync(nint a2600Handle, short* sampleBuffer, nuint* numSamples)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		var sp = a2600.AsSoundProvider();
		sp.GetSamplesSync(out var samples, out var nsamp);
		*numSamples = Math.Min(*numSamples, (uint)nsamp);
		fixed (short* samplesPtr = samples)
		{
			NativeMemory.Copy(samplesPtr, sampleBuffer, *numSamples);
		}
	}

	// IMemoryDomains exports
	
	public enum Atari2600MemoryDomains
	{
		TIA,
		PIA,
		CartRam,
		MainRAM,
		DPC,
		SystemBus,
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_GetMemoryDomain")]
	public static nint GetMemoryDomain(nint a2600Handle, Atari2600MemoryDomains which)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		var name = which switch
		{
			Atari2600MemoryDomains.TIA => "TIA",
			Atari2600MemoryDomains.PIA => "PIA",
			Atari2600MemoryDomains.CartRam => "Cart Ram",
			Atari2600MemoryDomains.MainRAM => "Main RAM",
			Atari2600MemoryDomains.DPC => "DPC",
			Atari2600MemoryDomains.SystemBus => "System Bus",
			_ => "",
		};

		var memoryDomains = a2600.AsMemoryDomains();
		if (!memoryDomains.Has(name))
		{
			return 0;
		}
		
		var domain = memoryDomains[name];
		var handle = GCHandle.Alloc(domain, GCHandleType.Weak);
		return GCHandle.ToIntPtr(handle);
	}

	// IStatable exports
	
	private sealed class StateStream(byte* backingPtr, uint backingLength, bool readable, bool writable) : Stream
	{
		private uint _length;

		public override void Flush()
		{
		}
		
		public override int Read(Span<byte> buffer)
		{
			if (!readable)
			{
				throw new NotSupportedException();
			}

			if (_length + (uint)buffer.Length <= backingLength)
			{
				var srcSpan = new Span<byte>(backingPtr + _length, buffer.Length);
				srcSpan.CopyTo(buffer);
			}

			_length += (uint)buffer.Length;
			return buffer.Length;
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			return Read(buffer.AsSpan().Slice(offset, count));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
		
		public override void Write(ReadOnlySpan<byte> buffer)
		{
			if (!writable)
			{
				throw new NotSupportedException();
			}
			
			if (backingPtr != null && _length + (uint)buffer.Length <= backingLength)
			{
				var destSpan = new Span<byte>(backingPtr + _length, (int)(backingLength - _length));
				buffer.CopyTo(destSpan);
			}

			_length += (uint)buffer.Length;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Write(buffer.AsSpan().Slice(offset, count));
		}

		public override bool CanRead => readable;
		public override bool CanSeek => false;
		public override bool CanWrite => writable;

		// ReSharper disable once ConvertToAutoPropertyWhenPossible
		public override long Length => _length;

		public override long Position
		{
			get => _length;
			set => throw new NotSupportedException();
		}
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_SaveStateBinary")]
	public static uint SaveStateBinary(nint a2600Handle, byte* stateBuffer, uint stateBufferSize)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		var stateStream = new StateStream(stateBuffer, stateBufferSize, readable: false, writable: true);
		a2600.AsStatable().SaveStateBinary(new BinaryWriter(stateStream));
		return (uint)stateStream.Length;
	}
	
	[UnmanagedCallersOnly(EntryPoint = "Atari2600Hawk_LoadStateBinary")]
	public static bool LoadStateBinary(nint a2600Handle, byte* stateBuffer, uint stateBufferSize)
	{
		var a2600 = (Atari2600)GCHandle.FromIntPtr(a2600Handle).Target!;
		var stateStream = new StateStream(stateBuffer, stateBufferSize, readable: true, writable: false);
		a2600.AsStatable().LoadStateBinary(new BinaryReader(stateStream));
		return (uint)stateStream.Length > stateBufferSize;
	}
}
