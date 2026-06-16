using Gameplay.BattleEncounter.Characters.Enums;
using Spine.Unity;
using UnityEngine;

namespace Gameplay.BattleEncounter.Characters.Data
{
    public abstract class CharacterDefinition : ScriptableObject
    {
        public int MaxHealth = 30;
        [Range(0, 10)]
        public int StartSize = 5;

        [Header("Form skeletons")]
        public SkeletonDataAsset TinySkeleton;
        public SkeletonDataAsset NormalSkeleton;
        public SkeletonDataAsset GiantSkeleton;

        [Header("Size root Y offset per form")]
        public float TinyOffsetY;
        public float NormalOffsetY;
        public float GiantOffsetY;

        public SkeletonDataAsset SkeletonFor(SizeForm form)
        {
            switch (form)
            {
                case SizeForm.Tiny: return TinySkeleton;
                case SizeForm.Giant: return GiantSkeleton;
                default: return NormalSkeleton;
            }
        }

        public float OffsetYFor(SizeForm form)
        {
            switch (form)
            {
                case SizeForm.Tiny: return TinyOffsetY;
                case SizeForm.Giant: return GiantOffsetY;
                default: return NormalOffsetY;
            }
        }
    }
}
