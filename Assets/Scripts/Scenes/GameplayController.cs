﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyGame.Hero;
using MyGame.Factory;
using MyGame.World;

namespace MyGame
{
	public sealed class GameplayController : MonoBehaviour, IGameplay
	{
		public bool isMapStart { get; private set; }
		public bool isMapStay { get; private set; }
		public bool isPaused { get; private set; }
		public bool isGameEnd { get { return !ship.isLive || map.isReached; } }
		public bool isWin { get { return isGameEnd && m_world.ship.isLive; } }
		public bool isPlaying { get { return !isPaused && isMapStart && !isGameEnd; } }

		[SerializeField]
		private GameWorld m_world;
		[SerializeField]
		private GameplayUI m_interface;
		[SerializeField]
		private ScenesController m_scenesController;
		[SerializeField]
		private Factories m_factory;

		private bool m_isMapStart;
		private bool m_isMapStay;
		private bool m_isPaused;
		private bool m_isGameEnd;
		private bool m_isWin;
		private bool m_isPlaying;

		private Ship ship { get; set; }
		private Map map { get; set; }
		private User user { get; set; }

		private const int FRAME_RATE = 40;

		private void Awake()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = FRAME_RATE;

			isMapStart = false;
			isPaused = false;
			isMapStay = true;
		}
		private void Start()
		{
			InitUser();
			InitFactory();
			InitShip();
			InitMap();
			InitWorld();
			InitInterface();

			UpdateChanges();
		}
		private void InitUser()
		{
			//user = GameData.LoadUser();
		}
		private void InitFactory()
		{
			m_factory.Init(m_world.container, m_interface);
			m_world.factory = m_factory;
		}
		private void InitShip()
		{
			ship = m_factory.GetShip(ShipType.VOYAGER);
			ship.InitWorld(m_world);
		}
		private void InitMap()
		{
			map = m_factory.GetMap();
			map.InitGameplay(this);
		}
		private void InitWorld()
		{
			m_world.ship = ship;
			m_world.playerBar = m_interface;
			m_world.map = map;
			m_world.InitGameplay(this);
		}
		private void InitInterface()
		{
			m_interface.InitGameplay(this);

			m_interface.onPause += map.Pause;

			m_interface.moveShip += ship.MoveTo;

			m_interface.uncontrollEvents += isTrue => m_world.SetSlowMode(isTrue);

			m_interface.firstTouchEvents += () => {
				ship.roadController.Play();
			};
			ship.roadController.OnEndReached.AddListener(T => {
				isMapStart = true;
				OnMapStart();
			});
		}

		private void FixedUpdate()
		{
			if (!IsAnyChange())
			{
				return;
			}

			UpdateChanges();

			if (!isGameEnd)
			{
				return;
			}

			if (isWin)
			{
				OnMapReached();
				return;
			}

			OnGameOver();
		}

		private void OnMapStart()
		{
			isMapStart = true;
			isMapStay = false;
			map.Play();
			m_interface.OnMapStart();
			Destroy(ship.roadController);
		}
		private void OnMapReached()
		{
		}
		private void OnGameOver()
		{
			m_world.KillPlayer();
			m_interface.GameOver();
			isMapStay = true;
			m_scenesController.SetScene("DemoScene");
		}

		private bool IsAnyChange()
		{
			return (
				m_isMapStart != isMapStart ||
				m_isMapStay != isMapStay ||
				m_isPaused != isPaused ||
				m_isGameEnd != isGameEnd ||
				m_isWin != isWin ||
				m_isPlaying != isPlaying);
		}
		private void UpdateChanges()
		{
			m_isMapStart = isMapStart;
			m_isMapStay = isMapStay;
			m_isPaused = isPaused;
			m_isGameEnd = isGameEnd;
			m_isWin = isWin;
			m_isPlaying = isPlaying;

			m_world.OnGameplayChange();
			m_interface.OnGameplayChange();
		} 
	}

	public interface IGameplay
	{
		bool isMapStart { get; }
		bool isPaused { get; }
		bool isMapStay { get; }
		bool isGameEnd { get; }
		bool isWin { get; }
		bool isPlaying { get; }
	}
}
