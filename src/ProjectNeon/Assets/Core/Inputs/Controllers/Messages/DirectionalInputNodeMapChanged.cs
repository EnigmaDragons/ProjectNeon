public class DirectionalInputNodeMapChanged
{
    public DirectionalInputNodeMap OutdatedMap { get; }
    public DirectionalInputNodeMap UpdatedMap { get; }

    public  DirectionalInputNodeMapChanged(DirectionalInputNodeMap outdatedMap, DirectionalInputNodeMap updatedMap)
    {
        OutdatedMap = outdatedMap;
        UpdatedMap = updatedMap;
    }
}