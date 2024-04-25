// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

#ifndef ATARI2600HAWK_H
#define ATARI2600HAWK_H

#include <stdbool.h>
#include <stddef.h>
#include <stdint.h>

#include "HawkImport.h"

enum Atari2600ControllerTypes
{
	Unplugged,
	Joystick,
	Paddle,
	BoostGrip,
	Driving,
	Keyboard,
};

enum Atari2600MemoryDomains
{
	TIA,
	PIA,
	CartRam,
	MainRAM,
	DPC,
	SystemBus,
};

struct Atari2600Settings
{
	bool ShowBG;
	bool ShowPlayer1;
	bool ShowPlayer2;
	bool ShowMissle1;
	bool ShowMissle2;
	bool ShowBall;
	bool ShowPlayfield;
	bool SECAMColors;
	int32_t NTSCTopLine;
	int32_t NTSCBottomLine;
	int32_t PALTopLine;
	int32_t PALBottomLine;
	uint32_t BackgroundColor; // 32-bit ARGB
};

struct Atari2600SyncSettings
{
	enum Atari2600ControllerTypes Port1;
	enum Atari2600ControllerTypes Port2;
	bool BW;
	bool LeftDifficulty;
	bool RightDifficulty;
	bool FastScBios;
};

struct Atari2600Hawk;
struct Atari2600Controller;
struct Atari2600MemoryDomain;

/**
  * Creates an Atari2600Hawk instance.
  *
  * @param rom           Buffer with ROM data. This may not be NULL.
  * @param romSize       Length of ROM data in bytes.
  * @param settings      Non-sync settings for Atari2600Hawk, or NULL for default non-sync settings.
  * @param syncSettings  Sync settings for Atari2600Hawk, or NULL for default sync settings.
  * @return Handle to Atari2600Hawk instance, or NULL on failure.
  */
HAWK_IMPORT struct Atari2600Hawk* Atari2600Hawk_Create(uint8_t* rom, size_t romSize, struct Atari2600Settings* settings, struct Atari2600SyncSettings* syncSettings);

/**
  * Destroys an Atari2600Hawk instance.
  *
  * @param a2600  Handle to Atari2600Hawk instance. This may not be NULL.
  */
HAWK_IMPORT void Atari2600Hawk_Destroy(struct Atari2600Hawk* a2600);

/**
  * Advances 1 frame.
  *
  * @param a2600        Handle to Atari2600Hawk instance. This may not be NULL.
  * @param controller   Handle to Atari2600Controller instance, or NULL for a controller instance which always returns false/0.
  * @param render       True to render video, false to not render video.
  * @param renderSound  True to render sound, false to not render sound.
  */
HAWK_IMPORT void Atari2600Hawk_FrameAdvance(struct Atari2600Hawk* a2600, struct Atari2600Controller* controller, bool render, bool renderSound);

/**
  * Obtains a copy of the video buffer.
  *
  * @param a2600        Handle to Atari2600Hawk instance. This may not be NULL.
  * @param videoBuffer  Buffer to store the video buffer copy, stored as XRGB32.
  *                     This may not be NULL, and must be at least Atari2600Hawk_GetBufferWidth(a2600) * Atari2600Hawk_GetBufferHeight(a2600) * sizeof(uint32_t) bytes large.
  */
HAWK_IMPORT void Atari2600Hawk_GetVideoBuffer(struct Atari2600Hawk* a2600, uint32_t* videoBuffer);

/**
  * Gets video buffer width.
  *
  * @param a2600  Handle to Atari2600Hawk instance. This may not be NULL.
  * @return Video buffer width in pixels.
  */
HAWK_IMPORT uint32_t Atari2600Hawk_GetBufferWidth(struct Atari2600Hawk* a2600);

/**
  * Gets video buffer height.
  *
  * @param a2600  Handle to Atari2600Hawk instance. This may not be NULL.
  * @return Video buffer height in pixels.
  */
HAWK_IMPORT uint32_t Atari2600Hawk_GetBufferHeight(struct Atari2600Hawk* a2600);

/**
  * Gets audio samples in a synchronous manner.
  *
  * @param a2600         Handle to Atari2600Hawk instance. This may not be NULL.
  * @param sampleBuffer  Buffer to store audio samples. This may not be NULL.
  * @param numSamples    In: Number of samples in sampleBuffer. Out: Number of samples actually written to sampleBuffer. This may not be NULL.
  *                      Samples are defined as stereo 16-bit interleaved samples. As such, 4 bytes constitutes 1 sample.
  */
HAWK_IMPORT void Atari2600Hawk_GetSamplesSync(struct Atari2600Hawk* a2600, int16_t* sampleBuffer, size_t* numSamples);

/**
  * Gets handle to an Atari2600MemoryDomain.
  *
  * @param a2600  Handle to Atari2600Hawk instance. This may not be NULL.
  * @param which  Which memory domain to obtain.
  * @return Memory domain handle, or NULL if the domain does not exist for this game.
  *         Note that this handle is a "weak" handle. Its underlying contents may not be valid once the owner Atari2600Hawk is destroyed.
  */
HAWK_IMPORT struct Atari2600MemoryDomain* Atari2600Hawk_GetMemoryDomain(struct Atari2600Hawk* a2600, enum Atari2600MemoryDomains which);

/**
  * Saves emulation state to a buffer, using a "binary" format.
  *
  * @param a2600            Handle to Atari2600Hawk instance. This may not be NULL.
  * @param stateBuffer      Buffer to store emulation state. This may be NULL, in which case no state is actually written.
  * @param stateBufferSize  Number of bytes in stateBuffer. Meaningless if stateBuffer is NULL.
  * @return Number of bytes the resulting state operation would have written.
  *         If stateBuffer is NULL, this can be used as a way to "measure" the resulting state size in bytes.
  *         If stateBuffer is not NULL and if the return value this is more than stateBufferSize, the contents of stateBuffer are not valid.
  *         In this case, stateBuffer should be reallocated accordingly, and Atari2600Hawk_SaveStateBinary should be called again with the new buffer.
  */
HAWK_IMPORT uint32_t Atari2600Hawk_SaveStateBinary(struct Atari2600Hawk* a2600, uint8_t* stateBuffer, uint32_t stateBufferSize);

/**
  * Loads emulation state from a buffer, using a "binary" format.
  *
  * @param a2600            Handle to Atari2600Hawk instance. This may not be NULL.
  * @param stateBuffer      Buffer to read emulation state. This may not be NULL.
  * @param stateBufferSize  Number of bytes in stateBuffer.
  * @return True if the state operation was successful. False if the state operation failed.
  *         If the state operation fails, the resulting Atari2600Hawk should be considered in a broken state and thrown out accordingly.
  */
HAWK_IMPORT bool Atari2600Hawk_LoadStateBinary(struct Atari2600Hawk* a2600, uint8_t* stateBuffer, uint32_t stateBufferSize);

#endif
