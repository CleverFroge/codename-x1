# Terraria 资源导出记录

- **日期**: 2026-07-03
- **源路径**: `D:\Program Files\Steam\steamapps\common\Terraria\Content`
- **输出路径**: `tools/TerrariaAssetExtract/extract_terraria_assets/terraria_assets/`（本地，未入库）
- **工具**: `tools/TerrariaAssetExtract`（C# / net10.0-windows / ReLogic.XnbReader）

## 统计

| 类型 | 数量 |
|------|------|
| PNG | 14,998 |
| WAV | 62 |
| 跳过 | 8（shader 等） |
| 错误 | 790（主要在 Sounds/） |

贴图（`Images/`）基本完整；音效需后续修复 `XnbSoundExporter` 解析逻辑。
