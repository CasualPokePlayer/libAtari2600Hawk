// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using HawkCommon.Interfaces;

namespace CInterface;

/// <summary>
/// IController implemetation for outside users interacting with Atari2600Hawk
/// </summary>
public class ExportedAtari2600Controller : IController
{
	public readonly Dictionary<string, bool> Buttons = new()
	{
		// Console buttons
		["Reset"] = false,
		["Select"] = false,
		["Power"] = false,
		["Toggle Left Difficulty"] = false,
		["Toggle Right Difficulty"] = false,
		// Standard/Paddle/Boost Grip/Driving controller buttons
		["P1 Up"] = false,
		["P1 Down"] = false,
		["P1 Left"] = false,
		["P1 Right"] = false,
		["P1 Button"] = false,
		["P1 Button 1"] = false,
		["P1 Button 2"] = false,
		["P2 Up"] = false,
		["P2 Down"] = false,
		["P2 Left"] = false,
		["P2 Right"] = false,
		["P2 Button"] = false,
		["P2 Button 1"] = false,
		["P2 Button 2"] = false,
		// Keyboard controller buttons
		["P1 1"] = false,
		["P1 2"] = false,
		["P1 3"] = false,
		["P1 4"] = false,
		["P1 5"] = false,
		["P1 6"] = false,
		["P1 7"] = false,
		["P1 8"] = false,
		["P1 9"] = false,
		["P1 *"] = false,
		["P1 0"] = false,
		["P1 #"] = false,
		["P2 1"] = false,
		["P2 2"] = false,
		["P2 3"] = false,
		["P2 4"] = false,
		["P2 5"] = false,
		["P2 6"] = false,
		["P2 7"] = false,
		["P2 8"] = false,
		["P2 9"] = false,
		["P2 *"] = false,
		["P2 0"] = false,
		["P2 #"] = false,
	};
	
	public readonly Dictionary<string, int> Axes = new()
	{
		// Paddle controller axes
		["P1 Paddle X 1"] = 0,
		["P1 Paddle X 2"] = 0,
		["P2 Paddle X 1"] = 0,
		["P2 Paddle X 2"] = 0,
		// Driving controller axes
		["P1 Wheel X 1"] = 0,
		["P1 Wheel X 2"] = 0,
		["P2 Wheel X 1"] = 0,
		["P2 Wheel X 2"] = 0,
	};

	public bool IsPressed(string button)
	{
		return Buttons[button];
	}

	public int AxisValue(string name)
	{
		return Axes[name];
	}
}

public static unsafe class Atari2600ControllerExports
{
	[Flags]
	public enum Atari2600ConsoleButtons
	{
		Reset = 1 << 0,
		Select = 1 << 1,
		Power = 1 << 2,
		ToggleLeftDifficulty = 1 << 3,
		ToggleRightDifficulty = 1 << 4,
	}
	
	[Flags]
	public enum Atari2600PortButtons
	{
		Up = 1 << 0,
		Down = 1 << 1,
		Left = 1 << 2,
		Right = 1 << 3,
		Button = 1 << 4,
		Button1 = 1 << 5,
		Button2 = 1 << 6,
		KP1 = 1 << 7,
		KP2 = 1 << 8,
		KP3 = 1 << 9,
		KP4 = 1 << 10,
		KP5 = 1 << 11,
		KP6 = 1 << 12,
		KP7 = 1 << 13,
		KP8 = 1 << 14,
		KP9 = 1 << 15,
		KPStar = 1 << 16,
		KP0 = 1 << 17,
		KPPound = 1 << 18,
	}

	public const sbyte ATARI2600_AXIS_MIN = -127;
	public const sbyte ATARI2600_AXIS_MAX = 127;

