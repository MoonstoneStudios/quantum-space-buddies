﻿using QSB.Tools;
using QSB.Utility;
using System.Reflection;
using UnityEngine;

namespace QSB.TransformSync
{
    public class PlayerProbeSync : TransformSync
    {
        public static PlayerProbeSync LocalInstance { get; private set; }

        protected override uint PlayerIdOffset => 3;
        
        public Transform bodyTransform;

        public override void OnStartLocalPlayer()
        {
            LocalInstance = this;
        }

        private Transform GetProbe()
        {
            return Locator.GetProbe().transform.Find("CameraPivot").Find("Geometry");
        }

        protected override Transform InitLocalTransform()
        {
            DebugLog.ToConsole($"{MethodBase.GetCurrentMethod().Name} for {GetType().Name}");
            var body = GetProbe();

            bodyTransform = body;

            Player.ProbeBody = body.gameObject;

            return body;
        }

        protected override Transform InitRemoteTransform()
        {
            DebugLog.ToConsole($"{MethodBase.GetCurrentMethod().Name} for {GetType().Name}");
            var probe = GetProbe();

            probe.gameObject.SetActive(false);
            var body = Instantiate(probe);
            probe.gameObject.SetActive(true);

            Destroy(body.GetComponentInChildren<ProbeAnimatorController>());

            PlayerToolsManager.CreateProbe(body, Player);

            bodyTransform = body;

            Player.ProbeBody = body.gameObject;

            return body;
        }

        protected override void UpdateTransform()
        {
            base.UpdateTransform();
            if (Player.GetState(State.ProbeActive))
            {
                return;
            }
            if (hasAuthority)
            {
                transform.position = ReferenceSector.Transform.InverseTransformPoint(Player.ProbeLauncher.ToolGameObject.transform.position);
                return;
            }
            if (SyncedTransform.position == Vector3.zero ||
                SyncedTransform.position == Locator.GetAstroObject(AstroObject.Name.Sun).transform.position)
            {
                return;
            }
            SyncedTransform.localPosition = ReferenceSector.Transform.InverseTransformPoint(Player.ProbeLauncher.ToolGameObject.transform.position);
        }

        public override bool IsReady => Locator.GetProbe() != null && PlayerRegistry.PlayerExists(PlayerId) && Player.IsReady;
    }
}
