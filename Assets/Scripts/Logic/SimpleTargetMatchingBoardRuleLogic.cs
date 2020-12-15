using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public class Equal2Logic : TargetMatchingLogic
    {
        public Equal2Logic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count != 1)
            {
                return -1;
            }
            return materials[0];
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 1)
            {
                return true;
            }
            return false;
        }
    }
    public class EqualNLogic : TargetMatchingLogic
    {
        public EqualNLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count < 1)
            {
                return -1;
            }
            if (materials.Count != materials[0])
            {
                return -1;
            }
            return materials[0];
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials[materials.Count - 1] != materials[0])
            {
                return true;
            }
            return false;
        }
    }
    public class Sum2Logic : TargetMatchingLogic2
    {
        public Sum2Logic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count != 2)
            {
                return -1;
            }
            return materials[0] + materials[1];
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 2)
            {
                return true;
            }
            return false;
        }
    }
    public class Product3Logic : TargetMatchingLogic
    {
        public Product3Logic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count != 3)
            {
                return -1;
            }
            return materials[0] * materials[1] * materials[2];
        }
        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 3)
            {
                return true;
            }
            return false;
        }
    }
}
