#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Hashing;
using System.Security.Cryptography;

namespace HawkCommon.Database;

public static class Database
{
	public const string MD5EmptyFile = "D41D8CD98F00B204E9800998ECF8427E";
	private const string SHA1EmptyFile = "DA39A3EE5E6B4B0D3255BFEF95601890AFD80709";

	private static readonly Dictionary<string, CompactGameInfo> DB = new();
	
	static Database()
	{
		using var database = typeof(Database).Assembly
			.GetManifestResourceStream("HawkCommon.Database.gamedb_a2600.txt")!;
		InitializeDatabase(database);
	}

	/// <summary>
	/// Removes hash type if present and enforces an uppercase hash
	/// </summary>
	/// <param name="hash">The hash to format, this is typically prefixed with a type (e.g. sha1:)</param>
	/// <returns>formatted hash</returns>
	private static string FormatHash(string hash)
		=> hash.Substring(hash.IndexOf(':') + 1).ToUpperInvariant();
	
	private static void InitializeDatabase(Stream database)
	{
		//reminder: this COULD be done on several threads, if it takes even longer
		using var reader = new StreamReader(database);
		while (reader.EndOfStream == false)
		{
			var line = reader.ReadLine() ?? "";
			try
			{
				if (line.StartsWith(';')) continue; // comment
				
				if (line.Trim().Length == 0)
				{
					continue;
				}
				
				var items = line.Split('\t');
				
				var game = new CompactGameInfo
				{
					Hash = FormatHash(items[0]),
					Status = items[1].Trim() switch
					{
						"B" => RomStatus.BadDump, // see /Assets/gamedb/gamedb.txt
						"V" => RomStatus.BadDump, // see /Assets/gamedb/gamedb.txt
						"T" => RomStatus.TranslatedRom,
						"O" => RomStatus.Overdump,
						"I" => RomStatus.Bios,
						"D" => RomStatus.Homebrew,
						"H" => RomStatus.Hack,
						"U" => RomStatus.Unknown,
						_ => RomStatus.GoodDump
					},
					Name = items[2],
					System = items[3],
					MetaData = items.Length >= 6 ? items[5] : null,
					Region = items.Length >= 7 ? items[6] : "",
					ForcedCore = items.Length >= 8 ? items[7].ToLowerInvariant() : ""
				};
				if (game.Hash is SHA1EmptyFile or MD5EmptyFile)
				{
					Console.WriteLine($"WARNING: gamedb contains entry for empty rom as \"{game.Name}\"!");
				}
				if (DB.TryGetValue(game.Hash, out var dupe))
				{
					Console.WriteLine("gamedb: Multiple hash entries {0}, duplicate detected on \"{1}\" and \"{2}\"", game.Hash, game.Name, dupe.Name);
				}

				DB[game.Hash] = game;
			}
			catch
			{
				Debug.WriteLine($"Error parsing database entry: {line}");
			}
		}
	}

	public static GameInfo CheckDatabase(string hash)
	{
		var hashFormatted = FormatHash(hash);
		_ = DB.TryGetValue(hashFormatted, out var cgi);
		if (cgi == null)
		{
			Console.WriteLine($"DB: hash {hash} not in game database.");
			return null;
		}
		
		return new GameInfo(cgi);
	}
	
	public static GameInfo GetGameInfo(byte[] romData, string fileName)
	{
		var hashSHA1 = Convert.ToHexString(SHA1.HashData(romData));
		if (DB.TryGetValue(hashSHA1, out var cgi))
		{
			return new GameInfo(cgi);
		}
		
		var hashMD5 = Convert.ToHexString(MD5.HashData(romData));
		if (DB.TryGetValue(hashMD5, out cgi))
		{
			return new GameInfo(cgi);
		}
		
		var hashCRC32 = Convert.ToHexString(Crc32.Hash(romData));
		if (DB.TryGetValue(hashCRC32, out cgi))
		{
			return new GameInfo(cgi);
		}
		
		// rom is not in database. make some best-guesses
		var game = new GameInfo
		{
			Hash = hashSHA1,
			Status = RomStatus.NotInDatabase,
			NotInDatabase = true
		};
		
		Console.WriteLine($"Game was not in DB. CRC: {hashCRC32} MD5: {hashMD5}");
		
		var ext = Path.GetExtension(fileName)?.ToUpperInvariant();
		
		switch (ext)
		{	
			case ".A26":
				game.System = VSystemID.Raw.A26;
				break;
		}

		game.Name = Path.GetFileNameWithoutExtension(fileName)?.Replace('_', ' ');
		return game;
	}
}

public class CompactGameInfo
{
	public string Name { get; set; }
	public string System { get; set; }
	public string MetaData { get; set; }
	public string Hash { get; set; }
	public string Region { get; set; }
	public RomStatus Status { get; set; }
	public string ForcedCore { get; set; }
}
