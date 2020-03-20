using UnityEngine;

namespace Object_Management
{
    public enum GameTileContentType
    {
        Empty,
        Destination,
        Wall,
        SpawnPoint,
        Tower
    }

    public class GameTileContent : MonoBehaviour
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
    
        public void Recycle ()
        {
            originFactory.Reclaim(this);
        }


    }
}