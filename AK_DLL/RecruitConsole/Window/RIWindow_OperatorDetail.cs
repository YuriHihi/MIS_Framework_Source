﻿using Verse;
using RimWorld;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using FSUI;
using FS_LivelyRim;
using AKA_Ability;

namespace AK_DLL.UI
{
    #region legacy 
    /*
    public class RIWindow_OperatorDetail : Dialog_NodeTree
    {
        public static bool isRecruit = true;
        public static readonly Color StackElementBackground = new Color(1f, 1f, 1f, 0.1f);
        public RIWindow_OperatorDetail(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {
        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1920f, 1080f);
            }
        }

        public Texture2D blackBack
        {
            get
            {
                return ContentFinder<Texture2D>.Get("UI/Frame/Frame_Skills");
            }
        }
        public OperatorDef Operator_Def
        {
            get { return RIWindowHandler.def; }
        }
        public Thing RecruitConsole
        {
            get { return RIWindowHandler.recruitConsole; }
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            Color temp = GUI.color;       
            try
            {
                Widgets.DrawTextureFitted(new Rect(inRect.x += 350f + Operator_Def.standOffset.x, inRect.y + 40f + Operator_Def.standOffset.y, inRect.width - 870f, inRect.height), ContentFinder<Texture2D>.Get(Operator_Def.stand), Operator_Def.standRatio);
            }
            catch
            {
                Log.Error("MIS. 立绘错误");
            }
            //立绘绘制
            GUI.DrawTexture(new Rect(970f, 0f, 550f, 720f), blackBack);
            //背景绘制
            Rect rect = new Rect(1000f, 20f, 150f, 70f);

            //返回按钮的绘制
            Rect rect_Back = new Rect(1130f, 620f, 100f, 60f);
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_List);
            }

            Widgets.Label(rect, Operator_Def.nickname + "：" + Operator_Def.name);
            //人名绘制
            Rect rect1 = new Rect(rect);
            rect.y += 20f;
            rect.height += 35f;
            Widgets.Label(rect, Operator_Def.description);
            //描述绘制
            rect.height -= 35f;
            rect.y += 100f;
            Widgets.Label(rect, "AK_Terait".Translate());
            rect.y += 25f;
            foreach (TraitAndDegree TraitAndDegree in Operator_Def.traits)
            {
                TraitDegreeData traitDef = TraitAndDegree.def.DataAtDegree(TraitAndDegree.degree);
                if (traitDef == null)
                {
                    Log.ErrorOnce($"MIS. {this.Operator_Def}'s {TraitAndDegree.def.defName} do not have {TraitAndDegree.degree} degree", 1);
                }
                else
                {
                    Rect traitRect = new Rect(rect.x, rect.y, Text.CalcSize(traitDef.label).x + 10f, 25);
                    temp = GUI.color;
                    GUI.color = StackElementBackground;
                    GUI.DrawTexture(traitRect, BaseContent.WhiteTex);
                    GUI.color = temp;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(traitRect, traitDef.label.Truncate(traitRect.width));
                    Text.Anchor = TextAnchor.UpperLeft;
                    if (Mouse.IsOver(traitRect)) { Widgets.DrawHighlight(traitRect); }
                    rect.y += 28f;
                }
                /*string label = "寄";
                label = TraitAndDegree.def.DataAtDegree(TraitAndDegree.degree)?.label;
                Widgets.Label(rect, label ?? "寄");*//*
            }
            //特性显示绘制
            Rect rect_AbilityImage = new Rect(rect.x, rect.y + 65f, 60f, 60f);
            Rect rect_AbilityText = new Rect(rect.x + 70f, rect.y + 50f, 100f, 60f);

            if (Operator_Def.abilities != null && Operator_Def.abilities.Count > 0)
            {
                foreach (OperatorAbilityDef ability in Operator_Def.abilities)
                {
                    Texture2D abilityImage = ContentFinder<Texture2D>.Get(ability.icon);
                    Widgets.DrawTextureFitted(rect_AbilityImage, abilityImage, 1f);
                    StringBuilder text = new StringBuilder();
                    text.AppendLine(ability.label);
                    text.AppendLine(ability.description);
                    Widgets.Label(rect_AbilityText, text.ToString().Trim());
                    rect_AbilityImage.y += 65f;
                    rect_AbilityText.y += 65f;
                }
            }
            //^绘制技能(放的那种)

            rect1.x = 80f;
            rect1.y = 350f;
            rect1.width -= 60f;
            rect1.height -= 30f;
            Texture2D smallFire = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinor");
            Texture2D bigFire = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajor");
            //获取兴趣贴图

            Rect rect2 = new Rect(rect1);
            rect2.width += 100f;
            rect2.x += 70f;
            Rect rect3 = new Rect(rect1);
            rect3.height = 152f;
            rect3.width = 152f;
            rect3.y -= 250f;
            Widgets.DrawTextureFitted(new Rect(rect3.x + Operator_Def.headPortraitOffset.x, rect3.y + Operator_Def.headPortraitOffset.y, rect3.width, rect3.height), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
            Widgets.DrawTextureFitted(new Rect(rect3.x + 3f + Operator_Def.headPortraitOffset.x, rect3.y + 2f + Operator_Def.headPortraitOffset.y, 145f, 148f), ContentFinder<Texture2D>.Get(Operator_Def.headPortrait), 1f);
            //绘制头像框与头像
            rect3.height = 150f;
            rect3.width = 150f;
            rect3.x += 5f;
            Widgets.DrawTextureFitted(new Rect(rect2.x - 45f, rect2.y + 95f, 180f, 105f), blackBack, 3f);

            foreach (SkillAndFire skillAndFire in Operator_Def.Skills)
            {
                int skillLv;
                if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(Operator_Def.OperatorID))
                {
                    skillLv = GameComp_OperatorDocumentation.operatorDocument[Operator_Def.OperatorID].skillLevel[skillAndFire.skill];
                }
                else
                {
                    skillLv = skillAndFire.level;
                }
                float verticalOffset = 25f * TypeDef.statType[skillAndFire.skill.defName];
                Widgets.FillableBar(new Rect(rect2.x, rect2.y + verticalOffset, 170f, 20f), skillLv / 20f, SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.3f)), ContentFinder<Texture2D>.Get("UI/Frame/Null"), false);
                Widgets.Label(new Rect(rect1.x - 35f, rect1.y + verticalOffset, 150f, rect1.height), skillAndFire.skill.label);
                Widgets.Label(new Rect(rect1.x + 50f, rect1.y + verticalOffset, 100f, rect1.height), skillLv.ToString());
                Rect rect4 = new Rect(rect1.x + 25f, rect1.y + 4f + verticalOffset, 10f, 10f);
                if (skillAndFire.fireLevel == Passion.Minor)
                {
                    Widgets.DrawTextureFitted(rect4, smallFire, 2.5f);
                }
                if (skillAndFire.fireLevel == Passion.Major)
                {
                    Widgets.DrawTextureFitted(rect4, bigFire, 2.5f);
                }
            }
            //技能绘制


            rect_Back.x -= 145f;
            OperatorDocument doc = null;
            if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(Operator_Def.OperatorID))
            {
                doc = GameComp_OperatorDocumentation.operatorDocument[Operator_Def.OperatorID];
            }

            if (Widgets.ButtonText(rect_Back, recruitText))
            {
                if (isRecruit == false)
                {
                    isRecruit = true;
                    AK_ModSettings.secretary = AK_Tool.GetOperatorIDFrom(Operator_Def.defName);
                    this.Close();
                    RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
                    return;
                }

                //如果招募曾经招过的干员
                if (doc != null && !doc.currentExist)
                {
                }
                //如果干员未招募过，或已死亡
                if (RecruitConsole.TryGetComp<CompRefuelable>().Fuel >= Operator_Def.ticketCost - 0.01)
                {
                    if (doc == null || !doc.currentExist)
                    {
                        RecruitConsole.TryGetComp<CompRefuelable>().ConsumeFuel(Operator_Def.ticketCost);
                        Operator_Def.Recruit(RecruitConsole.Map);
                        this.Close();

                        /*RIWindow_OperatorList window = new RIWindow_OperatorList(new DiaNode(new TaggedString()), true);
                        window.soundAmbient = SoundDefOf.RadioComms_Ambience;
                        Find.WindowStack.Add(window);*//*
                    }
                    else
                    {
                        recruitText = "AK_CanntRecruitOperator".Translate();
                    }
                }
                else
                {
                    recruitText = "AK_NoTicket".Translate();
                }
                
            }
            //招募
            //切换技能
            if (false && doc != null && doc.currentExist)
            {
                rect_Back.x -= 145f;
                if (Widgets.ButtonText(rect_Back, switchSkillText))
                {
                    doc.groupedAbilities[doc.preferedAbility].enabled = false;
                    doc.preferedAbility = (doc.preferedAbility + 1) % doc.groupedAbilities.Count;
                    doc.groupedAbilities[doc.preferedAbility].enabled = true;
                    Log.Message($"切换技能至{doc.groupedAbilities[doc.preferedAbility].AbilityDef.defName}");
                }
            }  
        }

        private string switchSkillText = "AK_SwitchSkill".Translate();
        public string recruitText = "AK_RecruitOperator".Translate();
    }*/
    #endregion

