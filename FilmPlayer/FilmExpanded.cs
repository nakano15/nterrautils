using Terraria.Cinematics;

namespace nterrautils.FilmPlayer
{
    public class FilmExpanded : Film
    {
        public virtual bool HideInterfaces => true;
        public virtual bool DrawInFrontOfInterfaces => true;

        public virtual void DrawOnScreen()
        {

        }

        public void StopMovie()
        {
            UpgradedFilmPlayer.StopMovie();
        }
    }
}