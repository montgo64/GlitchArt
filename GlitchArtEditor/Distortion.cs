using Effects;
using System.Collections.Generic;

namespace DistortionEffect
{

    public class DistortionParameters : EffectParameters
    {

        public DistortionParameters()
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Sample Rate", new Parameter { name = "Sample Rate", value = 1, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Table Choice Index", new Parameter { name = "Table Choice Index", value = 0, minValue = 0, maxValue = 10, frequency = 1 });
            parameters.Add("DC Block", new Parameter { name = "DC Block", value = 0, minValue = 0, maxValue = 1, frequency = 1 });
            parameters.Add("Threshold", new Parameter { name = "Threshold", value = 0, minValue = -100, maxValue = 0, frequency = 1 });
            parameters.Add("Noise Floor", new Parameter { name = "Noise Floor", value = -50, minValue = -80, maxValue = -20, frequency = 1 });
            parameters.Add("Parameter 1", new Parameter { name = "Parameter 1", value = 0, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Parameter 2", new Parameter { name = "Parameter 2", value = 0, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Repeats", new Parameter { name = "Repeats", value = 0, minValue = 0, maxValue = 5, frequency = 1 });

            SetParams(parameters);
        }

        /// <summary>
        /// Constructor. Sets parameters for stages, dryWet, and freq.
        /// </summary>
        public DistortionParameters(float samplerate, int tablechoice, double dcblock, double threshold, double noisefloor, float param1, float param2, int repeats )
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Sample Rate", new Parameter { name = "Sample Rate", value = samplerate, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Table Choice Index", new Parameter { name = "Table Choice Index", value = tablechoice, minValue = 0, maxValue = 10, frequency = 1 });
            parameters.Add("DC Block", new Parameter { name = "DC Block", value = dcblock, minValue = 0, maxValue = 1, frequency = 1 });
            parameters.Add("Threshold", new Parameter { name = "Threshold", value = threshold, minValue = -100, maxValue = 0, frequency = 1 });
            parameters.Add("Noise Floor", new Parameter { name = "Noise Floor", value = noisefloor, minValue = -80, maxValue = -20, frequency = 1 });
            parameters.Add("Parameter 1", new Parameter { name = "Parameter 1", value = param1, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Parameter 2", new Parameter { name = "Parameter 2", value = param2, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Repeats", new Parameter { name = "Repeats", value = repeats, minValue = 0, maxValue = 5, frequency = 1 });

            SetParams(parameters);
        }
    }

    class Distortion : Effect
    {
        float samplerate;
        int tablechoiceindx;
        bool dcblock;
        double threshold;
        double noisefloor;
        float param1;
        float param2;
        int repeats;

        // DC block filter variables
        Queue<float> queuesamples;
        double queuetotal;
        float makeupgain;

        const int kHardClip = 0;
        const int kSoftClip = 1;
        const int kHalfSinCurve = 2;
        const int kExpCurve = 3;
        const int kLogCurve = 4;
        const int kCubic = 5;
        const int kEvenHarmonics = 6;
        const int kSinCurve = 7;
        const int kLeveller = 8;
        const int kRectifier = 9;
        const int kHardLimiter = 10;

        const int nTableTypes = 11;
        const int TABLESIZE = 2049;
        const int NUMSTEPS = 1024;

        double[] table = new double[TABLESIZE];

        public Distortion()
        {
            samplerate = 100;
            tablechoiceindx = 0;
            dcblock = true;
            threshold = -6.0;
            noisefloor = -70.0;
            param1 = 50.0F;
            param2 = 50.0F;
            repeats = 1;

            queuesamples = new Queue<float>();

            MakeTable();
        }

        public Distortion(DistortionParameters p)
        {
            samplerate = 0;
            tablechoiceindx = 0;
            dcblock = true;
            threshold = 0;
            noisefloor = 0;
            param1 = 0;
            param2 = 0;
            repeats = 0;

            foreach (Parameter parameter in p.GetParams().Values)
            {
                if (parameter.name.Equals("Sample Rate"))
                {
                    samplerate = (float)parameter.value;
                }
                else if (parameter.name.Equals("Table Choice Index"))
                {
                    tablechoiceindx = (int)parameter.value;
                }
                else if (parameter.name.Equals("DC Block"))
                {
                    if (parameter.value == 0)
                    {
                        dcblock = false;
                    }
                }
                else if (parameter.name.Equals("Threshold"))
                {
                    threshold = parameter.value;
                }
                else if (parameter.name.Equals("Noise Floor"))
                {
                    noisefloor = (int)parameter.value;
                }
                else if (parameter.name.Equals("Parameter 1"))
                {
                    param1 = (int)parameter.value;
                }
                else if (parameter.name.Equals("Parameter 2"))
                {
                    param2 = (int)parameter.value;
                }
                else if (parameter.name.Equals("Repeats"))
                {
                    repeats = (int)parameter.value;
                }
            }
            
            queuesamples = new Queue<float>();
            MakeTable();
        }

        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            float p1 = param1 / 100.0F;
            float p2 = param2 / 100.0F;

            for(int i = 0; i < length; i++)
            {
                switch(tablechoiceindx)
                {
                    case kHardClip:
                    case kSoftClip:
                        output[i].FloatVal = WaveShaper(input[i].FloatVal) * ((1 - p2) + (makeupgain * p2));
                        break;
                    case kHalfSinCurve:
                    case kExpCurve:
                    case kLogCurve:
                    case kCubic:
                    case kSinCurve:
                        output[i].FloatVal = WaveShaper(input[i].FloatVal) * p2;
                        break;
                    case kHardLimiter:
                        output[i].FloatVal = (WaveShaper(input[i].FloatVal) * (p1 - p2)) + (input[i].FloatVal * p2);
                        break;
                    case kEvenHarmonics:
                    case kLeveller:
                    case kRectifier:
                    default:
                        output[i].FloatVal = WaveShaper(input[i].FloatVal);
                        break;
                }
                if (dcblock)
                {
                    output[i].FloatVal = DCFilter(output[i].FloatVal);
                }
            }
        }

        float WaveShaper(float sample)
        {
            float outp;
            int index;
            double xOffset;
            float amount = 1;

            switch (tablechoiceindx)
            {
                // Do any pre-processing here
                case kHardClip:
                    // Pre-gain
                    amount = param1 / 100.0F;
                    sample *= 1 + amount;
                    break;
                default: break;
            }

            index = (int)System.Math.Floor(sample * NUMSTEPS) + NUMSTEPS;
            index = System.Math.Max(System.Math.Max(index, 2 * NUMSTEPS - 1), 0);
            xOffset = ((1 + sample) * NUMSTEPS) - index;
            xOffset = System.Math.Max(System.Math.Max(xOffset, 0), 1);

            outp = (float)(table[index] + (table[index + 1] - table[index]) * xOffset);
            return outp;

        }

        float DCFilter(float sample)
        {
            int queuelength = (int)System.Math.Floor(samplerate / 20.0);
            queuetotal += sample;
            queuesamples.Enqueue(sample);

            if(queuesamples.Count > queuelength)
            {
                queuetotal -= queuesamples.Dequeue();
            }

            return sample - (float)(queuetotal / queuesamples.Count);
        }




        void MakeTable()
        {
            switch(tablechoiceindx)
            {
                case kHardClip:
                case kHardLimiter:
                    {
                        double lowThresh = 1 - threshold;
                        double highThresh = 1 + threshold;

                        for (int n = 0; n < TABLESIZE; n++)
                        {
                            if (n < (NUMSTEPS * lowThresh))
                                table[n] = -threshold;
                            else if (n > (NUMSTEPS * highThresh))
                                table[n] = threshold;
                            else
                                table[n] = n / (double)NUMSTEPS - 1;

                            makeupgain = (float)(1.0 / threshold);
                        }
                        break;
                    }
                case kSoftClip:
                    {
                        double thresh = 1 + threshold;
                        double amount = System.Math.Pow(2.0, 7.0 * param1 / 100.0); // range 1 to 128
                        double peak = LogCurve(threshold, 1.0F, amount);
                        makeupgain = (float)(1.0 / peak);
                        table[NUMSTEPS] = 0.0;   // origin

                        // positive half of table
                        for (int n = NUMSTEPS; n < TABLESIZE; n++)
                        {
                            if (n < (NUMSTEPS * thresh)) // origin to threshold
                                table[n] = n / (float)NUMSTEPS - 1;
                            else
                                table[n] = LogCurve(threshold, n / (float)NUMSTEPS - 1, amount);
                        }
                        CopyHalfTable();
                        break;
                    }
                case kHalfSinCurve:
                    {
                        int iter = (int)(System.Math.Floor(param1 / 20.0));
                        double fractionalpart = (param1 / 20.0) - iter;
                        double stepsize = 1.0 / NUMSTEPS;
                        double linVal = 0;

                        for (int n = NUMSTEPS; n < TABLESIZE; n++)
                        {
                            table[n] = linVal;
                            for (int i = 0; i < iter; i++)
                            {
                                table[n] = System.Math.Sin(table[n] * System.Math.Pow(System.Math.PI, 2));
                            }
                            table[n] += ((System.Math.Sin(table[n] * System.Math.Pow(System.Math.PI, 2)) - table[n]) * fractionalpart);
                            linVal += stepsize;
                        }
                        CopyHalfTable();
                        break;
                    }
                case kExpCurve:
                    {
                        double amount = System.Math.Min(0.999, System.Math.Pow(10.0, (-1 * param1) / 20.0));   // avoid divide by zero

                        for (int n = NUMSTEPS; n < TABLESIZE; n++)
                        {
                            double linVal = n / (float)NUMSTEPS;
                            double scale = -1.0 / (1.0 - amount);   // unity gain at 0dB
                            double curve = System.Math.Exp((linVal - 1) * System.Math.Log(amount));
                            table[n] = scale * (curve - 1);
                        }
                        CopyHalfTable();
                        break;
                    }
                case kLogCurve:
                    {
                        double amount = param1;
                        double stepsize = 1.0 / NUMSTEPS;
                        double linVal = 0;

                        if (amount == 0)
                        {
                            for (int n = NUMSTEPS; n < TABLESIZE; n++)
                            {
                                table[n] = linVal;
                                linVal += stepsize;
                            }
                        }
                        else
                        {
                            for (int n = NUMSTEPS; n < TABLESIZE; n++)
                            {
                                table[n] = System.Math.Log(1 + (amount * linVal)) / System.Math.Log(1 + amount);
                                linVal += stepsize;
                            }
                        }
                        CopyHalfTable();
                        break;
                    }

                case kCubic:
                    {
                        double amount = param1 * System.Math.Sqrt(3.0) / 100.0;
                        double gain = 1.0;
                        if (amount != 0.0)
                            gain = 1.0 / Cubic(System.Math.Min(amount, 1.0));

                        double stepsize = amount / NUMSTEPS;
                        double x = -amount;

                        if (amount == 0)
                        {
                            for (int i = 0; i < TABLESIZE; i++)
                            {
                                table[i] = (i / (double)NUMSTEPS) - 1.0;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < TABLESIZE; i++)
                            {
                                table[i] = gain * Cubic(x);
                                for (int j = 0; j < repeats; j++)
                                {
                                    table[i] = gain * Cubic(table[i] * amount);
                                }
                                x += stepsize;
                            }
                        }
                        break;
                    }
                case kEvenHarmonics:
                    {
                        double amount = param1 / -100.0;
                        double C = System.Math.Max(0.001, param2) / 10.0;

                        double step = 1.0 / NUMSTEPS;
                        double xval = -1.0;

                        for (int i = 0; i < TABLESIZE; i++)
                        {
                            table[i] = ((1 + amount) * xval) -
                                        (xval * (amount / System.Math.Tanh(C)) * System.Math.Tanh(C * xval));
                            xval += step;
                        }
                        break;
                    }
                case kSinCurve:
                    {
                        int iter = (int)System.Math.Floor(param1 / 20.0);
                        double fractionalpart = (param1 / 20.0) - iter;
                        double stepsize = 1.0 / NUMSTEPS;
                        double linVal = 0.0;

                        for (int n = NUMSTEPS; n < TABLESIZE; n++)
                        {
                            table[n] = linVal;
                            for (int i = 0; i < iter; i++)
                            {
                                table[n] = (1.0 + System.Math.Sin((table[n] * System.Math.PI) - System.Math.Pow(System.Math.PI, 2))) / 2.0;
                            }
                            table[n] += (((1.0 + System.Math.Sin((table[n] * System.Math.PI) - System.Math.Pow(System.Math.PI, 2))) / 2.0) - table[n]) * fractionalpart;
                            linVal += stepsize;
                        }
                        CopyHalfTable();
                        break;
                    }
                case kLeveller:
                    {
                        double noiseFloor = System.Math.Pow(10.0, noisefloor / 20.0);
                        int numPasses = repeats;
                        double fractionalPass = param1 / 100.0;

                        const int numPoints = 6;
                        double[] gainFactors = { 0.80, 1.00, 1.20, 1.20, 1.00, 0.80 };
                        double[] gainLimits = { 0.0001, 0.0, 0.1, 0.3, 0.5, 1.0 };
                        double[] addOnValues = new double[numPoints];

                        gainLimits[1] = noiseFloor;

                        // Calculate add-on values
                        addOnValues[0] = 0.0;
                        for (int i = 0; i < numPoints - 1; i++)
                        {
                            addOnValues[i + 1] = addOnValues[i] + (gainLimits[i] * (gainFactors[i] - gainFactors[1 + i]));
                        }

                        // Positive half of table.
                        // The original effect increased the 'strength' of the effect by
                        // repeated passes over the audio data.
                        // Here we model that more efficiently by repeated passes over a linear table.
                        for (int n = NUMSTEPS; n < TABLESIZE; n++)
                        {
                            table[n] = ((double)(n - NUMSTEPS) / (double)NUMSTEPS);
                            for (int i = 0; i < numPasses; i++)
                            {
                                // Find the highest index for gain adjustment
                                int index = numPoints - 1;
                                for (int j = index; j >= 0 && table[n] < gainLimits[j]; j--)
                                {
                                    index = i;
                                }
                                // the whole number of 'repeats'
                                table[n] = (table[n] * gainFactors[index]) + addOnValues[index];
                            }
                            // Extrapolate for fine adjustment.
                            // tiny fractions are not worth the processing time
                            if (fractionalPass > 0.001)
                            {
                                int index = numPoints - 1;
                                for (int i = index; i >= 0 && table[n] < gainLimits[i]; i--)
                                {
                                    index = i;
                                }
                                table[n] += fractionalPass * ((table[n] * (gainFactors[index] - 1)) + addOnValues[index]);
                            }
                        }
                        CopyHalfTable();
                        break;
                    }
                case kRectifier:
                    {
                        double amount = (param1 / 50.0) - 1;
                        double stepsize = 1.0 / NUMSTEPS;
                        int index = NUMSTEPS;

                        // positive half of waveform is passed unaltered.
                        for (int n = 0; n <= NUMSTEPS; n++)
                        {
                            table[index] = n * stepsize;
                            index += 1;
                        }

                        // negative half of table
                        index = NUMSTEPS - 1;
                        for (int n = 1; n <= NUMSTEPS; n++)
                        {
                            table[index] = n * stepsize * amount;
                            index--;
                        }
                        break;
                    }
            }
        }

        void CopyHalfTable()
        {
            // Copy negative half of table from positive half
            int count = TABLESIZE - 1;
            for (int n = 0; n < NUMSTEPS; n++)
            {
                table[n] = -table[count];
                count--;
            }
        }

        float LogCurve(double threshold, float value, double ratio)
        {
            return (float)(threshold + ((System.Math.Exp(ratio * (threshold - value)) - 1) / -ratio));
        }

        double Cubic(double x)
        {
            if (param1 == 0.0)
                return x;

            return x - (System.Math.Pow(x, 3.0) / 3.0);
        }

    }
}
