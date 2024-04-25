// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

#ifndef ATARI2600CONTROLLER_H
#define ATARI2600CONTROLLER_H

#include <stdint.h>

#include "HawkImport.h"

#define ATARI2600_AXIS_MIN -127
#define ATARI2600_AXIS_MAX 127

enum Atari2600ConsoleButtons
{
	Reset = 1 << 0,
	Select = 1 << 1,
	Power = 1 << 2,
	ToggleLeftDifficulty = 1 << 3,
	ToggleRightDifficulty = 1 << 4,
};

enum Atari2600PortButtons
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
};

struct Atari2600Axes
{
	int8_t PaddleX1;
	int8_t PaddleX2;
	int8_t WheelX1;
	int8_t WheelX2;
};

struct Atari2600Inputs
{
	enum Atari2600ConsoleButtons ConsoleButtons;
	enum Atari2600PortButtons P1Buttons;
	enum Atari2600PortButtons P2Buttons;
	struct Atari2600Axes P1Axes;
	struct Atari2600Axes P2Axes;
};

struct Atari2600Controller;

/**
  * Creates an Atari2600Controller instance.
  *
  * @return Handle to Atari2600Hawk instance, or NULL on failure.
  */
HAWK_IMPORT struct Atari2600Controller* Atari2600Controller_Create(void);

/**
  * Destroys an Atari2600Controller instance.
  *
  * @param controller  Handle to Atari2600Controller instance. This may not be NULL.
  */
HAWK_IMPORT void Atari2600Controller_Destroy(struct Atari2600Controller* controller);

/**
  * Sets the inputs for the Atari2600Controller.
  *
  * @param controller  Handle to Atari2600Controller instance. This may not be NULL.
  * @param inputs      Inputs for the Atari2600Controller. This may not be NULL.
  */
HAWK_IMPORT void Atari2600Controller_SetInputs(struct Atari2600Controller* controller, struct Atari2600Inputs* inputs);

#endif
