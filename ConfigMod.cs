using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace nterrautils
{
    public class ConfigMod : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(0)]
		[Range(-1000, 1000)]
        [Increment(1)]
        public int SideBarVerticalYOffset;

        public override void OnChanged()
        {
            Interfaces.LeftScreenInterface.YOffset = SideBarVerticalYOffset;
        }
    }
}