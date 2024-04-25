#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using HawkCommon.BaseImplementations;
using HawkCommon.BaseImplementations.Axes;
using HawkCommon.Database;
using HawkCommon.Interfaces;
using HawkCommon.Interfaces.Services;

namespace HawkCommon;

public static class EmulatorExtensions
{
	/// <remarks>need to think about e.g. Genesis / Mega Drive using one sysID but having a different display name depending on the BIOS region --yoshi</remarks>
	public static readonly IReadOnlyDictionary<string, string> SystemIDDisplayNames = new Dictionary<string, string>
	{
		[VSystemID.Raw.A26] = "Atari 2600",
	};
	
	public static CoreAttribute Attributes(this IEmulator core)
	{
		return (CoreAttribute)Attribute.GetCustomAttribute(core.GetType(), typeof(CoreAttribute));
	}
	
	// todo: most of the special cases involving the NullEmulator should probably go away
	public static bool IsNull(this IEmulator core)
	{
		return core == null || core is NullEmulator;
	}
	
	public static bool HasVideoProvider(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IVideoProvider>();
	}
	
	public static IVideoProvider AsVideoProvider(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IVideoProvider>();
	}
	
	/// <summary>
	/// Returns the core's VideoProvider, or a suitable dummy provider
	/// </summary>
	public static IVideoProvider AsVideoProviderOrDefault(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IVideoProvider>()
		       ?? NullVideo.Instance;
	}
	
	public static bool HasSoundProvider(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ISoundProvider>();
	}
	
	public static ISoundProvider AsSoundProvider(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ISoundProvider>();
	}
	
	private static readonly ConditionalWeakTable<IEmulator, ISoundProvider> CachedNullSoundProviders = new ConditionalWeakTable<IEmulator, ISoundProvider>();
	
	/// <summary>
	/// returns the core's SoundProvider, or a suitable dummy provider
	/// </summary>
	public static ISoundProvider AsSoundProviderOrDefault(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ISoundProvider>()
		       ?? CachedNullSoundProviders.GetValue(core, e => new NullSound(core.VsyncNumerator(), core.VsyncDenominator()));
	}
	
	public static bool HasMemoryDomains(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IMemoryDomains>();
	}
	
	public static IMemoryDomains AsMemoryDomains(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IMemoryDomains>();
	}
	
	public static bool HasSaveRam(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ISaveRam>();
	}
	
	public static ISaveRam AsSaveRam(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ISaveRam>();
	}
	
	public static bool HasSavestates(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IStatable>();
	}
	
	public static IStatable AsStatable(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IStatable>();
	}
	
	public static bool CanPollInput(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IInputPollable>();
	}
	
	public static IInputPollable AsInputPollable(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IInputPollable>();
	}
	
	public static bool InputCallbacksAvailable(this IEmulator core)
	{
		// TODO: this is a pretty ugly way to handle this
		var pollable = core?.ServiceProvider.GetService<IInputPollable>();
		if (pollable != null)
		{
			try
			{
				var callbacks = pollable.InputCallbacks;
				return true;
			}
			catch (NotImplementedException)
			{
				return false;
			}
		}
		
		return false;
	}
	
	public static bool HasDriveLight(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IDriveLight>();
	}
	
	public static IDriveLight AsDriveLight(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IDriveLight>();
	}
	
	public static bool CanDebug(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IDebuggable>();
	}
	
	public static IDebuggable AsDebuggable(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IDebuggable>();
	}
	
	public static bool CpuTraceAvailable(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ITraceable>();
	}
	
	public static ITraceable AsTracer(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ITraceable>();
	}
	
	public static bool MemoryCallbacksAvailable(this IEmulator core)
	{
		// TODO: this is a pretty ugly way to handle this
		var debuggable = core?.ServiceProvider.GetService<IDebuggable>();
		if (debuggable != null)
		{
			try
			{
				var callbacks = debuggable.MemoryCallbacks;
				return true;
			}
			catch (NotImplementedException)
			{
				return false;
			}
		}
		
		return false;
	}
	
	public static bool MemoryCallbacksAvailable(this IDebuggable core)
	{
		if (core == null)
		{
			return false;
		}
		
		try
		{
			var callbacks = core.MemoryCallbacks;
			return true;
		}
		catch (NotImplementedException)
		{
			return false;
		}
	}
	
	public static bool CanDisassemble(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IDisassemblable>();
	}
	
	public static IDisassemblable AsDisassembler(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IDisassemblable>();
	}
	
