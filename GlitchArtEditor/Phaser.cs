using Effects;
using System;
using System.Collections.Generic;

namespace PhaserEffect
{
    public struct PhaserState
    {
        public float samplerate;
        public int skipcount;
        public double[] old; // must be as large as MAX_STAGES
        public double gain;
        public double fbout;
        public double outgain;
        public double lfoskip;
        public double phase;
        public int laststages;
    }

    class PhaserParameters : EffectParameters
    {
        /// <summary>
        /// Default constructor. Sets stages to 1, dryWet to 1,
        /// and freq to 1.
        /// </summary>
        public PhaserParameters()
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Stages", new Parameter { name = "Stages", value = 2, minValue = 2, maxValue = 24, frequency = 2 });
            parameters.Add("Dry Wet", new Parameter { name = "Dry Wet", value = 128, minValue = 0, maxValue = 255, frequency = 1 });
            parameters.Add("Frequency", new Parameter { name = "Frequency", value = 1, minValue = 1, maxValue = 4, frequency = 0.1 });
            parameters.Add("Start Phase", new Parameter { name = "Start Phase", value = 0, minValue = 0, maxValue = 360, frequency = 10 });
            parameters.Add("Depth", new Parameter { name = "Depth", value = 100, minValue = 0, maxValue = 255, frequency = 1 });
            parameters.Add("Feedback", new Parameter { name = "Feedback", value = 0, minValue = -100, maxValue = 100, frequency = 10 });
            parameters.Add("Output Gain", new Parameter { name = "Output Gain", value = -6, minValue = -30, maxValue = 30, frequency = 1 });

            SetParams(parameters);
        }

        /// <summary>
        /// Constructor. Sets parameters for stages, dryWet, and freq.
        /// </summary>
        public PhaserParameters(int stages, int dryWet, double freq, double phase, int depth, int feedback, double outGain)
        {
            Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();
            parameters.Add("Stages", new Parameter { name = "Stages", value = stages, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Dry Wet", new Parameter { name = "Dry Wet", value = dryWet, minValue = 0, maxValue = 100, frequency = 1 });
            parameters.Add("Frequency", new Parameter { name = "Frequency", value = freq, minValue = 1, maxValue = 100, frequency = 1 });
            parameters.Add("Start Phase", new Parameter { name = "Start Phase", value = phase, minValue = 0, maxValue = 360, frequency = 10 });
            parameters.Add("Depth", new Parameter { name = "Depth", value = 100, minValue = depth, maxValue = 255, frequency = 1 });
            parameters.Add("Feedback", new Parameter { name = "Feedback", value = feedback, minValue = -100, maxValue = 100, frequency = 10 });
            parameters.Add("Output Gain", new Parameter { name = "Output Gain", value = outGain, minValue = -30, maxValue = 30, frequency = 1 });

            SetParams(parameters);
        }
    }

    class Phaser : Effect
    {
        const double phaser1foshape = 4.0;
        const int lfoskipsamples = 20;
        const int num_stages = 24;
        PhaserState state;

        int mStages;
        int mDryWet;
        double mFreq;
        double mPhase;
        int mDepth;
        int mFeedback;
        double mOutGain;

        public Phaser()
        {
            state.samplerate = 0;
            state.old = new double[num_stages];
            state.skipcount = 0;
            state.gain = 0;
            state.fbout = 0;
            state.laststages = 0;
            state.outgain = 0;

            mStages = 0;
            mDryWet = 0;
            mFreq = 0;
            mPhase = 0;
            mDepth = 0;
            mFeedback = 0;
            mOutGain = 0;
        }

        public Phaser(PhaserParameters phaser)
        {
            state.samplerate = 0;
            state.old = new double[num_stages];
            state.skipcount = 0;
            state.gain = 0;
            state.fbout = 0;
            state.laststages = 0;
            state.outgain = 0;

            foreach (Parameter parameter in phaser.GetParams().Values)
            {
                if (parameter.name.Equals("Stages"))
                {
                    mStages = (int)parameter.value;
                }
                else if (parameter.name.Equals("Dry Wet"))
                {
                    mDryWet = (int)parameter.value;
                }
                else if (parameter.name.Equals("Frequency"))
                {
                    mFreq = parameter.value;
                }
                else if (parameter.name.Equals("Start Phase"))
                {
                    mPhase = parameter.value;
                }
                else if (parameter.name.Equals("Depth"))
                {
                    mDepth = (int)parameter.value;
                }
                else if (parameter.name.Equals("Feedback"))
                {
                    mFeedback = (int)parameter.value;
                }
                else if (parameter.name.Equals("Output Gain"))
                {
                    mOutGain = parameter.value;
                }
            }
        }

        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            for (int j = state.laststages; j < mStages; j++)
            {
                state.old[j] = 0;
            }
            state.laststages = mStages;
            state.lfoskip = mFreq * 2 * System.Math.PI / state.samplerate;
            state.phase = mPhase * System.Math.PI / 180;
            state.outgain = System.Math.Pow(10.0, mOutGain / 20.0);

            for(int i = 0; i < length; i++)
            {
                double inv = input[i].FloatVal;
                double m = inv + state.fbout * mFeedback / 101;

                if(((double)state.skipcount % lfoskipsamples) == 0)
                {
                    state.gain = (1.0 + System.Math.Cos((double)state.skipcount * state.lfoskip + state.phase)) / 2.0;
                    state.gain = expm1(state.gain * phaser1foshape) / expm1(phaser1foshape);
                    state.gain = 1.0 - state.gain / 255.0 * mDepth;
                }

                for(int j = 0; j < mStages; j++)
                {
                    double tmp = state.old[j];
                    state.old[j] = state.gain * tmp + m;
                    m = tmp - state.gain * state.old[j];
                }
                state.fbout = m;

                output[i].FloatVal = (float)(state.outgain * (m * mDryWet + inv * (255 - mDryWet)) / 255);

            }
        }

        double expm1(double input)
        {
            if (System.Math.Abs(input) < 0.00001)
                return input + 0.5 * input * input;
            else
                return System.Math.Exp(input) - 1.0;
        }
    }
}
