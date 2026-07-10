# Terraria 资源提取

Terraria 的 `.xnb` 几乎全部使用 LZX 压缩（XNA 4.0 hi-res）。本工具用 `ReLogic.XnbReader` + MonoGame 批量导出 PNG/WAV。

## 试跑 5 个文件

```powershell
cd tools/TerrariaAssetExtract

dotnet run -c Release -- `
  --input "D:\Program Files\Steam\steamapps\common\Terraria\Content" `
  --output extract_terraria_assets\test_output `
  --limit 5 `
  --verbose
```

## 全量导出

```powershell
dotnet run -c Release -- `
  --input "D:\Program Files\Steam\steamapps\common\Terraria\Content" `
  --output extract_terraria_assets\terraria_assets `
  --verbose
```

或使用批处理：

```bat
extract.bat -i "D:\Program Files\Steam\steamapps\common\Terraria\Content" -o extract_terraria_assets\terraria_assets --limit 5 -v
```

## 输出

- `Texture2D` → `.png`
- `SoundEffect` → `.wav`
- 保持 `Content/` 子目录结构
- 默认输出目录：`extract_terraria_assets/`（本地生成，不入库）

导出记录见 [EXPORT_SUMMARY.md](extract_terraria_assets/EXPORT_SUMMARY.md)。
