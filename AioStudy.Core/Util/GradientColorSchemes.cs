using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AioStudy.Core.Util
{
    public static class GradientColorSchemes
    {
        public record GradientColors(string Color1, string Color2, string Color3);

        public static class Timer
        {
            /// <summary>Rotes Farbschema</summary>
            public static GradientColors Red => new("#FF6B6B", "#C92A2A", "#FF8787");

            /// <summary>Gelbes Farbschema</summary>
            public static GradientColors Yellow => new("#FFEB3B", "#FBC02D", "#FFF176");

            /// <summary>Oranges Farbschema</summary>
            public static GradientColors Orange => new("#FFA726", "#FB8C00", "#FFB74D");

            /// <summary>Grünes Farbschema</summary>
            public static GradientColors Green => new("#66BB6A", "#43A047", "#81C784");

            /// <summary>Blaues Farbschema</summary>
            public static GradientColors Blue => new("#608BC1", "#133E87", "#a5bacc");

            /// <summary>Graues Farbschema</summary>
            public static GradientColors Stopped => new("#3A3D45", "#3A3D45", "#a5bacc");
        }

        public static class TimerBar
        {
            /// <summary>Paused Farbschema</summary>
            public static GradientColors Paused => new("#6F1414", "#C92A2A", "#A03030");

            /// <summary>Running Farbschema</summary>
            public static GradientColors Running => new("#608BC1", "#133E87", "#0F67B3");

            /// <summary>ShortBreak Farbschema</summary>
            public static GradientColors ShortBreak => new("#FFEB3B", "#FBC02D", "#FFF176");

            /// <summary>MidBreak Farbschema</summary>
            public static GradientColors MidBreak => new("#FFA726", "#FB8C00", "#FFB74D");

            /// <summary>LongBreak Farbschema</summary>
            public static GradientColors LongBreak => new("#66BB6A", "#43A047", "#81C784");
        }

        public static void ApplyColors(GradientColors colors, Action<string, string, string> setColors)
        {
            setColors(colors.Color1, colors.Color2, colors.Color3);
        }
    }
}
