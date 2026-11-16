# Switch controller

このプロジェクトは Raspberry Pi Pico SDK 2.1.1 を使用します。
SDK はこのリポジトリに含まれていないため、別途インストールしてください。

## VS Code でビルド

1. `AutoGame` フォルダーを VS Code で開きます。
2. Pico SDK 2.1.1 と付属の ARM GCC、CMake、Ninja が `%USERPROFILE%\.pico-sdk` にインストールされていることを確認します。
3. **Ctrl+Shift+B** を押し、**Build Switch Controller** を実行します。初回はCMake構成後にビルドし、以後も同じタスクで再構成とビルドを行います。

生成物は `switch_controller\build-vscode` に出力されます。古い `build` フォルダーのキャッシュは再利用しません。
