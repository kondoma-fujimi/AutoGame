#pragma once

#include <stdio.h>
#include <stdint.h>

#include "GamepadEnums.h"
#include "GamepadDescriptors.h"
#include "GamepadState.h"

uint16_t f1Mask;
uint16_t f2Mask;
/**
 * 
		 * @brief Check for a button press. Used by `pressed[Button]` helper methods.
		 */
		inline bool __attribute__((always_inline)) pressedButton(const uint16_t mask) { return (state.buttons & mask) == mask; }

		/**
		 * @brief Check for a dpad press. Used by `pressed[Dpad]` helper methods.
		 */
		inline bool __attribute__((always_inline)) pressedDpad(const uint8_t mask) { return (state.dpad & mask) == mask; }
        inline bool __attribute__((always_inline)) pressedUp()    { return pressedDpad(GAMEPAD_MASK_UP); }
		inline bool __attribute__((always_inline)) pressedDown()  { return pressedDpad(GAMEPAD_MASK_DOWN); }
		inline bool __attribute__((always_inline)) pressedLeft()  { return pressedDpad(GAMEPAD_MASK_LEFT); }
		inline bool __attribute__((always_inline)) pressedRight() { return pressedDpad(GAMEPAD_MASK_RIGHT); }
		inline bool __attribute__((always_inline)) pressedB1()    { return pressedButton(GAMEPAD_MASK_B1); }
		inline bool __attribute__((always_inline)) pressedB2()    { return pressedButton(GAMEPAD_MASK_B2); }
		inline bool __attribute__((always_inline)) pressedB3()    { return pressedButton(GAMEPAD_MASK_B3); }
		inline bool __attribute__((always_inline)) pressedB4()    { return pressedButton(GAMEPAD_MASK_B4); }
		inline bool __attribute__((always_inline)) pressedL1()    { return pressedButton(GAMEPAD_MASK_L1); }
		inline bool __attribute__((always_inline)) pressedR1()    { return pressedButton(GAMEPAD_MASK_R1); }
		inline bool __attribute__((always_inline)) pressedL2()    { return pressedButton(GAMEPAD_MASK_L2); }
		inline bool __attribute__((always_inline)) pressedR2()    { return pressedButton(GAMEPAD_MASK_R2); }
		inline bool __attribute__((always_inline)) pressedS1()    { return pressedButton(GAMEPAD_MASK_S1); }
		inline bool __attribute__((always_inline)) pressedS2()    { return pressedButton(GAMEPAD_MASK_S2); }
		inline bool __attribute__((always_inline)) pressedL3()    { return pressedButton(GAMEPAD_MASK_L3); }
		inline bool __attribute__((always_inline)) pressedR3()    { return pressedButton(GAMEPAD_MASK_R3); }
		inline bool __attribute__((always_inline)) pressedA1()    { return pressedButton(GAMEPAD_MASK_A1); }
		inline bool __attribute__((always_inline)) pressedA2()    { return pressedButton(GAMEPAD_MASK_A2); }
		inline bool __attribute__((always_inline)) pressedF1()    { return pressedButton(f1Mask); }
		inline bool __attribute__((always_inline)) pressedF2()    { return pressedButton(f2Mask); }