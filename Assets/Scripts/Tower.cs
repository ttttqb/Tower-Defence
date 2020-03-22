using UnityEngine;

public abstract class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;

    private const int EnemyLayerMask = 1 << 9;
    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        var position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }

    private static Collider[] targetsBuffer = new Collider[100];

    protected bool AcquireTarget(out TargetPoint target)
    {
        var a = transform.localPosition;
        var b = a;
        b.y += 3;
        var hits = Physics.OverlapCapsuleNonAlloc(
            a, b, targetingRange, targetsBuffer, EnemyLayerMask
        );
        if (hits > 0)
        {
            target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
            Debug.Assert(target!=null,"Targeted non-enemy!",targetsBuffer[0]);
            return true;
        }

        target = null;
        return false;
    }
    
    protected bool TrackTarget (ref TargetPoint target) {
        if (target == null) {
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