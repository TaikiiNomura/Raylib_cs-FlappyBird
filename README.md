# Raylib-csでゲームを作る

## 1.	環境構築
- VSCodeインストール
- .NET SDKインストール
- VSCodeの拡張機能**C# Dev Kit**をインストール
- ※確認
    ```bash
    code --version
    dotnet --version
    ```

## 2.	ゲームプログラムを作成
- コマンドプロンプトを開く
- 作業用のプロジェクトを作成
    - 例：デスクトップ上に新しいプロジェクトを作成
        ```bash
        cd Desctop
        dotnet new console -o MyGameSpace
        ```

- Raylib-csライブラリを追加する
    - 作成したプロジェクトフォルダに移動
        ```bash
        cd MyGameSpace
        ```
    - Raylib-csを追加
        ```bash
        dotnet add package Raylib-cs
        ```
## 3. VSCodeでコードを書く
- VSCodeを開く
```bash
code .
```
- `Program.cs`にコードを貼り付け

趣味の延長で作成したものなので難しい質問はご勘弁ください
