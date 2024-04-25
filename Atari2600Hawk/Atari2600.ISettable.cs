using System;
using System.ComponentModel;
using System.Drawing;

using HawkCommon.Interfaces.Services;

namespace Atari2600Hawk;

public partial class Atari2600 : ISettable<Atari2600.A2600Settings, Atari2600.A2600SyncSettings>
{
	public A2600Settings GetSettings()
	{
		return Settings.Clone();
	}
	
	public A2600SyncSettings GetSyncSettings()
	{
		return SyncSettings.Clone();
	}
	
	public PutSettingsDirtyBits PutSettings(A2600Settings o)
	{
		if (Settings == null || Settings.SECAMColors != o.SECAMColors)
		{
			_tia?.SetSecam(o.SECAMColors);
		}
		
		Settings = o;
		return PutSettingsDirtyBits.None;
	}
	
	public PutSettingsDirtyBits PutSyncSettings(A2600SyncSettings o)
	{
		bool ret = A2600SyncSettings.NeedsReboot(SyncSettings, o);
		SyncSettings = o;
		return ret ? PutSettingsDirtyBits.RebootCore : PutSettingsDirtyBits.None;
	}
	
	internal A2600Settings Settings { get; private set; }
	internal A2600SyncSettings SyncSettings { get; private set; }
	
	[CoreSettings]
	public class A2600Settings
	{
		private int _ntscTopLine;
		
		private int _ntscBottomLine;
		
		private int _palTopLine;
		
		private int _palBottomLine;
		
		[DisplayName("Show Background")]
		[Description("Sets whether or not the Background layer will be displayed")]
		public bool ShowBG { get; set; }
		
		[DisplayName("Show Player 1")]
		[Description("Sets whether or not the Player 1 layer will be displayed")]
		public bool ShowPlayer1 { get; set; }
		
		[DisplayName("Show Player 2")]
		[Description("Sets whether or not the Player 2 layer will be displayed")]
		public bool ShowPlayer2 { get; set; }
		
		[DisplayName("Show Missle 1")]
		[Description("Sets whether or not the Missle 1 layer will be displayed")]
		public bool ShowMissle1 { get; set; }
		
		[DisplayName("Show Missle 2")]
		[Description("Sets whether or not the Missle 2 layer will be displayed")]
		public bool ShowMissle2 { get; set; }
		
		[DisplayName("Show Ball")]
		[Description("Sets whether or not the Ball layer will be displayed")]
		public bool ShowBall { get; set; }
		
		[DisplayName("Show Playfield")]
		[Description("Sets whether or not the Playfield layer will be displayed")]
		public bool ShowPlayfield { get; set; }
		
		[DisplayName("SECAM Colors")]
		[Description("If true, PAL mode will show with SECAM (French) colors.")]
		public bool SECAMColors { get; set; }
		
		[DisplayName("NTSC Top Line")]
		[Description("First line of the video image to display in NTSC mode.")]
		public int NTSCTopLine
		{
			get => _ntscTopLine;
			set => _ntscTopLine = Math.Min(64, Math.Max(value, 0));
		}
		
		[DisplayName("NTSC Bottom Line")]
		[Description("Last line of the video image to display in NTSC mode.")]
		public int NTSCBottomLine
		{
			get => _ntscBottomLine;
			set => _ntscBottomLine = Math.Min(260, Math.Max(value, 192));
		}
		
		[DisplayName("PAL Top Line")]
		[Description("First line of the video image to display in PAL mode.")]
		public int PALTopLine
		{
			get => _palTopLine;
			set => _palTopLine = Math.Min(64, Math.Max(value, 0));
		}
		
		[DisplayName("PAL Bottom Line")]
		[Description("Last line of the video image to display in PAL mode.")]
		public int PALBottomLine
		{
			get => _palBottomLine;
			set => _palBottomLine = Math.Min(310, Math.Max(value, 192));
		}
		
		[DisplayName("Background Color")]
		public Color BackgroundColor { get; set; }
		
		public A2600Settings Clone()
		{
			return (A2600Settings)MemberwiseClone();
		}
		
		public A2600Settings()
		{
			ShowBG = true;
			ShowPlayer1 = true;
			ShowPlayer2 = true;
			ShowMissle1 = true;
			ShowMissle2 = true;
			ShowBall = true;
			ShowPlayfield = true;
			SECAMColors = false;
			NTSCTopLine = 24;
			NTSCBottomLine = 248;
			PALTopLine = 24;
			PALBottomLine = 296;
			BackgroundColor = Color.Black;
		}
	}
	
	[CoreSettings]
	public class A2600SyncSettings
	{
		[DisplayName("Port 1 Device")]
		[Description("The type of controller plugged into the first controller port")]
		public Atari2600ControllerTypes Port1 { get; set; }
		
		[DisplayName("Port 2 Device")]
		[Description("The type of controller plugged into the second controller port")]
		public Atari2600ControllerTypes Port2 { get; set; }
		
		[DisplayName("Black and White Mode")]
		[Description("Set the TV Type switch on the console to B&W or Color.  This only affects the displayed image if the game supports it.")]
		public bool BW { get; set; }
		
		[DisplayName("Left Difficulty")]
		[Description("Set the Left Difficulty switch on the console")]
		public bool LeftDifficulty { get; set; }
		
		[DisplayName("Right Difficulty")]
		[Description("Set the Right Difficulty switch on the console")]
		public bool RightDifficulty { get; set; }
		
		[DisplayName("Super Charger BIOS Skip")]
		[Description("On Super Charger carts, this will skip the BIOS intro")]
		public bool FastScBios { get; set; }
		
		public A2600SyncSettings Clone()
		{
			return (A2600SyncSettings)MemberwiseClone();
		}
		
		public A2600SyncSettings()
		{
			Port1 = Atari2600ControllerTypes.Joystick;
			Port2 = Atari2600ControllerTypes.Joystick;
			BW = false;
			LeftDifficulty = true;
			RightDifficulty = true;
			FastScBios = false;
		}
		
		public static bool NeedsReboot(A2600SyncSettings x, A2600SyncSettings y)
		{
			return x.Port1 != y.Port1
			       || x.Port2 != y.Port2
			       || x.BW != y.BW
			       || x.LeftDifficulty != y.LeftDifficulty
			       || x.RightDifficulty != y.RightDifficulty
			       || x.FastScBios != y.FastScBios;
		}
	}
}
