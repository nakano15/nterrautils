

using Microsoft.Xna.Framework;

namespace nterrautils.Interfaces.Tabs
{
    public class QuestLogTab : BottomButton
    {
        public override string Text => "Quests";
        public override bool Visible => !QuestInterface.IsActive;
        public override bool JustAButton => true;
        public override Color TabColor => Color.Cyan;

        public override void OnClickAction(bool OpeningTab)
        {
            QuestInterface.Open();
        }
    }
}