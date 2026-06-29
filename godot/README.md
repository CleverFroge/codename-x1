# 代号X1 — Godot 工程

Godot 4.7 游戏客户端，位于 monorepo 的 `godot/` 子目录。

**技术栈**：Godot 4.7 Mono + **纯 C#**（Godot .NET 绑定；仅 `boot_check.gd` 用于检测 C# 是否加载）。

## 快速开始

```powershell
# 首次环境配置（检查 dotnet、安装 Godot Mono、构建、WorldGenSmoke 冒烟）
.\scripts\env\Setup-Env.ps1 -InstallGodot

# 构建并启动
.\Launch-X1.bat
```

也可手动：用 **Godot 4.7 .NET (Mono)** 打开 `project.godot` → Build Project → F5。

## 环境要求

- **[Godot 4.7 .NET 版（Mono）](https://godotengine.org/download)** — 标题须含 **.NET**，不能是标准版
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)（Windows，`net8.0-windows`）

### 常见错误

若输出出现：

```text
ERROR: No loader found for resource: res://scripts/Main.cs (expected type: Script)
```

说明当前打开的是 **标准版 Godot**（`v4.7.stable.official`）。请改用 **Mono/.NET 版**（`v4.7.stable.mono.official`），或运行 `Launch-X1.bat`。

安装（Windows）：

```powershell
winget install GodotEngine.GodotEngine.Mono
```

自定义 Godot 路径：设置环境变量 `GODOT_MONO` 指向 `Godot_v*-stable_mono_win64.exe`。

## 目录结构

```text
godot/
├── project.godot
├── CodenameX1.csproj
├── Launch-X1.bat / Launch-X1.ps1
├── TerrariaVanilla/          # 反编译 Terraria + WorldGenHost
├── scripts/
│   ├── env/                  # Find-GodotMono.ps1, Setup-Env.ps1
│   └── world/                # WorldGenerator, TerrariaWorldExporter
├── tools/WorldGenSmoke/        # 控制台冒烟测试（不依赖 Godot 编辑器）
└── assets/
```

## Terraria 世界生成

- `UseTerrariaVanillaPort = true` 时调用原版 `WorldGen.GenerateWorld`（97 Pass）
- 控制台验证：`dotnet run --project tools/WorldGenSmoke/WorldGenSmoke.csproj`
- 参考指标：4200×1200、seed=42、约 15–25 秒

## 备注

- 参考源码：`../tools/terraria-reverse/source/`
- 踩坑记录见 llmwiki：`codename-x1-route-a-worldgen-pitfalls`
