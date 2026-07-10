# Terraria 资源提取

## 问题说明

Terraria 的 `.xnb` **几乎全部使用 LZX 压缩**（XNA 4.0 hi-res）。  
当前环境的 **Python 2.7** 无法安装 `lzx` 包，因此 **Python 脚本无法单独完成提取**。

## 推荐方案：C# 提取器（MonoGame + ReLogic）

使用项目内已有的 `ReLogic.XnbReader`，由 MonoGame 内部处理 LZX 解压。

### 试跑 5 个文件

```powershell
cd C:\Users\zhipengjiao\Projects\代号X1\godot\tools\TerrariaAssetExtract

dotnet run -c Release -- `
  --input "D:\Program Files\Steam\steamapps\common\Terraria\Content" `
  --output ..\extract_terraria_assets\test_output `
  --limit 5 `
  --verbose
```

### 全量导出

```powershell
dotnet run -c Release -- `
  --input "D:\Program Files\Steam\steamapps\common\Terraria\Content" `
  --output ..\extract_terraria_assets\terraria_assets `
  --verbose
```

或使用批处理：

```bat
tools\TerrariaAssetExtract\extract.bat -i "D:\Program Files\Steam\steamapps\common\Terraria\Content" -o terraria_assets --limit 5 -v
```

### 输出

- `Texture2D` → `.png`
- `SoundEffect` → `.wav`
- 保持 `Content/` 子目录结构

Terraria 正式版资源请使用 C# 工具 `tools/TerrariaAssetExtract`。
