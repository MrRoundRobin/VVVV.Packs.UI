using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a slider UIElement", Name = "Slider")]
    public class WPFSliderNode : WPFGenericNode<Slider>, IPluginEvaluate //, IPartImportsSatisfiedNotification
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        [Input("Size", DefaultValue = 120, Order = 9, MinValue = 0)]
        public IDiffSpread<int> SizeIn;

        [Input("Orientation", Order = 1, DefaultEnumEntry = "Horizontal")]
        public IDiffSpread<Orientation> OrientationIn;
        
        [Output("Value", IsSingle = true, Order = 1)]
        public ISpread<double> ValueOut;

        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = SpreadMax;
            ValueOut.SliceCount = SpreadMax;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is Slider))
                    CreateElement(i);

                SetProperties(i);
            }
        }

        private new void CreateElement(int i)
        {
            base.CreateElement(i);
            var uiElement = (Slider)UIElementOut[i];

            uiElement.Width = SizeIn[0];
            uiElement.Height = SizeIn[0];

            uiElement.Minimum = 0;
            uiElement.Maximum = 1;
        }

        private new void SetProperties(int i, bool force = false)
        {
            base.SetProperties(i, force);

            var uiElement = (Slider)UIElementOut[i];

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (!ValueOut[i].Equals(uiElement.Value))
                ValueOut[i] = uiElement.Value;

            if (OrientationIn.IsChanged)
                uiElement.Orientation = OrientationIn[i];

            if (SizeIn.IsChanged)
            {
                uiElement.Width = SizeIn[0];
                uiElement.Height = SizeIn[0];
            }
        }

        //public void OnImportsSatisfied()
        //{
            
        //}
    }
}