	public static bool HasRegions(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IRegionable>();
	}
	
	public static IRegionable AsRegionable(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IRegionable>();
	}
	
	public static bool CanCDLog(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ICodeDataLogger>();
	}
	
	public static ICodeDataLogger AsCodeDataLogger(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ICodeDataLogger>();
	}
	
	public static ILinkable AsLinkable(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ILinkable>();
	}
	
	public static bool UsesLinkCable(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ILinkable>();
	}
	
	public static bool CanGenerateGameDBEntries(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<ICreateGameDBEntries>();
	}
	
	public static ICreateGameDBEntries AsGameDBEntryGenerator(this IEmulator core)
	{
		return core.ServiceProvider.GetService<ICreateGameDBEntries>();
	}
	
	public static bool HasBoardInfo(this IEmulator core)
	{
		return core != null && core.ServiceProvider.HasService<IBoardInfo>();
	}
	
	public static IBoardInfo AsBoardInfo(this IEmulator core)
	{
		return core.ServiceProvider.GetService<IBoardInfo>();
	}
	
	public static (int X, int Y) ScreenLogicalOffsets(this IEmulator core)
	{
		if (core != null && core.ServiceProvider.HasService<IVideoLogicalOffsets>())
		{
			var offsets = core.ServiceProvider.GetService<IVideoLogicalOffsets>();
			return (offsets.ScreenX, offsets.ScreenY);
		}
		
		return (0, 0);
	}
	
	public static string RomDetails(this IEmulator core)
	{
		if (core != null && core.ServiceProvider.HasService<IRomInfo>())
		{
			return core.ServiceProvider.GetService<IRomInfo>().RomDetails;
		}
		
		return "";
	}
	
	public static int VsyncNumerator(this IEmulator core)
	{
		if (core != null && core.HasVideoProvider())
		{
			return core.AsVideoProvider().VsyncNumerator;
		}
		
		return 60;
	}
	
	public static int VsyncDenominator(this IEmulator core)
	{
		if (core != null && core.HasVideoProvider())
		{
			return core.AsVideoProvider().VsyncDenominator;
		}
		
		return 1;
	}
	
	public static double VsyncRate(this IEmulator core)
	{
		return core.VsyncNumerator() / (double)core.VsyncDenominator();
	}
	
	public static bool IsImplemented(this MethodInfo info)
	{
		return !info.GetCustomAttributes(false).Any(a => a is FeatureNotImplementedAttribute);
	}
	
	private static List<string> ToControlNameList(IEnumerable<string> buttonList, int? controllerNum = null)
	{
		var buttons = new List<string>();
		foreach (var button in buttonList)
		{
			if (controllerNum != null && button.Length > 2 && button.Substring(0, 2) == $"P{controllerNum}")
			{
				var sub = button.Substring(3);
				buttons.Add(sub);
			}
			else if (controllerNum == null)
			{
				buttons.Add(button);
			}
		}
		return buttons;
	}
	
	public static string FilesystemSafeName(this IGameInfo game)
		=> game.Name.Replace('/', '+') // '/' is the path dir separator, obviously (methods in Path will treat it as such, even on Windows)
			.Replace('|', '+') // '|' is the filename-member separator for archives in HawkFile
			.Replace(":", " -") // ':' is the path separator in lists (Path.GetFileName will drop all but the last entry in such a list)
			.Replace("\"", "") // '"' is just annoying as it needs escaping on the command-line
			.RemoveInvalidFileSystemChars()
			.RemoveSuffix('.'); // trailing '.' would be duplicated when file extension is added
	
	/// <summary>
	/// Adds an axis to the receiver <see cref="ControllerDefinition"/>, and returns it.
	/// The new axis will appear after any that were previously defined.
	/// </summary>
	/// <param name="constraint">pass only for one axis in a pair, by convention the X axis</param>
	/// <returns>identical reference to <paramref name="def"/>; the object is mutated</returns>
	public static ControllerDefinition AddAxis(this ControllerDefinition def, string name, Range<int> range, int neutral, bool isReversed = false, AxisConstraint constraint = null)
	{
		def.Axes.Add(name, new AxisSpec(range, neutral, isReversed, constraint));
		return def;
	}
	
