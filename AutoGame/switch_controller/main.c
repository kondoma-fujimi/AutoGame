#include "pico/stdlib.h"
#include "pico/time.h"
#include "hardware/uart.h"
#include "tusb.h"
#include <ctype.h>
#include "GamepadDescriptors.h"
#include "usb_driver.h"
#include "GamepadState.h"
#include "BoardConfig.h"
#include "bsp/board.h"
#include "ws2812.pio.h"

#define GAMEPAD_POLL_MICRO 100
#define UART0_IRQ 20
#define NUM_LEDS 8

const uint8_t dpadMasks[] =
{
	GAMEPAD_MASK_UP,
	GAMEPAD_MASK_DOWN,
	GAMEPAD_MASK_LEFT,
	GAMEPAD_MASK_RIGHT,
};

const uint16_t buttonMasks[] =
{
	GAMEPAD_MASK_B1,
	GAMEPAD_MASK_B2,
	GAMEPAD_MASK_B3,
	GAMEPAD_MASK_B4,
	GAMEPAD_MASK_L1,
	GAMEPAD_MASK_R1,
	GAMEPAD_MASK_L2,
	GAMEPAD_MASK_R2,
	GAMEPAD_MASK_S1,
	GAMEPAD_MASK_S2,
	GAMEPAD_MASK_L3,
	GAMEPAD_MASK_R3,
	GAMEPAD_MASK_A1,
	GAMEPAD_MASK_A2,
};

GamepadState state = {
	0,            // dpad
    0,            // buttons
    0,            // aux
    GAMEPAD_JOYSTICK_MID, // lx
    GAMEPAD_JOYSTICK_MID, // ly
    GAMEPAD_JOYSTICK_MID, // rx
    GAMEPAD_JOYSTICK_MID, // ry
    0,            // lt
    0             // rt
};

bool key_state[256] = {false};
SwitchReport switchReport = {0};
uint64_t nextRuntime;
volatile uint8_t rxBuffer[8];
volatile uint8_t rxIndex = 0;


uint64_t getMicro() {
	return to_us_since_boot(get_absolute_time());
}
//void send_hid_report() {
//    static uint8_t prev_report[8] = {0};
//    static absolute_time_t last_sent_time;
//    const uint32_t resend_interval_ms = 8;  // 15ms���Ƃɍđ��M
//
//    uint8_t report[8] = {0};
//
//    // �{�^���iA, B, X, Y�j
//    if (key_state['H']) report[2] |= 0x08;
//    if (key_state['J']) report[2] |= 0x04;
//    if (key_state['K']) report[2] |= 0x02;
//    if (key_state['L']) report[2] |= 0x01;
//
//    // �{�^���iL, R, ZL, ZR�j
//    if (key_state['Y']) report[0] |= 0x40;
//    if (key_state['U']) report[0] |= 0x80;
//    if (key_state['I']) report[0] |= 0x20;
//    if (key_state['O']) report[0] |= 0x10;
//
//    // �{�^���i+, -, Home, Capture�j
//    if (key_state['T']) report[1] |= 0x01;
//    if (key_state['R']) report[1] |= 0x02;
//    if (key_state['F']) report[1] |= 0x10;
//    if (key_state['G']) report[1] |= 0x20;
//
//    // �{�^���iL�X�e��������, R�X�e�������݁j
//    if (key_state['E']) report[1] |= 0x04;
//    if (key_state['P']) report[1] |= 0x08;
//
//    // �\���L�[�iHAT�j
//    if (key_state['5']) report[3] = 0x00;
//    else if (key_state['6']) report[3] = 0x02;
//    else if (key_state['7']) report[3] = 0x04;
//    else if (key_state['8']) report[3] = 0x06;
//    else report[3] = 0x08;
//
//    // ���X�e�B�b�N
//    report[4] = key_state['A'] ? 0x00 : key_state['D'] ? 0xFF : 0x80;
//    report[5] = key_state['W'] ? 0x00 : key_state['S'] ? 0xFF : 0x80;
//
//    // �E�X�e�B�b�N
//    report[6] = key_state['1'] ? 0x00 : key_state['2'] ? 0xFF : 0x80;
//    report[7] = key_state['3'] ? 0x00 : key_state['4'] ? 0xFF : 0x80;
//
//    // ���ݎ���
//    absolute_time_t now = get_absolute_time();
//    bool changed = memcmp(report, prev_report, sizeof(report)) != 0;
//    bool timeout = absolute_time_diff_us(last_sent_time, now) / 1000 >= resend_interval_ms;
//
//    if (changed || timeout) {
//        tud_hid_report(0, report, sizeof(report));
//        memcpy(prev_report, report, sizeof(report));
//        last_sent_time = now;
//    }
//}

void on_uart_rx() {
    while (uart_is_readable(uart0)) {
        rxBuffer[rxIndex++] = uart_getc(uart0);

        if (rxIndex == 8) {
            memcpy(&switchReport, (const uint8_t *)rxBuffer, 8);
            rxIndex = 0;
            send_report(&switchReport, getReportSize());  // ������Switch�֔��f
        }
    }
}


int main() {
    board_init();
    stdio_init_all();
    uart_init(uart0, 115200);
    gpio_set_function(0, GPIO_FUNC_UART);
    gpio_set_function(1, GPIO_FUNC_UART);

    irq_set_exclusive_handler(UART0_IRQ, on_uart_rx);
    irq_set_enabled(UART0_IRQ, true);
    uart_set_irq_enables(uart0, true, false);
    
    PIO pio = pio0;
    uint offset = pio_add_program(pio, &ws2812_program);
    uint sm = pio_claim_unused_sm(pio, true);
    ws2812_program_init(pio, sm, offset, PIN_LED_DATA, 800000, true);
    gpio_init(PIN_LED_POWER);
    gpio_set_dir(PIN_LED_POWER, true);   // �o�͂ɐݒ�
    gpio_put(PIN_LED_POWER, 1);          // HIGH �� ��3.3V�o��


    tusb_init();
    InputMode inputMode = INPUT_MODE_SWITCH;
    initialize_driver(inputMode);

    while (true) {
        
        if (nextRuntime > getMicro()) { // fix for unsigned
			sleep_us(50); // Give some time back to our CPU (lower power consumption)
			continue;
		}

        for (int i = 0; i < NUM_LEDS; ++i) {
            uint32_t color = 0xFF000000; // �΁iGRB���j
            pio_sm_put_blocking(pio, sm, color);
        }
        //sleep_ms(500);
        // UART��M�i�u���b�L���O�܂��͔񓯊��ł��j
        //uart_read_blocking(uart0, (uint8_t*)&switchReport, sizeof(SwitchReport));
        // ���|�[�g��M��ɂ��̂܂ܕԂ���
        //uart_write_blocking(uart0, (uint8_t*)&switchReport, sizeof(SwitchReport));
        // USB FEATURES : Send/Get USB Features (including Player LEDs on X-Input)
        //switchReport = getSwitchReport();
		send_report(&switchReport, getReportSize());
        tud_task();
        nextRuntime = getMicro() + GAMEPAD_POLL_MICRO;


        // UART��M����
        //while (uart_is_readable(uart0)) {
        //    char c = uart_getc(uart0);
        //    if (islower(c)) key_state[toupper(c)] = true;   // ����
        //    else if (isupper(c)) key_state[c] = false;       // ����
        //}
//
        //send_hid_report();
        // sleep_ms(20);
    }
}
