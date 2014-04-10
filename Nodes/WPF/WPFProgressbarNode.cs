using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a progressbar UIElement", Name = "Progressbar")]
    public class WPFProgressbarNode : WPFGenericNode<ProgressBar>, IPluginEvaluate
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        [Input("Value", DefaultValue = 0, MinValue = 0, MaxValue = 1, Order = 0)]
        public IDiffSpread<double> ValueIn;

        [Input("Width", DefaultValue = 100, Order = 9, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<int> WidthIn;

        [Input("Height", DefaultValue = 10, Order = 10, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<int> HeightIn;

        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is ProgressBar))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void CreateElement(int i)
        {
            base.CreateElement(i);

            var uiElement = (ProgressBar) UIElementOut[i];

            uiElement.Minimum = 0;
            uiElement.Maximum = 1;
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = (ProgressBar) UIElementOut[i];

            if (ValueIn.IsChanged || force)
                uiElement.Value = ValueIn[i];

            if (WidthIn.IsChanged || force)
                uiElement.Width = WidthIn[i];

            if (HeightIn.IsChanged || force)
                uiElement.Height = HeightIn[i];
        }
    }
}
