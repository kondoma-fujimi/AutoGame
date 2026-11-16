"""Build a Sphinx singlehtml document and inline its local assets."""

# __future__の機能を有効にし、注釈の前方参照などを扱えるようにします。
from __future__ import annotations

# コマンドライン引数を解析します。
import argparse
# バイナリデータをBase64形式に変換します。
import base64
# ファイル名からMIMEタイプを推測します。
import mimetypes
# HTML/CSSのパターン検索と置換に使います。
import re
# Sphinxコマンドを子プロセスとして起動します。
import subprocess
# 実行中のPythonと終了コードを参照します。
import sys
# ファイルパスをOS非依存の方法で扱います。
from pathlib import Path
# URLのデコードと構成要素の解析に使います。
from urllib.parse import unquote, urlsplit


# HTML属性の名前と引用符付き値を取り出す正規表現です。
ATTRIBUTE_RE = re.compile(
    r"""(?P<name>[\w:-]+)\s*=\s*(?P<quote>["'])(?P<value>.*?)(?P=quote)""",
    re.IGNORECASE | re.DOTALL,
)
# linkタグ全体を見つける正規表現です。
LINK_RE = re.compile(r"<link\b[^>]*>", re.IGNORECASE)
# scriptタグと開始タグ属性・本文を取り出す正規表現です。
SCRIPT_RE = re.compile(
    r"<script\b(?P<opening>[^>]*)>(?P<body>.*?)</script\s*>",
    re.IGNORECASE | re.DOTALL,
)
# styleタグと開始タグ属性・本文を取り出す正規表現です。
STYLE_RE = re.compile(
    r"<style\b(?P<opening>[^>]*)>(?P<body>.*?)</style\s*>",
    re.IGNORECASE | re.DOTALL,
)
# リソースを参照するHTMLタグを見つける正規表現です。
RESOURCE_TAG_RE = re.compile(
    r"<(?:img|source|video|audio|image|embed|object|track)\b[^>]*>",
    re.IGNORECASE,
)
# CSSのurl(...)形式を見つける正規表現です。
URL_RE = re.compile(r"""url\(\s*(?P<quote>["']?)(?P<url>.*?)(?P=quote)\s*\)""", re.IGNORECASE)
# CSSの引用符付き@importを見つける正規表現です。
PLAIN_IMPORT_RE = re.compile(
    r"""@import\s+(?P<quote>["'])(?P<url>[^"']+)(?P=quote)""",
    re.IGNORECASE,
)
# HTMLリソース属性の名前とURLを取り出す正規表現です。
RESOURCE_ATTRIBUTE_RE = re.compile(
    r"""(?P<prefix>\b(?:src|poster|data|href|xlink:href)\s*=\s*)(?P<quote>["'])(?P<url>.*?)(?P=quote)""",
    re.IGNORECASE | re.DOTALL,
)
# 拡張子から判別しにくいフォントのMIMEタイプを定義します。
FONT_MEDIA_TYPES = {
    ".eot": "application/vnd.ms-fontobject",
    ".otf": "font/otf",
    ".ttf": "font/ttf",
    ".woff": "font/woff",
    ".woff2": "font/woff2",
}


# HTMLタグから指定名の属性値を返します。
def get_attribute(tag: str, name: str) -> str | None:
    # タグ内の属性を順に調べます。
    for match in ATTRIBUTE_RE.finditer(tag):
        # 属性名は大文字小文字を区別せずに比較します。
        if match.group("name").lower() == name.lower():
            # 一致した属性の値を返します。
            return match.group("value")
    # 指定された属性がなければNoneを返します。
    return None


