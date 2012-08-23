using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gemstone_Hunter
{
    public class TileProfiles
    {
        public static ProfileArea air = 
            new ProfileArea(new Vector2(0, 20), 
                            new Vector2(300,500), 
                            new Vector2(100,0), 
                            0.1f);
        public static ProfileArea water =
            new ProfileArea(new Vector2(0, 2),
                            new Vector2(150, 150),
                            new Vector2(50, 0),
                            0.3f);
        public static ProfileArea moon =
            new ProfileArea(new Vector2(0, 2),
                            new Vector2(150, 150),
                            new Vector2(50, 0),
                            0.3f);
        public static ProfileArea stuck =
            new ProfileArea(new Vector2(0, 20),
                            new Vector2(50, 500),
                            new Vector2(10, 0),
                            0.95f);
        public static ProfileFloor grass = 
            new ProfileFloor(1.0f, 1.0f);
        public static ProfileFloor sand =
            new ProfileFloor(1.0f, 0.7f);
        public static ProfileFloor ice =
            new ProfileFloor(0.1f, 1.0f);
    }
}
