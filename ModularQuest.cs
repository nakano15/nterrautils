using System;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using nterrautils.QuestObjectives;

namespace nterrautils
{
    public class ModularQuestBase : QuestBase
    {
        public override QuestData GetQuestData => new ModularQuestData();
        private List<ModularQuestStep> QuestSteps = new List<ModularQuestStep>();
        public virtual string StoryStartLore => "";
        public virtual string StoryEndLore => "";

        ModularQuestStep GetCurrentStep(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            if (d.Step < QuestSteps.Count)
                return QuestSteps[d.Step];
            return null;
        }

        ModularQuestStepData GetCurrentStepData(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            if (d.Step < QuestSteps.Count)
                return d.StepDatas[d.Step];
            return null;
        }

        ModularQuestStep GetLatestStep(bool CreateLastStepIfDoesntExist = true)
        {
            if (QuestSteps.Count == 0)
            {
                if (CreateLastStepIfDoesntExist)
                    AddNewQuestStep();
                else
                    return null;
            }
            return QuestSteps[QuestSteps.Count - 1];
        }

        public override string QuestStory(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            string Text = "";
            if (StoryStartLore != "")
            {
                Text = StoryStartLore;
            }
            for (int step = 0; step < d.Step; step++)
            {
                string StepText = QuestSteps[step].GetStepStoryText(d.Step > step);
                if (StepText.Length == 0) continue;
                if (Text.Length > 0) Text += "\n\n";
                Text += StepText;
            }
            if (d.Step >= QuestSteps.Count)
            {
                if (StoryEndLore != "")
                {
                    Text += "\n\n" + StoryEndLore;
                }
                Text += "\n\n" + GetTranslation("TheEnd");
                Dictionary<int, int> ItemRewards = new Dictionary<int, int>();
                int CoinsReward = 0;
                foreach (ModularQuestStep s in QuestSteps)
                {
                    CoinsReward += s.CoinsReward;
                    foreach (Item i in s.ItemRewards)
                    {
                        if (ItemRewards.ContainsKey(i.type))
                        {
                            ItemRewards[i.type] += i.stack;
                        }
                        else
                        {
                            ItemRewards.Add(i.type, i.stack);
                        }
                    }
                }
                if (CoinsReward > 0 || ItemRewards.Count > 0)
                    Text += "\n\n" + GetTranslation("ReceiveReward");
                if (CoinsReward > 0)
                {
                    GetCoinReward(CoinsReward, out int p, out int g, out int s, out int c);
                    Text += "\n";
                    string CoinsString = "";
                    bool First = true;
                    if (p > 0)
                    {
                        CoinsString += GetTranslation("Platinum").Replace("{count}", p.ToString());
                        First = false;
                    }
                    if (g > 0)
                    {
                        if (!First)
                            CoinsString += ", ";
                        CoinsString += GetTranslation("Gold").Replace("{count}", g.ToString());
                        First = false;
                    }
                    if (s > 0)
                    {
                        if (!First)
                            CoinsString += ", ";
                        CoinsString += GetTranslation("Silver").Replace("{count}", s.ToString());
                        First = false;
                    }
                    if (c > 0)
                    {
                        if (!First)
                            CoinsString += ", ";
                        CoinsString += GetTranslation("Copper").Replace("{count}", c.ToString());
                        First = false;
                    }
                    Text += GetTranslation("coins").Replace("{coins}", CoinsString);

                }
                Item item = new Item();
                foreach (int type in ItemRewards.Keys)
                {
                    item.SetDefaults(type);
                    item.Prefix(0);
                    item.stack = ItemRewards[type];
                    Text += "\n" + GetTranslation("Item").Replace("{item}", item.HoverName);
                }
                item = null;
                ItemRewards.Clear();
            }
            else
            {
                if (Text.Length > 0) Text += "\n\n";
                Text += QuestSteps[d.Step].GetStepStoryText(false);
                Text += "\n";
                for(int i = 0; i < QuestSteps[d.Step].Objectives.Count; i++)
                {
                    Text += "\n" + GetTranslation("ObjectiveLine").Replace("{objective}", QuestSteps[d.Step].Objectives[i].ObjectiveText(d.StepDatas[d.Step].ObjectiveDatas[i]));
                }
            }
            return Text;
        }
        
        public string GetTranslation(string Key, string ModID = "")
        {
            if (ModID == "") ModID = "nterrautils";
            return Terraria.Localization.Language.GetTextValue("Mods." + ModID + ".Quest."+Key);
        }

