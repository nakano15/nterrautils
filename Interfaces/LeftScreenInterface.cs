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
    public class LeftScreenInterface : LegacyGameInterfaceLayer
    {
        static List<LeftInterfaceElement> InterfaceElements = new List<LeftInterfaceElement>();

        public static void AddInterfaceElement(LeftInterfaceElement NewElement)
        {
            for(int i = 0; i < InterfaceElements.Count; i++)
            {
                if (InterfaceElements[i].Priority > NewElement.Priority)
                {
                    InterfaceElements.Insert(i, NewElement);
                    return;
                }
            }
            InterfaceElements.Add(NewElement);
        }

		public LeftScreenInterface() : 
			base ("N Terra Utils: Left Screen Interface", DrawInterface, InterfaceScaleType.UI)
		{
			
		}

        static bool DrawInterface()
        {
            if (!Main.playerInventory)
            {
                float PositionY = 120;
                foreach (LeftInterfaceElement element in InterfaceElements)
                {
                    if (element.IsVisible)
                    {
                        element.DrawInternal(ref PositionY);
                    }
                }
            }
            return true;
        }

        internal static void Unload()
        {
            InterfaceElements.Clear();
            InterfaceElements = null;
        }
    }
}