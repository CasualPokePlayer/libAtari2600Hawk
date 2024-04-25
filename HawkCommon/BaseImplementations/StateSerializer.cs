﻿#nullable disable

using System;
using System.IO;

using HawkCommon.Interfaces.Services;

namespace HawkCommon.BaseImplementations;

/// <summary>
/// A generic implementation of <see cref="IStatable" /> that also
/// implements <see cref="ITextStatable" /> using the <see cref="Serializer" /> class
/// </summary>
public class StateSerializer : IStatable
{
	private readonly Action<Serializer> _syncState;
	
	/// <summary>
	/// Instantiates a new instance of the <see cref="StateSerializer" /> class
	/// </summary>
	/// <param name="syncState">The callback that will be called on save and load methods </param>
	public StateSerializer(Action<Serializer> syncState)
	{
		_syncState = syncState;
	}
	
	/// <summary>
	/// If provided, will be called after a loadstate call
	/// </summary>
	public Action LoadStateCallback { get; set; }
	
	public bool AvoidRewind => false;
	
	public void SaveStateBinary(BinaryWriter bw)
	{
		_syncState(Serializer.CreateBinaryWriter(bw));
	}
	
	public void LoadStateBinary(BinaryReader br)
	{
		_syncState(Serializer.CreateBinaryReader(br));
		LoadStateCallback?.Invoke();
	}
}
