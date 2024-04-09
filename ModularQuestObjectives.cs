using System;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace nterrautils.QuestObjectives
{
    public class HuntObjective : ModularQuestBase.ObjectiveBase
    {
        public int MonsterID = 0;
        public int KillCount = 5;
        public string MonsterName = "";
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new HuntObjectiveData();

        public HuntObjective(int MonsterID, int KillCount = 5, string MonsterName = "")
        {
            this.MonsterID = MonsterID;
            this.KillCount = KillCount;
            if (this.MonsterName == "")
            {
                NPC n = new NPC();
                n.SetDefaults(MonsterID);
                this.MonsterName = n.GivenOrTypeName;
            }
            else
            {
                this.MonsterName = MonsterName;
            }
        }

        public override void OnMobKill(NPC killedNpc, ModularQuestBase.ObjectiveData Data)
        {
            HuntObjectiveData d = Data as HuntObjectiveData;
            if (killedNpc.type == MonsterID && d.Kills < KillCount)
            {
                d.Kills++;
                if (d.Kills == KillCount)
                {
                    Main.NewText("Killed all the " + MonsterName + " necessary.");
                }
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            HuntObjectiveData d = Data as HuntObjectiveData;
            return d.Kills >= KillCount;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            HuntObjectiveData d = Data as HuntObjectiveData;
            if (d.Kills < KillCount)
            {
                int Count = KillCount - d.Kills;
                return "Kill " + Count + " " + MonsterName + ".";
            }
            else
            {
                return "Killed all the " + KillCount + " " + MonsterName + ".";
            }
        }

        public class HuntObjectiveData : ModularQuestBase.ObjectiveData
        {
            public int Kills = 0;
            const ushort InternalVersion = 0;

            public override void Save(TagCompound save, string QuestID)
            {
                save.Add("Version_" + QuestID, InternalVersion);
                save.Add("KillCount_" + QuestID, Kills);
            }

            public override void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                ushort ThisLastVersion = load.Get<ushort>("Version_" + QuestID);
                Kills = load.GetInt("KillCount_" + QuestID);
            }
        }
    }

    public class ItemCollectionObjective : ModularQuestBase.ObjectiveBase
    {
        public int ItemID = 0;
        public int Stack = 5;
        public string ItemName = "";
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new ItemCollectionData();
        public bool TakeItems = true;

        public ItemCollectionObjective(int ItemID, int Stack = 5, bool TakeItems = true)
        {
            this.ItemID = ItemID;
            this.Stack = Stack;
            ItemName = new Item(ItemID).Name;
            this.TakeItems = TakeItems;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            ItemCollectionData data = Data as ItemCollectionData;
            if (data.ItemsContained < Stack)
            {
                int Count = Stack - data.ItemsContained;
                return "Collect " + Count + " " + ItemName + ".";
            }
            else
            {
                return "Collected " + Stack + " " + ItemName + ".";
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            ItemCollectionData data = Data as ItemCollectionData;
            return data.ItemsContained >= Stack;
        }

        public override void UpdatePlayer(Player player, ModularQuestBase.ObjectiveData Data)
        {
            ItemCollectionData data = Data as ItemCollectionData;
            int s = 0;
            for (int i = 0; i < 50; i++)
            {
                if (player.inventory[i].type == ItemID)
                {
                    s += player.inventory[i].stack;
                }
            }
            data.ItemsContained = s;
        }

        public override void OnStepChange(Player player, ModularQuestBase.ObjectiveData data)
        {
            if (!TakeItems) return;
            int ToRemove = Stack;
            for (int i = 49; i >= 0; i--)
            {
                if (player.inventory[i].type == ItemID)
                {
                    int ToRemoveStack = player.inventory[i].stack;
                    if (ToRemoveStack > ToRemove)
                    {
                        ToRemoveStack = ToRemove;
                    }
                    player.inventory[i].stack -= ToRemoveStack;
                    if (player.inventory[i].stack <= 0)
                        player.inventory[i].TurnToAir();
                    ToRemove -= ToRemoveStack;
                    if (ToRemove <= 0) return;
                }
            }
        }

        public class ItemCollectionData : ModularQuestBase.ObjectiveData
        {
            public int ItemsContained = 0;
        }
    }
    
    public class TalkObjective : ModularQuestBase.ObjectiveBase
    {
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new TalkObjectiveData();
        public string Message = "", NpcDefaultName = "";
        public int NpcID = 0;

        public TalkObjective(int NpcID, string Message)
        {
            this.NpcID = NpcID;
            this.Message = Message;
            NPC n = new NPC();
            n.SetDefaults(NpcID);
            NpcDefaultName = n.TypeName;
            n = null;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            TalkObjectiveData d = Data as TalkObjectiveData;
            string name = NPC.GetFirstNPCNameOrNull(NpcID);
            if (name == null)
            {
                name = NpcDefaultName;
            }
            if (!d.TalkedTo)
            {
                return "Speak with " + name + ".";
            }
            else
            {
                return "Has spoken with " + name + ".";
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            TalkObjectiveData d = Data as TalkObjectiveData;
            return d.TalkedTo;
        }

        public override string QuestNpcDialogue(NPC npc, ModularQuestBase.ObjectiveData data, out bool BlockOtherMessages)
        {
            if (npc.type == NpcID)
            {
                TalkObjectiveData d = data as TalkObjectiveData;
                if (!d.TalkedTo)
                {
                    BlockOtherMessages = true;
                    d.TalkedTo = true;
                    return Message;
                }
            }
            return base.QuestNpcDialogue(npc, data, out BlockOtherMessages);
        }

        public class TalkObjectiveData : ModularQuestBase.ObjectiveData
        {
            public bool TalkedTo = false;
            const ushort Version = 0;

            public override void Save(TagCompound save, string QuestID)
            {
                save.Add("Version" + QuestID, Version);
                save.Add("TalkedTo"+QuestID, TalkedTo);
            }

            public override void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                int Version = load.Get<ushort>("Version" + QuestID);
                TalkedTo = load.GetBool("TalkedTo"+QuestID);
            }
        }
    }
}