	/// <summary>
	/// Adds an X/Y pair of axes to the receiver <see cref="ControllerDefinition"/>, and returns it.
	/// The new axes will appear after any that were previously defined.
	/// </summary>
	/// <param name="nameFormat">format string e.g. <c>"P1 Left {0}"</c> (will be used to interpolate <c>"X"</c> and <c>"Y"</c>)</param>
	/// <returns>identical reference to <paramref name="def"/>; the object is mutated</returns>
	public static ControllerDefinition AddXYPair(this ControllerDefinition def, string nameFormat, AxisPairOrientation pDir, Range<int> rangeX, int neutralX, Range<int> rangeY, int neutralY, AxisConstraint constraint = null)
	{
		var yAxisName = string.Format(nameFormat, "Y");
		var finalConstraint = constraint ?? new NoOpAxisConstraint(yAxisName);
		return def.AddAxis(string.Format(nameFormat, "X"), rangeX, neutralX, ((byte) pDir & 2) != 0, finalConstraint)
			.AddAxis(yAxisName, rangeY, neutralY, ((byte) pDir & 1) != 0);
	}
	
	/// <summary>
	/// Adds an X/Y pair of axes to the receiver <see cref="ControllerDefinition"/>, and returns it.
	/// The new axes will appear after any that were previously defined.
	/// </summary>
	/// <param name="nameFormat">format string e.g. <c>"P1 Left {0}"</c> (will be used to interpolate <c>"X"</c> and <c>"Y"</c>)</param>
	/// <returns>identical reference to <paramref name="def"/>; the object is mutated</returns>
	public static ControllerDefinition AddXYPair(this ControllerDefinition def, string nameFormat, AxisPairOrientation pDir, Range<int> rangeBoth, int neutralBoth, AxisConstraint constraint = null)
		=> def.AddXYPair(nameFormat, pDir, rangeBoth, neutralBoth, rangeBoth, neutralBoth, constraint);
	
	/// <summary>
	/// Adds an X/Y/Z triple of axes to the receiver <see cref="ControllerDefinition"/>, and returns it.
	/// The new axes will appear after any that were previously defined.
	/// </summary>
	/// <param name="nameFormat">format string e.g. <c>"P1 Tilt {0}"</c> (will be used to interpolate <c>"X"</c>, <c>"Y"</c>, and <c>"Z"</c>)</param>
	/// <returns>identical reference to <paramref name="def"/>; the object is mutated</returns>
	public static ControllerDefinition AddXYZTriple(this ControllerDefinition def, string nameFormat, Range<int> rangeAll, int neutralAll)
		=> def.AddAxis(string.Format(nameFormat, "X"), rangeAll, neutralAll)
			.AddAxis(string.Format(nameFormat, "Y"), rangeAll, neutralAll)
			.AddAxis(string.Format(nameFormat, "Z"), rangeAll, neutralAll);
	
	public static AxisSpec With(this in AxisSpec spec, Range<int> range, int neutral) => new AxisSpec(range, neutral, spec.IsReversed, spec.Constraint);
	
	public static string SystemIDToDisplayName(string sysID)
		=> SystemIDDisplayNames.TryGetValue(sysID, out var dispName) ? dispName : string.Empty;
	
	public static bool IsEnabled(this ITraceable core) => core.Sink is not null;
	
	/// <remarks>TODO no-op instead of NRE when not "enabled"?</remarks>
	public static void Put(this ITraceable core, TraceInfo info) => core.Sink.Put(info);

