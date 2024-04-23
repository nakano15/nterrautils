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
	}
}