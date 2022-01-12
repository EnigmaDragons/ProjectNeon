using System.Collections.Generic;

public interface IEncounterBuilder
{
    List<EnemyInstance> Generate(int difficulty, int currentChapterNumber);
}