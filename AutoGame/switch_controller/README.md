# Switch controller

このプロジェクトは Raspberry Pi Pico SDK 2.1.1 を使用します。
SDK と `pico-examples` はこのリポジトリには含めていないため、別途 SDK を用意し、
サブモジュールを初期化してください。

ビルド前に `PICO_SDK_PATH` を SDK のフォルダに設定します。

```powershell
$env:PICO_SDK_PATH = "C:\path\to\pico-sdk"
cmake -S . -B build -DPICO_BOARD=pico
cmake --build build
```

`build` フォルダとビルド済みの `flash_nuke.uf2` はローカル生成物のため、
Git の追跡対象外です。
