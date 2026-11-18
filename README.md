# AutoGame
ゲームの自動化

## フォルダ構成

```text
.
├── README.md
└── AutoGame/
    ├── .vscode/
    ├── AI/
    ├── MainWindow/
    ├── Sphinx/
    │   └── docs/
    │       ├── manual/
    │       ├── 要件定義/
    │       └── 設計資料/
    ├── switch_controller/
    │   ├── .vscode/
    │   ├── include/
    │   └── src/
    └── XAML/
        └── AutoGameApprication/
```

## フォルダの説明

- `AutoGame/`: アプリケーション本体のプロジェクトフォルダです。
- `AutoGame/.vscode/`: VS Code の設定やビルドタスクを管理します。
- `AutoGame/AI/`: AI 関連機能を置くためのフォルダです。現在はプレースホルダーです。
- `AutoGame/MainWindow/`: Python / PySide6 版の GUI と、コントローラー・映像処理に関するコードや UI 定義を置きます。
- `AutoGame/Sphinx/`: Sphinx ドキュメントの設定、依存パッケージ、ビルド用スクリプトを置きます。
  - `docs/manual/`: 機器の接続方法や Git の使い方など、利用・作業マニュアルです。
  - `docs/要件定義/`: システムの背景、機能、制約などの要件定義資料です。
  - `docs/設計資料/`: GUI の画面・フローや開発タスクなどの設計資料です。
- `AutoGame/switch_controller/`: Raspberry Pi Pico 用の Switch コントローラーのファームウェアです。
  - `include/`: コントローラー状態、USB/HID、ボード設定などのヘッダーを置きます。
  - `src/`: ゲームパッド、USB ドライバー、USB ディスクリプターなどの実装を置きます。
  - `.vscode/`: Pico SDK を使った VS Code のビルド・デバッグ設定です。
- `AutoGame/XAML/AutoGameApprication/`: C# / WPF 版のデスクトップアプリケーションです。画面、シリアルポート接続、コントローラー制御などを扱います。

`AutoGame/requirements.txt` は Python 側の依存パッケージ一覧、`AutoGame/AutoGaming.drawio` はシステム構成図です。
