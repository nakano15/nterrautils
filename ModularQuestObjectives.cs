using System;
using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using System.Linq;

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
            if (MonsterIDs.Contains(killedNpc.type) && d.Kills < KillCount && ExtraChecksCanCount(killedNpc))
            {
                d.Kills++;
                if (d.Kills == KillCount)
                {
                    Main.NewText(GetTranslation("AllMobsKilledNotice").Replace("{name}", MonsterName));
                }
            }
        }

        bool ExtraChecksCanCount(NPC killedNPC)
        {
            if (killedNPC.type == Terraria.ID.NPCID.EaterofWorldsHead || killedNPC.type == Terraria.ID.NPCID.EaterofWorldsTail)
            {
                if (NPC.AnyNPCs(Terraria.ID.NPCID.EaterofWorldsBody)) return false;
            }
            return true;
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
                return GetTranslation("KillMobCount").Replace("{count}", Count.ToString()).Replace("{name}", MonsterName);
            }
            else
            {
                return GetTranslation("KilledMobCount").Replace("{count}", KillCount.ToString()).Replace("{name}", MonsterName);
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
        public int[] ItemID = new int[]{0};
        public int Stack = 5;
        public string ItemName = "";
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new ItemCollectionData();
        public bool TakeItems = true;

        public ItemCollectionObjective(int ItemID, int Stack = 5, bool TakeItems = true)
        {
            this.ItemID = new int[]{ItemID};
            this.Stack = Stack;
            ItemName = new Item(ItemID).Name;
            this.TakeItems = TakeItems;
        }

        public ItemCollectionObjective(int[] ItemIDs, string ItemNames, int Stack = 5, bool TakeItems = true)
        {
            this.ItemID = ItemIDs;
            this.Stack = Stack;
            ItemName = ItemNames;
            this.TakeItems = TakeItems;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            ItemCollectionData data = Data as ItemCollectionData;
            if (data.ItemsContained < Stack)
            {
                int Count = Stack - data.ItemsContained;
                return GetTranslation("CollectItemCount").Replace("{count}", Count.ToString()).Replace("{name}", ItemName);
            }
            else
            {
                return GetTranslation("CollectedItemCount").Replace("{count}", Stack.ToString()).Replace("{name}", ItemName);
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
                if (ItemID.Contains(player.inventory[i].type))
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
                if (ItemID.Contains(player.inventory[i].type))
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
                return GetTranslation("TalkTo").Replace("{name}", name);
            }
            else
            {
                return GetTranslation("TalkedTo").Replace("{name}", name);
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
                return GetTranslation("MoveIn").Replace("{name}", name);
            else
                return GetTranslation("MovedIn").Replace("{name}", name);
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
                return GetTranslation("CollectObjectCount").Replace("{count}", Count.ToString())
                    .Replace("{name}", ObjectName).Replace("{npcs}", NpcName)
                    .Replace("{droprate}", DropRate.ToString());
            }
            else
            {
                return GetTranslation("CollectedObjectCount").Replace("{name}", ObjectName).Replace("{count}", Stack.ToString());
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            ObjectCollectionData data = Data as ObjectCollectionData;
            return data.ItemsContained >= Stack;
        }

        public override void OnMobKill(NPC killedNpc, ModularQuestBase.ObjectiveData data)
        {
            int Type = killedNpc.type;
            if (killedNpc.realLife != -1)
            {
                Type = Main.npc[killedNpc.realLife].type;
            }
            if (NpcIDs.Contains(Type) && Main.rand.NextFloat(0f, 100f) < DropRate)
            {
                CombatText.NewText(killedNpc.getRect(), Color.Gray, GetTranslation("FoundObject").Replace("{name}", ObjectName), true);
                (data as ObjectCollectionData).ItemsContained++;
            }
        }

        public class ObjectCollectionData : ModularQuestBase.ObjectiveData
        {
            const byte SaveVersion = 0;
            public int ItemsContained = 0;

            public override void Save(TagCompound save, string QuestID)
            {
                save.Add(QuestID + "_SaveVer", SaveVersion);
                save.Add(QuestID + "_ItemCount", ItemsContained);
            }

            public override void Load(TagCompound load, string QuestID, ushort LastVersion)
            {
                byte Ver = load.GetByte(QuestID + "_SaveVer");
                ItemsContained = load.GetInt(QuestID + "_ItemCount");
            }
        }
    }
    
    public class MaxHealthObjective : ModularQuestBase.ObjectiveBase
    {
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new MaxStatObjectiveData();
        public int MaxHealthValue = 100;

        public MaxHealthObjective(int MaxHealthValue)
        {
            this.MaxHealthValue = MaxHealthValue;
        }

        public override void UpdatePlayer(Player player, ModularQuestBase.ObjectiveData data)
        {
            MaxStatObjectiveData Data = data as MaxStatObjectiveData;
            Data.PreviousValue = player.statLifeMax;
            Data.Achieved = player.statLifeMax >= MaxHealthValue;
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            return (Data as MaxStatObjectiveData).Achieved;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            if ((Data as MaxStatObjectiveData).Achieved)
                return GetTranslation("GotMaxHealth").Replace("{value}", MaxHealthValue.ToString());
            int CurrentDiference = MaxHealthValue - (Data as MaxStatObjectiveData).PreviousValue;
            return GetTranslation("GetMaxHealth").Replace("{value}", CurrentDiference.ToString());
        }
    }
    
    public class MaxManaObjective : ModularQuestBase.ObjectiveBase
    {
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new MaxStatObjectiveData();
        public int MaxManaValue = 100;

        public MaxManaObjective(int MaxManaValue)
        {
            this.MaxManaValue = MaxManaValue;
        }

        public override void UpdatePlayer(Player player, ModularQuestBase.ObjectiveData data)
        {
            MaxStatObjectiveData Data = data as MaxStatObjectiveData;
            Data.PreviousValue = player.statManaMax;
            Data.Achieved = player.statManaMax >= MaxManaValue;
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            return (Data as MaxStatObjectiveData).Achieved;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            if ((Data as MaxStatObjectiveData).Achieved)
                return GetTranslation("GotMaxHealth").Replace("{value}", MaxManaValue.ToString());
            int CurrentDiference = MaxManaValue - (Data as MaxStatObjectiveData).PreviousValue;
            return GetTranslation("GetMaxHealth").Replace("{value}", CurrentDiference.ToString());
        }
    }

    public class DefenseIncreaseObjective : ModularQuestBase.ObjectiveBase
    {
        public override ModularQuestBase.ObjectiveData GetObjectiveData => new DefenseIncreaseObjectiveData();
        public int DefenseToGet = 0;
        
        public DefenseIncreaseObjective(int DefenseToGet)
        {
            this.DefenseToGet = DefenseToGet;
        }

        public override void UpdatePlayer(Player player, ModularQuestBase.ObjectiveData data)
        {
            (data as DefenseIncreaseObjectiveData).DefenseValue = player.statDefense;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            if ((Data as DefenseIncreaseObjectiveData).DefenseValue >= DefenseToGet)
                return GetTranslation("GotDefense").Replace("{value}", DefenseToGet.ToString());
            return GetTranslation("GetDefense").Replace("{value}", DefenseToGet.ToString());
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            return (Data as DefenseIncreaseObjectiveData).DefenseValue >= DefenseToGet;
        }

        public class DefenseIncreaseObjectiveData : ModularQuestBase.ObjectiveData
        {
            public int DefenseValue = 0;
        }
    }

    public class CompleteQuestObjectiveBase : ModularQuestBase.ObjectiveBase
    {
        public uint QuestID;
        public string QuestModID;
        QuestBase quest;

        public CompleteQuestObjectiveBase(uint QuestID, string QuestModID)
        {
            this.QuestID = QuestID;
            this.QuestModID = QuestModID;
        }

        public override ModularQuestBase.ObjectiveData GetObjectiveData => new CompleteQuestObjectiveData();

        public override void UpdatePlayer(Player player, ModularQuestBase.ObjectiveData data)
        {
            if (quest == null)
            {
                quest = QuestContainer.GetQuest(QuestID, QuestModID);
            }
            if (!quest.IsInvalid)
            {
                CompleteQuestObjectiveData Data = data as CompleteQuestObjectiveData;
                if (Data.data == null)
                {
                    Data.data = PlayerMod.GetPlayerQuestData(player, QuestID, QuestModID);
                }
                Data.Completed = Data.data.IsCompleted;
            }
        }

        public override bool IsCompleted(ModularQuestBase.ObjectiveData Data)
        {
            return (Data as CompleteQuestObjectiveData).Completed;
        }

        public override string ObjectiveText(ModularQuestBase.ObjectiveData Data)
        {
            CompleteQuestObjectiveData data = (CompleteQuestObjectiveData)Data;
            if (quest == null || quest.IsInvalid)
                return GetTranslation("InvalidQuest");
            if (data.Completed)
            {
                return GetTranslation("CompletedQuest").Replace("{name}", quest.Name);
            }
            return GetTranslation("CompleteQuest").Replace("{name}", quest.Name);
        }

        public class CompleteQuestObjectiveData : ModularQuestBase.ObjectiveData
        {
            public bool Completed = false;
            internal QuestData data = null;
        }

    }

    public class MaxStatObjectiveData : ModularQuestBase.ObjectiveData
    {
        public bool Achieved = false;
        public int PreviousValue = 0;
    }
}