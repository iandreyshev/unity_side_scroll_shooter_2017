﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MyGame.Hero;

namespace MyGame
{
	public class Player
	{
		public Player(IPlayerBar bar, Ship ship)
		{
			m_points = 0;
			m_ship = ship;
			m_bar = bar;
		}

		public EventDelegate onDemaged;
		public EventDelegate onLossEnemy;

		public Vector3 shipPosition { get { return m_ship.position; } }
		public bool isWin { get; set; }
		public bool isDemaged { get { return m_isDemaged; } }
		public bool isLossEnemy { get { return m_isLossEnemy; } }

		public int stars { get { return m_stars; } }
		public int points { get { return m_points; } }
		public float bombProcess { get { return m_ship.mind.bombProcess; } }
		public float laserProcess { get { return m_ship.mind.shieldProcess; } }

		public void AddPoints(int pointsCount)
		{
			m_points = Mathf.Clamp(pointsCount + m_points, MIN_POINTS, MAX_POINTS);
			m_bar.points = m_points;
		}
		public void AddStars(int count)
		{
			m_stars += count;
		}
		public void Modify()
		{
			if (!m_ship)
			{
				return;
			}

			m_ship.mind.ModificateByOne();
			m_bar.modifications = m_ship.mind.modsCount;
		}
		public bool Shield()
		{
			if (!m_ship)
			{
				return false;
			}

			return m_ship.mind.Shield();
		}
		public bool Bomb()
		{
			if (!m_ship)
			{
				return false;
			}

			return m_ship.mind.Bomb();
		}
		public void Heal(int healthCount)
		{
			if (!m_ship)
			{
				return;
			}

			m_ship.ChangeHealth(healthCount);
		}
		public void KillEnemy(UnitType type)
		{
			if (!m_killings.ContainsKey(type))
			{
				m_killings.Add(type, 1);
				return;
			}

			uint killsCount;
			m_killings.TryGetValue(type, out killsCount);
			m_killings.Remove(type);
			m_killings.Add(type, killsCount + 1);
		}

		public void BeDemaged()
		{
			SetTrigger(ref m_isDemaged, onDemaged);
		}
		public void LossEnemy()
		{
			SetTrigger(ref m_isLossEnemy, onLossEnemy);
		}

		private int m_points;
		private int m_stars;
		private bool m_isDemaged = false;
		private bool m_isLossEnemy = false;

		private const int MIN_POINTS = 0;
		private const int MAX_POINTS = 999999999;

		private IPlayerBar m_bar;
		private Ship m_ship;

		private Dictionary<UnitType, uint> m_killings = new Dictionary<UnitType, uint>();

		private void SetTrigger(ref bool trigger, EventDelegate onTriggerEvent)
		{
			if (trigger)
			{
				return;
			}

			if (onTriggerEvent != null) onTriggerEvent();
			onTriggerEvent = null;
			trigger = true;
		}
	}
}
