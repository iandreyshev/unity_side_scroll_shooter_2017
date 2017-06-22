﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
	public sealed class BaseGun : Gun
	{
		public SimpleBullet m_ammo;
		public Transform m_leftSpawn;
		public Transform m_rightSpawn;

		protected override void DoAfterInit()
		{
			isTimerWork = true;
			coldown = 2;
			bulletsSpeed = 16;
		}
		protected override void Shoot()
		{
			SpawnBullet(m_leftSpawn);
			SpawnBullet(m_rightSpawn);
		}

		private float bulletsSpeed { get; set; }

		private void SpawnBullet(Transform spawn)
		{
			SimpleBullet bullet = Instantiate(m_ammo);
			bullet.position = spawn.position;
			bullet.speed = bulletsSpeed;
			mapPhysics.AddPlayerBullet(bullet);
			bullet.Start();
		}
	}
}