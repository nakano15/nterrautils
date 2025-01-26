using System;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace nterrautils.QuestObjectives
{
    public class HuntObjective : ModularQuestBase.ObjectiveBase
    {
        public List<int> MonsterIDs = new List<int>();
        public int KillCount = 5;
        public string MonsterName = "";
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new HuntObjectiveData();

        public HuntObjective(int MonsterID, int KillCount = 5, string MonsterName = "")
        {
            MonsterIDs.Add(MonsterID);
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

        public HuntObjective(int[] MonsterIDs, int KillCount = 5, string MonsterName = "")
        {
            this.MonsterIDs.AddRange(MonsterIDs);
            this.KillCount = KillCount;
            if (this.MonsterName == "")
            {
                NPC n = new NPC();
                n.SetDefaults(MonsterIDs[0]);
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
            if (MonsterIDs.Contains(killedNpc.type) && d.Kills < KillCount)
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

    public class NpcMoveInObjective : ModularQuestBase.ObjectiveBase
    {
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new NpcMoveInData();
        public int NpcID = 0;
        public string NpcDefaultName = "";

        public NpcMoveInObjective(int NpcID)
        {
            this.NpcID = NpcID;
            NPC n = new NPC();
            n.SetDefaults(NpcID);
            NpcDefaultName = n.TypeName;
            n = null;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            NpcMoveInData d = Data as NpcMoveInData;
            string name = NPC.GetFirstNPCNameOrNull(NpcID);
            if (name == null) name = NpcDefaultName;
            if (!d.MovedIn)
                return "Have " + name + " move in to a house.";
            else
                return name + " moved in to a house.";
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            int index = NPC.FindFirstNPC(NpcID);
            return index > -1 && !Main.npc[index].homeless;
        }

        public class NpcMoveInData : ModularQuestBase.ObjectiveData
        {
            public bool MovedIn = false;
            const ushort Version = 0;

            public override void Save(TagCompound save, string QuestID)
            {
                save.Add("Version" + QuestID, Version);
                save.Add("MovedIn" + QuestID, MovedIn);
            }

            public override void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                int Version = load.Get<ushort>("Version" + QuestID);
                MovedIn = load.GetBool("MovedIn" + QuestID);
            }
        }
    }

    public class ObjectCollectionObjective : ModularQuestBase.ObjectiveBase
    {
        public int Stack = 5;
        public string ObjectName = "";
        public float DropRate = 100f;
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new ObjectCollectionData();
        public bool TakeItems = true;
        public List<int> NpcIDs = new List<int>();
        public string NpcName = "";

        public ObjectCollectionObjective(string ObjectName, int NpcID, float DropRate = 50f, int Stack = 5, string CustomName = null)
        {
            this.ObjectName = ObjectName;
            this.Stack = Stack;
            this.DropRate = DropRate;
            NpcIDs.Add(NpcID);
            if (CustomName != null)
                NpcName = CustomName;
            else
            {
                NPC n = new NPC();
                n.SetDefaults(NpcID);
                NpcName = n.GivenOrTypeName;
                n = null;
            }
        }

        public ObjectCollectionObjective(string ObjectName, int[] NpcIDs, string NpcGroupName, float DropRate = 50f, int Stack = 5)
        {
            this.ObjectName = ObjectName;
            this.Stack = Stack;
            this.DropRate = DropRate;
            this.NpcIDs.AddRange(NpcIDs);
            NpcName = NpcGroupName;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            ObjectCollectionData data = Data as ObjectCollectionData;
            if (data.ItemsContained < Stack)
            {
                int Count = Stack - data.ItemsContained;
                return "Collect " + Count + " " + ObjectName + " from "+NpcName+"("+DropRate+"%).";
            }
            else
            {
                return "Collected " + Stack + " " + ObjectName + ".";
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            ObjectCollectionData data = Data as ObjectCollectionData;
            return data.ItemsContained >= Stack;
        }

        public override void OnMobKill(NPC killedNpc, ModularQuestBase.ObjectiveData data)
        {
            if (NpcIDs.Contains(killedNpc.type) && Main.rand.NextFloat(0f, 100f) < DropRate)
            {
                CombatText.NewText(killedNpc.getRect(), Color.Gray, "Found a " + ObjectName + "!", true);
                (data as ObjectCollectionData).ItemsContained++;
            }
        }

        public class ObjectCollectionData : ModularQuestBase.ObjectiveData
        {
            public int ItemsContained = 0;
        }
    }
    
}