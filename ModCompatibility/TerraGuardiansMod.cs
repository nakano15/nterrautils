using Terraria;
using Terraria.ModLoader;

namespace nterrautils.ModCompatibility
{
    internal class TerraGuardiansMod
    {
        static Mod TgMod;

        public static void Initialize()
        {
            if (!ModLoader.TryGetMod("terraguardians", out TgMod))
            {
                TgMod = null;
            }
        }

        public static void Unload()
        {
            TgMod = null;
        }

        public static bool IsPlayerCharacter(Player player)
        {
            if (TgMod != null)
            {
                return (bool)TgMod.Call("IsPC", player);
            }
            return player.whoAmI == Main.myPlayer;
        }

        public static Player GetPlayerCharacter()
        {
            if (TgMod != null)
            {
                return (Player)TgMod.Call("GetPC");
            }
            return Main.LocalPlayer;
        }
    }
}