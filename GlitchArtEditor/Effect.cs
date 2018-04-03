namespace Effects
{
    public class EffectParameters
    {
        public EffectParameters() { }
    }

    interface Effect
    {
        void ProcessBlock(ref float[] input, ref float[] output, int length);

        void SetParameters(ref EffectParameters param);

        EffectParameters GetParameters();


    }

}