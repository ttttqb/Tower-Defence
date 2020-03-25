using UnityEngine;

public enum TowerType
{
    Laser, Mortar
}

public abstract class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;

    public abstract TowerType TowerType { get; }
    
    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        var position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }

    protected bool AcquireTarget(out TargetPoint target)
    {
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
        {
            target = TargetPoint.RandomBuffered;
            return true;
        }
        target = null;
        return false;
    }
    
    protected bool TrackTarget (ref TargetPoint target) {
        if (target == null || !target.Enemy.IsValidTarget) {
            return false;
        }
        var a = transform.localPosition;
        var b = target.Position;
        var x = a.x - b.x;
        var z = a.z - b.z;
        var r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z < r * r) return true;
        target = null;
        return false;
    }
}