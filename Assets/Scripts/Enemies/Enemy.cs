﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyGame.GameUtils;

namespace MyGame.Enemies
{
	using BonusCount = Pair<BonusType, int>;

	public abstract class Enemy : Body
	{
		public byte starsCount { get; protected set; }
		public UnitType type { get; set; }

		protected bool isTimerWork { get; set; }
		protected float coldown { get; set; }

		protected sealed override void OnInitEnd()
		{
			isTimerWork = false;
			exitAllowed = true;
			openAllowed = true;
			distmantleAllowed = true;

			InitProperties();

			if (healthBar)
			{
				healthBar.SetValue(healthPercents);
				healthBar.isFadable = true;
				toDestroy.Add(healthBar.gameObject);
			}
			if (roadController) roadController.OnEndReached.AddListener(T =>
			{
				world.player.LossEnemy();
				Exit();
			});
		}
		protected sealed override void PlayingUpdate()
		{
			TryShoot();
		}
		protected sealed override void OnDeath()
		{
			world.player.KillEnemy(type);

			if (Utils.IsHappen(0.1f))
			{
				bonuses.Add(BonusCount.Create(BonusType.HEALTH, 1));
			}
			else if (world.player.isAllowedModify && Utils.IsHappen(0.4f))
			{
				bonuses.Add(BonusCount.Create(BonusType.AMMO_UP, 1));
			}
		}
		protected sealed override void OnPlaying()
		{
			if (roadController) roadController.Play();
		}
		protected sealed override void OnPause()
		{
			if (roadController) roadController.Pause();
		}
		protected sealed override void OnEndGameplay()
		{
			if (roadController) roadController.Pause();
		}
		protected sealed override void OnExitFromWorld()
		{
			world.player.LossEnemy();
		}
		protected abstract void InitProperties();
		protected abstract void Shoot();

		private float m_timer = 0;

		private void TryShoot()
		{
			if (isTimerWork && Utils.UpdateTimer(ref m_timer, coldown))
			{
				Shoot();
			}
		}
	}
}
