# -*- coding: utf-8 -*-

################################################################################
## Form generated from reading UI file 'auto_game.ui'
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
    QPalette, QPixmap, QRadialGradient, QTransform)
from PySide6.QtWidgets import (QApplication, QComboBox, QLabel, QPushButton,
    QRadioButton, QSizePolicy, QTextEdit, QWidget)

class Ui_Form(object):
    def setupUi(self, Form):
        if not Form.objectName():
            Form.setObjectName(u"Form")
        Form.resize(437, 326)
        self.egg_pokemon_label = QLabel(Form)
        self.egg_pokemon_label.setObjectName(u"egg_pokemon_label")
        self.egg_pokemon_label.setGeometry(QRect(20, 50, 111, 21))
        self.egg_radioButton = QRadioButton(Form)
        self.egg_radioButton.setObjectName(u"egg_radioButton")
        self.egg_radioButton.setGeometry(QRect(20, 20, 89, 20))
        self.egg_pokemon_comboBox = QComboBox(Form)
        self.egg_pokemon_comboBox.setObjectName(u"egg_pokemon_comboBox")
        self.egg_pokemon_comboBox.setGeometry(QRect(140, 50, 161, 22))
        self.parent1_label = QLabel(Form)
        self.parent1_label.setObjectName(u"parent1_label")
        self.parent1_label.setGeometry(QRect(20, 80, 41, 21))
        self.parent1_box_label = QLabel(Form)
        self.parent1_box_label.setObjectName(u"parent1_box_label")
        self.parent1_box_label.setGeometry(QRect(180, 80, 141, 21))
        self.parent1_textEdit = QTextEdit(Form)
        self.parent1_textEdit.setObjectName(u"parent1_textEdit")
        self.parent1_textEdit.setGeometry(QRect(70, 80, 104, 21))
        self.parent1_box_textEdit = QTextEdit(Form)
        self.parent1_box_textEdit.setObjectName(u"parent1_box_textEdit")
        self.parent1_box_textEdit.setGeometry(QRect(320, 80, 104, 21))
        self.parent2_box_textEdit = QTextEdit(Form)
        self.parent2_box_textEdit.setObjectName(u"parent2_box_textEdit")
        self.parent2_box_textEdit.setGeometry(QRect(320, 110, 104, 21))
        self.parent2_textEdit = QTextEdit(Form)
        self.parent2_textEdit.setObjectName(u"parent2_textEdit")
        self.parent2_textEdit.setGeometry(QRect(70, 110, 104, 21))
        self.parent2_label = QLabel(Form)
        self.parent2_label.setObjectName(u"parent2_label")
        self.parent2_label.setGeometry(QRect(20, 110, 41, 21))
        self.parent2_box_label = QLabel(Form)
        self.parent2_box_label.setObjectName(u"parent2_box_label")
        self.parent2_box_label.setGeometry(QRect(180, 110, 141, 21))
        self.egg_fireBody_box_label = QLabel(Form)
        self.egg_fireBody_box_label.setObjectName(u"egg_fireBody_box_label")
        self.egg_fireBody_box_label.setGeometry(QRect(180, 140, 141, 21))
        self.egg_fireBody_textEdit = QTextEdit(Form)
        self.egg_fireBody_textEdit.setObjectName(u"egg_fireBody_textEdit")
        self.egg_fireBody_textEdit.setGeometry(QRect(70, 140, 104, 21))
        self.egg_fireBody_box_textEdit_2 = QTextEdit(Form)
        self.egg_fireBody_box_textEdit_2.setObjectName(u"egg_fireBody_box_textEdit_2")
        self.egg_fireBody_box_textEdit_2.setGeometry(QRect(320, 140, 104, 21))
        self.egg_fireBody_label = QLabel(Form)
        self.egg_fireBody_label.setObjectName(u"egg_fireBody_label")
        self.egg_fireBody_label.setGeometry(QRect(20, 140, 51, 21))
        self.want_count_label = QLabel(Form)
        self.want_count_label.setObjectName(u"want_count_label")
        self.want_count_label.setGeometry(QRect(20, 170, 111, 21))
        self.want_count_textEdit = QTextEdit(Form)
        self.want_count_textEdit.setObjectName(u"want_count_textEdit")
        self.want_count_textEdit.setGeometry(QRect(70, 170, 104, 21))
        self.get_count_label = QLabel(Form)
        self.get_count_label.setObjectName(u"get_count_label")
        self.get_count_label.setGeometry(QRect(20, 200, 111, 21))
        self.get_count_textEdit = QTextEdit(Form)
        self.get_count_textEdit.setObjectName(u"get_count_textEdit")
        self.get_count_textEdit.setGeometry(QRect(70, 200, 104, 21))
        self.egg_pokemon_pushButton = QPushButton(Form)
        self.egg_pokemon_pushButton.setObjectName(u"egg_pokemon_pushButton")
        self.egg_pokemon_pushButton.setGeometry(QRect(320, 50, 101, 24))
        self.start_pushButton = QPushButton(Form)
        self.start_pushButton.setObjectName(u"start_pushButton")
        self.start_pushButton.setGeometry(QRect(230, 290, 101, 24))
        self.end_pushButton = QPushButton(Form)
        self.end_pushButton.setObjectName(u"end_pushButton")
        self.end_pushButton.setGeometry(QRect(330, 290, 101, 24))

        self.retranslateUi(Form)

        QMetaObject.connectSlotsByName(Form)
    # setupUi

    def retranslateUi(self, Form):
        Form.setWindowTitle(QCoreApplication.translate("Form", u"Form", None))
        self.egg_pokemon_label.setText(QCoreApplication.translate("Form", u"\u5b75\u5316\u3055\u305b\u305f\u3044\u30dd\u30b1\u30e2\u30f3\uff1a", None))
        self.egg_radioButton.setText(QCoreApplication.translate("Form", u"\u5b75\u5316", None))
        self.parent1_label.setText(QCoreApplication.translate("Form", u"\u89aa1\uff1a", None))
        self.parent1_box_label.setText(QCoreApplication.translate("Form", u"\u89aa1\u306e\u30dd\u30b1\u30e2\u30f3\u304c\u3044\u308b\u30dc\u30c3\u30af\u30b9", None))
        self.parent2_label.setText(QCoreApplication.translate("Form", u"\u89aa2\uff1a", None))
        self.parent2_box_label.setText(QCoreApplication.translate("Form", u"\u89aa2\u306e\u30dd\u30b1\u30e2\u30f3\u304c\u3044\u308b\u30dc\u30c3\u30af\u30b9", None))
        self.egg_fireBody_box_label.setText(QCoreApplication.translate("Form", u"\u5b75\u5316\u7528\u30dd\u30b1\u30e2\u30f3\u304c\u3044\u308b\u30dc\u30c3\u30af\u30b9", None))
        self.egg_fireBody_label.setText(QCoreApplication.translate("Form", u"\u5b75\u5316\u7528\uff1a", None))
        self.want_count_label.setText(QCoreApplication.translate("Form", u"\u6b32\u3057\u3044\u6570", None))
        self.get_count_label.setText(QCoreApplication.translate("Form", u"\u30b2\u30c3\u30c8\u6570", None))
        self.egg_pokemon_pushButton.setText(QCoreApplication.translate("Form", u"\u753b\u50cf\u304b\u3089\u691c\u7d22", None))
        self.start_pushButton.setText(QCoreApplication.translate("Form", u"Start", None))
        self.end_pushButton.setText(QCoreApplication.translate("Form", u"End", None))
    # retranslateUi
class AutoGameWindow(QWidget):
    def __init__(self):
        super().__init__()
        self.ui = Ui_Form()
        self.ui.setupUi(self)
