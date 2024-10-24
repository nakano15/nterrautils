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
                static QuestInterface QuestInterfaceDef;
                static LeftScreenInterface LeftScreenInterfaceDef;
                static MouseOverInterface MouseOverInterfaceDef;
                static Interface.DrawMovieOnScreenInterface DrawMovieOnScreenInterfaceDef;

                public override void Load()
                {
                        BottomInterfaceDef = new BottomButtonsInterface();
                        QuestInterfaceDef = new QuestInterface();
                        LeftScreenInterfaceDef = new LeftScreenInterface();
                        MouseOverInterfaceDef = new MouseOverInterface();
                        DrawMovieOnScreenInterfaceDef = new Interface.DrawMovieOnScreenInterface();
                }

                public override void Unload()
                {
                        BottomInterfaceDef = null;
                        QuestInterface.Unload();
                        QuestInterfaceDef = null;
                        LeftScreenInterfaceDef = null;
                        MouseOverInterfaceDef = null;
                        DrawMovieOnScreenInterfaceDef = null;
                }

                public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
                {
                        int InventoryPos = -1, MouseOver = -1;
                        bool HideInterfaceForMovie = FilmPlayer.UpgradedFilmPlayer.MovieHidesInterface;
                        for (int i = 0; i < layers.Count; i++)
                        {
                                switch(layers[i].Name)
                                {
                                        case "Vanilla: Inventory":
                                                InventoryPos = i;
                                                break;
                                        case "Vanilla: Mouse Over":
                                                MouseOver = i;
                                                break;
                                }
                        }
                        /*if (HideInterfaceForMovie)
                        {
                                layers.Insert(0, DrawMovieOnScreenInterfaceDef);
                                return;
                        }*/
                        if (MouseOver > -1)
                        {
                                layers.Insert(MouseOver, MouseOverInterfaceDef);
                                if (FilmPlayer.UpgradedFilmPlayer.DrawInFrontOfInterface)
                                        layers.Add(DrawMovieOnScreenInterfaceDef);
                        }
                        if (InventoryPos == -1)
                                InventoryPos = 0;
                        //if (InventoryPos > -1)
                        {
                                layers.Insert(InventoryPos, QuestInterfaceDef);
                                layers.Insert(InventoryPos, BottomInterfaceDef);
                                layers.Insert(InventoryPos, LeftScreenInterfaceDef);
                        }
                        if (!FilmPlayer.UpgradedFilmPlayer.DrawInFrontOfInterface)
                                layers.Add(DrawMovieOnScreenInterfaceDef);
                }

                public override void PostUpdateEverything()
                {
                        FilmPlayer.UpgradedFilmPlayer.Update(Main.gameTimeCache);
                }
        }
}