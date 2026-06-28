# 代号X1

项目 monorepo 根目录。

**技术栈（2026-06-27）**：**Godot 4.4 + 纯 C#**。引擎为 Godot，逻辑代码全部用 C# 编写（Godot .NET），不使用 GDScript。

## 目录结构

```text
代号X1/
├── godot/          # Godot 4 + C# 游戏客户端
├── docs/           # 设计文档（预留）
├── tools/          # 工具链（含 Terraria 逆向）
└── README.md
```

## 子项目

| 目录 | 说明 |
|------|------|
| [godot/](godot/) | Godot 4.4 客户端工程（C#） |
| [tools/terraria-reverse/](tools/terraria-reverse/) | Terraria 反编译 + XNB 解包 |

## 环境要求

- Godot Engine 4.4+（**.NET 版**）
- .NET SDK 8.0+

## GitHub

https://github.com/CleverFroge/codename-x1
