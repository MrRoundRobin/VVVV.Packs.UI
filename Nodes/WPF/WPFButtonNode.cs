using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a button UIElement", Name = "Button")]
    public class WPFButtonNode : WPFGenericNode<Button>, IPluginEvaluate
    {
        private List<int> _clicks = new List<int>();

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Click", IsBang = true, Order = 1)]
        public ISpread<bool> OnClickOut;

        [Input("Text", DefaultString = "Button", Order = 2)]
        public IDiffSpread<string> TextIn;

        [Input("Width", DefaultValue = 75, Order = 9, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<double> WidthIn;

        [Input("Height", DefaultValue = 22, Order = 10, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<double> HeightIn;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;
            OnClickOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is Button))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void CreateElement(int i)
        {
            base.CreateElement(i);
            var uiElement = (ButtonBase)UIElementOut[i];

            uiElement.Click += (sender, args) => _clicks.Add((int)((FrameworkElement)sender).Tag);
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = (ContentControl)UIElementOut[i];

            if (TextIn.IsChanged || force)
                uiElement.Content = TextIn[i];

            if (WidthIn.IsChanged || force)
                uiElement.SetValue(FrameworkElement.WidthProperty, WidthIn[i]);

            if (HeightIn.IsChanged || force)
                uiElement.SetValue(FrameworkElement.HeightProperty, HeightIn[i]);

            OnClickOut[i] = _clicks.Contains(i);
            _clicks.Remove(i);
        }
    }
}
