# Agent 工作规范 — 代号X1

## 仓库结构

```
代号X1/
├── Launch-X1.bat          # 根启动器 → tools/Launcher/
├── tools/
│   ├── Launcher/          # Godot Mono 查找、编译、启动
│   └── TerrariaAssetExtract/
├── TerrariaVanilla/       # 泰拉世界生成 DLL（独立工程）
├── graphify-out/          # 代码知识图谱（gitignore，仓库根目录生成）
└── godot/                 # Godot 4.7 + C# 主工程
    ├── scenes/
    │   ├── main/          # 游戏主场景（一键生成浏览地图）
    │   └── pass_editor/   # Pass 逐步调试编辑器
    └── scripts/
        ├── Runtime/       # 游戏运行时（CodenameX1.Runtime / CodenameX1.World）
        └── Editor/        # 开发工具（CodenameX1.Editor）
```
