using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;

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
            Texture2D ChatBackground = MainMod.InterfaceBackgroundTexture.Value;
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
		
		public static string[] WordwrapText(string Text, DynamicSpriteFont font, float MaxWidth = 100, float Scale = 1f)
		{
			List<string> CurrentText = new List<string>();
			float CurWidth = 0;
			string LastWord = "";
			string LastLine = "";
			float SpaceWidth = font.MeasureString(" ").X * Scale;
			foreach (char c in Text)
			{
				if (c == '\n')
				{
					float Width = font.MeasureString(LastWord).X * Scale;
					if (Width + CurWidth >= MaxWidth)
					{
						CurrentText.Add(LastLine);
						LastLine = LastWord;
					}
					else
					{
						LastLine += LastWord;
					}
					CurrentText.Add(LastLine);
					LastLine = "";
					LastWord = "";
					CurWidth = 0;
				}
				else if (c == ' ')
				{
					float Width = font.MeasureString(LastWord).X * Scale;
					if (CurWidth + Width + SpaceWidth < MaxWidth)
					{
						LastLine += LastWord + " ";
						CurWidth += Width + SpaceWidth;
					}
					else if (CurWidth + Width < MaxWidth)
					{
						LastLine += LastWord;
						CurrentText.Add(LastLine);
						LastLine = "";
						CurWidth = 0;
					}
					else
					{
						CurrentText.Add(LastLine);
						LastLine = LastWord + ' ';
						CurWidth = Width + SpaceWidth;
					}
					LastWord = "";
				}
				else if (char.IsPunctuation(c))
				{
					float Width = font.MeasureString(LastWord + c).X * Scale;
					if (CurWidth + Width >= MaxWidth)
					{
						CurrentText.Add(LastLine);
						LastLine = LastWord + c;
						CurWidth = Width;
						LastWord = "";
					}
					else
					{
						LastLine += LastWord + c;
						CurWidth += Width;
						LastWord = "";
					}
				}
				else if (char.IsSymbol(c))
				{
					if (LastWord.Length > 0)
					{
						float Width = font.MeasureString(LastWord).X * Scale;
						if (Width + CurWidth < MaxWidth)
						{
							LastLine += LastWord;
							LastWord = "";
							CurWidth += Width;
						}
						else
						{
							CurrentText.Add(LastLine);
							LastLine = LastWord;
							CurWidth = Width;
							LastWord = "";
						}
					}
					{
						float Width = font.MeasureString(c.ToString()).X * Scale;
						if (Width + CurWidth >= MaxWidth)
						{
							CurrentText.Add(LastLine);
							LastLine = c.ToString();
							CurWidth = Width;
						}
						else
						{
							LastLine += c.ToString();
							CurWidth += Width;
						}
					}
				}
				else
				{
					LastWord += c;
				}
			}
			if (LastWord.Length > 0)
				LastLine += LastWord;
			if (LastLine.Length > 0)
				CurrentText.Add(LastLine);
			return CurrentText.ToArray();
		}

		public static string PluralizeString(string Text, int Count)
		{
			if (System.Math.Abs(Count) <= 1 || Text.EndsWith('s')) return Text;
			if(Text.EndsWith("fe"))
				return Text.Substring(0, Text.Length - 2) + "ves";
			if(Text.EndsWith("o"))
				return Text + "es";
			return Text + 's';
		}

		public static string GetDirectionText(Vector2 Position1, Vector2 Position2)
        {
            return GetDirectionText(Position2 - Position1);
        }

		public static string GetDirectionText(Vector2 Direction)
        {
            Direction.Normalize();
            bool CountVerticalDiference = Math.Abs(Direction.Y) >= 0.33f, CountHorizontalDiference = Math.Abs(Direction.X) >= 0.33f;
            string DirectionText = "";
            if (CountVerticalDiference && CountHorizontalDiference)
            {
                if (Direction.Y > 0) DirectionText += "South";
                else DirectionText += "North";
                if (Direction.X > 0) DirectionText += "east";
                else DirectionText += "west";
            }
            else if (CountVerticalDiference)
            {
                if (Direction.Y > 0) DirectionText = "South";
                else DirectionText = "North";
            }
            else if (CountHorizontalDiference)
            {
                if (Direction.X > 0) DirectionText = "East";
                else DirectionText = "West";
            }
            return DirectionText;
        }

		public static string GlyphfyItem(Item item)
		{
			return GlyphfyItem(item.type, item.stack, item.prefix);
		}

		public static string GlyphfyItem(int ID, int Stack = 1, int Prefix = 0)
		{
			if (Prefix > 0)
			{
				return "[i/p"+Prefix+":"+ID+"]";
			}
			return "[i/s"+Stack+":"+ID+"]";
		}

		public static string GlyphfyCoins(int value)
		{
			int c = value, s = 0, g = 0, p = 0;
			if (c >= 100)
			{
				s += c / 100;
				c -= s * 100;
			}
			if (s >= 100)
			{
				g += s / 100;
				s -= g * 100;
			}
			if (g >= 100)
			{
				p += g / 100;
				g -= p * 100;
			}
			return GlyphfyCoins(c, s, g, p);
		}

		public static string GlyphfyCoins(int c, int s = 0, int g = 0, int p = 0)
		{
			string Text = "";
			if (p > 0)
			{
				Text += "[i/s"+p+":"+ItemID.PlatinumCoin+"]";
			}
			if (g > 0)
			{
				Text += "[i/s"+g+":"+ItemID.GoldCoin+"]";
			}
			if (s > 0)
			{
				Text += "[i/s"+s+":"+ItemID.SilverCoin+"]";
			}
			if (c > 0)
			{
				Text += "[i/s"+c+":"+ItemID.CopperCoin+"]";
			}
			return Text;
		}
    }
}