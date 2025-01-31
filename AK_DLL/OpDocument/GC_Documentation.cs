﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using AK_TypeDef;

namespace AK_DLL
{
    //下面这个GameComponent使用了和原版不一样的存读档流程。除非真的知道你在干什么，不然别改。
    //一个干员的档案
    //这玩意不和单个pawn一对一绑定，不是通用文档
    public class OperatorDocument : IExposable, ILoadReferenceable
    {
        //开放开盒干员的权限
        public string operatorID;
        public bool currentExist;
        public Pawn pawn;
        public Thing weapon;
        //public Hediff_Operator hediff;

        public int preferredFashionSet = -1;
        public List<Thing> apparel;

        public Dictionary<SkillDef, int> skillLevel;
        public VoicePackDef voicePack;
        public int preferedAbility = 0;
        public OperatorDef operatorDef;
        public int oripathySeverity = 0;

        public int preferedSkin = 1;    //选定立绘,0是精0, 1是精2, 后面是换装

        /*[Obsolete]
        public int pendingFashion = -1; //将要换装的序号*/
        public OperatorFashionSetDef pendingFashionDef = null;

        public OperatorDocument()
        {
            this.skillLevel = new Dictionary<SkillDef, int>();
        }
        public OperatorDocument(string defName, Pawn p, Thing weapon, OperatorDef operatorDef) : this()
        {
            this.operatorDef = operatorDef;
            this.operatorID = defName;
            this.currentExist = true;
            this.pawn = p;
            this.weapon = weapon;
            this.RecordSkills();
        }

        //在档案中注册衣服。衣服会在换装时回收
        public void RegisterFashionSet(List<Thing> fashionSet)
        {
            DestroyFashionSet();
            if (apparel == null) apparel = new List<Thing>();
            foreach (Thing i in fashionSet)
            {
                apparel.Add(i);
            }
        }

        //回收当前衣服。
        public void DestroyFashionSet()
        {
            if (apparel == null || apparel.Count == 0) return;
            foreach (Thing i in apparel)
            {
                if (i != null && !i.Destroyed) i.Destroy(DestroyMode.Vanish);
            }
            apparel.Clear();
        }

        //手动注册
        public void ManualRegister(Pawn p)
        {
            OperatorDocument doc = p.GetDoc();
            if (false && doc != null)
            {
                Log.Error("[AK] 此殖民者已经被视为干员，不能手动绑定");
                return;
            }

            IntVec3 cell = p.Position;
            Map map = p.Map;
            p.Destroy();
            this.operatorDef.Recruit(cell, map);
        }
        public string GetUniqueLoadID()
        {
            return (this.operatorID + "Doc");
        }

        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.RecordSkills();
            }
            Scribe_Defs.Look(ref this.voicePack, "voicePackDef");
            Scribe_Values.Look(ref this.operatorID, "defName");
            Scribe_Values.Look(ref this.currentExist, "alive");
            Scribe_References.Look<Pawn>(ref this.pawn, "operator", true);
            Scribe_References.Look(ref this.weapon, "weapon", true);

            Scribe_Values.Look(ref this.preferredFashionSet, "fashionSet", -1);
            Scribe_Collections.Look(ref this.apparel, "apparel", LookMode.Reference);

            Scribe_Collections.Look(ref this.skillLevel, "skill", LookMode.Def, LookMode.Value);
            Scribe_Defs.Look(ref this.operatorDef, "def");
            Scribe_Values.Look<int>(ref this.preferedAbility, "preferedAbility", 0, true);
            Scribe_Values.Look<int>(ref this.oripathySeverity, "oriSev", 0, true);
            Scribe_Values.Look<int>(ref this.preferedSkin, "skin");