	/// <inheritdoc cref="List{T}.AddRange"/>
	/// <remarks>
	/// (This is an extension method which reimplements <see cref="List{T}.AddRange"/> for other <see cref="ICollection{T}">collections</see>.
	/// It defers to the existing <see cref="List{T}.AddRange">AddRange</see> if the receiver's type is <see cref="List{T}"/> or a subclass.)
	/// </remarks>
	public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> collection)
	{
		if (list is List<T> listImpl)
		{
			listImpl.AddRange(collection);
			return;
		}
		foreach (var item in collection) list.Add(item);
	}
	
	/// <summary>2^-53</summary>
	private const double ExtremelySmallNumber = 1.1102230246251565E-16;
	
	/// <inheritdoc cref="HawkFloatEquality(float,float,float)"/>
	public static bool HawkFloatEquality(this double d, double other, double ε = ExtremelySmallNumber) => Math.Abs(other - d) < ε;
	
	/// <summary>2^-24</summary>
	private const float ReallySmallNumber = 5.96046448E-08f;
	
	/// <remarks>don't use this in cores without picking a suitable ε</remarks>
	public static bool HawkFloatEquality(this float f, float other, float ε = ReallySmallNumber) => Math.Abs(other - f) < ε;
	
	/// <returns>
	/// <paramref name="str"/> with the first char removed, or
	/// the original <paramref name="str"/> if the first char of <paramref name="str"/> is not <paramref name="prefix"/>
	/// </returns>
	public static string RemovePrefix(this string str, char prefix) => str.RemovePrefix(prefix, notFoundValue: str);
	
	/// <returns>
	/// <paramref name="str"/> with the first char removed, or
	/// <paramref name="notFoundValue"/> if the first char of <paramref name="str"/> is not <paramref name="prefix"/>
	/// </returns>
	public static string RemovePrefix(this string str, char prefix, string notFoundValue)
		=> str.StartsWith(prefix) ? str.Substring(1) : notFoundValue;
	
	/// <returns>
	/// <paramref name="str"/> with the leading substring <paramref name="prefix"/> removed, or
	/// the original <paramref name="str"/> if <paramref name="str"/> does not start with <paramref name="prefix"/>
	/// </returns>
	public static string RemovePrefix(this string str, string prefix) => str.RemovePrefix(prefix, notFoundValue: str);
	
	/// <returns>
	/// <paramref name="str"/> with the leading substring <paramref name="prefix"/> removed, or
	/// <paramref name="notFoundValue"/> if <paramref name="str"/> does not start with <paramref name="prefix"/>
	/// </returns>
	public static string RemovePrefix(this string str, string prefix, string notFoundValue) => str.StartsWith(prefix, StringComparison.Ordinal) ? str.Substring(prefix.Length, str.Length - prefix.Length) : notFoundValue;
	
	/// <returns>
	/// <paramref name="str"/> with the last char removed, or
	/// the original <paramref name="str"/> if the last char of <paramref name="str"/> is not <paramref name="suffix"/>
	/// </returns>
	public static string RemoveSuffix(this string str, char suffix) =>
		str.Length != 0 && str[str.Length - 1] == suffix
			? str.Substring(0, str.Length - 1)
			: str;

	/// <returns>
	/// <paramref name="str"/> with the trailing substring <paramref name="suffix"/> removed, or
	/// the original <paramref name="str"/> if <paramref name="str"/> does not end with <paramref name="suffix"/>
	/// </returns>
	public static string RemoveSuffix(this string str, string suffix) => str.RemoveSuffix(suffix, notFoundValue: str);
	
	/// <returns>
	/// <paramref name="str"/> with the trailing substring <paramref name="suffix"/> removed, or
	/// <paramref name="notFoundValue"/> if <paramref name="str"/> does not end with <paramref name="suffix"/>
	/// </returns>
	public static string RemoveSuffix(this string str, string suffix, string notFoundValue) => str.EndsWith(suffix, StringComparison.Ordinal) ? str.Substring(0, str.Length - suffix.Length) : notFoundValue;

	public static string RemoveInvalidFileSystemChars(this string name) => string.Concat(name.Split(Path.GetInvalidFileNameChars()));
	
	public static bool Bit(this byte b, int index)
	{
		return (b & (1 << index)) != 0;
	}
	
	public static bool Bit(this int b, int index)
	{
		return (b & (1 << index)) != 0;
	}
	
	public static bool Bit(this ushort b, int index)
	{
		return (b & (1 << index)) != 0;
	}

	/// <returns>
	/// the substring of <paramref name="str"/> after the last occurrence of <paramref name="delimiter"/>, or
	/// the original <paramref name="str"/> if not found
	/// </returns>
	public static string SubstringAfterLast(this string str, char delimiter)
		=> str.SubstringAfterLast(delimiter, notFoundValue: str);
	
	/// <returns>
	/// the substring of <paramref name="str"/> after the last occurrence of <paramref name="delimiter"/>, or
	/// <paramref name="notFoundValue"/> if not found
	/// </returns>
	public static string SubstringAfterLast(this string str, char delimiter, string notFoundValue)
	{
		var index = str.LastIndexOf(delimiter);
		return index < 0 ? notFoundValue : str.Substring(index + 1, str.Length - index - 1);
	}

	public static bool FindBytes(this byte[] array, byte[] pattern)
	{
		var fidx = 0;
		int result = Array.FindIndex(array, 0, array.Length, (byte b) =>
		{
			fidx = b == pattern[fidx] ? fidx + 1 : 0;
			return fidx == pattern.Length;
		});
		
		return result >= pattern.Length - 1;
	}
}
