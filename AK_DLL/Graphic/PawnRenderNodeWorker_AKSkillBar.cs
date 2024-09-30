﻿using AKA_Ability;
using TMPro;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace AK_DLL
{
    //泰南我草你妈 Draw方法是交替执行的 Worker还是唯一实例
    public class PawnRenderNodeWorker_AKSkillBar : PawnRenderNodeWorker
    {
        //locOffset
        private ProgramState CurrentProgramState => Current.ProgramState;
        private static bool CameraPlusModEnabled => AK_BarUITool.CameraPlusModEnabled;
        private static bool SimpleCameraModEnabled => AK_BarUITool.SimpleCameraModEnabled;
        private float ZoomRootSize => Find.CameraDriver.ZoomRootSize;
        private static float Width => AK_ModSettings.barWidth * 0.01f;
        private static float Height => AK_ModSettings.barHeight * 0.001f;
        private static float Margin => AK_ModSettings.barMargin * 0.01f;
        private static Vector2 BarSize => new Vector2(Width, Height);
        private static Vector3 BottomMargin => new Vector3(0f, 0f, Margin - Height);
        private static Vector3 TopMargin => Vector3.forward * 1f;
        //Mat
        private static Material BarFilledMat => AK_BarUITool.SkillBarFilledMat;
        private static Material BarUnfilledMat => AK_BarUITool.BarUnfilledMat;
        private static Material Timer_Icon => AK_BarUITool.Timer_Icon;
        private static Material RotateRing => AK_BarUITool.RotateRingIcon;
        private static Material BurstButton => AK_BarUITool.BurstIcon;
        private string OperatorID(Pawn p) => (p.GetDoc()?.operatorID ?? p.Label);
        private string ObjectName(Pawn p) => (p.GetDoc()?.operatorID ?? p.Label) + ".objTMP";
        private GameObject PrefabTMP => AK_Tool.PAAsset.LoadAsset<GameObject>("PrefabTMPPopup");
        //使用prefab可以避免new GameObject出来的object被回收不能用Find方法找到；
        private Dictionary<string, GameObject> PrefabTMPInstancesDictionary => AK_BarUITool.PrefabTMPInstancesDictionary;
        private void InitObjectOnce(Pawn p)
        {
            if (PrefabTMPInstancesDictionary.NullOrEmpty() || !PrefabTMPInstancesDictionary.ContainsKey(OperatorID(p)))
            {
                GameObject PrefabTMPInstance = GameObject.Instantiate(PrefabTMP);
                PrefabTMPInstance.name = ObjectName(p);
                PrefabTMPInstance.layer = 2;
                PrefabTMPInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                PrefabTMPInstancesDictionary.Add(OperatorID(p), PrefabTMPInstance);
            }
            else
            {
                if (p.Dead)
                {
                    GameObject PrefabTMPInstance = PrefabTMPInstancesDictionary.TryGetValue(OperatorID(p));
                    if (PrefabTMPInstance == GameObject.Find(ObjectName(p)))
                    {
                        GameObject.DestroyImmediate(PrefabTMPInstance);
                        PrefabTMPInstancesDictionary.Remove(OperatorID(p));
                    }
                }
                return;
            }
        }
        private float CooldownPercent(AKAbility ability)
        {
            //Log.Message(ability.cooldown.CooldownPercent());
            return ability.cooldown.CooldownPercent();
            /*if (ability.cooldown.charge == ability.cooldown.MaxCharge)
            {
                return 1;
            }
            return 1f - (float)ability.cooldown.CDCurrent / (float)ability.cooldown.CDPerCharge;*/
        }
        private float GetZoomRatio()
        {
            if (AK_ModSettings.zoomWithCamera)
            {
                return Mathf.Max(ZoomRootSize, 11) / 11;
            }
            return 1f;
        }
        private void DrawSkillBar(Material mat, Pawn pawn, float percent)
        {
            Vector3 drawPos = pawn.DrawPos;
            float zoomRatio = GetZoomRatio();
            float zoomWidthRatio;
            float zoomYRatio;
            if (CameraPlusModEnabled || SimpleCameraModEnabled)
            {
                zoomWidthRatio = zoomRatio > 4.35f ? 4.35f : zoomRatio;
                zoomYRatio = zoomRatio > 5f ? 5f : zoomRatio;
            }
            else
            {
                zoomWidthRatio = zoomRatio > 3.75f ? 3.75f : zoomRatio;
                zoomYRatio = zoomRatio > 3f ? 3f : zoomRatio;
            }
            //
            GenDraw.FillableBarRequest fbr = default;
            if (CameraPlusModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.9f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 1.75f ? zoomRatio * 1.5f : zoomRatio;
            }
            else if (SimpleCameraModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 3f ? zoomRatio * 1.05f : zoomRatio;
            }
            else
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 6.5f ? zoomRatio * 1.25f : zoomRatio;
            }
            fbr.fillPercent = (percent < 0f) ? 0f : percent;
            fbr.filledMat = mat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            Vector3 iconPos = new Vector3(fbr.center.x - (fbr.size.x / 2) - 0.075f, fbr.center.y, fbr.center.z);
            DrawIcon(iconPos, new Vector3(0.25f, 1f, 0.25f), Timer_Icon, MeshPool.plane025);
        }
        private void DrawIcon(Vector3 pos, Vector3 scale, Material icon, Mesh plane)
        {
            Matrix4x4 matrix = default;
            matrix.SetTRS(pos, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(plane, matrix, material: icon, 2);
        }
        private void DrawIcon(Vector3 pos, Vector3 scale, Quaternion quat, Material icon, Mesh plane, int layer)
        {
            Matrix4x4 matrix = default;
            matrix.SetTRS(pos, quat, scale);
            Graphics.DrawMesh(plane, matrix, material: icon, 2);
        }
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            Pawn pawn = parms.pawn;
            if (CurrentProgramState != ProgramState.Playing || pawn == null)
            {
                return false;
            }
            if (pawn.GetDoc() != null)
            {
                return true;
            }
            return false;
        }
        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            Pawn pawn = parms.pawn;
            //倒地销毁TMP物件D
            /*if (!AK_ModSettings.enable_Skillbar)
            {
                return;
            }*/
            GameObject PrefabTMPInstance = PrefabTMPInstancesDictionary.TryGetValue(OperatorID(pawn));
            if (pawn.Downed || pawn.Dead || !AK_ModSettings.enable_Skillbar || (AK_ModSettings.display_Skillbar_OnDraftedOnly && !pawn.Drafted))
            {
                if (PrefabTMPInstance == GameObject.Find(ObjectName(pawn)))
                {
                    GameObject.Destroy(PrefabTMPInstance);
                    PrefabTMPInstancesDictionary.Remove(OperatorID(pawn));
                }
            }
            //征召显示开关
            if (PrefabTMPInstance != null)
            {
                PrefabTMPInstance?.SetActive(!AK_ModSettings.enable_Skillbar || pawn.Drafted || pawn.Downed);
            }
            if (!AK_ModSettings.enable_Skillbar || (AK_ModSettings.display_Skillbar_OnDraftedOnly && !pawn.Drafted))
            {
                return;
            }
            float SkillPercent = 0f;
            bool IsGrouped = false;
            VAbility_Operator operatorID = null;
            AKAbility ability = null;
            if (operatorID == null)
            {
                operatorID = pawn.GetVAbility();
            }
            if (operatorID != null && ability == null)
            {
                ability = operatorID?.AKATracker?.innateAbilities.Find((AKAbility a) => !a.def.grouped);
                IsGrouped = false;
                if (ability == null)
                {
                    ability = operatorID?.AKATracker?.groupedAbilities.Find((AKAbility a) => a.def.grouped);
                    IsGrouped = true;
                }
            }
            //有AKA技能的Pawn才会显示技能CD进度，否则全为空
            if (ability != null)
            {
                SkillPercent = CooldownPercent(ability);
            }
            else
            {
                SkillPercent = 0f;
            }
            DrawSkillBar(BarFilledMat, pawn, SkillPercent);
            Vector3 OriginCenter = pawn.DrawPos + TopMargin + (Vector3.up * 3f);
            Vector3 Scale = new Vector3(0.3f, 1f, 0.3f);
            //自动回复技能
            if (!AK_ModSettings.display_Skillbar_OnDraftedOnly && !pawn.Drafted)
            {
                return;
            }
            if (ability != null && ability.cooldown.charge == ability.cooldown.MaxCharge && !IsGrouped)
            {
                if (BurstButton != null)
                {
                    DrawIcon(OriginCenter, Scale, BurstButton, MeshPool.plane10);
                    //闪烁
                    float BurstFlashFactor = GlobalFactor_Accumulator.BurstFlashFactor;
                    float factor = Mathf.Sqrt(BurstFlashFactor);
                    float transparency = 120 - (BurstFlashFactor / 1.2f * 70);
                    //float transparency = Mathf.Lerp(150, 0, factor);
                    AK_BarUITool.SimpleRectBarRequest sbr = default;
                    sbr.center = OriginCenter + Vector3.down;
                    sbr.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(255, 255, 0, (byte)Mathf.Max(transparency, 20f))); ;
                    sbr.rotation = Quaternion.AngleAxis(45f, Vector3.up);
                    sbr.size = new Vector2(0.25f * BurstFlashFactor, 0.25f * BurstFlashFactor);
                    AK_BarUITool.DrawSimpleRectBar(sbr);
                }
            }
            //充能技能
            if (ability != null && IsGrouped)
            {
                InitObjectOnce(pawn);
                if (RotateRing != null)
                {
                    float RotateAngle = GlobalFactor_Accumulator.RotateAngle;
                    DrawIcon(OriginCenter, Scale, Quaternion.AngleAxis(RotateAngle, Vector3.up), RotateRing, MeshPool.plane10, 1);
                }
                if (PrefabTMPInstance != null)
                {
                    TextMeshPro tmp = PrefabTMPInstance.GetComponent<TextMeshPro>();
                    Vector3 scale = new Vector3(0.25f, 0.25f, 1f);
                    int ChargeTimes = ability.cooldown.charge;
                    tmp.transform.position = OriginCenter;
                    tmp.transform.localScale = scale;
                    tmp.fontSize = 6;
                    tmp.SetText(ChargeTimes.ToString());
                    if (!PrefabTMPInstance.activeSelf)
                    {
                        PrefabTMPInstance.SetActive(true);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

    }
}
