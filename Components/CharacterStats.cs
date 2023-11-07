namespace RayLibECS.Components;

public class CharacterStats : Component
{
    public float Speed;
    public float JumpHeight;

    public CharacterStats(){
        Speed = 0f;
        JumpHeight = 0F;
    }
}
