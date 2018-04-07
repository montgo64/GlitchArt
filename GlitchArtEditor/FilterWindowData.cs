using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlitchArtEditor
{
    public class FilterWindowData
    {
        public FilterWindowData()
        {
        }

        private static String[] EchoParameters = { "Delay", "Decay", "History Length" };
        private static int[] EchoValues = { 1, 1, 10 };

        private static String[] ReverbParameters = { "One", "Two", "Three" };
        private static int[] ReverbValues = { 1, 2, 3 };

        Dictionary<String, String[]> parameters = new Dictionary<String, String[]>()
        {
            {"Echo", EchoParameters },
            {"Reverb", ReverbParameters },
        };

        Dictionary<String, int[]> defaults = new Dictionary<String, int[]>()
        {
            {"Echo", EchoValues },
            {"Reverb", ReverbValues },
        };

        public String[] getParameters(String filter)
        {
            return parameters[filter];
        }

        public int[] getDefaults(String filter)
        {
            return defaults[filter];
        }

        public bool containsKey(String filter)
        {
            return (parameters.ContainsKey(filter) && defaults.ContainsKey(filter));
        }
    }
}
