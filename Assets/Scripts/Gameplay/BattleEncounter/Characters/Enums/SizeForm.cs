namespace Gameplay.BattleEncounter.Characters.Enums
{
    public enum SizeForm
    {
        Dead,    
        Tiny,    
        Normal,  
        Giant,   
    }

    public static class SizeFormUtil
    {
        public static SizeForm FormOf(int size)
        {
            if (size <= 0 || size >= 10) return SizeForm.Dead;
            if (size <= 3) return SizeForm.Tiny;
            if (size <= 6) return SizeForm.Normal;
            return SizeForm.Giant; 
        }
    }
}
