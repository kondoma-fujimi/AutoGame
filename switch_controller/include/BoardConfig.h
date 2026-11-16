/*
 * SPDX-License-Identifier: MIT
 * SPDX-FileCopyrightText: Copyright (c) 2021 Jason Skuby (mytechtoybox.com)
 */

#ifndef PICO_BOARD_CONFIG_H_
#define PICO_BOARD_CONFIG_H_

//#include <GamepadEnums.h>


// This is the main pin definition section.
// This will let you specify which GPIO pin each button is assigned too. 
// You can set any of the main pins as `-1` to disable it.
// The Turbo pin and LS + RS slider pins can also be set to `-1` to disable that functionality.
// Please note that only when `PIN_BUTTON_TURBO` is set to `-1` will the `T##` be removed from a connected display.
// Please note that only when `PIN_SLIDER_LS` and  `PIN_SLIDER_RS` are set to `-1` will the button combo shortcut for DP/LS/RS work.
// The buttons are listed in GP2040 configuration, beside each the listed order is *GP2040 / Xinput / Switch / PS3 / Directinput / Arcade*
#define PIN_UART_TX     0          // Uart TX
#define PIN_UART_RX     1          // Uart RX
#define PIN_LEFTSTIC_UP     2      // LEFT-UP
#define PIN_LEFTSTIC_DOWN   3      // LEFT-DOWN
#define PIN_LEFTSTIC_RIGHT  4      // LEFT-RIGHT
#define PIN_LEFTSTIC_LEFT   5      // LEFT-LEFT
#define PIN_BUTTON_A        6      // A
#define PIN_BUTTON_B        7      // B
#define PIN_BUTTON_Y        8      // Y
#define PIN_BUTTON_X        9      // X
#define PIN_BUTTON_L        10     // L
#define PIN_BUTTON_R        11     // R
#define PIN_BUTTON_ZL       12     // ZL
#define PIN_BUTTON_ZR       13     // ZR
#define PIN_LED_POWER       14     // LEDの電源
#define PIN_LED_DATA        15     // LEDのデータ
#define PIN_MCP_I2C_SDA     16     // PIN拡張用MCP23017との通信用
#define PIN_MCP_I2C_SCL     17     // PIN拡張用MCP23017との通信用
#define PIN_OLED_I2C_SDA    18     // OLEDとの通信用
#define PIN_OLED_I2C_SCL    19     // OLEDとの通信用
#define PIN_RIGHTSTIC_RIGHT 20     // RIGHT-UP
#define PIN_RIGHTSTIC_LEFT  21     // RIGHT-DOWN
#define PIN_RIGHTSTIC_DOWN  22     // RIGHT-RIGHT
#define PIN_RIGHTSTIC_UP    26     // RIGHT-LEFT
#define PIN_LITTLE_DUSH     27     // 微ダッシュ用
#define PIN_SLIDER_RS       28     // 歩き用

// This is the SOCD section.
// SOCD stands for `simultaneous opposing cardinal directions`.
// There are three options for `DEFAULT_SOCD_MODE` currently:
// 1 - `SOCD_MODE_NEUTRAL` - This is a neutral SOCD.  EG. when you press `up` + `down` no input will be registered.
// 2 - `SOCD_MODE_UP_PRIORITY` - This is up priority SOCD.  EG. when you press `up` + `down` `up` will be registered.
// 3 - `SOCD_MODE_SECOND_INPUT_PRIORITY` - This is last priority SOCD.  EG. when you press and hold `up` then press `down` `down` will be registered.

#define DEFAULT_SOCD_MODE SOCD_MODE_NEUTRAL

#endif
