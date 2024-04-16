using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace nterrautils
{
	public class MainMod : Mod
	{
		public static Asset<Texture2D> BottomButtonTexture;
		static Mod _Mod;
		internal static Mod GetMod => _Mod;
		internal static string GetModName => _Mod.Name;
        internal const int SaveVersion = 1;

        public override void Load()
        {
			_Mod = this;
			ModCompatibility.TerraGuardiansMod.Initialize();
			if (!Terraria.Main.dedServ)
			{
				BottomButtonTexture = ModContent.Request<Texture2D>("nterrautils/Content/Interface/BottomButton");
				Interfaces.BottomButtonsInterface.AddNewTab(new Interfaces.Tabs.QuestLogTab());
			}
			QuestContainer.Initialize();
			//QuestContainer.AddQuestContainer(this, new TestContainer()); //Used only for testing purposes.
        }

        public override void PostSetupContent()
        {
			Interfaces.LeftScreenInterface.AddInterfaceElement(new TrackQuestObjective());
        }

        public override void Unload()
        {
			Interfaces.BottomButtonsInterface.Unload();
			Interfaces.LeftScreenInterface.Unload();
			BottomButtonTexture = null;
			QuestContainer.Unload();
			ModCompatibility.TerraGuardiansMod.Unload();
			_Mod = null;
        }

		public static Player GetPlayerCharacter()
		{
			return ModCompatibility.TerraGuardiansMod.GetPlayerCharacter();
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
	}
}