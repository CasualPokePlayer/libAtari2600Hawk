#nullable disable

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HawkCommon;

public unsafe class Serializer
{
	public bool IsReader => _isReader;

	public bool IsWriter => !IsReader;

	
	public BinaryReader BinaryReader => _br;
	
	public BinaryWriter BinaryWriter => _bw;
	
	public Serializer(BinaryWriter bw)
	{
		StartWrite(bw);
	}
	
	public Serializer(BinaryReader br)
	{
		StartRead(br);
	}
	
	public static Serializer CreateBinaryWriter(BinaryWriter bw)
	{
		return new Serializer(bw);
	}
	
	public static Serializer CreateBinaryReader(BinaryReader br)
	{
		return new Serializer(br);
	}
	
	public void StartWrite(BinaryWriter bw)
	{
		_bw = bw;
		_isReader = false;
	}
	
	public void StartRead(BinaryReader br)
	{
		_br = br;
		_isReader = true;
	}
	
	public void BeginSection(string name)
	{
	}

	public void EndSection()
	{
	}
	
	/// <exception cref="InvalidOperationException"><typeparamref name="T"/> does not inherit <see cref="Enum"/></exception>
	public void SyncEnum<T>(string name, ref T val) where T : struct
	{
		if (typeof(T).BaseType != typeof(Enum))
		{
			throw new InvalidOperationException();
		}

		if (IsReader)
		{
			val = (T)Enum.ToObject(typeof(T), _br.ReadInt32());
		}
		else
		{
			_bw.Write(Convert.ToInt32(val));
		}
	}

	private static void ReadByteBuffer<T>(BinaryReader br, ref T[] buffer, bool returnNull)
		where T : unmanaged
	{
		var len = br.ReadInt32();
		if (len == 0)
		{
			buffer = returnNull ? null : [];
			return;
		}

		if (buffer.Length != len)
		{
			buffer = new T[len];
		}

		var bufferAsBytes = MemoryMarshal.AsBytes(buffer.AsSpan());
		var lengthInBytes = len * sizeof(T);
		var ofs = 0;
		while (lengthInBytes > 0)
		{
			var done = br.Read(bufferAsBytes.Slice(ofs, lengthInBytes));
			ofs += done;
			lengthInBytes -= done;
		}
	}

	private static void WriteByteBuffer<T>(BinaryWriter bw, T[] data)
		where T : unmanaged
	{
		if (data == null)
		{
			bw.Write(0);
		}
		else
		{
			bw.Write(data.Length);
			bw.Write(MemoryMarshal.AsBytes<T>(data));
		}
	}

	public void Sync(string name, ref byte[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}

	public void Sync(string name, ref bool[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}

	public void Sync(string name, ref short[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}
	
	public void Sync(string name, ref ushort[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}
	
	public void Sync(string name, ref int[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}

	public void Sync(string name, ref uint[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}

	public void Sync(string name, ref float[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}
	
	public void Sync(string name, ref double[] val, bool useNull)
	{
		if (IsReader)
		{
			ReadByteBuffer(_br, ref val, useNull);
		}
		else
		{
			WriteByteBuffer(_bw, val);
		}
	}

	public void Sync(string name, ref Bit val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref byte val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref ushort val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref uint val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref sbyte val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref short val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref int val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref long val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref ulong val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref float val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref double val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}
	
	public void Sync(string name, ref bool val)
	{
		if (IsReader)
		{
			Read(ref val);
		}
		else
		{
			Write(ref val);
		}
	}

	private BinaryReader _br;
	private BinaryWriter _bw;

	private bool _isReader;

	private void Read(ref Bit val)
	{
		val = _br.ReadBoolean();
	}

	private void Write(ref Bit val)
	{
		_bw.Write((bool)val);
	}
	
	private void Read(ref byte val)
	{
		val = _br.ReadByte();
	}
	
	private void Write(ref byte val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref ushort val)
	{
		val = _br.ReadUInt16();
	}
	
	private void Write(ref ushort val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref uint val)
	{
		val = _br.ReadUInt32();
	}

	private void Write(ref uint val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref sbyte val)
	{
		val = _br.ReadSByte();
	}
	
	private void Write(ref sbyte val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref short val)
	{
		val = _br.ReadInt16();
	}
	
	private void Write(ref short val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref int val)
	{
		val = _br.ReadInt32();
	}
	
	private void Write(ref int val)
	{
		_bw.Write(val);
	}

	private void Read(ref long val)
	{
		val = _br.ReadInt64();
	}
	
	private void Write(ref long val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref ulong val)
	{
		val = _br.ReadUInt64();
	}
	
	private void Write(ref ulong val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref float val)
	{
		val = _br.ReadSingle();
	}
	
	private void Write(ref float val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref double val)
	{
		val = _br.ReadDouble();
	}
	
	private void Write(ref double val)
	{
		_bw.Write(val);
	}
	
	private void Read(ref bool val)
	{
		val = _br.ReadBoolean();
	}
	
	private void Write(ref bool val)
	{
		_bw.Write(val);
	}
}
