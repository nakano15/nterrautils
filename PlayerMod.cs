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

        public static List<QuestData> GetPlayerQuests(Player p)
        {
            return p.GetModPlayer<PlayerMod>().QuestDatas;
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

        public override void PostUpdate()
        {
            foreach (QuestData q in QuestDatas)
            {
                q.Base.UpdatePlayer(Player, q);
            }
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
        }
    }
}