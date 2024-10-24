using Terraria;
using Terraria.Cinematics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nterrautils.FilmPlayer
{
    public class UpgradedFilmPlayer //Named it like this as a joke. This is a upgrade of Terraria's Movie player.
    {
        private static FilmExpanded PlayedMovie = null;
        public static bool IsPlayingMovie { get { return PlayedMovie != null; } }
        public static bool DrawInFrontOfInterface { get { return PlayedMovie != null && PlayedMovie.DrawInFrontOfInterfaces; } }
        public static bool MovieHidesInterface { get { return PlayedMovie != null && PlayedMovie.HideInterfaces; } }

        public static void Update(GameTime gameTime)
        {
            if (PlayedMovie == null) return;
            if (Main.hasFocus && !Main.gamePaused && !PlayedMovie.OnUpdate(gameTime))
            {
                PlayedMovie.OnEnd();
                PlayedMovie = null;
            }
        }

        public static void Draw()
        {
            if (PlayedMovie != null)
            {
                SamplerState state = Main.DefaultSamplerState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);
                PlayedMovie.DrawOnScreen();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, Main.DefaultSamplerState, null, null);
            }
        }

        internal static void Unload()
        {
            PlayedMovie = null;
        }

        public static void PlayMovie(FilmExpanded Movie)
        {
            PlayedMovie = Movie;
            Movie.OnBegin();
        }

        public static void StopMovie()
        {
            if (PlayedMovie != null)
            {
                PlayedMovie.OnEnd();
                PlayedMovie = null;
            }
        }
    }
}