# -*- coding: utf-8 -*-

################################################################################
## Form generated from reading UI file 'form.ui'
##
## Created by: Qt User Interface Compiler version 6.6.1
##
## WARNING! All changes made in this file will be lost when recompiling UI file!
################################################################################

from PySide6.QtCore import (QCoreApplication, QDate, QDateTime, QLocale,
    QMetaObject, QObject, QPoint, QRect,
    QSize, QTime, QUrl, Qt)
from PySide6.QtGui import (QBrush, QColor, QConicalGradient, QCursor,
    QFont, QFontDatabase, QGradient, QIcon,
    QImage, QKeySequence, QLinearGradient, QPainter,
    QPalette, QPixmap, QRadialGradient, QTransform, QPen)
from PySide6.QtWidgets import (QApplication, QLabel, QPushButton, QSizePolicy,
    QTextEdit, QWidget)
from PySide6.QtCore import QTimer, Qt
from ui_auto_game  import AutoGameWindow
import cv2
import datetime
import os
import SwitchController 
save_dir = r"d:\dev\my_Project\AutoGame\image"

class Ui_Widget(object):
    def setupUi(self, Widget):
        if not Widget.objectName():
            Widget.setObjectName(u"Widget")
        Widget.resize(1500, 980)
        Widget.setAutoFillBackground(True)
        self.game_label = QLabel(Widget)
        self.game_label.setObjectName(u"game_label")
        self.game_label.setGeometry(QRect(0, 10, 1500, 700))
        self.game_label.setAutoFillBackground(False)
        self.game_label.setStyleSheet(u"background-color: black;")
        self.game_label.setAlignment(Qt.AlignCenter)
        self.caputureboad_connection_button = QPushButton(Widget)
        self.caputureboad_connection_button.setObjectName(u"caputureboad_connection_button")
        self.caputureboad_connection_button.setGeometry(QRect(10, 710, 151, 40))
        self.caputureboad_connection_button.clicked.connect(self.caputureboad_connection_button_clicked)
        self.screenshot_button = QPushButton(Widget)
        self.screenshot_button.setObjectName(u"screenshot_button")
        self.screenshot_button.setGeometry(QRect(160, 710, 151, 40))
        self.screenshot_button.clicked.connect(self.screenshot_button_clicked)
        self.capture_button = QPushButton(Widget)
        self.capture_button.setObjectName(u"capture_button")
        self.capture_button.setGeometry(QRect(310, 710, 151, 40))
        self.capture_button.clicked.connect(self.capture_button_clicked)
        self.manual_operation_button = QPushButton(Widget)
        self.manual_operation_button.setObjectName(u"manual_operation_button")
        self.manual_operation_button.setGeometry(QRect(460, 710, 151, 40))
        self.auto_operation_button = QPushButton(Widget)
        self.auto_operation_button.setObjectName(u"auto_operation_button")
        self.auto_operation_button.setGeometry(QRect(610, 710, 150, 40))
        self.auto_operation_button.clicked.connect(self.auto_operation_button_clicked)
        self.log_text = QTextEdit(Widget)
        self.log_text.setObjectName(u"log_text")
        self.log_text.setGeometry(QRect(10, 780, 1480, 200))
        self.log_label = QLabel(Widget)
        self.log_label.setObjectName(u"log_label")
        self.log_label.setGeometry(QRect(10, 760, 51, 16))
        self.log_label.setAlignment(Qt.AlignmentFlag.AlignCenter)

        self.conection = 0
        # ビデオキャプチャ（デバイス番号は環境によって異なる）
        self.cap = None
        self.timer = QTimer()
        self.timer.timeout.connect(self.update_frame)
        self.retranslateUi(Widget)

        QMetaObject.connectSlotsByName(Widget)
    # setupUi

    def retranslateUi(self, Widget):
        Widget.setWindowTitle(QCoreApplication.translate("Widget", u"Widget", None))
        self.game_label.setText("")
        self.caputureboad_connection_button.setText(QCoreApplication.translate("Widget", u"\u30ad\u30e3\u30d7\u30c1\u30e3\u30dc\u30fc\u30c9\u63a5\u7d9a / \u7d42\u4e86", None))
        self.screenshot_button.setText(QCoreApplication.translate("Widget", u"\u30b9\u30af\u30ea\u30fc\u30f3\u30b7\u30e7\u30c3\u30c8", None))
        self.capture_button.setText(QCoreApplication.translate("Widget", u"\u9332\u753b", None))
        self.manual_operation_button.setText(QCoreApplication.translate("Widget", u"Manual\u64cd\u4f5c", None))
        self.auto_operation_button.setText(QCoreApplication.translate("Widget", u"Auto\u64cd\u4f5c", None))
        self.log_label.setText(QCoreApplication.translate("Widget", u"\u30ed\u30b0\u51fa\u529b", None))
    # retranslateUi
    def manual_operation_button_clicked(self):
        self.controller = SwitchController();
    def auto_operation_button_clicked(self):
        self.auto_game_window = AutoGameWindow()
        self.auto_game_window.show()

    def caputureboad_connection_button_clicked(self):
        if self.conection == 0:
            self.start_capture()
            self.conection = 1
        else:
            self.end_capture()
            self.conection = 0

    def screenshot_button_clicked(self):
        # QLabelの内容をQPixmapとして取得
        pixmap = self.game_label.pixmap()
        if pixmap:
            timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
            filename = f"capture_{timestamp}.png"
            full_path = os.path.join(save_dir, filename)
            pixmap.save(full_path)
            print(f"{full_path} を保存しました")
        else:
            print("ラベルに表示されている画像がありません")        

    def capture_button_clicked(self):
        output_dir = "frames"
        os.makedirs(output_dir, exist_ok=True)

        i = 0
        while True:
            ret, frame = self.cap.read()
            if not ret:
                break
            cv2.imwrite(f"{output_dir}/frame_{i:05d}.png", frame)
            i += 1

    def start_capture(self):
        if self.cap == None:
            self.cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)  # 適切なデバイス番号に調整
            self.cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1920)
            self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 1080)

            self.timer.start(30)  # 約30FPS
    def end_capture(self):
        self.timer.stop()
        if self.cap:
            self.cap.release()
            self.cap = None
        self.game_label.clear()
        self.game_label.setStyleSheet("background-color: black;")

    def update_frame(self):
        if self.cap and self.cap.isOpened():
            ret, frame = self.cap.read()
            if ret:
                frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
                h, w, ch = frame.shape
                qimg = QImage(frame.data, w, h, ch * w, QImage.Format_RGB888)
                pix = QPixmap.fromImage(qimg).scaled(
                    self.game_label.size(), Qt.KeepAspectRatio, Qt.SmoothTransformation)
                #painter = QPainter(pix)
                #pen = QPen(QColor("yellow"), 4)
                #painter.setPen(pen)
                #painter.drawEllipse(28, 28, 115, 115)  # 枠の位置とサイズ
                #painter.end()

                self.game_label.setPixmap(pix)

    def closeEvent(self, event):
        if self.cap:
            self.cap.release()
        if self.sw_ctr:
            self.sw_ctr.disconnect()
            self.sw_ctr = None
        for i in range(self.stack.count()):
            widget = self.stack.widget(i)
            widget.deleteLater()
        self.stack.close()
        print(self.stack.widget(0))
