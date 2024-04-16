using System;
using Terraria;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using nterrautils;
using nterrautils.Interfaces;

namespace nterrautils
{
    public class TrackQuestObjective : LeftInterfaceElement
    {
        public override string Name => "Quest Objective Tracker";
        public override bool Visible => MainMod.GetPlayerCharacter().GetModPlayer<PlayerMod>().TrackedQuest > -1;
        public override int Priority => 100;

        public override void DrawInternal(ref float PositionY)
        {
            PlayerMod pm = MainMod.GetPlayerCharacter().GetModPlayer<PlayerMod>();
            QuestData quest = pm.QuestDatas[pm.TrackedQuest];
            Utils.DrawBorderString(Main.spriteBatch, quest.Name, new Vector2(8, PositionY), Color.Yellow, 1f);
            PositionY += 25;
            Utils.DrawBorderString(Main.spriteBatch, quest.GetObjective, new Vector2(8, PositionY), Color.White, 0.8f);
            PositionY += 20;
        }
    }
}