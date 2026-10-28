#include "Gamepad.h"

//void read()
//{
//	// Need to invert since we're using pullups
//	uint32_t values = ~gpio_get_all();
//
//	#ifdef PIN_SETTINGS
//	state.aux = 0
//		| ((values & (1 << PIN_SETTINGS)) ? (1 << 0) : 0)
//	;
//	#endif
//
//	state.dpad = 0
//		| ((values & mapDpadUp->pinMask)    ? (options.invertYAxis ? mapDpadDown->buttonMask : mapDpadUp->buttonMask) : 0)
//		| ((values & mapDpadDown->pinMask)  ? (options.invertYAxis ? mapDpadUp->buttonMask : mapDpadDown->buttonMask) : 0)
//		| ((values & mapDpadLeft->pinMask)  ? mapDpadLeft->buttonMask  : 0)
//		| ((values & mapDpadRight->pinMask) ? mapDpadRight->buttonMask : 0)
//	;
//
//	state.buttons = 0
//		| ((values & mapButtonB1->pinMask)  ? mapButtonB1->buttonMask  : 0)
//		| ((values & mapButtonB2->pinMask)  ? mapButtonB2->buttonMask  : 0)
//		| ((values & mapButtonB3->pinMask)  ? mapButtonB3->buttonMask  : 0)
//		| ((values & mapButtonB4->pinMask)  ? mapButtonB4->buttonMask  : 0)
//		| ((values & mapButtonL1->pinMask)  ? mapButtonL1->buttonMask  : 0)
//		| ((values & mapButtonR1->pinMask)  ? mapButtonR1->buttonMask  : 0)
//		| ((values & mapButtonL2->pinMask)  ? mapButtonL2->buttonMask  : 0)
//		| ((values & mapButtonR2->pinMask)  ? mapButtonR2->buttonMask  : 0)
//		| ((values & mapButtonS1->pinMask)  ? mapButtonS1->buttonMask  : 0)
//		| ((values & mapButtonS2->pinMask)  ? mapButtonS2->buttonMask  : 0)
//		| ((values & mapButtonL3->pinMask)  ? mapButtonL3->buttonMask  : 0)
//		| ((values & mapButtonR3->pinMask)  ? mapButtonR3->buttonMask  : 0)
//		| ((values & mapButtonA1->pinMask)  ? mapButtonA1->buttonMask  : 0)
//		| ((values & mapButtonA2->pinMask)  ? mapButtonA2->buttonMask  : 0)
//		/*| ((values & mapButtonReserve1->pinMask)  ? mapButtonReserve1->buttonMask  : 0)
//		| ((values & mapButtonReserve2->pinMask)  ? mapButtonReserve2->buttonMask  : 0)
//		| ((values & mapButtonReserve3->pinMask)  ? mapButtonReserve3->buttonMask  : 0)
//		*/
//	;
//
//	state.lx = GAMEPAD_JOYSTICK_MID;
//	state.ly = GAMEPAD_JOYSTICK_MID;
//	state.rx = GAMEPAD_JOYSTICK_MID;
//	state.ry = GAMEPAD_JOYSTICK_MID;
//	state.lt = 0;
//	state.rt = 0;
//}