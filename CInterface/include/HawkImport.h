// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

#ifndef HAWK_IMPORT

#ifdef __cplusplus
	#define HAWK_EXTERN extern "C"
#else
	#define HAWK_EXTERN extern
#endif

#ifdef _WIN32
#define HAWK_IMPORT HAWK_EXTERN __declspec(dllimport)
#else
#define HAWK_IMPORT HAWK_EXTERN
#endif

#endif
