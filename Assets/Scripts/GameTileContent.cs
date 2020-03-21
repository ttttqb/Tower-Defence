using UnityEngine;

public enum GameTileContentType
{
    Empty,
    Destination,
    Wall,
    SpawnPoint,
    Tower
}

[SelectionBase] public class GameTileContent : MonoBehaviour
{
    [SerializeField]
    GameTileContentType type = default;
    public GameTileContentType Type => type;

    GameTileContentFactory originFactory;
    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redined origin factory!");
            originFactory = value;
        }
    }

    public bool BlocksPath =>
        Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;
    
    public void Recycle ()
    {
        originFactory.Reclaim(this);
    }

    public virtual void GameUpdate()
    {
    }

}