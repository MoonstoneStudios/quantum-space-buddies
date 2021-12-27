﻿using OWML.Common;
using QSB.Messaging;
using QSB.Player;
using QSB.QuantumSync.Messages;
using QSB.QuantumSync.WorldObjects;
using QSB.Utility;
using QSB.WorldSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QSB.QuantumSync
{
	internal class QuantumManager : WorldObjectManager
	{
		public override WorldObjectType WorldObjectType => WorldObjectType.Both;

		public static QuantumShrine Shrine { get; private set; }

		public override void Awake()
		{
			base.Awake();
			QSBPlayerManager.OnRemovePlayer += PlayerLeave;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			QSBPlayerManager.OnRemovePlayer -= PlayerLeave;
		}

		protected override void RebuildWorldObjects(OWScene scene)
		{
			DebugLog.DebugWrite("Rebuilding quantum objects...", MessageType.Info);
			QSBWorldSync.Init<QSBQuantumState, QuantumState>();
			QSBWorldSync.Init<QSBSocketedQuantumObject, SocketedQuantumObject>();
			QSBWorldSync.Init<QSBMultiStateQuantumObject, MultiStateQuantumObject>();
			QSBWorldSync.Init<QSBQuantumSocket, QuantumSocket>();
			QSBWorldSync.Init<QSBQuantumShuffleObject, QuantumShuffleObject>();
			QSBWorldSync.Init<QSBQuantumMoon, QuantumMoon>();
			QSBWorldSync.Init<QSBEyeProxyQuantumMoon, EyeProxyQuantumMoon>();
			if (scene == OWScene.SolarSystem)
			{
				Shrine = QSBWorldSync.GetUnityObjects<QuantumShrine>().First();
			}
		}

		public void PlayerLeave(uint playerId)
		{
			if (!QSBCore.IsHost)
			{
				return;
			}

			foreach (var obj in QSBWorldSync.GetWorldObjects<IQSBQuantumObject>())
			{
				if (obj.ControllingPlayer == playerId)
				{
					obj.SendMessage(new QuantumAuthorityMessage(obj.IsEnabled ? QSBPlayerManager.LocalPlayerId : 0u));
				}
			}
		}

		public void OnRenderObject()
		{
			if (!WorldObjectManager.AllObjectsReady || !QSBCore.ShowLinesInDebug)
			{
				return;
			}

			if (Shrine != null)
			{
				Popcron.Gizmos.Sphere(Shrine.transform.position, 10f, Color.magenta);
			}

			foreach (var quantumObject in QSBWorldSync.GetWorldObjects<IQSBQuantumObject>())
			{
				if (quantumObject.ControllingPlayer == 0)
				{
					if (quantumObject.IsEnabled)
					{
						Popcron.Gizmos.Line(quantumObject.ReturnObject().transform.position,
							QSBPlayerManager.LocalPlayer.Body.transform.position,
							Color.magenta * 0.25f);
					}

					continue;
				}

				Popcron.Gizmos.Line(quantumObject.ReturnObject().transform.position,
					QSBPlayerManager.GetPlayer(quantumObject.ControllingPlayer).Body.transform.position,
					Color.magenta);
			}
		}

		public static Tuple<bool, List<PlayerInfo>> IsVisibleUsingCameraFrustum(ShapeVisibilityTracker tracker, bool ignoreLocalCamera)
		{
			if (!AllObjectsReady)
			{
				return new Tuple<bool, List<PlayerInfo>>(false, new List<PlayerInfo>());
			}

			var playersWithCameras = QSBPlayerManager.GetPlayersWithCameras(!ignoreLocalCamera);
			if (playersWithCameras.Count == 0)
			{
				DebugLog.ToConsole($"Warning - Could not find any players with cameras!", MessageType.Warning);
				return new Tuple<bool, List<PlayerInfo>>(false, new List<PlayerInfo>());
			}

			if (!tracker.gameObject.activeInHierarchy)
			{
				return new Tuple<bool, List<PlayerInfo>>(false, new List<PlayerInfo>());
			}

			var playersWhoCanSee = new List<PlayerInfo>();
			var foundPlayers = false;
			foreach (var player in playersWithCameras)
			{
				if (player.Camera == null)
				{
					DebugLog.ToConsole($"Warning - Camera is null for id:{player.PlayerId}!", MessageType.Warning);
					continue;
				}

				var isInFrustum = tracker.IsInFrustum(player.Camera.GetFrustumPlanes());
				if (isInFrustum)
				{
					playersWhoCanSee.Add(player);
					foundPlayers = true;
				}
			}

			return new Tuple<bool, List<PlayerInfo>>(foundPlayers, playersWhoCanSee);
		}

		public static bool IsVisible(ShapeVisibilityTracker tracker, bool ignoreLocalCamera) => tracker.gameObject.activeInHierarchy
			&& IsVisibleUsingCameraFrustum(tracker, ignoreLocalCamera).Item1
			&& QSBPlayerManager.GetPlayersWithCameras(!ignoreLocalCamera)
				.Any(x => VisibilityOccluder.CanYouSee(tracker, x.Camera.mainCamera.transform.position));

		public static IEnumerable<PlayerInfo> GetEntangledPlayers(QuantumObject obj)
		{
			if (!AllObjectsReady)
			{
				return Enumerable.Empty<PlayerInfo>();
			}

			var worldObj = obj.GetWorldObject<IQSBQuantumObject>();
			return QSBPlayerManager.PlayerList.Where(x => x.EntangledObject == worldObj);
		}
	}
}