	[StructLayout(LayoutKind.Sequential)]
	public struct Atari2600Axes
	{
		public sbyte PaddleX1;
		public sbyte PaddleX2;
		public sbyte WheelX1;
		public sbyte WheelX2;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Atari2600Inputs
	{
		public Atari2600ConsoleButtons ConsoleButtons;
		public Atari2600PortButtons P1Buttons;
		public Atari2600PortButtons P2Buttons;
		public Atari2600Axes P1Axes;
		public Atari2600Axes P2Axes;
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Controller_Create")]
	public static nint Create()
	{
		var ret = new ExportedAtari2600Controller();
		var handle = GCHandle.Alloc(ret, GCHandleType.Normal);
		return GCHandle.ToIntPtr(handle);
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Controller_Destroy")]
	public static void Destroy(nint controllerHandle)
	{
		GCHandle.FromIntPtr(controllerHandle).Free();
	}

	[UnmanagedCallersOnly(EntryPoint = "Atari2600Controller_SetInputs")]
	public static void SetInputs(nint controllerHandle, Atari2600Inputs* inputs)
	{
		var controller = (ExportedAtari2600Controller)GCHandle.FromIntPtr(controllerHandle).Target!;
		// console buttons
		controller.Buttons["Reset"] = (inputs->ConsoleButtons & Atari2600ConsoleButtons.Reset) != 0;
		controller.Buttons["Select"] = (inputs->ConsoleButtons & Atari2600ConsoleButtons.Select) != 0;
		controller.Buttons["Power"] = (inputs->ConsoleButtons & Atari2600ConsoleButtons.Power) != 0;
		controller.Buttons["Toggle Left Difficulty"] = (inputs->ConsoleButtons & Atari2600ConsoleButtons.ToggleLeftDifficulty) != 0;
		controller.Buttons["Toggle Right Difficulty"] = (inputs->ConsoleButtons & Atari2600ConsoleButtons.ToggleRightDifficulty) != 0;
		// P1 buttons
		controller.Buttons["P1 Up"] = (inputs->P1Buttons & Atari2600PortButtons.Up) != 0;
		controller.Buttons["P1 Down"] = (inputs->P1Buttons & Atari2600PortButtons.Down) != 0;
		controller.Buttons["P1 Left"] = (inputs->P1Buttons & Atari2600PortButtons.Left) != 0;
		controller.Buttons["P1 Right"] = (inputs->P1Buttons & Atari2600PortButtons.Right) != 0;
		controller.Buttons["P1 Button"] = (inputs->P1Buttons & Atari2600PortButtons.Button) != 0;
		controller.Buttons["P1 Button 1"] = (inputs->P1Buttons & Atari2600PortButtons.Button1) != 0;
		controller.Buttons["P1 Button 2"] = (inputs->P1Buttons & Atari2600PortButtons.Button2) != 0;
		controller.Buttons["P1 1"] = (inputs->P1Buttons & Atari2600PortButtons.KP1) != 0;
		controller.Buttons["P1 2"] = (inputs->P1Buttons & Atari2600PortButtons.KP2) != 0;
		controller.Buttons["P1 3"] = (inputs->P1Buttons & Atari2600PortButtons.KP3) != 0;
		controller.Buttons["P1 4"] = (inputs->P1Buttons & Atari2600PortButtons.KP4) != 0;
		controller.Buttons["P1 5"] = (inputs->P1Buttons & Atari2600PortButtons.KP5) != 0;
		controller.Buttons["P1 6"] = (inputs->P1Buttons & Atari2600PortButtons.KP6) != 0;
		controller.Buttons["P1 7"] = (inputs->P1Buttons & Atari2600PortButtons.KP7) != 0;
		controller.Buttons["P1 8"] = (inputs->P1Buttons & Atari2600PortButtons.KP8) != 0;
		controller.Buttons["P1 9"] = (inputs->P1Buttons & Atari2600PortButtons.KP9) != 0;
		controller.Buttons["P1 *"] = (inputs->P1Buttons & Atari2600PortButtons.KPStar) != 0;
		controller.Buttons["P1 0"] = (inputs->P1Buttons & Atari2600PortButtons.KP0) != 0;
		controller.Buttons["P1 #"] = (inputs->P1Buttons & Atari2600PortButtons.KPPound) != 0;
		// P2 buttons
		controller.Buttons["P2 Up"] = (inputs->P2Buttons & Atari2600PortButtons.Up) != 0;
		controller.Buttons["P2 Down"] = (inputs->P2Buttons & Atari2600PortButtons.Down) != 0;
		controller.Buttons["P2 Left"] = (inputs->P2Buttons & Atari2600PortButtons.Left) != 0;
		controller.Buttons["P2 Right"] = (inputs->P2Buttons & Atari2600PortButtons.Right) != 0;
		controller.Buttons["P2 Button"] = (inputs->P2Buttons & Atari2600PortButtons.Button) != 0;
		controller.Buttons["P2 Button 1"] = (inputs->P2Buttons & Atari2600PortButtons.Button1) != 0;
		controller.Buttons["P2 Button 2"] = (inputs->P2Buttons & Atari2600PortButtons.Button2) != 0;
		controller.Buttons["P2 1"] = (inputs->P2Buttons & Atari2600PortButtons.KP2) != 0;
		controller.Buttons["P2 2"] = (inputs->P2Buttons & Atari2600PortButtons.KP2) != 0;
		controller.Buttons["P2 3"] = (inputs->P2Buttons & Atari2600PortButtons.KP3) != 0;
		controller.Buttons["P2 4"] = (inputs->P2Buttons & Atari2600PortButtons.KP4) != 0;
		controller.Buttons["P2 5"] = (inputs->P2Buttons & Atari2600PortButtons.KP5) != 0;
		controller.Buttons["P2 6"] = (inputs->P2Buttons & Atari2600PortButtons.KP6) != 0;
		controller.Buttons["P2 7"] = (inputs->P2Buttons & Atari2600PortButtons.KP7) != 0;
		controller.Buttons["P2 8"] = (inputs->P2Buttons & Atari2600PortButtons.KP8) != 0;
		controller.Buttons["P2 9"] = (inputs->P2Buttons & Atari2600PortButtons.KP9) != 0;
		controller.Buttons["P2 *"] = (inputs->P2Buttons & Atari2600PortButtons.KPStar) != 0;
		controller.Buttons["P2 0"] = (inputs->P2Buttons & Atari2600PortButtons.KP0) != 0;
		controller.Buttons["P2 #"] = (inputs->P2Buttons & Atari2600PortButtons.KPPound) != 0;
		// P1 Axes
		controller.Axes["P1 Paddle X 1"] = Math.Clamp(inputs->P1Axes.PaddleX1, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P1 Paddle X 2"] = Math.Clamp(inputs->P1Axes.PaddleX2, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P1 Wheel X 1"] = Math.Clamp(inputs->P1Axes.WheelX1, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P1 Wheel X 2"] = Math.Clamp(inputs->P1Axes.WheelX2, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		// P2 Axes
		controller.Axes["P2 Paddle X 1"] = Math.Clamp(inputs->P2Axes.PaddleX1, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P2 Paddle X 2"] = Math.Clamp(inputs->P2Axes.PaddleX2, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P2 Wheel X 1"] = Math.Clamp(inputs->P2Axes.WheelX1, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
		controller.Axes["P2 Wheel X 2"] = Math.Clamp(inputs->P2Axes.WheelX2, ATARI2600_AXIS_MIN, ATARI2600_AXIS_MAX);
	}
}
