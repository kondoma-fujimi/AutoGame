# Sphinxの最小構成のMakefileです。
# Minimal makefile for Sphinx documentation

# Sphinxに追加で渡すオプションです。未指定なら空文字列です。
SPHINXOPTS    ?=
# Sphinxビルドコマンドです。環境変数やmake引数で上書きできます。
SPHINXBUILD   ?= sphinx-build
# Pythonコマンドです。環境変数やmake引数で上書きできます。
PYTHON        ?= python
# Sphinxの入力元はこのMakefileがある資料フォルダです。
SOURCEDIR     = .
# ビルド結果の出力先は資料フォルダ内の_buildです。
BUILDDIR      = _build

# makeだけで実行した場合に表示する、利用可能なターゲットです。
help:
	@$(SPHINXBUILD) -M help "$(SOURCEDIR)" "$(BUILDDIR)" $(SPHINXOPTS) $(O)

# 1つのHTMLファイルを作る独自ターゲットです。
# 共通スクリプトに現在の資料フォルダを渡して単一HTMLを作成します。
manual:
	@$(PYTHON) ../../build_single_html.py "$(SOURCEDIR)"

# help・manual・Makefileは実ファイルを作るターゲットではありません。
.PHONY: help manual Makefile

# 上記以外のターゲット（例: html）はSphinxへそのまま渡します。
# 指定されたターゲットをSphinxのmakeモードでビルドします。
%: Makefile
	@$(SPHINXBUILD) -M $@ "$(SOURCEDIR)" "$(BUILDDIR)" $(SPHINXOPTS) $(O)
