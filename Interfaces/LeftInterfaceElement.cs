using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace nterrautils
{
    public class LeftInterfaceElement
    {
        public virtual bool Visible => true;
        public virtual string Name => "";
        public virtual int Priority => 0;
        public bool _Visible = true;

        public bool IsVisible => _Visible && Visible;
        
        public virtual void DrawInternal(ref float PositionY)
        {

        }
    }
}