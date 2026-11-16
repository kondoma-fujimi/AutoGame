#include "tusb.h"
#include "class/hid/hid.h"
#include "GamepadDescriptors.h"

// HIDレポートディスクリプタ（ゲームパッドなど）
const uint8_t hid_report_desc[] = {
   0x05, 0x01,       // Usage Page (Generic Desktop)
  0x09, 0x05,       // Usage (Gamepad)
  0xA1, 0x01,       // Collection (Application)
  // ボタン（16個）
  0x05, 0x09, 0x19, 0x01, 0x29, 0x10,
  0x15, 0x00, 0x25, 0x01,
  0x95, 0x10, 0x75, 0x01, 0x81, 0x02,
  // HATスイッチ
  0x05, 0x01, 0x09, 0x39,
  0x15, 0x00, 0x25, 0x07,
  0x95, 0x01, 0x75, 0x04, 0x81, 0x02,
  // パディング
  0x75, 0x04, 0x95, 0x01, 0x81, 0x03,
  // アナログスティック（4軸）
  0x09, 0x30, 0x09, 0x31, 0x09, 0x32, 0x09, 0x35,
  0x15, 0x00, 0x26, 0xFF, 0x00,
  0x75, 0x08, 0x95, 0x04, 0x81, 0x02,
  0xC0
};

// デバイスディスクリプタ
tusb_desc_device_t const desc_device = {
  .bLength            = sizeof(tusb_desc_device_t),
  .bDescriptorType    = TUSB_DESC_DEVICE,
  .bcdUSB             = 0x0200,
  .bDeviceClass       = 0x00,
  .bDeviceSubClass    = 0x00,
  .bDeviceProtocol    = 0x00,
  .bMaxPacketSize0    = CFG_TUD_ENDPOINT0_SIZE,
  .idVendor           = 0x057E,
  .idProduct          = 0x4000,
  .bcdDevice          = 0x0100,
  .iManufacturer      = 0x01,
  .iProduct           = 0x02,
  .iSerialNumber      = 0x03,
  .bNumConfigurations = 0x01
};

// 構成ディスクリプタ（CDC + HID）
uint8_t const desc_configuration[] = {
  TUD_CONFIG_DESCRIPTOR(1, 3, 0, TUD_CONFIG_DESC_LEN + TUD_CDC_DESC_LEN + TUD_HID_DESC_LEN, 0x00, 100),
  //TUD_CDC_DESCRIPTOR(0, 4, 0x81, 8, 0x02, 0x82, 64),
  TUD_HID_DESCRIPTOR(0, 4, HID_ITF_PROTOCOL_NONE, sizeof(hid_report_desc), 0x83, 64, 1)
};

// ストリングディスクリプタ
char const* string_desc_arr[] = {
  (const char[]) { 0x09, 0x04 }, // 言語ID（英語）
  "Masahiro Inc.",               // Manufacturer
  "Switch Controller",           // Product
  "123456",                      // Serial
  "CDC Interface",              // CDC
  "HID Interface"               // HID
};

// コールバック関数
uint8_t const * tud_descriptor_device_cb(void) {
  return switch_device_descriptor;
}

uint8_t const * tud_descriptor_configuration_cb(uint8_t index) {
  return switch_configuration_descriptor;
}

uint16_t const* tud_descriptor_string_cb(uint8_t index, uint16_t langid) {
  // 省略：文字列ディスクリプタの生成処理
  (void)langid;

  uint16_t size = 0;
	return getStringDescriptor(&size, INPUT_MODE_SWITCH, 3);
}

uint8_t const * tud_hid_descriptor_report_cb(uint8_t instance) {
  (void) instance;
  return switch_report_descriptor;
  //hid_report_descriptor
}

// ホストからの SET_REPORT リクエストを処理
void tud_hid_set_report_cb(uint8_t instance,
                            uint8_t report_id, hid_report_type_t report_type,
                           uint8_t const* buffer, uint16_t bufsize)
{
    // 必要に応じてバッファの内容を処理
    (void)instance;
    // echo back anything we received from host
	  tud_hid_report(report_id, buffer, bufsize);
    // 例：LEDの状態を変える、バッファを保存するなど
}

uint16_t tud_hid_get_report_cb(uint8_t instance, uint8_t report_id, hid_report_type_t report_type,
                               uint8_t* buffer, uint16_t reqlen)
{
    // 必要なレポートを buffer に書き込んで返す
    // TODO: Handle the correct report type, if required
	(void)instance;
	(void)report_id;
	(void)report_type;
	(void)reqlen;

	uint8_t report_size = 0;
	SwitchReport switch_report;
	report_size = sizeof(SwitchReport);
	memcpy(buffer, &switch_report, report_size);

	return report_size;
}
