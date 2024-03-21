using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nterrautils
{
    public class BottomButton
    {
        public virtual bool Visible => true;
        public virtual string Text => "";
        public virtual Color TabColor => Color.White;
        public virtual int InternalWidth => 300;
        public virtual int InternalHeight => 200;

        public virtual void OnClickAction(bool OpeningTab)
        {
            
        }

        public virtual void DrawInternal(Vector2 DrawPosition)
        {

        }

        public virtual void GetIcon(out Texture2D Texture, out Rectangle DrawRect)
        {
            Texture = null;
            DrawRect = Rectangle.Empty;
        }

        public virtual void OnUnload()
        {

        }
    }
}