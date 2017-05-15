using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Creates a tabcontrol UIElement", Name = "Tab")]
    public class WPFTabsNode : WPFGenericNode<TabControl>, IPluginEvaluate, IPartImportsSatisfiedNotification
    {

        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        private readonly Spread<IIOContainer<IDiffSpread<UIElement>>> _elementInputs = new Spread<IIOContainer<IDiffSpread<UIElement>>>();
        private readonly Spread<IIOContainer<IDiffSpread<string>>> _nameInputs = new Spread<IIOContainer<IDiffSpread<string>>>();

        [Import]
        public IIOFactory IOFactory;

        [Config("Input Count", DefaultValue = 2, MinValue = 1, Order = 0)]
        public IDiffSpread<int> InputCountConfig;

        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            SpreadMax = 1; //TODO: Spreadable

            UIElementOut.SliceCount = 1;

            for (var i = 0; i < SpreadMax; i++)
            {
                if (UIElementOut == null || !(UIElementOut[i] is TabControl))
                    CreateElement(i);

                SetProperties(i);
            }

            //for (var i = ((TabControl)ControlOut[0]).Items.Count - 1; ((TabControl)ControlOut[0]).Items.Count > _elementInputs.SliceCount; i--)
            //    ((TabControl)ControlOut[0]).Items.RemoveAt(i);

            for (var i = 0; i < _elementInputs.SliceCount; i++)
            {
                if  (((TabControl)UIElementOut[0]).Items.Count <= i)
                    ((TabControl)UIElementOut[0]).Items.Add(new TabItem { Header = _nameInputs[i].IOObject[0], Content = _elementInputs[i].IOObject[0] });

                if (_nameInputs[i].IOObject.IsChanged)
                    ((TabItem)((TabControl)UIElementOut[0]).Items[i]).Header = _nameInputs[i].IOObject[0];

                if (_elementInputs[i].IOObject.IsChanged)
                    ((TabItem) ((TabControl) UIElementOut[0]).Items[i]).Content = _elementInputs[i].IOObject[0];
            }
        }

        private new void CreateElement(int i)
        {
            base.CreateElement(i);

            ((TabControl)UIElementOut[i]).VerticalAlignment = VerticalAlignment.Stretch;
            ((TabControl)UIElementOut[i]).HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public void OnImportsSatisfied()
        {
            InputCountConfig.Changed += InputCountConfigOnChanged;
        }

        private void InputCountConfigOnChanged(IDiffSpread<int> sender)
        {
            if (_elementInputs.SliceCount > sender[0])
            {
                for (var i = _elementInputs.SliceCount - 1; i > sender[0]; i--)
                {
                    _elementInputs.RemoveAt(i);
                    _elementInputs.SliceCount = i;
                    _nameInputs.RemoveAt(i);
                    _nameInputs.SliceCount = i;

                    ((TabControl)UIElementOut[0]).Items.RemoveAt(i);
                }
            }
            else if (_elementInputs.SliceCount < sender[0])
            {
                _elementInputs.SliceCount = sender[0];
                _nameInputs.SliceCount = sender[0];

                for (var i = sender[0] - _elementInputs.SliceCount; i < sender[0]; i++)
                {
                    _elementInputs[i] = IOFactory.CreateIOContainer<IDiffSpread<UIElement>>(new InputAttribute("Element " + (i + 1)) {IsSingle = true, Order = i + 11});
                    _nameInputs[i] = IOFactory.CreateIOContainer<IDiffSpread<string>>(new InputAttribute("Name " + (i + 1)) {IsSingle = true, DefaultString = "Tab " + (i + 1), Order = i + 10});
                }
            }
        }
    }
}
