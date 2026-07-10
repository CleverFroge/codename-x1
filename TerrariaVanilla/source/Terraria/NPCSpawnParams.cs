namespace Terraria;

public struct NPCSpawnParams
{
	public float? sizeScaleOverride;

	public int? playerCountForMultiplayerDifficultyOverride;

	public float? difficultyOverride;

	public NPCSpawnParams WithScale(float scaleOverride)
	{
		NPCSpawnParams result = default(NPCSpawnParams);
		result.sizeScaleOverride = scaleOverride;
		result.playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride;
		result.difficultyOverride = difficultyOverride;
		return result;
	}
}
