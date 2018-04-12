using Effects;
using System;
using System.Runtime.InteropServices;

namespace Reverb
{
    class ReverbParameters : EffectParameters
    {
        public int rate;
        public int oversamplefactor;
        public float ertolate;
        public float erefwet;
        public float dry;
        public float ereffactor;
        public float erefwidth;
        public float width;
        public float wet;
        public float wander;
        public float bassb;
        public float spin;
        public float inputlpf;
        public float basslpf;
        public float damplpf;
        public float outputlpf;
        public float rt60;
        public float delay;

        public ReverbParameters()
        {

        }
    }


    class Reverb : Effect
    {
        [DllImport("EffectsLib.dll")]
        public static extern void reverb_helper_def(IntPtr inbuf, IntPtr outbuf, int length);
        [DllImport("EffectsLib.dll")]
        public static extern void reverb_helper_param(IntPtr inbuf, IntPtr outbuf, int length, int rate,
        int oversamplefactor, float ertolate, float erefwet, float dry, float ereffactor,
        float erefwidth, float width, float wet, float wander, float bassb, float spin, float inputlpf,
        float basslpf, float damplpf, float outputlpf, float rt60, float delay);

        private int rate;
        private int oversamplefactor;
        private float ertolate;
        private float erefwet;
        private float dry;
        private float ereffactor;
        private float erefwidth;
        private float width;
        private float wet;
        private float wander;
        private float bassb;
        private float spin;
        private float inputlpf;
        private float basslpf;
        private float damplpf;
        private float outputlpf;
        private float rt60;
        private float delay;
        private bool use_defaults;

        public Reverb()
        {
            rate = 0;
            oversamplefactor = 0;
            ertolate = 0;
            erefwet = 0;
            dry = 0;
            ereffactor = 0;
            erefwidth = 0;
            width = 0;
            wet = 0;
            wander = 0;
            bassb = 0;
            spin = 0;
            inputlpf = 0;
            basslpf = 0;
            damplpf = 0;
            outputlpf = 0;
            rt60 = 0;
            delay = 0;
            use_defaults = true;
        }

        public Reverb(ReverbParameters param)
        {
            rate = param.rate;
            oversamplefactor = param.oversamplefactor;
            ertolate = param.ertolate;
            erefwet = param.erefwet;
            dry = param.dry;
            ereffactor = param.ereffactor;
            erefwidth = param.erefwidth;
            width = param.width;
            wet = param.wet;
            wander = param.wander;
            bassb = param.bassb;
            spin = param.spin;
            inputlpf = param.inputlpf;
            basslpf = param.basslpf;
            damplpf = param.damplpf;
            outputlpf = param.outputlpf;
            rt60 = param.rt60;
            delay = param.delay;

            use_defaults = false;
        }

        public EffectParameters GetParameters()
        {
            ReverbParameters param = new ReverbParameters();
            param.rate = rate;
            param.oversamplefactor = oversamplefactor;
            param.ertolate = ertolate;
            param.erefwet = erefwet;
            param.dry = dry;
            param.ereffactor = ereffactor;
            param.erefwidth = erefwidth;
            param.width = width;
            param.wet = wet;
            param.wander = wander;
            param.bassb = bassb;
            param.spin = spin;
            param.inputlpf = inputlpf;
            param.basslpf = basslpf;
            param.damplpf = damplpf;
            param.outputlpf = outputlpf;
            param.rt60 = rt60;
            param.delay = delay;

            return (EffectParameters)param;
        }

        public void ProcessBlock(ref FloatToInt[] input, ref FloatToInt[] output, int length)
        {
            IntPtr inBuf = new IntPtr(input[0].IntVal);
            IntPtr outBuf = new IntPtr(output[0].IntVal);
            if (use_defaults)
            {
                reverb_helper_def(inBuf, outBuf, length);
            }
            else
            {
                reverb_helper_param(inBuf, outBuf, length, rate, oversamplefactor, ertolate, erefwet, dry, ereffactor, erefwidth, width,
                wet, wander, bassb, spin, inputlpf, basslpf, damplpf, outputlpf, rt60, delay);
            }
        }

        public void SetParameters(ref EffectParameters parameters)
        {
            ReverbParameters param = (ReverbParameters)parameters;
            rate = param.rate;
            oversamplefactor = param.oversamplefactor;
            ertolate = param.ertolate;
            erefwet = param.erefwet;
            dry = param.dry;
            ereffactor = param.ereffactor;
            erefwidth = param.erefwidth;
            width = param.width;
            wet = param.wet;
            wander = param.wander;
            bassb = param.bassb;
            spin = param.spin;
            inputlpf = param.inputlpf;
            basslpf = param.basslpf;
            damplpf = param.damplpf;
            outputlpf = param.outputlpf;
            rt60 = param.rt60;
            delay = param.delay;

            use_defaults = false;
        }
    }
}