        public override string GetQuestCurrentObjective(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            if (d.Step < d.StepDatas.Length)
            {
                ModularQuestStepData qstepd = d.StepDatas[d.Step];
                for(int o = 0; o < qstepd.ObjectiveDatas.Length; o++)
                {
                    if (!QuestSteps[d.Step].Objectives[o].IsCompleted(qstepd.ObjectiveDatas[o]))
                    {
                        return QuestSteps[d.Step].Objectives[o].ObjectiveText(qstepd.ObjectiveDatas[o]);
                    }
                }
            }
            return base.GetQuestCurrentObjective(data);
        }

        public override bool IsQuestActive(QuestData data)
        {
            return base.IsQuestActive(data);
        }

        public override bool IsQuestCompleted(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            return d.Step >= QuestSteps.Count;
        }

        public override void OnMobKill(NPC killedNpc, QuestData data)
        {
            ModularQuestStepData Data = GetCurrentStepData(data);
            ModularQuestStep Base = GetCurrentStep(data);
            if (Data == null || Base == null) return;
            Base.OnMobKill(killedNpc, Data);
        }

        public override void UpdatePlayer(Player player, QuestData data)
        {
            ModularQuestStepData Data = GetCurrentStepData(data);
            ModularQuestStep Base = GetCurrentStep(data);
            if (Data == null || Base == null) return;
            Base.UpdatePlayer(player, Data);
            bool AnyIncompleteObjective = false;
            for(int o = 0; o < Base.Objectives.Count; o++)
            {
                //Base.Objectives[o].UpdatePlayer(player, Data.ObjectiveDatas[o]);
                if (!Base.Objectives[o].IsCompleted(Data.ObjectiveDatas[o]))
                {
                    AnyIncompleteObjective = true;
                }
            }
            if (!AnyIncompleteObjective)
            {
                ChangeStep(player, Base, Data, data);
            }
        }

        void ChangeStep(Player player, ModularQuestStep Base, ModularQuestStepData Data, QuestData qdata)
        {
            Base.OnStepChange(player, Data);
            ModularQuestData data = qdata as ModularQuestData;
            QuestSteps[data.Step].OnQuestStepEnd(player, Data);
            GetRewards(player, QuestSteps[data.Step].CoinsReward, QuestSteps[data.Step].ItemRewards, QuestSteps[data.Step].ExpReward);
            data.Step++;
            if (data.Step == QuestSteps.Count)
            {
                data.ShowQuestCompletedNotification();
            }
            else
            {
                QuestSteps[data.Step].OnQuestStepStart(player, Data);
            }
        }

        void GetRewards(Player player, int Coins, List<Item> Rewards, ExpRewardValue ExpReward)
        {
            GetCoinReward(Coins, out int p, out int g, out int s, out int c);
            Vector2 SpawnPos = player.Center;
            if (p > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.PlatinumCoin, p, noGrabDelay: true);
            if (g > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.GoldCoin, g, noGrabDelay: true);
            if (s > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.SilverCoin, s, noGrabDelay: true);
            if (c > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.CopperCoin, c, noGrabDelay: true);
            foreach (Item i in Rewards)
            {
                Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, i.type, i.stack, true, i.prefix, true);
            }
            nterrautils.MainMod.TriggerExpRewardHooks(player, ExpReward.Level, ExpReward.Percentage);
        }

        static void GetCoinReward(int Value, out int p, out int g, out int s, out int c)
        {
            p = 0;
            g = 0;
            s = 0;
            c = Value;
            if (c >= 100)
            {
                s += c / 100;
                c -= s * 100;
            }
            if (s >= 100)
            {
                g += s / 100;
                s -= g * 100;
            }
            if (g >= 100)
            {
                p += g / 100;
                g -= p * 100;
            }
        }

        public override void OnTalkToNpc(NPC npc, QuestData data)
        {
            ModularQuestStepData Data = GetCurrentStepData(data);
            ModularQuestStep Base = GetCurrentStep(data);
            if (Data == null || Base == null) return;
            Base.OnTalkToNpc(npc, Data);
        }

        public override string QuestNpcDialogue(NPC npc, QuestData data, out bool BlockOtherMessages)
        {
            BlockOtherMessages = false;
            ModularQuestStepData Data = GetCurrentStepData(data);
            ModularQuestStep Base = GetCurrentStep(data);
            if (Data == null || Base == null) return "";
            return Base.QuestNpcDialogue(npc, Data, out BlockOtherMessages);
        }

        protected ModularQuestStep AddNewQuestStep()
        {
            ModularQuestStep NewStep = new ModularQuestStep();
            QuestSteps.Add(NewStep);
            return NewStep;
        }

        protected void AddCustomQuestStep(ModularQuestStep CustomStep)
        {
            QuestSteps.Add(CustomStep);
        }

