using System.Collections.Generic;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private EnemyArea enemies;
    
    // @todo #1:10min Replace this with Member once implemented.
    [SerializeField] private Enemy tempTarget;

    public List<Member> GetAllies()
    {
        /**
         * @todo #55:30min We need references to Member Decks in BattleState so we can return all Members with their 
         *  correct state on GetAllies and GetEnemies methods. After we get these references, update GetAllies and 
         *  GetEnemies methods to return the correct Members
         */
        List<Member> allies = new List<Member>();
        allies.Add(new Member().Init(party.characterOne));
        allies.Add(new Member().Init(party.characterTwo));
        allies.Add(new Member().Init(party.characterThree));
        return allies;
    }

    public List<Member> GetEnemies()
    {
        List<Member> enemyList = new List<Member>();
        enemies.enemies.ForEach(
            enemy =>
            {
                Character character = new Character();
                character.Bust = enemy.image;
                character.Stats = enemy.stats;
                character.ClassName = enemy.enemyName;
                enemyList.Add(new Member().Init(character));
            }
        );
        return enemyList;
    }
}
