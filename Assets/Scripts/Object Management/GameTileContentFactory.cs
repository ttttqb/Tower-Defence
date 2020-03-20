using UnityEngine;

//using UnityEngine.SceneManagement;

namespace Object_Management
{
    [CreateAssetMenu]
    public class GameTileContentFactory : GameObjectFactory
    {
        //Scene contentScene;

        [SerializeField]
        GameTileContent destinationPrefab = default;
        [SerializeField]
        GameTileContent emptyPrefab = default;
        [SerializeField]
        GameTileContent wallPrefab = default;
        [SerializeField]
        GameTileContent spawnPointPrefab = default;
        [SerializeField] 
        Tower towerPrefab = default;

        public void Reclaim(GameTileContent content)
        {
            Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
            Destroy(content.gameObject);
        }

        GameTileContent Get(GameTileContent prefab)
        {
            GameTileContent instance = CreateGameObjectInstance(prefab);
            instance.OriginFactory = this;
            //MoveToFactoryScene(instance.gameObject);
            return instance;
        }

        public GameTileContent Get(GameTileContentType type)
        {
            switch (type)
            {
                case GameTileContentType.Destination: return Get(destinationPrefab);
                case GameTileContentType.Empty: return Get(emptyPrefab);
                case GameTileContentType.Wall: return Get(wallPrefab);
                case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);
                case GameTileContentType.Tower: return Get(towerPrefab);
            }
            Debug.Assert(false, "Unsupported type: " + type);
            return null;
        }
    }
}