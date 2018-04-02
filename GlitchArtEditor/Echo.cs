

public class Echo : Effect
{
    private double delay;
    private double decay;
    private double[] history;
    private int histPos;
    private int histLen;

    Echo()
    {
        delay = 1.0;
        decay = 0.5;
        histPos = 0;
    }

    void ProcessBlock(ref float[] input, ref float[] output, int length)
    {
        for (int i = 0; i < length; i++, histPos++)
        {
            if (histPos == histLen)
            {
                histPos = 0;
            }
            history[histPos] = output[i] = input[i] + history[histPos] * decay;
        }
    }
}