    public class RIWindow_OperatorDetail : RIWindow
    {
        public static OpDetailType windowPurpose = OpDetailType.Recruit;

        OperatorDocument doc = null;

        static string recruitText;

        private bool canRecruit;

        int preferredSkin = 1;  //当前选中皮肤。同时存储于干员文档（如果有）来实现主界面左下角显示立绘，和mod设置的秘书选择。

        static int preferredVanillaSkillChart = 0;

        Dictionary<int, GameObject> fashionBtns;

        List<GameObject> vanillaSkillBtns;  //0,1: 条形图; 2,3: 雷达图

        List<GameObject> opSkills;  //只有可选技能被加进来。

        GameObject floatingBubbleInstance;

        public GameObject OpStand; //干员静态立绘的渲染目标
        public GameObject OpL2D;   //干员动态立绘的渲染目标（不是模型本身）
        static string OpL2DRenderTargetName = "L2DRenderTarget";  //干员动态立绘的渲染目标的名字

        #region 快捷属性
        private OperatorDef Def
        {
            get { return RIWindowHandler.def; }
        }
        public Thing RecruitConsole
        {
            get { return RIWindowHandler.RecruitConsole; }
        }
        private GameObject ClickedBtn
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject;
            }
        }

        private GameObject ClickedBtnParent
        {
            get
            {
                return ClickedBtn.transform.parent.gameObject;
            }
        }

        private int btnOrder(GameObject clickedBtn)
        {
            return int.Parse(clickedBtn.name.Substring(RIWindow_OperatorList.orderInName));
        }

        /*private int btnOpAbilityAbsOrder(GameObject clickedBtn)
        {
            // FSUI_OpAb_{i}_{logicOrder}
            return int.Parse(clickedBtn.name[10].ToString());
        }*/

        private int PreferredAbility
        {
            get { return doc.preferedAbility; }
            set { doc.preferedAbility = value; }
        }

        #endregion

        public override void DoContent()
        {
            DrawNavBtn();
            //Initialize();
            DrawFashionBtn();
            ChangeStandTo(preferredSkin, true);

            DrawOperatorAbility();
            if (doc != null) SwitchGroupedSkillTo(doc.preferedAbility);

            DrawWeapon();
            DrawTrait();

            DrawVanillaSkills();
            ChangeVanillaSkillChartTo(preferredVanillaSkillChart);

            DrawDescription();

            DrawDebugPanel();
        }

        public override void Initialize()
        {
            base.Initialize();
            if (GC_OperatorDocumentation.opDocArchive.ContainsKey(Def.OperatorID))
            {
                doc = GC_OperatorDocumentation.opDocArchive[Def.OperatorID];
                preferredSkin = doc.preferedSkin;
            }
            canRecruit = false;
            if (RecruitConsole.TryGetComp<CompRefuelable>().Fuel >= Def.ticketCost - 0.01)
            {
                if (doc == null || !doc.currentExist)
                {
                    canRecruit = true;
                    recruitText = "可以招募"; //残留
                }
                else
                {
                    recruitText = "AK_CanntRecruitOperator".Translate();
                }
            }
            else
            {
                recruitText = "AK_NoTicket".Translate();
            }
            floatingBubbleInstance = GameObject.Find("FloatingInfPanel");
            floatingBubbleInstance.SetActive(false);

            OpStand = GameObject.Find("OpStand");
            OpL2D = GameObject.Find(OpL2DRenderTargetName);
        }
        #region 绘制UI

        //换装按钮会被记录于 this.fashionbtns

        #region 左边界面
        private void DrawDescription()
        {
            GameObject OpDescPanel = GameObject.Find("OpDescPanel");
            OpDescPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Def.nickname.Translate();
            OpDescPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Def.description.Translate();

            //职业图标
            GameObject opClassIcon = OpDescPanel.transform.GetChild(1).gameObject;
            if (Def.operatorType.Icon == null)
            {
                TextMeshProUGUI TMP = opClassIcon.GetComponentInChildren<TextMeshProUGUI>();
                TMP.gameObject.SetActive(true);
                TMP.text = Def.operatorType.label.Translate(); 
            }
            else
            {
                Texture2D classImage = Def.operatorType.Icon;
                opClassIcon.GetComponent<Image>().sprite = Sprite.Create(classImage, new Rect(0, 0, classImage.width, classImage.height), new Vector2(0.5f, 0.5f));
            }
        }

        //FIXME：自动给武器打标签
        private void DrawWeapon()
        {
            GameObject weaponIconObj = GameObject.Find("WeaponIcon");
            if (Def.weapon == null)
            {
                weaponIconObj.SetActive(false);
                return;
            }
            GameObject WeaponPanel = GameObject.Find("WeaponPanel");
            Texture2D weaponIcon = ContentFinder<Texture2D>.Get(Def.weapon.graphicData.texPath);
            weaponIconObj.GetComponent<Image>().sprite = Sprite.Create(weaponIcon, new Rect(0, 0, weaponIcon.width, weaponIcon.height), Vector2.zero);

            EventTrigger.Entry entry = weaponIconObj.GetComponent<EventTrigger>().triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);

            entry.callback.AddListener((data) =>
            {
                DrawFloatingBubble(Def.weapon.description.Translate());
            });
        }

        private void DrawFashionBtn()
        {
            Transform FashionPanel = GameObject.Find("FashionPanel").transform;  //切换换装按钮的面板。因为做的时候不会用Grid，所以需要手动设置按钮位置，乐
            GameObject fashionIconPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("FashionIcon");
            GameObject fashionIcon;
            Vector3 v3;

            fashionBtns = new Dictionary<int, GameObject>();

            //精0/精1立绘 切换按钮
            fashionIcon = GameObject.Find("Elite0");
            fashionBtns.Add(0, fashionIcon);
            if (Def.commonStand != null)
            {
                fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    ChangeStandTo(0);
                });
            }
            else fashionIcon.SetActive(false);

            //精2立绘按钮。因为历史问题，这是默认立绘，必须有。
            fashionIcon = GameObject.Find("Elite2");
            fashionBtns.Add(1, fashionIcon);
            fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                ChangeStandTo(1);
            });

            //换装按钮。第一个换装（面板上第3个）在数组内是2。
            int logicOrder = 2;
            if (Def.fashion != null)
            {
                v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < Def.fashion.Count; ++i)
                {
                    //逻辑顺序 代表这按钮在面板上实际的位置（即精2按钮之后）
                    fashionIcon = GameObject.Instantiate(fashionIconPrefab, FashionPanel);
                    fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    fashionIcon.SetActive(true);
                    fashionIcon.name = "FSUI_FashIc_" + logicOrder;
                    int j = logicOrder;
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        //ChangeStandTo(btnOrder(ClickedBtn));
                        ChangeStandTo(j);
                    });
                    fashionBtns.Add(logicOrder, fashionIcon);
                    ++logicOrder;
                }
            }
            //在服装按钮界面实例化l2d换装按钮
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") != null)
            {
                v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < Def.live2dModel.Count; ++i)
                {
                    fashionIcon = GameObject.Instantiate(fashionIconPrefab, FashionPanel);
                    fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    int j = i + 1000; //用j来标记选中的哪个l2d。+1000代表选的l2d而不是静态换装。
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        ChangeStandTo(j);
                    });
                    fashionBtns.Add(j, fashionIcon);
                    ++logicOrder;
                }
            }
        }

        //原版的驯兽等技能的面板
        private void DrawVanillaSkills()
        {
            //柱状图按钮
            GameObject skillTypeBtn = GameObject.Find("BtnBarChart");
            vanillaSkillBtns = new List<GameObject>();
            vanillaSkillBtns.Add(skillTypeBtn);
            skillTypeBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                preferredVanillaSkillChart = 0;
                ChangeVanillaSkillChartTo(0);
            });

            //柱状图
            //GameObject skillBarPanelPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("SkillBarPanel");
            GameObject skillBarPanel = GameObject.Find("SkillBarPanel");
            GameObject skillBar;
            for (int i = 0; i < TypeDef.SortOrderSkill.Count; ++i)
            {
                skillBar = skillBarPanel.transform.GetChild(i).gameObject;
                //技能名字

                skillBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color=\"{GetSkillLabelColor(Def.SortedSkills[i])}\">{Def.SortedSkills[i].skill.label.Translate()}</color>";
                //技能等级 显示与滑动条
                skillBar.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = Def.SortedSkills[i].level.ToString();
                skillBar.GetComponentInChildren<Slider>().value = Def.SortedSkills[i].level;
            }
            vanillaSkillBtns.Add(skillBarPanel);

            //雷达图按钮
            skillTypeBtn = GameObject.Find("BtnRadarChart");
            vanillaSkillBtns.Add(skillTypeBtn);

            skillTypeBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                preferredVanillaSkillChart = 2;
                ChangeVanillaSkillChartTo(2);
            });

            //雷达图
            GameObject radarPanel = GameObject.Find("SkillRadarPanel");
            vanillaSkillBtns.Add(radarPanel);
            RadarChart radarChart = radarPanel.GetComponentInChildren<RadarChart>();
            radarChart.data.Add(new GraphData());
            radarChart.data[0].values = new List<float>();
            radarChart.data[0].color = new Color(0.9686275f, 0.5882353f, 0.03137255f);
            for (int i = 0; i < TypeDef.SortOrderSkill.Count; ++i)
            {
                radarChart.vertexLabelValues[i] = $"<color=\"{GetSkillLabelColor(Def.SortedSkills[i])}\">{Def.SortedSkills[i].skill.label.Translate()}</color>";
                radarChart.data[0].values.Add((float)Def.SortedSkills[i].level / 20.0f);
            }
        }

        private string GetSkillLabelColor(SkillAndFire i)
        {
            int j = (int)i.fireLevel;

            switch (j)
            {
                case 1:
                    return "yellow";
                    //break;
                case 2:
                    return "red";
                    //break;
                default:
                    return "white";
                    //break;
            }

        }

        //0和2是按钮，1和3是图表本身
        private void ChangeVanillaSkillChartTo(int val)
        {
            if (val == 0)
            {
                vanillaSkillBtns[0].transform.GetChild(0).gameObject.SetActive(true);
                vanillaSkillBtns[1].SetActive(true);
                vanillaSkillBtns[2].transform.GetChild(0).gameObject.SetActive(false);
                vanillaSkillBtns[3].SetActive(false);
            }
            else
            {
                vanillaSkillBtns[0].transform.GetChild(0).gameObject.SetActive(false);
                vanillaSkillBtns[1].SetActive(false);
                vanillaSkillBtns[2].transform.GetChild(0).gameObject.SetActive(true);
                vanillaSkillBtns[3].SetActive(true);
            }
        }

        void DrawDebugPanel()
        {
            if (Prefs.DevMode == false)
            {
                GameObject.Find("DebugToolPanel").SetActive(false);
            }
            else
            {
                GameObject.Find("_DPlus").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localScale;
                    loc.localScale = new Vector3(v3.z + 0.1f, v3.z + 0.1f, v3.z + 0.1f);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DMinus").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localScale;
                    loc.localScale = new Vector3(v3.z - 0.1f, v3.z - 0.1f, v3.z - 0.1f);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DUP").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x, v3.y + 10f, v3.z);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DDown").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x, v3.y - 10f, v3.z);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DLeft").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x - 10f, v3.y, v3.z);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DRight").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x + 10f, v3.y, v3.z);
                    Log.Message($"MIS. {Def.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
            }
        }

        #endregion

        #region 右边界面

        //确认招募和取消也是导航键
        private void DrawNavBtn()
        {
            GameObject navBtn;
            //Home
            navBtn = GameObject.Find("BtnHome");
            navBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                this.Close(false);
                RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu, purpose : OpDetailType.Recruit/* RIWindowType.MainMenu*/);
            });
            //取消
            navBtn = GameObject.Find("BtnCancel");
            navBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                this.ReturnToParent(false);
            });
            //确认招募
            navBtn = GameObject.Find("BtnConfirm");
            Button button = navBtn.GetComponentInChildren<Button>();
            if (windowPurpose == OpDetailType.Recruit)
            {
                GameObject.Find("TexChangeSec").SetActive(false);
                //FIXME: 更换贴图
                button.onClick.AddListener(delegate ()
                {
                    //如果招募曾经招过的干员
                    if (doc != null && !doc.currentExist)
                    {
                    }
                    //如果干员未招募过，或已死亡
                    if (canRecruit)
                    {
                        this.Close(true);
                        RecruitConsole.TryGetComp<CompRefuelable>().ConsumeFuel(Def.ticketCost);
                        Def.Recruit(RecruitConsole.Map);
                        if (RIWindowHandler.continuousRecruit) //连续招募
                        {
                            RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList /*RIWindowType.Op_List*/);
                        }
                        /*RIWindow_OperatorList window = new RIWindow_OperatorList(new DiaNode(new TaggedString()), true);
                        window.soundAmbient = SoundDefOf.RadioComms_Ambience;
                        Find.WindowStack.Add(window);*/
                    }
                });
            }
            // 更换助理按钮
            else if (windowPurpose == OpDetailType.Secretary)
            {
                //fixme
                button.onClick.AddListener(delegate ()
                {
                    windowPurpose = OpDetailType.Recruit;
                    AK_ModSettings.secretary = AK_Tool.GetOperatorIDFrom(Def.defName);
                    AK_ModSettings.secretarySkin = preferredSkin;
                    if (preferredSkin < 1000)
                    {
                        AK_ModSettings.secretaryLoc = TypeDef.defaultSecLoc;
                    }
                    else AK_ModSettings.secretaryLoc = TypeDef.defaultSecLocLive;

                    //手动保存设置
                    //AK_ModSettings settings = LoadedModManager.GetMod<AK_Mod>().settings;
                    AK_Mod.settings.Write();

                    this.Close(false);
                    RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu /*RIWindowType.MainMenu*/);
                });
            }
            else if (windowPurpose == OpDetailType.Fashion)
            {
                
            }
        }

        //FIXME:切换技能逻辑不对 需要大修
        private void DrawOperatorAbility()
        {
            int skillCnt = Def.AKAbilities.Count;
            if (skillCnt == 0) return;
            else if (skillCnt >= 10)
            {
                Log.Error("目前不支持单个干员有十个及以上技能");
                return;
            }
            Transform opAbilityPanel = GameObject.Find("OpAbilityPanel").transform;
            GameObject opAbilityPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("OpAbilityIcon");
            GameObject opAbilityInstance;
            opSkills = new List<GameObject>();
            int logicOrder = 0; //在技能组内，实际的顺序
            for (int i = 0; i < skillCnt; ++i)
            {
                AKAbilityDef opAbilty = Def.AKAbilities[i];
                opAbilityInstance = GameObject.Instantiate(opAbilityPrefab, opAbilityPanel);
                Texture2D icon = opAbilty.Icon;
                opAbilityInstance.GetComponent<Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector3.zero);

                opAbilityInstance.name = $"FSUI_OpAb_{i}_{logicOrder}";

                if (!opAbilty.grouped)
                {
                    //右下角的勾 常驻技能橙色。
                    opAbilityInstance.transform.GetChild(1).GetComponent<Image>().sprite = AK_Tool.FSAsset.LoadAsset<Sprite>("InnateAb");
                }
                //可选技能
                else
                {
                    opSkills.Add(opAbilityInstance);
                    logicOrder++;
                    opAbilityInstance.transform.GetChild(1).gameObject.SetActive(false);
                    opAbilityInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        SwitchGroupedSkillTo(btnOrder(ClickedBtn));
                    });
                }

                InitializeEventTrigger(opAbilityInstance.GetComponentInChildren<EventTrigger>(), Def.AKAbilities[i].description.Translate());

                opAbilityInstance.SetActive(true);
            }
        }

        private void SwitchGroupedSkillTo(int val)
        {
            if (AK_ModSettings.debugOverride) Log.Message($"try switch skills to {val}");
            if (doc == null || doc.currentExist == false || Def.AKAbilities.Count == 0) return;
            opSkills[PreferredAbility].transform.GetChild(1).gameObject.SetActive(false);
            PreferredAbility = val;
            opSkills[PreferredAbility].transform.GetChild(1).gameObject.SetActive(true);
        }

        private void DrawTrait()
        {
            if (Def.traits == null || Def.traits.Count == 0) return;
            GameObject traitPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("TraitTemplate");
            GameObject traitInstance;
            Transform traitPanel = GameObject.Find("ActualTraitsPanel").transform;

            for (int i = 0; i < Def.traits.Count; ++i)
            {
                traitInstance = GameObject.Instantiate(traitPrefab, traitPanel);
                TraitDegreeData traitDef = Def.traits[i].def.DataAtDegree(Def.traits[i].degree);
                traitInstance.GetComponentInChildren<TextMeshProUGUI>().text = traitDef.label.Translate();
                traitInstance.name = "FSUI_Traits_" + i;
                InitializeEventTrigger(traitInstance.GetComponent<EventTrigger>(), AK_Tool.DescriptionManualResolve(Def.traits[i].def.DataAtDegree(Def.traits[i].degree).description, Def.nickname, Def.isMale ? Gender.Male : Gender.Female));
            }
        }
        #endregion
        private void DrawStand()
        {
            //禁用掉之前的立绘
            OpStand.SetActive(false);
            OpL2D.SetActive(false);
            if (L2DInstance != null) L2DInstance.SetActive(false);

            //静态立绘
            if (preferredSkin < 1000)
            {
                OpStand.SetActive(true);
                AK_Tool.DrawStaticOperatorStand(Def, preferredSkin, OpStand);
            }
            //l2d
            else
            {
                OpL2D.SetActive(true);
                FS_Tool.SetDefaultCanvas(false);
                L2DInstance = FS_Utilities.DrawModel(DisplayModelAt.RIWDetail, Def.live2dModel[preferredSkin - 1000], OpL2D);
            }
        }


        private void ChangeStandTo(int val, bool forceChange = false, StandType standType = StandType.Static)
        {
            GameObject fBtn;
            if (!forceChange && val == preferredSkin) return;

            if (doc != null)
            {
                if (val < 1000)
                {
                    doc.preferedSkin = val;
                }
            }
            //禁用之前的换装按钮
            fBtn = fashionBtns[preferredSkin];
            fBtn.transform.GetChild(0).gameObject.SetActive(true);
            fBtn.transform.GetChild(1).gameObject.SetActive(false);

            //启用新的换装按钮
            preferredSkin = val;
            fBtn = fashionBtns[preferredSkin];
            fBtn.transform.GetChild(0).gameObject.SetActive(false);
            fBtn.transform.GetChild(1).gameObject.SetActive(true);
            DrawStand(); //实际刷新立绘立绘
        }


        //对于不存在于界面预制体内的obj，不能在unity里面设定关掉悬浮窗。
        void InitializeEventTrigger(EventTrigger ev, string text)
        {
            EventTrigger.Entry entry = ev.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);

            entry.callback.AddListener((data) =>
            {
                DrawFloatingBubble(text);
            });

            entry = ev.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);


            entry.callback.AddListener((data) =>
            {
                floatingBubbleInstance.SetActive(false);
            });

        }

        //鼠标指上去的悬浮窗 
        void DrawFloatingBubble(string text)
        {
            floatingBubbleInstance.GetComponentInChildren<TextMeshProUGUI>().text = text;
            Vector3 mousepos = Input.mousePosition;
            Vector2 pivot;
            if (mousepos.x < Screen.currentResolution.width / 2) pivot = new Vector2(0, 1);
            else pivot = new Vector2(1, 1);
            ((RectTransform)floatingBubbleInstance.transform).pivot = pivot;
            floatingBubbleInstance.transform.position = Input.mousePosition;

            //自动计算大小
            floatingBubbleInstance.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)floatingBubbleInstance.transform);
        }

        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList/* RIWindowType.Op_List*/);
            base.ReturnToParent(closeEV);
        }

        #endregion
    }
}