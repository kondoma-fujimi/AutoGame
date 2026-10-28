#include "GamepadDescriptors.h"
#include "GamepadState.h"
#include "usb_driver.h"
#include "MPG.h"

//static SwitchReport switchReport;
//uint8_t endpoint_in = 0;
//void * getReport()
//{
//	switchReport = getSwitchReport();
//	return &switchReport;
//}
//
//SwitchReport getSwitchReport()
//{
//	SwitchReport report = 
//	{
//		.buttons = 0,
//		.hat = SWITCH_HAT_NOTHING,
//		.lx = (uint8_t)(state.lx >> 8),
//		.ly = (uint8_t)(state.ly >> 8),
//		.rx = (uint8_t)(state.rx >> 8),
//		.ry = (uint8_t)(state.ry >> 8),
//		.vendor = 0,
//	};
//
//	switch (state.dpad & GAMEPAD_MASK_DPAD)
//	{
//		case GAMEPAD_MASK_UP:                        report.hat = SWITCH_HAT_UP;        break;
//		case GAMEPAD_MASK_UP | GAMEPAD_MASK_RIGHT:   report.hat = SWITCH_HAT_UPRIGHT;   break;
//		case GAMEPAD_MASK_RIGHT:                     report.hat = SWITCH_HAT_RIGHT;     break;
//		case GAMEPAD_MASK_DOWN | GAMEPAD_MASK_RIGHT: report.hat = SWITCH_HAT_DOWNRIGHT; break;
//		case GAMEPAD_MASK_DOWN:                      report.hat = SWITCH_HAT_DOWN;      break;
//		case GAMEPAD_MASK_DOWN | GAMEPAD_MASK_LEFT:  report.hat = SWITCH_HAT_DOWNLEFT;  break;
//		case GAMEPAD_MASK_LEFT:                      report.hat = SWITCH_HAT_LEFT;      break;
//		case GAMEPAD_MASK_UP | GAMEPAD_MASK_LEFT:    report.hat = SWITCH_HAT_UPLEFT;    break;
//		default:                                     report.hat = SWITCH_HAT_NOTHING;   break;
//	}
//
//	report.buttons = 0
//		| (pressedB1() ? SWITCH_MASK_B       : 0)
//		| (pressedB2() ? SWITCH_MASK_A       : 0)
//		| (pressedB3() ? SWITCH_MASK_Y       : 0)
//		| (pressedB4() ? SWITCH_MASK_X       : 0)
//		| (pressedL1() ? SWITCH_MASK_L       : 0)
//		| (pressedR1() ? SWITCH_MASK_R       : 0)
//		| (pressedL2() ? SWITCH_MASK_ZL      : 0)
//		| (pressedR2() ? SWITCH_MASK_ZR      : 0)
//		| (pressedS1() ? SWITCH_MASK_MINUS   : 0)
//		| (pressedS2() ? SWITCH_MASK_PLUS    : 0)
//		| (pressedL3() ? SWITCH_MASK_L3      : 0)
//		| (pressedR3() ? SWITCH_MASK_R3      : 0)
//		| (pressedA1() ? SWITCH_MASK_HOME    : 0)
//		| (pressedA2() ? SWITCH_MASK_CAPTURE : 0)
//	;
//
//	return report;
//}
//
//
uint16_t getReportSize()
{
	return sizeof(SwitchReport);
}