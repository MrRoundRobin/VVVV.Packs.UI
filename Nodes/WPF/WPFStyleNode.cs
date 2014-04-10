using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using Size = System.Drawing.Size;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a Style", Name = "Style")]
    public class WPFStylenNode : IPluginEvaluate
    {
        private readonly Dictionary<int, bool> _onClick = new Dictionary<int, bool>();

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Font Size", DefaultValue = 12, Order = 0)]
        public IDiffSpread<double> FontSizeIn;

        [Input("Background Color", DefaultColor = new[] {0.95, 0.95, 0.95, 1.0}, HasAlpha = false, Order = 5)]
        public IDiffSpread<RGBAColor> BackColorIn;

        [Output("Style", Order = 0)]
        public ISpread<Style> StyleOut;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            if (StyleOut.SliceCount != SpreadMax)
            {
                if (StyleOut.SliceCount < SpreadMax)
                {
                    var i = StyleOut.SliceCount;
                    for (StyleOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        StyleOut[i] = CreateStyle(i);
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (StyleOut[i] == null)
                    StyleOut[i] = CreateStyle(i);

                if (FontSizeIn.IsChanged)
                    StyleOut[i].Setters.Where(setter => (setter as Setter).Property == Control.FontSizeProperty).ToList().ForEach(fontSizeSetter => (fontSizeSetter as Setter).Value = FontSizeIn[i]);
            }
        }

        private Style CreateStyle(int i)
        {
            var style = new Style();
            
            style.Setters.Add(new Setter(Control.FontSizeProperty,FontSizeIn[i]));
            
            return style;
        }
    }
}
