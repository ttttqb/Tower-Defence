using System;
using UnityEngine;

namespace Object_Management
{
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
		private float progress, progressFactor;

		Direction direction;
		DirectionChange directionChange;
		float directionAngleFrom, directionAngleTo;
		
		[SerializeField]
		Transform model = default;

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
			positionTo = tileFrom.ExitPoint;
			direction = tileFrom.PathDirection;
			directionChange = DirectionChange.None;
			directionAngleFrom = directionAngleTo = direction.GetAngle();
			transform.localRotation = direction.GetRotation();
			progressFactor = 2f;
		}

		void PrepareOutro () {
			positionTo = tileFrom.transform.localPosition;
			directionChange = DirectionChange.None;
			directionAngleTo = direction.GetAngle();
			model.localPosition = Vector3.zero;
			transform.localRotation = direction.GetRotation();
			progressFactor = 2f;
		}

		public bool GameUpdate()
		{
			progress += Time.deltaTime * progressFactor;
			while (progress >= 1)
			{
				// tileFrom = tileTo;
				// tileTo = tileTo.NextTileOnPath;
				if (tileTo == null)
				{
					OriginFactory.Reclaim(this);
					return false;
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
			model.localPosition = Vector3.zero;
			progressFactor = 1f;
		}

		void PrepareTurnRight()
		{
			directionAngleTo = directionAngleFrom + 90f;
			model.localPosition = new Vector3(-0.5f, 0f);
			transform.localPosition = positionFrom + direction.GetHalfVector();
			progressFactor = 1f / (Mathf.PI * 0.25f);
		}

		void PrepareTurnLeft()
		{
			directionAngleTo = directionAngleFrom - 90f;
			model.localPosition = new Vector3(0.5f, 0f);
			transform.localPosition = positionFrom + direction.GetHalfVector();
			progressFactor = 1f / (Mathf.PI * 0.25f);
		}

		void PrepareTurnAround()
		{
			directionAngleTo = directionAngleFrom + 180f;
			model.localPosition = Vector3.zero;
			transform.localPosition = positionFrom;
			progressFactor = 2f;
		}
	}
}