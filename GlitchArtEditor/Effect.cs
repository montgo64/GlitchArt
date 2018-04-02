

interface Effect
{
    Effect();

    void ProcessBlock(ref float[] input, ref float[] output, int length);

}