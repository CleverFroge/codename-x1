# 代号X1 — Godot 工程

Godot 4 游戏客户端，位于 monorepo 的 `godot/` 子目录。

**技术栈**：Godot 4.7 + **纯 C#**（Godot .NET 绑定；仅 `boot_check.gd` 用于检测 C# 是否加载）。

## 环境要求

- **[Godot 4.7 .NET 版（Mono）](https://godotengine.org/download)** — 标题须含 **.NET**，不能是标准版
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)（Windows，`net8.0-windows`）

### 常见错误

若输出出现：

```text
ERROR: No loader found for resource: res://scripts/Main.cs (expected type: Script)
```

说明当前打开的是 **标准版 Godot**（启动画面为 `v4.7.stable.official`）。请改用 **Mono/.NET 版**（`v4.7.stable.mono.official`）。

安装（Windows）：

```powershell
winget install GodotEngine.GodotEngine.Mono
```

## 打开项目

1. 启动 **Godot 4.7 .NET (Mono)** 编辑器（不是标准版）
2. Import / 打开本目录下的 `project.godot`
3. 菜单 **Project → Tools → C# → Create C# solution**（若尚未生成）
4. **Build → Build Project**（或编辑器右上角 Build）
5. **F5** 运行；4200×1200 世界生成约需 15 秒

命令行（需 Mono 版可执行文件在 PATH 中，例如 `Godot_mono`）：

```powershell
godot --path "C:\Users\15778\Projects\codename-x1\godot"
```

## 目录结构

```text
godot/
├── project.godot
├── CodenameX1.csproj
├── TerrariaVanilla/     # 反编译 Terraria + WorldGenHost
├── icon.svg
├── scenes/main/
├── scripts/             # C# 游戏逻辑
├── tools/WorldGenSmoke/ # 控制台冒烟测试（不依赖 Godot）
└── assets/
```

## Terraria 世界生成

- `UseTerrariaVanillaPort = true` 时调用原版 `WorldGen.GenerateWorld`（97 Pass）
- 控制台验证：`dotnet run --project tools/WorldGenSmoke/WorldGenSmoke.csproj`

## 备注

- 参考源码：`../tools/terraria-reverse/source/`