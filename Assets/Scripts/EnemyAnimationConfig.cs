using UnityEngine;

[CreateAssetMenu]
public class EnemyAnimationConfig : ScriptableObject
{
    [SerializeField] private AnimationClip move = default,
        intro = default,
        outro = default,
        dying = default,
        appear = default,
        disappear = default;

    public AnimationClip Move => move;
    public AnimationClip Intro => intro;
    public AnimationClip Outro => outro;
    public AnimationClip Dying => dying;
    public AnimationClip Appear => appear;
    public AnimationClip Disappear => disappear;

    [SerializeField] private float moveAnimationSpeed = 1f;

    public float MoveAnimationSpeed => moveAnimationSpeed;
}