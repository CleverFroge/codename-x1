# 代号X1 本地开发环境

## 前置条件

| 工具 | 要求 |
|------|------|
| .NET SDK | 8.0+（本机已验证 9.x 可用） |
| Godot | **4.7 .NET 版（Mono）**，非标准版 |
| OS | Windows（TerrariaVanilla 目标 `net8.0-windows`） |

## 一键配置

在 `godot/` 目录下：

```powershell
.\scripts\env\Setup-Env.ps1 -InstallGodot
```

该脚本会：

1. 检查 `dotnet`
2. 若未安装则通过 winget 安装 Godot Mono
3. `dotnet build CodenameX1.csproj`
4. 运行 `WorldGenSmoke` 冒烟测试

## 启动游戏

```powershell
.\Launch-X1.bat
```

或在 Godot Mono 编辑器中打开 `project.godot`，Build → F5。

## 策划 vs 工程

| 文档 | 引擎描述 |
|------|----------|
| [llmwiki 游戏设计](../) | **Godot 4.7 + C#**（2026-06-29 已与工程对齐） |
| 本仓库 `godot/` | Godot 4.7 Mono + C# |

GDD 中的玩法/题材内容不变；技术实现统一为 Godot + C#。

## 常见问题

### C# 脚本不加载

- 确认 Godot 启动画面含 `mono.official`
- 运行 `Launch-X1.bat` 先构建再启动
- 查看 Autoload `boot_check.gd` 是否在 UI 提示「C# 未加载」

### WorldGen 生成失败 (0%)

- 确认 `System.Security.Permissions.dll` 已复制到 `.godot/mono/temp/bin/Debug/`
- `Launch-X1.ps1` 构建后会自动复制；也可手动 `dotnet build`

### 仅验证世界生成（无需 Godot 编辑器）

```powershell
dotnet run --project tools/WorldGenSmoke/WorldGenSmoke.csproj
```
