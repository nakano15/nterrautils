using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace nterrautils
{
    public class NpcMod : GlobalNPC
    {
        internal static string LatestQuestText = "";
        protected override bool CloneNewInstances => false;
        //public override bool InstancePerEntity => true;
        
        public override void OnKill(NPC npc)
        {
            if (npc.AnyInteractions())
            {
                foreach (QuestData q in PlayerMod.GetPlayerActiveQuests(MainMod.GetPlayerCharacter()))
                {
                    q.Base.OnMobKill(npc, q);
                }
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            foreach (QuestData q in PlayerMod.GetPlayerActiveQuests(spawnInfo.Player))
            {
                q.Base.ModifySpawnPool(pool, spawnInfo, q);
            }
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            foreach (QuestData q in PlayerMod.GetPlayerActiveQuests(MainMod.GetPlayerCharacter()))
            {
                string s = q.Base.QuestNpcDialogue(npc, q, out bool BlockOtherMessages);
                if (s != "" && s != null)
                {
                    chat = s;
                    LatestQuestText = s;
                }
                if (BlockOtherMessages)
                    break;
            }
        }

        internal static void UpdateCheckQuestText(Player player)
        {
            if (LatestQuestText != "")
            {
                if (player.talkNPC == -1)
                {
                    LatestQuestText = "";
                }
                else
                {
                    if (Main.npcChatText != LatestQuestText)
                    {
                        Main.npcChatText = LatestQuestText;
                    }
                }
            }
        }
    }
}