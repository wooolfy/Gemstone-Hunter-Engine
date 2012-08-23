using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gemstone_Hunter
{
    class ProfileArea
    {
        public Vector2 gravity;
        public Vector2 maxVelocity;
        public Vector2 acceleration;
        public float friction;

        public ProfileArea(Vector2 gravity, Vector2 maxVelocity, Vector2 acceleration, float friction)
        {
            this.gravity = gravity;
            this.maxVelocity = maxVelocity;
            this.acceleration = acceleration;
            this.friction = friction;
        }

    }

}
