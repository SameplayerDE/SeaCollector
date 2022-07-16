using HxTime;
using Microsoft.Xna.Framework;

namespace SeaCollector.Entities
{
    public class VelocityGameObject : GameObject
    {
        public long SimulationSteps = 1;
        public Vector3 Velocity;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var simulationTime = Time.Instance.DeltaSecondsF / SimulationSteps;
            Position += Velocity * simulationTime;
            Velocity = Vector3.Lerp(Velocity, Vector3.Zero, 0.1f);
        }
    }
}