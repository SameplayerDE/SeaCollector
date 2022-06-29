using System;

namespace SeaCollector.Entities
{
    public class Player : GameObject
    {

        public float SailFactor = 0f;
        public float SailDirection = 0f;

        public void ChangeSailFactor(float value)
        {
            SailFactor += value;
            SailFactor = Math.Clamp(SailFactor, 0f, 1f);
        }
        
        public void ChangeSailDirection(float value)
        {
            SailDirection += value;
            SailDirection = Math.Clamp(SailDirection, -1f, 1f);
        }
    }
}