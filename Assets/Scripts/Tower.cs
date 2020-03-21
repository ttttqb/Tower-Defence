using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    float targetingRange = 1.5f;
    
    public override void GameUpdate()
    {
        Debug.Log("Searching for target...");
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
}