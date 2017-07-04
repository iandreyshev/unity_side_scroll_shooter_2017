﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.Hero
{
	public sealed class Ship : Body
	{
		public ShipMind mind { get; set; }
		public ShipProperties properties
		{
			set
			{
				mind.properties = value;
				health = maxHealth = value.health;
			}
		}

		public void MoveTo(Vector3 newPosition)
		{
			Vector3 direction = (newPosition - position).normalized;
			m_smoothDir = Vector3.MoveTowards(m_smoothDir, direction, SMOOTHING);
			direction = m_smoothDir;
			Vector3 movement = new Vector3(direction.x, 0, direction.z);
			physicsBody.velocity = movement * SPEED;
			m_isMoved = true;
		}

		protected override void OnAwakeEnd()
		{
			mind = GetComponent<ShipMind>();
		}
		protected override void OnInitEnd()
		{
			healthBar = world.factory.GetBar(BarType.PLAYER_HEALTH);
			healthBar.SetValue(healthPart);
			roadController.Spline = world.factory.GetRoad(RoadType.PLAYER);
			touchDemage = int.MaxValue;
			isEraseOnDeath = false;
		}
		protected override void PlayingUpdate()
		{
			UpdatePositionOnField();
			UpdateRotation();
			UpdateMoveingSpeed();

			healthBar.isFadable = maxHealth == health;
		}
		protected override void DoAfterDemaged()
		{
			healthBar.SetValue(healthPart);
		}

		private Vector3 m_smoothDir;
		private bool m_isMoved = false;

		private const float SPEED = 80;
		private const float SMOOTHING = 15;
		private const float TILT = 2;
		private const float X_ANGLE = 180;
		private const float MAX_VELOCITY_ANGLE = 80;

		private void UpdatePositionOnField()
		{
			position = new Vector3(
				Mathf.Clamp(position.x, mapBox.xMin, mapBox.xMax),
				GameWorld.FLY_HEIGHT,
				Mathf.Clamp(position.z, mapBox.zMin, mapBox.zMax)
			);
		}
		private void UpdateRotation()
		{
			float zEuler = physicsBody.velocity.x * -TILT;
			physicsBody.rotation = Quaternion.Euler(
				0,
				X_ANGLE,
				Mathf.Clamp(-zEuler, -MAX_VELOCITY_ANGLE, MAX_VELOCITY_ANGLE));
		}
		private void UpdateMoveingSpeed()
		{
			Vector3 velocity = physicsBody.velocity;
			physicsBody.velocity = (m_isMoved) ? velocity : Vector3.zero;
			m_isMoved = false;
		}
	}
}
