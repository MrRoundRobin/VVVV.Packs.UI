using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a checkbox UIElement", Name = "Checkbox")]
    public class WPFCheckboxNode : WPFGenericNode<CheckBox>, IPluginEvaluate
    {
        private readonly Dictionary<int, bool> _onChange = new Dictionary<int, bool>();

        private List<int> _changes = new List<int>();

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Change", IsBang = true, Order = 1)]
        public ISpread<bool> OnChangeOut;

        [Output("Checked", IsToggle = true, Order = 1)]
        public ISpread<bool> CheckedOut;

        [Input("Text", DefaultString = "Button", Order = 2)]
        public IDiffSpread<string> TextIn;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;
            OnChangeOut.SliceCount = SpreadMax;
            CheckedOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is CheckBox))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void CreateElement(int i)
        {
            base.CreateElement(i);
            var uiElement = (CheckBox)UIElementOut[i];

            uiElement.Checked += (sender, args) => _changes.Add((int)((FrameworkElement)sender).Tag);
            uiElement.Unchecked += (sender, args) => _changes.Add((int)((FrameworkElement)sender).Tag);
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = (CheckBox)UIElementOut[i];

            if (TextIn.IsChanged || force)
                uiElement.Content = TextIn[i];

            if (uiElement.IsChecked != null && uiElement.IsChecked != CheckedOut[i])
                CheckedOut[i] = (bool)uiElement.IsChecked;

            OnChangeOut[i] = _changes.Contains(i);
            _changes.Remove(i);
        }
    }
}
