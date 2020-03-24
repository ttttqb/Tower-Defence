using System;
using UnityEngine;
using UnityEngine.Playables;

public class Enemy : GameBehavior
{
	private EnemyFactory originFactory;

	public EnemyFactory OriginFactory
	{
		get => originFactory;
		set
		{
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}

	private GameTile tileFrom, tileTo;
	private Vector3 positionFrom, positionTo;
	private float progress, progressFactor;

	private Direction direction;
	private DirectionChange directionChange;
	private float directionAngleFrom, directionAngleTo;
		
	[SerializeField] private Transform model = default;

	private float pathOffset;
	private float speed;
	
	public float Scale { get; private set; }

	private float Health { get; set; }

	[SerializeField] private EnemyAnimationConfig animationConfig = default;
	private EnemyAnimator animator;

	private void Awake()
	{
		animator.Configure(
			model.GetChild(0).gameObject.AddComponent<Animator>(),
			animationConfig
			);
	}

	private void OnDestroy()
	{
		animator.Destroy();
	}

	public void SpawnOn(GameTile tile)
	{
		//transform.localPosition = tile.transform.localPosition;
		Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
		tileFrom = tile;
		tileTo = tile.NextTileOnPath;
		// positionFrom = tileFrom.transform.localPosition;
		// positionTo = tileFrom.ExitPoint;
		// transform.localRotation = tileFrom.PathDirection.GetRotation();
		progress = 0f;
		PrepareIntro();
	}

	void PrepareIntro()
	{
		positionFrom = tileFrom.transform.localPosition;
		transform.localPosition = positionFrom;
		positionTo = tileFrom.ExitPoint;
		direction = tileFrom.PathDirection;
		directionChange = DirectionChange.None;
		directionAngleFrom = directionAngleTo = direction.GetAngle();
		transform.localRotation = direction.GetRotation();
		model.localPosition = new Vector3(pathOffset, 0f);
		progressFactor = speed;
	}

	void PrepareOutro () {
		positionTo = tileFrom.transform.localPosition;
		directionChange = DirectionChange.None;
		directionAngleTo = direction.GetAngle();
		transform.localRotation = direction.GetRotation();
		model.localPosition = new Vector3(pathOffset,0f);
		progressFactor = speed;
	}

	public override bool  GameUpdate()
	{
		if (animator.CurrentClip == EnemyAnimator.Clip.Intro) {
			if (!animator.IsDone) {
				return true;
			}
			animator.PlayMove(speed / Scale);
		}
		else if (animator.CurrentClip == EnemyAnimator.Clip.Outro) {
			if (animator.IsDone) {
				Recycle();
				return false;
			}
			return true;
		}
		
		if (Health <= 0f)
		{
			Recycle();
			return false;
		}
		progress += Time.deltaTime * progressFactor;
		while (progress >= 1)
		{
			// tileFrom = tileTo;
			// tileTo = tileTo.NextTileOnPath;
			if (tileTo == null)
			{
				Game.EnemyReachedDestination();
				animator.PlayOutro();
				return true;
			}

			// positionFrom = positionTo;
			// positionTo = tileFrom.ExitPoint;
			// transform.localRotation = tileFrom.PathDirection.GetRotation();
			//progress -= 1f;
			progress = (progress - 1f) / progressFactor;
			PrepareNextState();
			progress *= progressFactor;
		}

		if (directionChange == DirectionChange.None) {
			transform.localPosition =
				Vector3.LerpUnclamped(positionFrom, positionTo, progress);
		}
		//if (directionChange != DirectionChange.None) {
		else {
			float angle = Mathf.LerpUnclamped(
				directionAngleFrom, directionAngleTo, progress
			);
			transform.localRotation = Quaternion.Euler(0f, angle, 0f);
		}

		return true;
	}

	void PrepareNextState()
	{
		tileFrom = tileTo;
		tileTo = tileTo.NextTileOnPath;
		positionFrom = positionTo;
		if (tileTo == null)
		{
			PrepareOutro();
			return;
		}
		positionTo = tileFrom.ExitPoint;
		directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
		direction = tileFrom.PathDirection;
		directionAngleFrom = directionAngleTo;
		switch (directionChange)
		{
			case DirectionChange.None:
				PrepareForward();
				break;
			case DirectionChange.TurnRight:
				PrepareTurnRight();
				break;
			case DirectionChange.TurnLeft:
				PrepareTurnLeft();
				break;
			default:
				PrepareTurnAround();
				break;
		}
	}

	void PrepareForward()
	{
		transform.localRotation = direction.GetRotation();
		directionAngleTo = direction.GetAngle();
		model.localPosition = new Vector3(pathOffset, 0f);
		progressFactor = speed;
	}

	void PrepareTurnRight()
	{
		directionAngleTo = directionAngleFrom + 90f;
		model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
	}

	void PrepareTurnLeft()
	{
		directionAngleTo = directionAngleFrom - 90f;
		model.localPosition = new Vector3(pathOffset + .5f, 0f);
		transform.localPosition = positionFrom + direction.GetHalfVector();
		progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
	}

	void PrepareTurnAround()
	{
		directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
		model.localPosition = new Vector3(pathOffset, 0f);
		transform.localPosition = positionFrom;
		progressFactor = speed / (Mathf.PI *Mathf.Max(Mathf.Abs(pathOffset),0.2f));
	}

	public void Initialize(float scale, float speed, float pathOffset, float health)
	{
		Scale = scale;
		model.localScale = new Vector3(scale, scale,scale);
		this.speed = speed;
		this.pathOffset = pathOffset;
		Health = health;

		animator.PlayIntro();
	}

	public void ApplyDamage(float damage)
	{
		Debug.Assert(damage >= 0f, "Negative damage applied.");
		Health -= damage;
	}

	public override void Recycle()
	{
		animator.Stop();
		originFactory.Reclaim(this);
	}
}