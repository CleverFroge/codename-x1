# 代号X1

项目 monorepo 根目录。

**技术栈（2026-06-29）**：**Godot 4.7 Mono + 纯 C#**。引擎为 Godot .NET 版，逻辑代码全部用 C# 编写；`boot_check.gd` 仅用于检测 C# 是否成功加载。

## 快速开始

```powershell
cd godot
.\scripts\env\Setup-Env.ps1 -InstallGodot   # 首次：检查/安装 Godot Mono + 构建 + 冒烟测试
.\Launch-X1.bat                              # 构建并启动游戏
```

## 目录结构

```text
代号X1/
├── godot/          # Godot 4.7 + C# 游戏客户端
├── docs/           # 设计文档（预留）
├── tools/          # 工具链（含 Terraria 逆向）
└── README.md
```

## 子项目

| 目录 | 说明 |
|------|------|
| [godot/](godot/) | Godot 4.7 Mono 客户端（Terraria 原版 WorldGen 已集成） |
| [tools/terraria-reverse/](tools/terraria-reverse/) | Terraria 反编译 + XNB 解包 |

## 环境要求

| 工具 | 版本 | 说明 |
|------|------|------|
| Godot Engine | **4.7 .NET 版（Mono）** | 标准版不支持 C# |
| .NET SDK | 8.0+ | `net8.0-windows` |
| Git | 2.x | 版本控制 |

安装 Godot Mono：

```powershell
winget install GodotEngine.GodotEngine.Mono
```

## GitHub

https://github.com/CleverFroge/codename-x1
