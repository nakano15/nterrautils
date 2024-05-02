using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.IO;
using Terraria.Audio;
using Terraria.UI;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;

namespace nterrautils
{
    public class MouseOverInterface : LegacyGameInterfaceLayer
    {
        static string MouseOverText = "";

        public MouseOverInterface() : base("N Terra Utils: Mouse Over", DrawInterface, InterfaceScaleType.UI)
        {

        }

        static bool DrawInterface()
        {
            if (MouseOverText != "")
            {
                Vector2 Position = new Vector2(Main.mouseX + 16, Main.mouseY + 16);
                Vector2 Dimension = FontAssets.MouseText.Value.MeasureString(MouseOverText);
                const float Padding = 6f;
                if (Position.X + Dimension.X + Padding * 2 > Main.screenWidth)
                    Position.X = Main.screenWidth - Dimension.X - Padding * 2;
                if (Position.Y + Dimension.Y + Padding * 2 > Main.screenHeight)
                    Position.Y = Main.screenHeight - Dimension.Y - Padding * 2;
                InterfaceHelper.DrawBackgroundPanel(Position, (int)(Dimension.X + Padding * 2), (int)(Dimension.Y + Padding * 2), Color.Blue);
                Position.X += Padding;
                Position.Y += Padding;
                Utils.DrawBorderString(Main.spriteBatch, MouseOverText, Position, Color.White);
                MouseOverText = "";
            }
            return true;
        }

        public static void ChangeMouseText(string NewText, bool ReplaceIfAlreadyExists = true)
        {
            if (ReplaceIfAlreadyExists || MouseOverText == "")
                MouseOverText = NewText;
        }
    }
}