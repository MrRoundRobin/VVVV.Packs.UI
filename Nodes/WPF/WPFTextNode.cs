using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a textblock UIElement", Name = "Text")]
    public class WPFTextNode : WPFGenericNode<TextBlock>, IPluginEvaluate
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global
        
        [Input("Text", DefaultString = "VVVV", Order = 2)]
        public IDiffSpread<string> TextIn;

        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is TextBlock))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = (TextBlock)UIElementOut[i];

            if (TextIn.IsChanged || force)
                uiElement.Text = TextIn[i];
        }
    }
}
