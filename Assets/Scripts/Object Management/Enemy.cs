using UnityEngine;

public class Enemy : MonoBehaviour
{

	EnemyFactory originFactory;

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
	private float progress;

	public void SpawnOn (GameTile tile)
	{
		//transform.localPosition = tile.transform.localPosition;
		Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
		tileFrom = tile;
		tileTo = tile.NextTileOnPath;
		positionFrom = tileFrom.transform.localPosition;
		positionTo = tileFrom.ExitPoint;
		progress = 0f;
	}

	public bool GameUpdate()
	{
		progress += Time.deltaTime;
		while (progress >= 1)
		{
			tileFrom = tileTo;
			tileTo = tileTo.NextTileOnPath;
			if (tileTo == null)
			{
				OriginFactory.Reclaim(this);
				return false;
			}
			positionFrom = positionTo;
			positionTo = tileFrom.ExitPoint;
			progress -= 1f;
		}

		transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
		return true;
	}
}