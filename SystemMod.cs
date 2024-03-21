using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using nterrautils.Interfaces;

namespace nterrautils
{
        public class SystemMod : ModSystem
        {
                static BottomButtonsInterface BottomInterfaceDef;

                public override void Load()
                {
                        BottomInterfaceDef = new BottomButtonsInterface();
                }

                public override void Unload()
                {
                        BottomInterfaceDef = null;
                }

                public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
                {
                        int InventoryPos = -1;
                        for (int i = 0; i < layers.Count; i++)
                        {
                                switch(layers[i].Name)
                                {
                                        case "Vanilla: Inventory":
                                                InventoryPos = i;
                                                break;
                                }
                        }
                        if (InventoryPos > -1)
                        {
                                layers.Insert(InventoryPos, BottomInterfaceDef);
                        }
                }
        }
}