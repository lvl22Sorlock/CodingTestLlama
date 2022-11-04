using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class ActivateCheckpoint : Simulation.Event<ActivateCheckpoint>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public Checkpoint checkpoint;

        public override void Execute()
        {
            model.spawnPoint.transform.position = checkpoint.transform.position;
        }
    }
}