// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

#ifndef ATARI2600MEMORYDOMAIN_H
#define ATARI2600MEMORYDOMAIN_H

#include <stdbool.h>
#include <stddef.h>
#include <stdint.h>

#include "HawkImport.h"

struct Atari2600MemoryDomain;

/**
  * Releases a handle to a Atari2600MemoryDomain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  */
HAWK_IMPORT void Atari2600MemoryDomain_Release(struct Atari2600MemoryDomain* memoryDomain);

/**
  * Gets number of bytes represented by a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @return Number of bytes represented by this memory domain.
  */
HAWK_IMPORT int64_t Atari2600MemoryDomain_GetSize(struct Atari2600MemoryDomain* memoryDomain);

/**
  * Peeks a byte from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to peek at. Must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @return Peeked byte at address.
  */
HAWK_IMPORT uint8_t Atari2600MemoryDomain_PeekByte(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr);

/**
  * Peeks a ushort from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to peek at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the memory should be read in a big endian manner, false if the memory should be read in a little endian manner.
  * @return Peeked ushort at address.
  */
HAWK_IMPORT uint16_t Atari2600MemoryDomain_PeekUshort(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr, bool bigEndian);

/**
  * Peeks a uint from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to peek at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the memory should be read in a big endian manner, false if the memory should be read in a little endian manner.
  * @return Peeked uint at address.
  */
HAWK_IMPORT uint32_t Atari2600MemoryDomain_PeekUint(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr, bool bigEndian);

/**
  * Pokes a byte to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to poke at. Must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param val           Value to poke at address.
  */
HAWK_IMPORT void Atari2600MemoryDomain_PokeByte(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr, uint8_t val);

/**
  * Pokes a ushort to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to poke at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param val           Value to poke at address.
  * @param bigEndian     True if the value should be written in a big endian manner, false if the value should be written in a little endian manner.
  */
HAWK_IMPORT void Atari2600MemoryDomain_PokeUshort(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr, uint16_t val, bool bigEndian);

/**
  * Pokes a uint to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param addr          Address to poke at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the value should be written in a big endian manner, false if the value should be written in a little endian manner.
  * @return Peeked uint at address.
  */
HAWK_IMPORT void Atari2600MemoryDomain_PokeUint(struct Atari2600MemoryDomain* memoryDomain, uint32_t addr, uint32_t val, bool bigEndian);

/**
  * Peeks bytes in bulk from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start peeking bytes at. Must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param values        Buffer to store peeked bytes. This may not be NULL.
  * @param numValues     Number of bytes able to be stored in buffer.
  *                      startAddress + numValues must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPeekByte(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, uint8_t* values, size_t numValues);

/**
  * Peeks ushorts in bulk from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start peeking ushorts at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the memory should be read in a big endian manner, false if the memory should be read in a little endian manner.
  * @param values        Buffer to store peeked ushorts. This may not be NULL.
  * @param numValues     Number of ushorts able to be stored in buffer.
  *                      startAddress + numValues * sizeof(uint16_t) must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPeekUshort(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, bool bigEndian, uint16_t* values, size_t numValues);

/**
  * Peeks uints in bulk from a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start peeking uints at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the memory should be read in a big endian manner, false if the memory should be read in a little endian manner.
  * @param values        Buffer to store peeked uints. This may not be NULL.
  * @param numValues     Number of uints able to be stored in buffer.
  *                      startAddress + numValues * sizeof(uint32_t) must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPeekUint(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, bool bigEndian, uint32_t* values, size_t numValues);

/**
  * Pokes bytes in bulk to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start poking bytes at. Must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param values        Buffer with bytes used to poke. This may not be NULL.
  * @param numValues     Number of bytes stored in buffer.
  *                      startAddress + numValues must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPokeByte(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, uint8_t* values, size_t numValues);

/**
  * Pokes ushorts in bulk to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start poking ushorts at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param values        Buffer with ushorts used to poke. This may not be NULL.
  * @param numValues     Number of ushorts stored in buffer.
  *                      startAddress + numValues * sizeof(uint16_t) must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the values should be written in a big endian manner, false if the values should be written in a little endian manner.
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPokeUshort(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, uint16_t* values, size_t numValues, bool bigEndian);

/**
  * Pokes uints in bulk to a memory domain.
  *
  * @param memoryDomain  Handle to Atari2600MemoryDomain instance. This may not be NULL.
  * @param startAddress  Address to start poking uints at. Must be suitably aligned and less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param values        Buffer with ushorts used to poke. This may not be NULL.
  * @param numValues     Number of uints stored in buffer.
  *                      startAddress + numValues * sizeof(uint32_t) must be less than Atari2600MemoryDomain_GetSize(memoryDomain).
  * @param bigEndian     True if the values should be written in a big endian manner, false if the values should be written in a little endian manner.
  */
HAWK_IMPORT void Atari2600MemoryDomain_BulkPokeUint(struct Atari2600MemoryDomain* memoryDomain, uint32_t startAddress, uint32_t* values, size_t numValues, bool bigEndian);


#endif
