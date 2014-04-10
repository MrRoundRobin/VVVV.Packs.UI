using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a textbox UIElement", Name = "Textbox")]
    public class WPFTextboxNode : WPFGenericNode<TextBox>, IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Width", DefaultValue = 120, Order = 9)]
        public IDiffSpread<int> WidthIn;

        [Output("Value", Order = 1)]
        public ISpread<string> ValueOut;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is TextBox))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = ((TextBox)UIElementOut[i]);

            if (WidthIn.IsChanged || force)
                uiElement.Width = WidthIn[i];

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (ValueOut[i] != uiElement.Text || force)
                ValueOut[i] = uiElement.Text;
        }
    }
}
