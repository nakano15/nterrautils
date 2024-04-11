using System;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace nterrautils
{
    public class ModularQuestBase : QuestBase
    {
        public override QuestData GetQuestData => new ModularQuestData();
        private List<ModularQuestStep> QuestSteps = new List<ModularQuestStep>();
        public int CoinsReward = 0;
        public List<Item> ItemRewards = new List<Item>();
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

        public void SetCoinReward(int Platinum = 0, int Gold = 0, int Silver = 0, int Copper = 0)
        {
            CoinsReward = Copper + Silver * 100 + Gold * 10000 + Platinum * 1000000;
        }

        public void AddItemReward(int ItemID, int Stack = 1, int Prefix = 0)
        {
            ItemRewards.Add(new Item(ItemID, Stack, Prefix));
        }

        public override string QuestStory(QuestData data)
        {
            ModularQuestData d = (ModularQuestData)data;
            string Text = "";
            if (StoryStartLore != "")
            {
                Text = StoryStartLore + "\n\n";
            }
            for (int step = 0; step < d.Step; step++)
            {
                if (Text.Length > 0) Text += "\n\n";
                Text += QuestSteps[step].GetStepStoryText(true);
            }
            if (d.Step >= QuestSteps.Count)
            {
                if (StoryEndLore != "")
                {
                    Text = "\n\n" + StoryEndLore;
                }
                Text += "\n\nTHE END";
                if (CoinsReward > 0 || ItemRewards.Count > 0)
                    Text += "\n\nReceived as reward: ";
                if (CoinsReward > 0)
                {
                    GetCoinReward(CoinsReward, out int p, out int g, out int s, out int c);
                    Text += "\n* ";
                    bool First = true;
                    if (p > 0)
                    {
                        Text += p + " Platinum";
                        First = false;
                    }
                    if (g > 0)
                    {
                        if (!First)
                            Text += ", ";
                        Text += g + " Gold";
                        First = false;
                    }
                    if (s > 0)
                    {
                        if (!First)
                            Text += ", ";
                        Text += s + " Silver";
                        First = false;
                    }
                    if (c > 0)
                    {
                        if (!First)
                            Text += ", ";
                        Text += c + " Copper";
                        First = false;
                    }
                    Text += " coins.";
                }
                foreach (Item i in ItemRewards)
                {
                    Text += "\n* " + i.HoverName;
                }
            }
            else
            {
                if (Text.Length > 0) Text += "\n\n";
                Text += QuestSteps[d.Step].GetStepStoryText(false);
                Text += "\n";
                for(int i = 0; i < QuestSteps[d.Step].Objectives.Count; i++)
                {
                    Text += "\n* " + QuestSteps[d.Step].Objectives[i].ObjectiveText(d.StepDatas[d.Step].ObjectiveDatas[i]);
                }
            }
            return Text;
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
            data.Step++;
            if (data.Step == QuestSteps.Count)
            {
                data.ShowQuestCompletedNotification();
                GetCoinReward(CoinsReward, out int p, out int g, out int s, out int c);
                Vector2 SpawnPos = player.Center;
                if (p > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.PlatinumCoin, p, noGrabDelay: true);
                if (g > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.GoldCoin, g, noGrabDelay: true);
                if (s > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.SilverCoin, s, noGrabDelay: true);
                if (c > 0) Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, Terraria.ID.ItemID.CopperCoin, c, noGrabDelay: true);
                foreach (Item i in ItemRewards)
                {
                    Item.NewItem(Item.GetSource_None(), SpawnPos, Vector2.Zero, i.type, i.stack, true, i.prefix, true);
                }
            }
            else
            {
                QuestSteps[data.Step].OnQuestStepStart(player, Data);
            }
        }

        void GetCoinReward(int Value, out int p, out int g, out int s, out int c)
        {
            p = 0;
            g = 0;
            s = 0;
            c = CoinsReward;
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