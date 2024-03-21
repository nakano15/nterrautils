using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using ReLogic.Graphics;


namespace nterrautils.Interfaces
{
	public class BottomButtonsInterface : LegacyGameInterfaceLayer
	{
		static bool Visible = false, LastInventoryOpened = false;
		static List<BottomButton> Buttons = new List<BottomButton>();
		static List<string> ButtonTexts = new List<string>();
		static List<Texture2D> ButtonTexture = new List<Texture2D>();
		static List<Rectangle> ButtonDrawRect = new List<Rectangle>();
		static int SelectedTab = -1;

		public BottomButtonsInterface() : 
			base ("N Terra Utils: Bottom Interface", DrawInterface, InterfaceScaleType.UI)
		{
			
		}

		public static void AddNewTab(BottomButton NewTab)
		{
			Buttons.Add(NewTab);
		}

		static bool DrawInterface()
		{
			if (LastInventoryOpened != Main.playerInventory)
			{
				Visible = Main.playerInventory;
				if (SelectedTab > -1)
				{
					Buttons[SelectedTab].OnClickAction(Main.playerInventory);
				}
			}
			LastInventoryOpened = Main.playerInventory;
			if (!Visible) return true;
			const int PaddingWidth = 8;
			DynamicSpriteFont Font = FontAssets.MouseText.Value;
			if (SelectedTab > -1)
			{
				Vector2 TabStartPosition = new Vector2(Main.screenWidth * .5f, Main.screenHeight);
				BottomButton Button = Buttons[SelectedTab];
				int TabWidth = Button.InternalWidth, 
					TabHeight = Button.InternalHeight;
				string Text = Button.Text;
				Texture2D Texture;
				Rectangle Rect;
				Color color = Button.TabColor;
				Button.GetIcon(out Texture, out Rect);
				TabStartPosition.X -= TabWidth * .5f;
				TabStartPosition.Y -= TabHeight;
				DrawTab(TabStartPosition, TabWidth, Button.TabColor, TabHeight);
				Button.DrawInternal(TabStartPosition);
				TabStartPosition.Y -= 32;
				if (DrawTabFull(TabStartPosition, PaddingWidth * 2 + (int)Font.MeasureString(Text).X + Rect.Width, Text, Texture, Rect, color, 8))
				{
					SelectedTab = -1;
					Button.OnClickAction(false);
				}
			}
			else
			{
				float WidthStack = 0;
				foreach (BottomButton button in Buttons)
				{
					string Text = button.Text;
					ButtonTexts.Add(Text);
					Texture2D Texture;
					Rectangle Rect;
					button.GetIcon(out Texture, out Rect);
					ButtonTexture.Add(Texture);
					ButtonDrawRect.Add(Rect);
					if (button.Visible)
					{
						WidthStack += PaddingWidth * 2 + Font.MeasureString(Text).X + Rect.Width;
					}
				}
				Vector2 ButtonStartPosition = new Vector2((Main.screenWidth - WidthStack) * .5f, Main.screenHeight - 48);
				for (int i = 0; i < ButtonTexts.Count; i++)
				{
					if (!Buttons[i].Visible) continue;
					int TabWidth = PaddingWidth * 2 + (int)Font.MeasureString(ButtonTexts[i]).X + ButtonDrawRect[i].Width;
					if (DrawTabFull(ButtonStartPosition, TabWidth, ButtonTexts[i], ButtonTexture[i], ButtonDrawRect[i], Buttons[i].TabColor))
					{
						if (SelectedTab == i)
						{
							SelectedTab = -1;
						}
						else
						{
							SelectedTab = i;
						}
						Buttons[i].OnClickAction(SelectedTab > -1);
					}
					ButtonStartPosition.X += TabWidth;
				}
				ButtonTexts.Clear();
				ButtonTexture.Clear();
				ButtonDrawRect.Clear();
			}
			return true;
		}

		static bool DrawTab(Vector2 Position, int Width, Color color, int Height = 32)
		{
			if (Width == 0) return false;
			int StartX = (int)Position.X, StartY = (int)Position.Y;
			Texture2D Background = nterrautils.BottomButtonTexture.Value;
			const int PartDimensions = 16;
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					int px = StartX, py = StartY + y * PartDimensions, pw = PartDimensions, ph = PartDimensions;
					int dx = x * PartDimensions, dy = y * PartDimensions, d = PartDimensions;
					switch(x)
					{
						case 1:
							px += PartDimensions;
							pw = Width - PartDimensions * 2;
							break;
						case 2:
							px += Width - PartDimensions;
							break;
					}
					if (y == 1)
					{
						ph = Height - PartDimensions;
					}
					if (pw > 0)
					{
						Main.spriteBatch.Draw(Background, new Rectangle(px, py, pw, ph), new Rectangle(dx, dy, d, d), color);
					}
				}
			}
			bool MouseOver = Main.mouseX >= Position.X && Main.mouseX < Position.X + Width && 
				Main.mouseY >= Position.Y && Main.mouseY < Position.Y + Height;
			if (MouseOver)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
			return MouseOver;
		}

		static bool DrawTabFull(Vector2 Position, int Width, string Text, Texture2D Texture, Rectangle rect, Color color, int ExtraHeight = 0)
		{
			if (Width == 0) return false;
			bool MouseOver = DrawTab(Position, Width, color, 32 + ExtraHeight);
			if (Texture != null)
			{
				Vector2 ImagePosition = Position;
				ImagePosition.X += 8;
				ImagePosition.Y += 24 - rect.Height / 2;
				Main.spriteBatch.Draw(Texture, ImagePosition, rect, Color.White);
				Position.X += rect.Width;
			}
			Position.X += (Width - rect.Width) * .5f;
			Position.Y += 24;
			Utils.DrawBorderString(Main.spriteBatch, Text, Position, MouseOver ? Color.Yellow : Color.White, anchorx: .5f, anchory: .5f);
			return MouseOver && Main.mouseLeft && Main.mouseLeftRelease;
		}

		internal static void Unload()
		{
			foreach (BottomButton button in Buttons) button.OnUnload();
			Buttons.Clear();
			Buttons = null;
			ButtonTexts = null;
			ButtonTexture = null;
			ButtonDrawRect = null;
		}
    }
}