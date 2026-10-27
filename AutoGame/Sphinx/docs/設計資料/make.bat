@ECHO OFF
REM コマンドを画面に表示しないようにします。

REM このバッチファイルがあるフォルダをスタックに保存して移動します。
pushd %~dp0

REM Sphinxの実行コマンドが未指定なら、現在のPythonでSphinxを起動します。
if "%SPHINXBUILD%" == "" set "SPHINXBUILD=python -m sphinx"
REM Pythonの実行コマンドが未指定なら、PATH上のpythonを使用します。
if "%PYTHON%" == "" set "PYTHON=python"
REM Sphinxの入力元を現在の資料フォルダに設定します。
set SOURCEDIR=.
REM Sphinxの出力先を資料フォルダ内の_buildに設定します。
set BUILDDIR=_build

REM 引数がmanualなら、通常のSphinxビルダーではなく独自スクリプトを使います。
if /I "%1" == "manual" (
	REM 共通スクリプトに入力元フォルダを渡して単一HTMLを作成します。
	%PYTHON% ..\..\build_single_html.py %SOURCEDIR%
	REM 通常のSphinxビルド処理を飛ばして終了処理へ進みます。
	goto end
)

REM Sphinxが起動可能か確認します。標準出力とエラー出力は抑制します。
%SPHINXBUILD% >NUL 2>NUL
REM errorlevel 9009はコマンドが見つからない場合を示します。
if errorlevel 9009 (
	REM 依存パッケージのインストール方法を案内します。
	echo.
	echo.The Sphinx command was not found. Install the dependencies with:
	echo.python -m pip install -r ..\..\requirements.txt
	echo.
	REM エラーコード1でバッチを終了します。
	exit /b 1
)

REM 引数がない場合は、Sphinxが利用可能なビルドターゲットを表示します。
if "%1" == "" goto help

REM 指定されたビルダー（例: html）で資料をビルドします。
%SPHINXBUILD% -M %1 %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%
REM ビルド後に終了処理へ進みます。
goto end

REM 引数がない場合に呼び出されるヘルプ表示用ラベルです。
:help
REM Sphinx自身にビルドターゲット一覧を表示させます。
%SPHINXBUILD% -M help %SOURCEDIR% %BUILDDIR% %SPHINXOPTS% %O%

REM 共通の終了処理を行うラベルです。
:end
REM pushdで保存していた元のフォルダへ戻ります。
popd
