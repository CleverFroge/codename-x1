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
    │   ├── main_menu/     # 主界面 + PassEditor（启动入口）
    │   └── main/          # 游戏主场景（一键生成浏览地图）
    └── scripts/
        ├── System/            # UI 系统（仅主界面等 UI；命名空间勿用 .System）
        ├── GamePlay/          # 玩法逻辑
        │   └── World/         # 世界数据与生成（CodenameX1.World）
        │       ├── WorldGenPipeline.cs   # 自研 Pass 管线
        │       └── TerrariaPassCatalog.cs # 仅加载泰拉 GenPass
        ├── Main.cs            # 游戏场景逻辑
        ├── WorldView.cs       # 世界渲染
        ├── PassEditor.cs      # Pass 逐步调试（进入前选原生/泰拉）
        ├── PassInfo.cs
        └── WorldGenHostExt.cs
```
