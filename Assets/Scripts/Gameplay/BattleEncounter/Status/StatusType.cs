namespace Gameplay.BattleEncounter.Status
{
    // ชุดสถานะเริ่มต้น — ขยายเพิ่มได้
    public enum StatusType
    {
        Poison,      // ลด HP ต้นเทิร์น
        Strength,    // +ดาเมจโจมตี
        Weak,        // -ดาเมจที่ทำ
        Vulnerable,  // โดนดาเมจเพิ่ม
        Stun,        // ข้ามเทิร์น
    }
}