# URLをビルド出力内の実ファイルパスに解決します。
def resolve_asset(reference: str, base_dir: Path, build_root: Path) -> Path | None:
    # URLを解析して、スキーム・ホスト・パスを分けます。
    parsed = urlsplit(reference.strip())
    # 外部URL、ホスト指定URL、空のパスはローカル資産ではありません。
    if parsed.scheme or parsed.netloc or not parsed.path:
        return None

    # URLデコード後、ルート相対か現在ファイル相対かに応じてパスを作ります。
    path = Path(unquote(parsed.path).lstrip("/"))
    asset = (build_root / path if parsed.path.startswith("/") else base_dir / path).resolve()
    try:
        # 解決先がビルドフォルダの中にあることを確認します。
        asset.relative_to(build_root)
    # ビルドフォルダ外を指すパスは安全のため拒否します。
    except ValueError as error:
        raise ValueError(f"Asset URL escapes the Sphinx build directory: {reference}") from error
    # 参照先ファイルが存在しなければ、場所を示してエラーにします。
    if not asset.is_file():
        raise FileNotFoundError(f"Referenced asset does not exist: {asset}")
    # 存在するローカル資産の絶対パスを返します。
    return asset


# バイナリをBase64にしてdata URIへ変換します。
def to_data_uri(content: bytes, media_type: str) -> str:
    # バイナリをASCII文字列のBase64へ変換します。
    encoded = base64.b64encode(content).decode("ascii")
    # MIMEタイプとBase64データを組み合わせたURIを返します。
    return f"data:{media_type};base64,{encoded}"


# ファイルの拡張子などからMIMEタイプを決定します。
def get_media_type(asset: Path) -> str:
    # フォントは明示表を優先し、それ以外は標準推測、最後は汎用型を使います。
    return FONT_MEDIA_TYPES.get(
        asset.suffix.lower(),
        mimetypes.guess_type(asset.name)[0] or "application/octet-stream",
    )


# CSS内のimportとurl参照をdata URIに置き換えます。
def inline_css(
    css: str,
    css_path: Path,
    build_root: Path,
    resolving: set[Path],
    external_assets: set[str],
) -> str:
    # 再帰中のCSSを再度処理すると循環参照になるため検出します。
    if css_path in resolving:
        raise ValueError(f"CSS import cycle detected at {css_path}")

    # このCSSを再帰処理中として記録します。
    resolving.add(css_path)
    try:
        # CSS url(...)の参照をローカル資産または外部資産として処理します。
        def replace_url(match: re.Match[str]) -> str:
            # URL前後の空白を除いて参照先を解決します。
            reference = match.group("url").strip()
            asset = resolve_asset(reference, css_path.parent, build_root)
            # ローカルファイルでない参照はそのまま残します。
            if asset is None:
                # 外部URLはあとで利用者に通知できるよう記録します。
                scheme = urlsplit(reference).scheme
                if reference.startswith("//") or (scheme and scheme.lower() != "data"):
                    external_assets.add(reference)
                return match.group(0)

            # 資産ファイルの内容とMIMEタイプを取得します。
            content = asset.read_bytes()
            media_type = get_media_type(asset)
            # CSS自身が別CSSを参照する場合も再帰的に埋め込みます。
            if media_type == "text/css":
                nested_css = inline_css(
                    content.decode("utf-8", errors="replace"),
                    asset,
                    build_root,
                    resolving,
                    external_assets,
                )
                content = nested_css.encode("utf-8")
                media_type = "text/css"

            # 読み込んだ資産をdata URIへ変換します。
            data_uri = to_data_uri(content, media_type)
            # URLのフラグメント（例: SVGの#symbol）を保持します。
            parsed = urlsplit(reference)
            if parsed.fragment:
                data_uri += f"#{parsed.fragment}"
            # 元の引用符を維持したurl(...)表記を返します。
            quote = match.group("quote")
            return f"url({quote}{data_uri}{quote})"

        # CSSの@import参照を読み込んで埋め込みます。
        def replace_plain_import(match: re.Match[str]) -> str:
            # import先をCSSファイルの場所から解決します。
            reference = match.group("url").strip()
            asset = resolve_asset(reference, css_path.parent, build_root)
            # 外部importは変更せず、外部資産として記録します。
            if asset is None:
                scheme = urlsplit(reference).scheme
                if reference.startswith("//") or (scheme and scheme.lower() != "data"):
                    external_assets.add(reference)
                return match.group(0)
            # import先のCSSを読み、さらにその中の資産も埋め込みます。
            imported = inline_css(
                asset.read_text(encoding="utf-8", errors="replace"),
                asset,
                build_root,
                resolving,
                external_assets,
            )
            # 埋め込み済みCSSをdata URIとして返します。
            data_uri = to_data_uri(imported.encode("utf-8"), "text/css")
            return f"@import url('{data_uri}')"

        # 先に通常の@importを置換してからurl(...)を置換します。
        css = PLAIN_IMPORT_RE.sub(replace_plain_import, css)
        return URL_RE.sub(replace_url, css)
    finally:
        # 成功・失敗に関わらず再帰処理中の記録を解除します。
        resolving.remove(css_path)


