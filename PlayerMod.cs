using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace nterrautils
{
    public class PlayerMod : ModPlayer
    {
        protected override bool CloneNewInstances => false;
        public override bool IsCloneable => false;
        
        public List<QuestData> QuestDatas { get { return _QuestDatas; } internal set { _QuestDatas = value; } }
        List<QuestData> _QuestDatas = new List<QuestData>();
        public int TrackedQuest = -1;
        public List<QuestData> ActiveQuestDatas
        {
            get
            {
                return _ActiveQuestDatas;
            }
        }
        List<QuestData> _ActiveQuestDatas = new List<QuestData>();

        public static List<QuestData> GetPlayerQuests(Player p)
        {
            return p.GetModPlayer<PlayerMod>().QuestDatas;
        }

        public static List<QuestData> GetPlayerActiveQuests(Player p)
        {
            return p.GetModPlayer<PlayerMod>().ActiveQuestDatas;
        }

        public static QuestData GetPlayerQuestData(Player p, uint ID, string ModID = "")
        {
            if (ModID == "") ModID = MainMod.GetModName;
            foreach (QuestData d in GetPlayerQuests(p))
            {
                if (d.ID == ID && d.ModID == ModID)
                {
                    return d;
                }
            }
            return null;
        }
        
        public PlayerMod()
        {
            QuestContainer.CreateQuestListToPlayer(this);
        }

        public override void OnEnterWorld()
        {
            if (MainMod.GetPlayerCharacter() == Player && (TrackedQuest == -1 || QuestDatas[TrackedQuest].IsCompleted))
            {
                TrackNewQuest();
                foreach (QuestData q in QuestDatas)
                {
                    q.UpdateQuestStartedStates(Player, true);
                }
            }
        }

        public void TrackNewQuest()
        {
            for (int i = 0; i < QuestDatas.Count; i++)
            {
                if (QuestDatas[i].IsActive && !QuestDatas[i].IsCompleted)
                {
                    TrackedQuest = i;
                    break;
                }
            }
        }

        public override void PostUpdate()
        {
            _ActiveQuestDatas.Clear();
            foreach (QuestData q in QuestDatas)
            {
                q.UpdatePlayer(Player);
            }
            NpcMod.UpdateCheckQuestText(Player);
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("LastVersion", MainMod.SaveVersion);
            tag.Add("QuestProgressCount", QuestDatas.Count);
            for (int i = 0; i < QuestDatas.Count; i++)
            {
                QuestData quest = QuestDatas[i];
                string ID = "q" + i;
                tag.Add("ID_" + ID, quest.ID);
                tag.Add("ModID_" + ID, quest.ModID);
                quest.SaveQuest(tag, ID);
            }
            bool TrackingQuest = TrackedQuest > -1 && TrackedQuest < QuestDatas.Count;
            tag.Add("IsTrackingQuest", TrackingQuest);
            if (TrackingQuest)
            {
                tag.Add("TrackedQuestID", QuestDatas[TrackedQuest].ID);
                tag.Add("TrackedQuestModID", QuestDatas[TrackedQuest].ModID);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (!tag.ContainsKey("LastVersion")) return;
            int LastVersion = tag.GetInt("LastVersion");
            int Count = tag.GetInt("QuestProgressCount");
            for (int i = 0; i < Count; i++)
            {
                //QuestData quest = QuestDatas[i];
                string QuestID = "q" + i;
                uint ID = tag.Get<uint>("ID_" + QuestID);
                string ModID = tag.GetString("ModID_" + QuestID);
                QuestBase qb = QuestContainer.GetQuest(ID, ModID);
                if (!qb.IsInvalid)
                {
                    QuestData qd = null;
                    foreach(QuestData q in _QuestDatas)
                    {
                        if (q.ID == ID && q.ModID == ModID)
                        {
                            qd = q;
                            break;
                        }
                    }
                    if (qd == null)
                    {
                        qd = qb.GetQuestData;
                        qd.Initialize(qb);
                        _QuestDatas.Add(qd);
                    }
                    qd.LoadQuest(tag, QuestID);
                }
            }
            if (LastVersion >= 1)
            {
                TrackedQuest = -1;
                if (tag.GetBool("IsTrackingQuest"))
                {
                    uint ID = tag.Get<uint>("TrackedQuestID");
                    string ModID = tag.GetString("TrackedQuestModID");
                    for(int q = 0; q < QuestDatas.Count; q++)
                    {
                        if (QuestDatas[q].ID == ID && QuestDatas[q].ModID == ModID)
                        {
                            TrackedQuest = q;
                            break;
                        }
                    }
                }
            }
        }
    }
}