namespace Assets.Scripts
{
    public class Mod : ModApi.Mods.GameMod
    {
        private Mod() : base()
        {
        }

        public static Mod Instance { get; } = GetModInstance<Mod>();

        protected override void OnModInitialized()
        {

        }
    }
}