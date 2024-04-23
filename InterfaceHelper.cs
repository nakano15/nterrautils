using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace nterrautils
{
    public class InterfaceHelper
    {
        public static void DrawSeparator(Vector2 Position, int Length, bool Horizontal, Color color)
        {
            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, 2, 2);
            if (Horizontal)
                rect.Width = Length;
            else
                rect.Height = Length;
            Main.spriteBatch.Draw(TextureAssets.BlackTile.Value, rect, null, color);
        }
        
        public static void DrawBackgroundPanel(Vector2 Position, int Width, int Height, Color color)
        {
            int HalfHeight = (int)(Height * 0.5f);
            Texture2D ChatBackground = TextureAssets.ChatBack.Value;
            for(byte y = 0; y < 3; y++)
            {
                for(byte x = 0; x < 3; x++)
                {
                    const int DrawDimension = 30;
                    int px = (int)Position.X, py = (int)Position.Y, pw = DrawDimension, ph = DrawDimension, 
                        dx = 0, dy = 0, dh = DrawDimension;
                    if (x == 2)
                    {
                        px += Width - pw;
                        dx = ChatBackground.Width - DrawDimension;
                    }
                    else if (x == 1)
                    {
                        px += pw;
                        pw = Width - pw * 2;
                        dx = DrawDimension;
                    }
                    if (y == 2)
                    {
                        py += Height - ph;
                        dy = ChatBackground.Height - DrawDimension;
                        if (ph > HalfHeight)
                        {
                            dy += DrawDimension - HalfHeight;
                            py += (int)(DrawDimension - HalfHeight);
                            ph = dh = HalfHeight;
                        }
                    }
                    else if (y == 1)
                    {
                        py += ph;
                        ph = Height - ph * 2;
                        dy = DrawDimension;
                    }
                    else
                    {
                        if (ph > HalfHeight)
                        {
                            ph = dh = HalfHeight;
                        }
                    }
                    if (pw > 0 && ph > 0)
                    {
                        Main.spriteBatch.Draw(ChatBackground, new Rectangle(px, py, pw, ph), new Rectangle(dx, dy, DrawDimension, dh), color);
                    }
                }
            }
        }
    }
}