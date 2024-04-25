#nullable disable

using System.Collections.Generic;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo
namespace HawkCommon.Database;

public static class FirmwareDatabase
{
	public static IEnumerable<FirmwareFile> FirmwareFiles => FirmwareFilesByHash.Values;
	
	public static readonly IReadOnlyDictionary<string, FirmwareFile> FirmwareFilesByHash = new Dictionary<string, FirmwareFile>();
	
	public static readonly IReadOnlyDictionary<FirmwareOption, FirmwareFile> FirmwareFilesByOption = new Dictionary<FirmwareOption, FirmwareFile>();
	
	public static readonly IReadOnlyCollection<FirmwareOption> FirmwareOptions = [];
	
	public static readonly IReadOnlyCollection<FirmwareRecord> FirmwareRecords = [];
	
	public static readonly IReadOnlyList<FirmwarePatchOption> AllPatches = [];
}