# HTML文書内のスタイル・スクリプト・その他の資産を埋め込みます。
def inline_document(html: str, build_root: Path) -> tuple[str, set[str]]:
    # 埋め込めなかった外部資産を集めます。
    external_assets: set[str] = set()
    # CSSの循環import検出に使う共有状態です。
    resolving: set[Path] = set()

    # linkタグを調べ、スタイルシートをstyleタグへ置き換えます。
    def replace_stylesheet(match: re.Match[str]) -> str:
        # 元のlinkタグから属性を取得します。
        tag = match.group(0)
        rel = get_attribute(tag, "rel") or ""
        href = get_attribute(tag, "href")
        # 検索・索引リンクは単一HTMLには不要なので取り除きます。
        if set(rel.lower().split()).intersection({"search", "index"}):
            return ""
        # stylesheetでないリンクやhrefのないリンクはそのまま残します。
        if "stylesheet" not in rel.lower().split() or not href:
            return tag
        # スタイルシートのファイルをビルド出力内から探します。
        asset = resolve_asset(href, build_root, build_root)
        # 外部スタイルシートは残し、外部資産として記録します。
        if asset is None:
            external_assets.add(href)
            return tag

        # CSSとその参照資産を埋め込みます。
        css = inline_css(
            asset.read_text(encoding="utf-8", errors="replace"),
            asset,
            build_root,
            resolving,
            external_assets,
        )
        # media属性があれば保持してstyleタグを作ります。
        media = get_attribute(tag, "media")
        media_attribute = f' media="{media}"' if media else ""
        return f"<style{media_attribute}>\n{css}\n</style>"

    # すべてのlinkタグに上記の置換を適用します。
    html = LINK_RE.sub(replace_stylesheet, html)

    # 外部scriptファイルを読み込んでscriptタグ本文へ埋め込みます。
    def replace_script(match: re.Match[str]) -> str:
        # script開始タグとsrc属性を取り出します。
        opening = match.group("opening")
        src = get_attribute(opening, "src")
        # srcがなければ既に本文内スクリプトなのでそのままにします。
        if not src:
            return match.group(0)
        # スクリプトファイルの場所をビルド出力内から解決します。
        asset = resolve_asset(src, build_root, build_root)
        # 外部スクリプトは元のタグを残し、外部資産として記録します。
        if asset is None:
            external_assets.add(src)
            return match.group(0)

        # スクリプトのテキストを読み込みます。
        source = asset.read_text(encoding="utf-8", errors="replace")
        # src属性を削除し、インラインスクリプトとして扱えるようにします。
        opening = re.sub(
            r"""\s+src\s*=\s*(["']).*?\1""",
            "",
            opening,
            count=1,
            flags=re.IGNORECASE | re.DOTALL,
        )
        # 読み込んだコードをscriptタグ本文に入れて返します。
        return f"<script{opening}>{source}</script>"

    # すべてのscriptタグに上記の置換を適用します。
    html = SCRIPT_RE.sub(replace_script, html)

    # HTML内に直接書かれたstyleタグのCSSを処理します。
    def replace_style(match: re.Match[str]) -> str:
        # インラインCSSの相対URLを解決するための仮想パスを用意します。
        css_path = build_root / "__inline_style__.css"
        # CSS内で参照されるローカル資産も埋め込みます。
        css = inline_css(match.group("body"), css_path, build_root, resolving, external_assets)
        # 元の属性を保ったstyleタグを返します。
        return f"<style{match.group('opening')}>{css}</style>"

    # すべてのstyleタグに上記の置換を適用します。
    html = STYLE_RE.sub(replace_style, html)

    # 画像・動画などのリソースタグ内のURL属性を処理します。
    def replace_resource_tag(match: re.Match[str]) -> str:
        # ひとつのsrc、poster、hrefなどの属性を処理します。
        def replace_attribute(attribute: re.Match[str]) -> str:
            # 属性のURLからローカル資産を解決します。
            reference = attribute.group("url")
            asset = resolve_asset(reference, build_root, build_root)
            # ローカルでない参照はそのまま残します。
            if asset is None:
                # 外部URLは利用者への通知用に記録します。
                scheme = urlsplit(reference).scheme
                if reference.startswith("//") or (scheme and scheme.lower() != "data"):
                    external_assets.add(reference)
                return attribute.group(0)
            # ファイル内容をMIMEタイプ付きdata URIにします。
            media_type = get_media_type(asset)
            data_uri = to_data_uri(asset.read_bytes(), media_type)
            # 参照先にフラグメントがあればdata URIにも残します。
            parsed = urlsplit(reference)
            if parsed.fragment:
                data_uri += f"#{parsed.fragment}"
            # 属性名と引用符を維持して、URL値だけ置き換えます。
            return f"{attribute.group('prefix')}{attribute.group('quote')}{data_uri}{attribute.group('quote')}"

        # 対象タグ内のリソース属性をすべて置き換えます。
        return RESOURCE_ATTRIBUTE_RE.sub(replace_attribute, match.group(0))

    # HTML内の対象リソースタグすべてに上記処理を適用します。
    html = RESOURCE_TAG_RE.sub(replace_resource_tag, html)
    # 変換後のHTMLと外部参照の一覧を呼び出し元へ返します。
    return html, external_assets


