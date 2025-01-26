using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using nterrautils.FilmPlayer;

namespace nterrautils
{
	public class MainMod : Mod
	{
		public static Asset<Texture2D> BottomButtonTexture;
		public static Asset<Texture2D> InterfaceBackgroundTexture;
		static Mod _Mod;
		internal static Mod GetMod => _Mod;
		internal static string GetModName => _Mod.Name;
        internal const int SaveVersion = 1;
		public const string QuestExpRewardHooksString = "addquestexpreward";
		static List<Action<Player, int, float>> QuestExpRewardHooks = new List<Action<Player, int, float>>();

        public override void Load()
        {
			_Mod = this;
			ModCompatibility.TerraGuardiansMod.Initialize();
			if (!Terraria.Main.dedServ)
			{
				BottomButtonTexture = ModContent.Request<Texture2D>("nterrautils/Content/Interface/BottomButton");
				InterfaceBackgroundTexture = ModContent.Request<Texture2D>("nterrautils/Content/Interface/Background");
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
			UpgradedFilmPlayer.Unload();
			QuestExpRewardHooks.Clear();
			QuestExpRewardHooks = null;
			_Mod = null;
        }

		public static Player GetPlayerCharacter()
		{
			return ModCompatibility.TerraGuardiansMod.GetPlayerCharacter();
		}

        public override object Call(params object[] args)
        {
			if (args.Length > 0 && args[0] is string)
			{
				switch ((string)args[0])
				{
					case QuestExpRewardHooksString:
						if (args[1] is Action<Player, int, float>)
						{
							QuestExpRewardHooks.Add((Action<Player, int, float>)args[1]);
						}
						break;
				}
			}
            return base.Call(args);
        }

		public static void AddQuestRewardHook(Action<Player, int, float> hook)
		{
			QuestExpRewardHooks.Add(hook);
		}

		public static void TriggerExpRewardHooks(Player player, int Level, float Percentage)
		{
			foreach (Action<Player, int, float> hook in QuestExpRewardHooks)
			{
				hook(player, Level, Percentage);
			}
		}
    }
}