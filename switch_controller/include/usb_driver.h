/*
 * SPDX-License-Identifier: MIT
 * SPDX-FileCopyrightText: Copyright (c) 2021 Jason Skuby (mytechtoybox.com)
 */

#pragma once

#include "GamepadDescriptors.h"
#include <stdbool.h>

typedef enum
{
	USB_MODE_HID,
	USB_MODE_NET,
} UsbMode;

InputMode get_input_mode(void);
void initialize_driver(InputMode mode);
void receive_report(uint8_t *buffer);
void send_report(void *report, uint16_t report_size);
bool send_hid_report(uint8_t report_id, void *report, uint8_t report_size);
// bool send_xinput_report(void *report, uint8_t report_size);

