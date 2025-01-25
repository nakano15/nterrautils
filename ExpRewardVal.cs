
namespace nterrautils;

public struct ExpRewardValue
{
    public int Level;
    public float Percentage;

    public ExpRewardValue()
    {
        Level = 0;
        Percentage = 0f;
    }

    public ExpRewardValue(int Level, float Percentage)
    {
        this.Level = Level;
        this.Percentage = Percentage;
    }
}