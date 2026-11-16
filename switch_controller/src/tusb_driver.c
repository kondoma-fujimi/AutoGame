#include <stdint.h>
#include "usb_driver.h"
#include "tusb_config.h"
#include "tusb.h"
#include "class/hid/hid.h"
#include "GamepadDescriptors.h"

InputMode input_mode = INPUT_MODE_XINPUT;

InputMode get_input_mode(void)
{
	return input_mode;
}

void initialize_driver(InputMode mode)
{
	input_mode = mode;
	//if (mode == INPUT_MODE_CONFIG)
	//	usb_mode = USB_MODE_NET;

	//tusb_init();
}

void send_report(void *report, uint16_t report_size)
{
	static uint8_t previous_report[CFG_TUD_ENDPOINT0_SIZE] = { };

	if (tud_suspended())
		tud_remote_wakeup();

	if (memcmp(previous_report, report, report_size) != 0)
	{
		bool sent = false;
		sent = send_hid_report(0, report, report_size);

		if (sent)
			memcpy(previous_report, report, report_size);
	}
}

bool send_hid_report(uint8_t report_id, void *report, uint8_t report_size)
{
	if (tud_hid_ready())
		return tud_hid_report(report_id, report, report_size);

	return false;
}

//bool send_xinput_report(void *report, uint8_t report_size)
//{
//	bool sent = false;
//
//	if (
//		tud_ready() &&                                          // Is the device ready?
//		(endpoint_in != 0) && (!usbd_edpt_busy(0, endpoint_in)) // Is the IN endpoint available?
//	) {
//		usbd_edpt_claim(0, endpoint_in);                        // Take control of IN endpoint
//		usbd_edpt_xfer(0, endpoint_in, (uint8_t *)report, report_size);    // Send report buffer
//		usbd_edpt_release(0, endpoint_in);                      // Release control of IN endpoint
//		sent = true;
//	}
//
//	return sent;
//}