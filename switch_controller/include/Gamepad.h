#ifndef _GAMEPAD_H_
#define _GAMEPAD_H_

#include "BoardConfig.h"
#include <string.h>
#include "pico/stdlib.h"
#include "GamepadState.h"

// MUST BE DEFINED FOR MPG
extern uint32_t getMillis();
extern uint64_t getMicro();

#define GAMEPAD_POLL_MS 1
#define GAMEPAD_POLL_MICRO 100

//#define GAMEPAD_FEATURE_REPORT_SIZE 32
//
//struct GamepadButtonMapping
//{
//	GamepadButtonMapping(uint8_t p, uint16_t bm) : pin(p), pinMask((1 << p)), buttonMask(bm) {}
//
//	uint8_t pin;
//	uint32_t pinMask;
//	const uint16_t buttonMask;
//
//	inline void setPin(uint8_t p)
//	{
//		pin = p;
//		pinMask = 1 << p;
//	}
//};
//
//GamepadState rawState;
//GamepadButtonMapping *mapDpadUp;
//GamepadButtonMapping *mapDpadDown;
//GamepadButtonMapping *mapDpadLeft;
//GamepadButtonMapping *mapDpadRight;
//GamepadButtonMapping *mapButtonB1;
//GamepadButtonMapping *mapButtonB2;
//GamepadButtonMapping *mapButtonB3;
//GamepadButtonMapping *mapButtonB4;
//GamepadButtonMapping *mapButtonL1;
//GamepadButtonMapping *mapButtonR1;
//GamepadButtonMapping *mapButtonL2;
//GamepadButtonMapping *mapButtonR2;
//GamepadButtonMapping *mapButtonS1;
//GamepadButtonMapping *mapButtonS2;
//GamepadButtonMapping *mapButtonL3;
//GamepadButtonMapping *mapButtonR3;
//GamepadButtonMapping *mapButtonA1;
//GamepadButtonMapping *mapButtonA2;
//GamepadButtonMapping *mapButtonReserve1;
//GamepadButtonMapping *mapButtonReserve2;
//GamepadButtonMapping *mapButtonReserve3;
//GamepadButtonMapping **gamepadMappings;
//
//void setup();
//void process();
//void read();
//
////struct GamepadOptions
////{
////	InputMode inputMode {InputMode::INPUT_MODE_XINPUT}; 
////	DpadMode dpadMode {DpadMode::DPAD_MODE_DIGITAL};
////	SOCDMode socdMode {SOCDMode::SOCD_MODE_NEUTRAL};
////	uint32_t checksum;
////	bool invertXAxis;
////	bool invertYAxis;
////};

#endif