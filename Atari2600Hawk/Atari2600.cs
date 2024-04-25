using System;
using System.Security.Cryptography;

using HawkCommon;
using HawkCommon.BaseImplementations;
using HawkCommon.Database;
using HawkCommon.Interfaces.Services;

namespace Atari2600Hawk;

public partial class Atari2600 : IBoardInfo, IRomInfo, IRegionable, ICreateGameDBEntries
{
	internal static class RomChecksums
	{
		public const string CongoBongo = "SHA1:3A77DB43B6583E8689435F0F14AA04B9E57BDDED";
		
		public const string KangarooNotInGameDB = "SHA1:982B8016B393A9AA7DD110295A53C4612ECF2141";
		
		public const string Tapper = "SHA1:E986E1818E747BEB9B33CE4DFF1CDC6B55BDB620";
	}
	
	public Atari2600(GameInfo game, byte[] rom, A2600Settings settings, A2600SyncSettings syncSettings)
	{
		var ser = new BasicServiceProvider(this);
		ser.Register<IBoardInfo>(this);
		ser.Register<IRomInfo>(this);
		ser.Register<IRegionable>(this);
		ser.Register<ICreateGameDBEntries>(this);
		ser.Register<IDebuggable>(this);
		ser.Register<IInputPollable>(this);
		ser.Register<ISettable<A2600Settings, A2600SyncSettings>>(this);
		ServiceProvider = ser;
		
		_ram = new byte[128];
		Settings = settings ?? new A2600Settings();
		SyncSettings = syncSettings ?? new A2600SyncSettings();
		
		_controllerDeck = new Atari2600ControllerDeck(SyncSettings.Port1, SyncSettings.Port2);
		
		_leftDifficultySwitchPressed = SyncSettings.LeftDifficulty;
		_rightDifficultySwitchPressed = SyncSettings.RightDifficulty;
		
		Rom = rom;
		_game = game;
		
#pragma warning disable MEN014 // could rewrite this to be 1 read + 0-1 writes, but nah --yoshi
		if (!game.GetOptions().ContainsKey("m"))
		{
			game.AddOption("m", DetectMapper(rom));
		}
		
		var romHashSHA1 = $"SHA1:{Convert.ToHexString(SHA1.HashData(Rom))}";
		if (romHashSHA1 is RomChecksums.CongoBongo or RomChecksums.Tapper or RomChecksums.KangarooNotInGameDB)
		{
			game.RemoveOption("m");
			game.AddOption("m", "F8_sega");
		}
		
		Console.WriteLine("Game uses mapper " + game.GetOptions()["m"]);
#pragma warning restore MEN014
		Console.WriteLine(romHashSHA1);
		RebootCore();
		SetupMemoryDomains();
		
		Tracer = new TraceBuffer(Cpu.TraceHeader);
		
		ser.Register<IDisassemblable>(Cpu);
		ser.Register<ITraceable>(Tracer);
		ser.Register<IVideoProvider>(_tia);
		ser.Register<ISoundProvider>(_dcfilter);
		ser.Register<IStatable>(new StateSerializer(SyncState));
	}
	
	// IRomInfo
	public string RomDetails { get; private set; }
	
	private readonly Atari2600ControllerDeck _controllerDeck;
	
	// IRegionable
	public DisplayType Region => _pal ? DisplayType.PAL : DisplayType.NTSC;
	
	// ITraceable
	private ITraceable Tracer { get; }
	
	// ICreateGameDBEntries
	public CompactGameInfo GenerateGameDbEntry()
	{
		return new CompactGameInfo
		{
			Name = _game.Name,
			System = VSystemID.Raw.A26,
			MetaData = "m=" + _mapper.GetType().ToString().SubstringAfterLast('.'),
			Hash = Convert.ToHexString(SHA1.HashData(Rom)),
			Region = _game.Region,
			Status = RomStatus.Unknown
		};
	}
	
	// IBoardInfo
	public string BoardName => _mapper.GetType().Name;
	
	private static bool DetectPal(GameInfo game, byte[] rom)
	{
		// force NTSC mode for the new core we instantiate
		var newGame = game.Clone();
		if (newGame["PAL"])
		{
			newGame.RemoveOption("PAL");
		}
		
		if (!newGame["NTSC"])
		{
			newGame.AddOption("NTSC", "");
		}
		
		// here we advance past start up irregularities to see how long a frame is based on calls to Vsync
		// we run 72 frames, then run 270 scanlines worth of cycles.
		// if we don't hit a new frame, we can be pretty confident we are in PAL
		using var emu = new Atari2600(newGame, rom, null, null);
		for (int i = 0; i < 72; i++)
		{
			emu.FrameAdvance(NullController.Instance, false, false);
		}
		
		for (int i = 0; i < 61560; i++)
		{
			emu.Cycle();
		}
		
		bool pal = !emu._tia.New_Frame;
		
		Console.WriteLine("PAL Detection: {0}", pal);
		return pal;
	}
}
