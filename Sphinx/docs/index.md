# AutoPlayCraft ドキュメント

資料は種類ごとに独立した Sphinx プロジェクトとして管理しています。
各フォルダから個別にビルドできます。

## 資料

- [要件定義](要件定義/index.md)
- [設計資料](設計資料/index.md)

## ビルド方法

事前にリポジトリのルートで Sphinx と拡張機能をインストールします。

```powershell
python -m pip install -r Sphinx\requirements.txt
```

Windows では、対象フォルダへ移動して `make.bat` を実行します。

```powershell
Set-Location Sphinx\docs\要件定義
.\make.bat html
```

通常の HTML は `_build\html\` に、CSS・JavaScript・画像を埋め込んだ
単一ファイルは次のコマンドで `_build\manual.html` に出力されます。

```powershell
.\make.bat manual
```

設計資料も同じ手順でビルドできます。Sphinx 標準の `singlehtml` ビルダーを
使う場合は `.\make.bat singlehtml` を実行してください。
