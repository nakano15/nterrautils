using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using nterrautils.QuestObjectives;

namespace nterrautils
{
    //Script file used to test quests. After initializing quest containers, add TestContainer as a new quest container.
    public class TestContainer : QuestContainer
    {
        public const uint TestModularQuest = 0;

        protected override void CreateQuestDB()
        {
            AddQuest(TestModularQuest, new TestModularQuest());
        }
    }

    public class TestModularQuest : ModularQuestBase
    {
        public override string Name => "Test Modular Quest";

        public override bool IsQuestActive(QuestData data)
        {
            return true;
        }

        public TestModularQuest()
        {
            ModularQuestStep step = AddNewQuestStep();
            step.ChangeStoryText("I've just begun my first quest, and I'm eager to speak with that weirdo over there.", 
                "I've just begun my first quest, and I spoke with the weirdo, and he asked me to kill some slimes.");
            step.AddNewObjective(new TalkObjective(NPCID.Guide, "Hello! This is a quest test. I mean, a test quest.\nI will ask you to kill some Slimes. Would you do that?"));
            step = AddNewQuestStep();
            step.ChangeStoryText("The weirdo asked me to kill some Slimes... I guess it wont be that hard, as long as I don't touch them.",
                "I've killed the slimes as the weirdo asked me.");
            step.AddNewObjective(new HuntObjective(NPCID.BlueSlime, 5));
            step = AddNewQuestStep();
            step.ChangeStoryText("I will need to talk with the weirdo to see what he has to say about this.",
                "The weirdo congratulated me for killing the slimes, so good to be recognized as the slime killer of all Terraria.");
            step.AddNewObjective(new TalkObjective(NPCID.Guide, 
                "That's a great job! I knew you would do that. Now, what about collecting some wood? About 20 is fine."));
            step = AddNewQuestStep();
            step.ChangeStoryText("I was asked to gather about 20 wood and bring it to the weirdo. I guess I should make use of the axe I've got.",
                "I was asked to gather about 20 wood and bring it to the weirdo. Good thing that I could rely on my axe for this.");
            step.AddNewObjective(new ItemCollectionObjective(ItemID.Wood, 20));
            step = AddNewQuestStep();
            step.ChangeStoryText("I've managed to get all the 20 wood the weirdo asked me to get, but I still haven't managed to deliver them to him.",
                "I've managed to get all the 20 wood the weirdo asked me to get, and he said that by doing those tasks I proved to be the hero that will save us all from impending death.\nAnd then he asked me to kill the Eye of Cthulhu.");
            step.AddNewObjective(new TalkObjective(NPCID.Guide, "Amazing job. That shows all the signs that you're the hero that will save us all.\nNow please go kill Eye of Cthulhu."));
            step = AddNewQuestStep();
            step.AddNewObjective(new HuntObjective(NPCID.EyeofCthulhu, 1));
            step.ChangeStoryText("The Eye of Cthulhu... I heard legends of people being terrorized by it... Should I wait for it to appear, or should I craft its summoning object?",
                "I've faced the Eye of Cthulhu, and beside the heroic battle I had against it, flashing, teleporting in the air and the genkidama I had to dodge, it exploded to its death. The weirdo will surelly be impressed by this.");
            step = AddNewQuestStep();
            step.OnQuestStepStart = GetVoodooDoll;
            step.ChangeStoryText("I still have to go talk with the weirdo about this.", "I went to talk to the weirdo, and he said that I should kill the Dungeon Guardian. I wonder if he went nuts if he think I will do that.");
            step.AddNewObjective(new TalkObjective(NPCID.Guide, "Congratulations! Now you can take up on the next challenge. Go inside the Dungeon and kill the Dungeon Guardian."));
            step = AddNewQuestStep();
            step.ChangeStoryText("There is no way I will do that at all, better I discuss this with the weirdo.",
                "The weirdo is no more, and I feel like I've beaten this game. Yay!");
            step.AddNewObjective(new HuntObjective(NPCID.Guide, 1));
            step.OnQuestStepEnd = PlayCreditsIfPossible;
            AddItemReward(ItemID.RedPotion, 5);
            CoinsReward += 5500;
        }

        void GetVoodooDoll(Player player, ModularQuestStepData StepData)
        {
            Item.NewItem(Item.GetSource_None(), player.Center, Vector2.Zero, ItemID.GuideVoodooDoll);
        }

        void PlayCreditsIfPossible(Player player, ModularQuestStepData StepData)
        {
            Terraria.GameContent.Events.CreditsRollEvent.TryStartingCreditsRoll();
        }
    }
}