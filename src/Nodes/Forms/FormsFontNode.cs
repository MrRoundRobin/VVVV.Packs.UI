using System.Drawing;

using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Font for controls", Name = "Font")]
    public class FormsFontNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Bold", IsToggle = true, Order = 2)]
        public IDiffSpread<bool> BoldIn;

        [Input("Font", EnumName = "SystemFonts", Order = 0)]
        public IDiffSpread<EnumEntry> FontIn;

        [Output("Font", Order = 0)]
        public ISpread<Font> FontOut;

        [Input("Italic", IsToggle = true, Order = 3)]
        public IDiffSpread<bool> ItalicIn;

        [Input("Size", DefaultValue = 8.25, MinValue = 0.01, Order = 1)]
        public IDiffSpread<double> SizeIn;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            if (!FontIn.IsChanged && !ItalicIn.IsChanged && !BoldIn.IsChanged && !SizeIn.IsChanged) return;

            FontOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (SizeIn[i] <= 0) continue;

                var style = FontStyle.Regular;
                if (BoldIn[i] && ItalicIn[i])
                    style = FontStyle.Bold | FontStyle.Italic;
                else if (BoldIn[i])
                    style = FontStyle.Bold;
                else if (ItalicIn[i])
                    style = FontStyle.Italic;

                FontOut[i] = new Font(FontIn[i].Name, (float)SizeIn[i], style);
            }
        }
    }
}