# コマンドラインから単一HTMLのビルド全体を実行します。
def main() -> int:
    # コマンドライン引数の説明と解析器を用意します。
    parser = argparse.ArgumentParser(
        description="Build a self-contained HTML file from a Sphinx documentation folder."
    )
    # 入力元フォルダは省略時にカレントディレクトリです。
    parser.add_argument("source", nargs="?", default=".", help="Sphinx source directory")
    # 出力先フォルダは任意で指定できます。
    parser.add_argument(
        "--output-dir",
        help="Output directory (defaults to <source>/_build)",
    )
    # 指定された引数を解析します。
    args = parser.parse_args()

    # 入力元と出力先を絶対パスに変換します。
    source_dir = Path(args.source).resolve()
    output_dir = Path(args.output_dir).resolve() if args.output_dir else source_dir / "_build"
    # Sphinx singlehtmlの一時出力先と最終ファイル名を決めます。
    singlehtml_dir = output_dir / "singlehtml"
    output_file = output_dir / "manual.html"

    # 現在のPython環境でSphinxのsinglehtmlビルダーを実行します。
    subprocess.run(
        [
            sys.executable,
            "-m",
            "sphinx",
            "-b",
            "singlehtml",
            "-c",
            str(source_dir),
            str(source_dir),
            str(singlehtml_dir),
        ],
        # Sphinxの失敗を成功扱いせず、例外として報告します。
        check=True,
    )

    # Sphinxが作成したHTMLを読み込み、ローカル資産を埋め込みます。
    singlehtml_file = singlehtml_dir / "index.html"
    html, external_assets = inline_document(
        singlehtml_file.read_text(encoding="utf-8"),
        singlehtml_dir,
    )
    # 出力フォルダが存在しない場合は作成します。
    output_dir.mkdir(parents=True, exist_ok=True)
    # 完成した単一HTMLを書き込みます。
    output_file.write_text(html, encoding="utf-8")
    # 作成したファイルのパスを表示します。
    print(f"Created {output_file}")
    # ネット接続が必要な外部資産があれば一覧を表示します。
    if external_assets:
        print("External assets still require network access:")
        # 外部資産の表示順を一定にして出力します。
        for asset in sorted(external_assets):
            print(f"  {asset}")
    # 正常終了を示す終了コードを返します。
    return 0


# 他のモジュールから読み込まれたときは実行せず、直接起動時だけmainを呼びます。
if __name__ == "__main__":
    # mainの戻り値をプロセスの終了コードとして返します。
    raise SystemExit(main())
