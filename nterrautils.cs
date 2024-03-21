using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using ReLogic.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace nterrautils
{
	public class nterrautils : Mod
	{
		public static Asset<Texture2D> BottomButtonTexture;

        public override void Load()
        {
			if (!Terraria.Main.dedServ)
			{
				BottomButtonTexture = ModContent.Request<Texture2D>("nterrautils/Content/Interface/BottomButton");
			}
        }

        public override void Unload()
        {
			Interfaces.BottomButtonsInterface.Unload();
			BottomButtonTexture = null;
        }
	}
}