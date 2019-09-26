
/**
 * A target for some effect in battlefield
 */
public interface Target  
{
    TeamType TeamType { get; }
    Member[] Members { get; }
}
