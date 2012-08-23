using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gemstone_Hunter
{
    class ProfileFloor
    {

        public float grip;
        public float jump;

        public ProfileFloor(float grip, float jump)
        {
            this.jump = jump;
            this.grip = grip;
        }

    }
}