        protected void AddHuntObjective(int MonsterID, int KillCount = 5, string MonsterName = "")
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new HuntObjective(MonsterID, KillCount, MonsterName));
            }
        }

        protected void AddHuntObjective(int[] MonsterIDs, int KillCount = 5, string MonsterName = "")
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new HuntObjective(MonsterIDs, KillCount, MonsterName));
            }
        }

        protected void AddItemCollectionObjective(int ItemID, int Stack = 5, bool TakeItems = true)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new ItemCollectionObjective(ItemID, Stack, TakeItems));
            }
        }

        protected void AddTalkObjective(int NpcID, string Message)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new TalkObjective(NpcID, Message));
            }
        }

        protected void AddNpcMoveInObjective(int NpcID)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new NpcMoveInObjective(NpcID));
            }
        }

        protected void AddObjectCollectionObjective(string ObjectName, int NpcID, float DropRate = 50f, int Stack = 5, string CustomName = null)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new ObjectCollectionObjective(ObjectName, NpcID, DropRate, Stack, CustomName));
            }
        }

        protected void AddObjectCollectionObjective(string ObjectName, int[] NpcIDs, string NpcGroupName, float DropRate = 50f, int Stack = 5)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new ObjectCollectionObjective(ObjectName, NpcIDs, NpcGroupName, DropRate, Stack));
            }
        }

        protected void AddMaxHealthObjective(int MaxHealth)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new MaxHealthObjective(MaxHealth));
            }
        }

        protected void AddMaxManaObjective(int MaxMana)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.AddNewObjective(new MaxManaObjective(MaxMana));
            }
        }
        
        public void SetStepCoinReward(int Platinum = 0, int Gold = 0, int Silver = 0, int Copper = 0)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.CoinsReward = Copper + Silver * 100 + Gold * 10000 + Platinum * 1000000;
            }
        }

        public void AddStepItemReward(int ItemID, int Stack = 1, int Prefix = 0)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.ItemRewards.Add(new Item(ItemID, Stack, Prefix));
            }
        }

        public void SetStepExpReward(int Level, float Percentage)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.ExpReward = new ExpRewardValue(Level, Percentage);
            }
        }

        public void SetQuestStepStoryText(string BeforeCompletion, string AfterCompletion)
        {
            ModularQuestStep LatestStep = GetLatestStep();
            if (LatestStep != null)
            {
                LatestStep.ChangeStoryText(BeforeCompletion, AfterCompletion);
            }
        }

        public class ModularQuestStep
        {
            public List<ObjectiveBase> Objectives = new List<ObjectiveBase>();
            public string StepIncompletedText = "", StepCompletedText = "";
            public virtual string GetStepStoryText(bool Completed)
            {
                if (Completed) return StepCompletedText;
                return StepIncompletedText;
            }
            public Action<Player, ModularQuestStepData> OnQuestStepStart = delegate(Player player, ModularQuestStepData Data){ };
            public Action<Player, ModularQuestStepData> OnQuestStepEnd = delegate(Player player, ModularQuestStepData Data){ };
            public int CoinsReward = 0;
            public List<Item> ItemRewards = new List<Item>();
            public ExpRewardValue ExpReward = new ExpRewardValue();
            
            public void SetCoinReward(int Platinum = 0, int Gold = 0, int Silver = 0, int Copper = 0)
            {
                CoinsReward = Copper + Silver * 100 + Gold * 10000 + Platinum * 1000000;
            }

            public void AddItemReward(int ItemID, int Stack = 1, int Prefix = 0)
            {
                ItemRewards.Add(new Item(ItemID, Stack, Prefix));
            }

            public void SetExpReward(int Level, float Percentage)
            {
                ExpReward = new ExpRewardValue(Level, Percentage);
            }

            public void ChangeStoryText(string BeforeCompletting, string AfterCompletting)
            {
                StepIncompletedText = BeforeCompletting;
                StepCompletedText = AfterCompletting;
            }

            public bool IsCompleted(ModularQuestStepData Data)
            {
                for (int i = 0; i < Objectives.Count; i++)
                {
                    ObjectiveBase objective = Objectives[i];
                    if (!objective.IsCompleted(Data.ObjectiveDatas[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            
            public virtual bool IsCompleted(ObjectiveData Data)
            {
                return true;
            }

            public virtual string ObjectiveText(ModularQuestStepData Data)
            {
                return "";
            }

            public virtual void UpdatePlayer(Player player, ModularQuestStepData data)
            {
                for(int o = 0; o < Objectives.Count; o++)
                {
                    Objectives[o].UpdatePlayer(player, data.ObjectiveDatas[o]);
                }
            }

            public virtual void OnMobKill(NPC killedNpc, ModularQuestStepData data)
            {
                for(int o = 0; o < Objectives.Count; o++)
                {
                    Objectives[o].OnMobKill(killedNpc, data.ObjectiveDatas[o]);
                }
            }

            public virtual void OnTalkToNpc(NPC npc, ModularQuestStepData data)
            {
                for(int o = 0; o < Objectives.Count; o++)
                {
                    Objectives[o].OnTalkToNpc(npc, data.ObjectiveDatas[o]);
                }
            }

            public virtual void OnStepChange(Player player, ModularQuestStepData data)
            {
                for(int o = 0; o < Objectives.Count; o++)
                {
                    Objectives[o].OnStepChange(player, data.ObjectiveDatas[o]);
                }
            }

            public virtual string QuestNpcDialogue(NPC npc, ModularQuestStepData data, out bool BlockOtherMessages)
            {
                BlockOtherMessages = false;
                for(int o = 0; o < Objectives.Count; o++)
                {
                    string s = Objectives[o].QuestNpcDialogue(npc, data.ObjectiveDatas[o], out BlockOtherMessages);
                    if (s != "") return s;
                }
                return "";
            }

            public void AddNewObjective(ObjectiveBase NewObjective)
            {
                Objectives.Add(NewObjective);
            }
        }

        public class ModularQuestStepData
        {
            public ObjectiveData[] ObjectiveDatas = new ObjectiveData[0];
        }

        public class ModularQuestData : QuestData
        {
            public int Step = 0;
            public ModularQuestStepData[] StepDatas = new ModularQuestStepData[0];
            const short InternalVersion = 0;

            protected override void OnInitialize(QuestBase Quest)
            {
                ModularQuestBase Base = Quest as ModularQuestBase;
                StepDatas = new ModularQuestStepData[Base.QuestSteps.Count];
                for (int s = 0; s < Base.QuestSteps.Count; s++)
                {
                    StepDatas[s] = new ModularQuestStepData();
                    ModularQuestStep step = Base.QuestSteps[s];
                    StepDatas[s].ObjectiveDatas = new ObjectiveData[Base.QuestSteps[s].Objectives.Count];
                    for (int o = 0; o < Base.QuestSteps[s].Objectives.Count; o++)
                    {
                        StepDatas[s].ObjectiveDatas[o] = Base.QuestSteps[s].Objectives[o].GetObjectiveData;
                    }
                }
            }

            protected override void Save(TagCompound save, string QuestID)
            {
                for (int d = 0; d < StepDatas.Length; d++)
                {
                    ModularQuestStepData data = StepDatas[d];
                    string StepQuestID = QuestID + "_s" + d;
                    save.Add("Step" + StepQuestID, Step);
                    for (int o = 0; o < data.ObjectiveDatas.Length; o++)
                    {
                        ObjectiveData obdata = data.ObjectiveDatas[o];
                        string QuestObjectiveID = StepQuestID + "_o" + o;
                        save.Add("ObjectiveType" + QuestObjectiveID, obdata.GetType().Name);
                        obdata.Save(save, QuestObjectiveID);
                    }
                }
            }

            protected override void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                for (int d = 0; d < StepDatas.Length; d++)
                {
                    ModularQuestStepData data = StepDatas[d];
                    string StepQuestID = QuestID + "_s" + d;
                    Step = load.GetInt("Step" + StepQuestID);
                    for (int o = 0; o < data.ObjectiveDatas.Length; o++)
                    {
                        ObjectiveData obdata = data.ObjectiveDatas[o];
                        string QuestObjectiveID = StepQuestID + "_o" + o;
                        string ObjectName = load.GetString("ObjectiveType" + QuestObjectiveID);
                        if (ObjectName != obdata.GetType().Name)
                            obdata.Load(load, QuestObjectiveID, LastVersion);
                    }
                }
            }
        }

        public class ObjectiveBase
        {
            public virtual ObjectiveData GetObjectiveData => new ObjectiveData();

            public string GetTranslation(string Key, string ModID = "")
            {
                if (ModID == "") ModID = "nterrautils";
                return Terraria.Localization.Language.GetTextValue("Mods." + ModID + ".Quest.Objective."+Key);
            }
            
            public virtual bool IsCompleted(ObjectiveData Data)
            {
                return true;
            }

            public virtual string ObjectiveText(ObjectiveData Data)
            {
                return "";
            }

            public virtual void UpdatePlayer(Player player, ObjectiveData data)
            {

            }

            public virtual void OnMobKill(NPC killedNpc, ObjectiveData data)
            {

            }

            public virtual void OnTalkToNpc(NPC npc, ObjectiveData data)
            {

            }

            public virtual void OnStepChange(Player player, ObjectiveData data)
            {

            }

            public virtual string QuestNpcDialogue(NPC npc, ObjectiveData data, out bool BlockOtherMessages)
            {
                BlockOtherMessages = false;
                return "";
            }
        }

        public class ObjectiveData
        {
            public virtual void Save(TagCompound save, string QuestID)
            {
                
            }

            public virtual void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                
            }
        }
    }
}