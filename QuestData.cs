using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace nterrautils
{
    public class QuestData
    {
        QuestBase _Base;
        public QuestBase Base { get 
        {
            if (_Base == null)
                _Base = QuestContainer.GetQuest(ID, ModID);
            return _Base;
        }}
        public string Name { get { return Base.Name; } }
        public uint ID { get { return _ID; } internal set { _ID = value; } }
        public string ModID { get { return _ModID; } internal set { _ModID = value; } }
        uint _ID = 0;
        string _ModID = "";
        public bool IsActive { get { return Base.IsQuestActive(this); } }
        bool LastActive = false;
        public bool IsCompleted { get { return Base.IsQuestCompleted(this); } }
        public string GetObjective { get { return Base.GetQuestCurrentObjective(this); } }
        public string GetStory { get { return Base.QuestStory(this); } }
        public virtual ushort Version => 0;

        public void ShowQuestStartedNotification()
        {
            Main.NewText("[" + Name + "] quest has started.", Microsoft.Xna.Framework.Color.Chocolate);
        }

        public void ShowQuestCompletedNotification()
        {
            Main.NewText("[" + Name + "] quest has been completed.", Microsoft.Xna.Framework.Color.Chocolate);
        }

        internal void SaveQuest(TagCompound save, string QuestID)
        {
            save.Add("Version_" + QuestID, Version);
            Save(save, QuestID);
        }

        internal void LoadQuest(TagCompound load, string QuestID)
        {
            ushort LastVersion = load.Get<ushort>("Version_" + QuestID);
            Load(load, QuestID, LastVersion);
        }

        internal void Initialize(QuestBase Quest)
        {
            OnInitialize(Quest);
        }

        internal void UpdatePlayer(Player player)
        {
            Base.UpdatePlayer(player, this);
            UpdateQuestStartedStates(player);
        }

        internal void UpdateQuestStartedStates(Player player, bool Silent = false)
        {
            if (MainMod.GetPlayerCharacter() == player)
            {
                bool NewActive = IsActive;
                if (!Silent && NewActive && !LastActive)
                {
                    ShowQuestStartedNotification();
                }
                LastActive = NewActive;
                if (NewActive && !IsCompleted)
                {
                    player.GetModPlayer<PlayerMod>().ActiveQuestDatas.Add(this);
                }
            }
        }

        protected virtual void OnInitialize(QuestBase Quest)
        {

        }

        protected virtual void Save(TagCompound save, string QuestID)
        {
            
        }

        protected virtual void Load(TagCompound load, string QuestID, ushort LastVersion)
        {
            
        }
    }
}