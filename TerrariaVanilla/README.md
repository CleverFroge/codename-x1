# TerrariaVanilla

泰拉瑞亚反编译源码的独立工程，仅供代号X1 做 WorldGen 参考与 PassEditor 调试。

`godot/` 工程**不收录此目录源码**，构建时只引用编译产物 `bin/Debug/net8.0-windows/TerrariaVanilla.dll`。

## 构建

```powershell
dotnet build TerrariaVanilla.csproj
```

或在仓库根目录：

```powershell
.\TerrariaVanilla\build.ps1
```

## 依赖

- `tools/terraria-reverse/libs/` 下的第三方 DLL（Newtonsoft.Json 等）
- ReLogic（同目录子工程）

## 与 godot 的关系

| 组件 | 位置 |
|------|------|
| Terraria 源码 | `TerrariaVanilla/source/` |
| 无头启动桥接 | `TerrariaVanilla/port/WorldGenHost.cs` |
| PassEditor 反射桥接 | `godot/scripts/world/WorldGenHostExt.cs` |
| 游戏侧引用 | `godot/CodenameX1.csproj` → DLL 引用 |

修改 Terraria 源码后需重新 build，Godot 下次编译会自动拉起此工程。
