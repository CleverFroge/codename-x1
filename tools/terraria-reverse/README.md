# Terraria 逆向工程工具集

> 从 Steam 版 Terraria（1.4.4.x，内部版本 319）反编译 C# 源码 + 解包 XNB 资源为 PNG 的工具集合。
> Terraria 是 .NET/XNA 原生程序集（非 IL2CPP），符号信息完整，反编译质量极高。

## 目录结构

```
terraria-reverse/
├── scripts/
│   ├── decompile.ps1      # ILSpy 反编译脚本（Terraria.exe → C# 源码）
│   └── unpack_assets.ps1  # XNB 解包脚本（.xnb → .png）
├── source/                 # 反编译产出的核心源码（精选 12 个关键 .cs）
│   ├── Terraria/
│   │   ├── Program.cs          # 程序入口
│   │   ├── Main.cs (2.1MB)     # 游戏主循环 + 全局状态
│   │   ├── WorldGen.cs (2.2MB) # 世界生成算法
│   │   ├── NPC.cs (2.4MB)      # 怪物/Boss AI
│   │   ├── Player.cs (1.4MB)   # 玩家逻辑
│   │   ├── Projectile.cs (2MB) # 弹幕逻辑
│   │   ├── Item.cs (949KB)     # 物品系统
│   │   └── Recipe.cs (645KB)   # 合成配方
│   ├── Terraria.GameContent.Biomes/
│   │   ├── TerrainPass.cs      # 地表生成算法
│   │   └── JunglePass.cs       # 丛林生成
│   └── Terraria.GameContent.Generation.Dungeon/
│       ├── DungeonCrawler.cs      # 地下城生成入口
│       └── DungeonLayoutProvider.cs # 房间布局算法
├── xnbcli/                 # XNB 解包工具（基于 LeonBlade/xnbcli，patch 了 lz4 依赖）
│   ├── xnbcli.js           # CLI 入口
│   ├── package.json
│   └── app/                # 核心代码（含 LZX 解压器 Presser/Lzx.js）
└── .gitignore
```

## 环境要求

| 工具 | 版本 | 用途 |
|------|------|------|
| .NET SDK | 9.0+ | 运行 ilspycmd |
| ilspycmd | 8.2.0.7535 | 反编译 .NET 程序集 |
| Node.js | 18+ | 运行 xnbcli |
| Steam 版 Terraria | 1.4.4.x | 提供 Terraria.exe 和 Content/ |

## 使用方法

### 1. 反编译 Terraria.exe

```powershell
# 安装 ilspycmd（一次性）
dotnet tool install -g ilspycmd --version 8.2.0.7535

# 反编译（会输出 1499 个 .cs 文件，约 20MB）
cd scripts
./decompile.ps1 "D:\Program Files\Steam\steamapps\common\Terraria\Terraria.exe" "../source_full"
```

> **注意**：`source/` 目录只保留了 12 个核心源码文件。完整反编译产出（1499 个 .cs）不提交 git，用上面的脚本本地生成。

### 2. 解包 XNB 资源为 PNG

```powershell
# 安装 xnbcli 依赖（一次性）
cd xnbcli
npm install --ignore-scripts
npm install lz4js  # 纯 JS LZ4 替代原生模块

# 解包图片
cd ../scripts
./unpack_assets.ps1 "D:\Program Files\Steam\steamapps\common\Terraria\Content\Images" "../output_png"
```

> xnbcli 支持 LZX 和 LZ4 两种压缩格式。Terraria 1.4 的 XNB 使用 **LZX 压缩**（flag=0x80），xnbcli 自带纯 JS LZX 解压器（`app/Presser/Lzx.js`），无需原生编译。

## XNB 格式说明

Terraria 的 XNB 文件头格式（FNA 版本）：

```
偏移  长度  内容
0     3     "XNB" magic
3     1     target ('w'=Windows)
4     1     formatVersion (0x05=XNA 4.0)
5     1     flags (bit0=HiDef, bit6=LZ4, bit7=LZX)
6     4     fileSize (uint32 LE)
10    4     decompressedSize (uint32 LE, 仅压缩时)
14+   ...   compressed/raw body
```

Terraria 全部使用 LZX 压缩（flag=0x80）。解压后是标准的 .NET managed resource body（7-bit encoded type reader count + readers + shared resources + primary object）。

## 关键发现

- **Terraria 不用柏林噪声生成主地形**——靠「UnifiedRandom 伪随机 + TileRunner 衰减半径醉汉游走 + TerrainPass 5 态状态机」
- 世界本质 = 一张 `Main.tile[,]` 二维表（大世界约 2000 万格）
- 联机 = C/S + 裸二进制协议（`MessageBuffer.GetData` 巨型 switch）
- 详细算法分析见 [地形生成算法文档](../../../docs/terraria-worldgen/)

## 相关文档

- [Terraria 逆向分析报告](https://llmwiki 中的 terraria/terraria-逆向分析报告.md)
- [Terraria 世界地形生成算法详解](https://llmwiki 中的 terraria/terraria-世界地形生成算法详解.md)

## 声明

本工具仅供学习研究用途。Terraria 受版权保护，反编译源码请勿分发或用于商业用途。二次开发请走官方 tModLoader 渠道。
