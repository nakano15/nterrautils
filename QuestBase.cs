using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace nterrautils
{
    public class QuestBase
    {
        uint _ID;
        string _ModID;
        internal void SetQuestModInfos(uint ID, string ModID)
        {
            this._ID = ID;
            this._ModID = ModID;
        }
        public QuestData Data { get { return PlayerMod.GetPlayerQuestData(MainMod.GetPlayerCharacter(), _ID, _ModID); }}
        public virtual string Name => "";
        public virtual string QuestStory(QuestData data)
        {
            return "";
        }
        public virtual string GetQuestCurrentObjective(QuestData data)
        {
            return "";
        }
        public virtual bool IsQuestActive(QuestData data)
        {
            return false;
        }
        public virtual bool IsQuestCompleted(QuestData data)
        {
            return false;
        }
        public virtual QuestData GetQuestData => new QuestData();
        bool Invalid = false;
        public bool IsInvalid => Invalid;

        public QuestBase()
        {

        }

        internal QuestBase(bool Invalid)
        {
            this.Invalid = true;
        }

        public virtual void UpdatePlayer(Player player, QuestData data)
        {

        }

        public virtual void OnMobKill(NPC killedNpc, QuestData data)
        {

        }

        public virtual void OnTalkToNpc(NPC npc, QuestData data)
        {

        }

        public virtual string QuestNpcDialogue(NPC npc, QuestData data, out bool BlockOtherMessages)
        {
            BlockOtherMessages = false;
            return null;
        }
    }
}