            Scribe_Defs.Look(ref this.pendingFashionDef, "fashionDef");
        }

        //记录当前干员所有技能
        public void RecordSkills()
        {
            if (this.pawn == null || this.pawn.skills == null) return;
            foreach (SkillRecord i in this.pawn.skills.skills)
            {
                if (this.skillLevel.ContainsKey(i.def) == false)
                {
                    this.skillLevel.Add(i.def, i.levelInt);
                }
                else
                {
                    this.skillLevel[i.def] = Math.Max(this.skillLevel[i.def], i.levelInt);
                }
            }
        }
    }

    public class GC_OperatorDocumentation : GameComponent
    {
        public static Dictionary<string, OperatorDocument> opDocArchive;

        //提升性能的缓存
        public static Dictionary<Pawn, OperatorDocument> cachedOperators = new Dictionary<Pawn, OperatorDocument>();
        public static HashSet<Pawn> cachedNonOperators = new HashSet<Pawn>();
        public GC_OperatorDocumentation(Game game)
        {
            opDocArchive = new Dictionary<string, OperatorDocument>();
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if (ModLister.GetActiveModWithIdentifier("MIS.Arknights") != null)
            {
                Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(Translator.Translate("AK_StartLabel"), Translator.Translate("AK_StartDesc"), LetterDefOf.NeutralEvent, null, null));
            }
            if (opDocArchive == null) opDocArchive = new Dictionary<string, OperatorDocument>();
        }


        //先FinalizeInit再LoadedGame
        public override void LoadedGame()
        {
            base.LoadedGame();
            VoicePlayer.LoadedGame();
            if (opDocArchive == null) opDocArchive = new Dictionary<string, OperatorDocument>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //GC是先于entity加载的，所以在这里去指pawn一定是个空指针。去指丢进下面那个函数去做了。
            if (Scribe.mode != LoadSaveMode.ResolvingCrossRefs)
            {
                List<string> key = new List<string>();
                List<OperatorDocument> value = new List<OperatorDocument>();
                try
                {
                    Scribe_Collections.Look(ref opDocArchive, "operatorDocument", LookMode.Value, LookMode.Deep, ref key, ref value);
                }
                catch { Log.Error("没保存起"); }
            }
        }

        //会在加载流程很后面加载。执行pawn的去指。
        public override void FinalizeInit()
        {
            List<string> key = new List<string>();
            List<OperatorDocument> value = new List<OperatorDocument>();
            Scribe.mode = LoadSaveMode.ResolvingCrossRefs;
            try
            {
                Scribe_Collections.Look(ref opDocArchive, "operatorDocument", LookMode.Value, LookMode.Deep, ref key, ref value);
            }
            catch
            {
                Log.Error("没保存起");
            }
            Scribe.mode = LoadSaveMode.Inactive;
            if (AK_ModSettings.debugOverride)
            {
                foreach (KeyValuePair<string, OperatorDocument> node in opDocArchive)
                {
                    node.Value.RecordSkills();
                    Log.Message($"当前已招募 {node.Value.operatorID}");
                }
            }
        }

        public static void AddPawn(string ID, OperatorDef operatorDef, Pawn pawn, Thing weapon, List<Thing> fashionSet)
        {
            OperatorDocument doc;
            if (opDocArchive.ContainsKey(ID) == false)
            {
                doc = new OperatorDocument(ID, pawn, weapon, operatorDef);
                opDocArchive.Add(ID, doc);
            }
            else
            {
                doc = opDocArchive[ID];
                DestroyHeritage(doc);
                ReRecruit(doc, ID, pawn, weapon);
            }
            doc.RegisterFashionSet(fashionSet);
        }

        public override void GameComponentTick()
        {
            //base.GameComponentTick(); 感觉没用
        }

        public static void DestroyHeritage(OperatorDocument doc)
        {
            doc.currentExist = false;
            //doc.groupedAbilities.Clear();
            doc.preferedAbility = 0;
            if (doc.pawn != null)
            {
                if (doc.pawn.Destroyed == false) doc.pawn.Destroy();
                doc.pawn.Discard();
                if (Find.WorldPawns.Contains(doc.pawn)) Find.WorldPawns.RemoveAndDiscardPawnViaGC(doc.pawn);
            }
            if (doc.weapon != null) doc.weapon.Destroy();
            //if (doc.hediff != null) Log.Error("人都没了但hediff还存在,请提交此bug至制作组FS");
        }

        public static void ReRecruit(OperatorDocument doc, string defName, Pawn p, Thing weapon)
        {
            doc.pawn = p;
            doc.weapon = weapon;
            //doc.hediff = p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) as Hediff_Operator;
            doc.currentExist = true;
        }
    }

}