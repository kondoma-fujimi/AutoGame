from PySide6.QtWidgets import QWidget, QMessageBox
from PySide6.QtCore import QTimer
import serial
import struct
import keyboard
import serial.tools.list_ports

# ボタンマスク定義（C側と一致させる）
SWITCH_MASK_A = 1 << 2
SWITCH_MASK_B = 1 << 1
SWITCH_MASK_X = 1 << 3
SWITCH_MASK_Y = 1 << 0
SWITCH_MASK_L = 1 << 4
SWITCH_MASK_R = 1 << 5
SWITCH_MASK_ZL = 1 << 6
SWITCH_MASK_ZR = 1 << 7
SWITCH_MASK_PLUS = 1 << 9
SWITCH_MASK_MINUS = 1 << 8
SWITCH_MASK_HOME = 1 << 12
SWITCH_MASK_CAP = 1 << 13

# HAT定義（D-Pad）
HAT_UP = 0x00
HAT_RIGHT = 0x02
HAT_DOWN = 0x04
HAT_LEFT = 0x06
HAT_NONE = 0x08

# 初期状態
buttons = 0
hat = HAT_NONE
lx = 0x80
ly = 0x80
rx = 0x80
ry = 0x80
vendor = 0x00

class SwitchController(QWidget):
    def __init__(self):
        super().__init__()
        # タイマーで定期送信
        self.timer = QTimer(self)
        self.timer.timeout.connect(self.send_report)
        self.ser = serial.Serial()

    def send_report(self):
        report = self.build_report()
        self.ser.write(report)
        print(report)

    def build_report(self):
        global buttons, hat, lx, ly, rx, ry, vendor

        buttons = 0
        hat = HAT_NONE

        # ボタン入力
        if keyboard.is_pressed('h'): buttons |= SWITCH_MASK_A
        if keyboard.is_pressed('j'): buttons |= SWITCH_MASK_B
        if keyboard.is_pressed('k'): buttons |= SWITCH_MASK_X
        if keyboard.is_pressed('l'): buttons |= SWITCH_MASK_Y
        if keyboard.is_pressed('y'): buttons |= SWITCH_MASK_L
        if keyboard.is_pressed('u'): buttons |= SWITCH_MASK_R
        if keyboard.is_pressed('i'): buttons |= SWITCH_MASK_ZL
        if keyboard.is_pressed('o'): buttons |= SWITCH_MASK_ZR
        if keyboard.is_pressed('t'): buttons |= SWITCH_MASK_PLUS
        if keyboard.is_pressed('r'): buttons |= SWITCH_MASK_MINUS
        if keyboard.is_pressed('f'): buttons |= SWITCH_MASK_HOME
        if keyboard.is_pressed('g'): buttons |= SWITCH_MASK_CAP

        # D-Pad入力
        if keyboard.is_pressed('8'):
            hat = HAT_UP
        elif keyboard.is_pressed('6'):
            hat = HAT_RIGHT
        elif keyboard.is_pressed('2'):
            hat = HAT_DOWN
        elif keyboard.is_pressed('4'):
            hat = HAT_LEFT

        # アナログスティック（例：WASDで左スティック）
        lx = 0x80
        ly = 0x80
        if keyboard.is_pressed('a'): lx = 0x00  # 左
        if keyboard.is_pressed('d'): lx = 0xFF  # 右
        if keyboard.is_pressed('w'): ly = 0x00  # 上
        if keyboard.is_pressed('s'): ly = 0xFF  # 下

        rx = 0x80
        ry = 0x80
        if keyboard.is_pressed(';'): rx = 0x00  # 左
        if keyboard.is_pressed(']'): rx = 0xFF  # 右
        if keyboard.is_pressed('@'): ry = 0x00  # 上
        if keyboard.is_pressed(':'): ry = 0xFF  # 下

        # vendor領域（任意）
        vendor = 0x00
        return struct.pack('<H6B', buttons, hat, lx, ly, rx, ry, vendor)

    def control_timer(self, state):
        available_ports = [port.device for port in serial.tools.list_ports.comports()]
        if state:
            if 'COM3' in available_ports:
                try:
                    # UARTポート設定
                    self.ser.port = 'COM3'
                    self.ser.baudrate = 115200
                    self.ser.timeout = 1
                    self.ser.open()
                    # self.ser = serial.Serial('COM3', 115200)  # FTDIのポートに合わせて変更
                    self.timer.start(20)

                except serial.SerialException as e:
                    QMessageBox.critical(None, "通信エラー", f"COM3には接続できませんでした。\n原因: {str(e)}")
                    if self.ser.is_open:
                        self.ser.close()
                        self.timer.stop()
            else:
                QMessageBox.information(None, "接続確認", "COM3ポートが見つかりませんでした！")
        else:
            if self.ser.is_open:
                self.ser.close()
            self.timer.stop()

    def disconnect(self):
        print("GameWindowによって閉じられました")
        if self.ser.is_open:
            self.ser.close()
        self.timer.stop()