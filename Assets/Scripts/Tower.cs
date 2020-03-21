using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    float targetingRange = 1.5f;

    private TargetPoint target;
    private const int EnemyLayerMask = 1 << 9;

    [SerializeField] private Transform turret = default,
        laserBeam = default;
    
    Vector3 laserBeamScale;

    [SerializeField, Range(1f, 100f)] private float damagePerSecond = 10f;

    void Awake () {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget() || AcquireTarget())
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        var position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
        if (target != null) {
            Gizmos.DrawLine(position, target.Position);
        }
    }

    private static Collider[] targetsBuffer = new Collider[100];

    private bool AcquireTarget()
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
    
    private bool TrackTarget () {
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

    private void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition = turret.localPosition + 0.5f * d * laserBeam.forward;
        
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}