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
        protected override bool CloneNewInstances => false;
        //public override bool InstancePerEntity => true;
        
        public override void OnKill(NPC npc)
        {
            if (npc.AnyInteractions())
            {
                foreach (QuestData q in PlayerMod.GetPlayerQuests(MainMod.GetPlayerCharacter()))
                {
                    q.Base.OnMobKill(npc, q);
                }
            }
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            foreach (QuestData q in PlayerMod.GetPlayerQuests(MainMod.GetPlayerCharacter()))
            {
                string s = q.Base.QuestNpcDialogue(npc, q, out bool BlockOtherMessages);
                if (BlockOtherMessages)
                    break;
            }
        }
    }
}