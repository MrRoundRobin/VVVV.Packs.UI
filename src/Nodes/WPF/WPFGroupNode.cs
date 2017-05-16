using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Category = "WPF", Help = "Groups UIElements", Name = "Group")]
    public class WPFGroupNode : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        private readonly Spread<IIOContainer<IDiffSpread<UIElement>>> _elementInputs = new Spread<IIOContainer<IDiffSpread<UIElement>>>();

        [Output("Element", Order = 0, IsSingle = true)]
        public ISpread<UIElement> UIElementOut;
        
        [Import]
        public IIOFactory IOFactory;

        [Config("Input Count", DefaultValue = 2, MinValue = 1, Order = 0)]
        public IDiffSpread<int> InputCountConfig;

        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global


        public void Evaluate(int SpreadMax)
        {
            UIElementOut.SliceCount = 1;

            if (UIElementOut == null || !(UIElementOut[0] is Grid))
                UIElementOut[0] = new Grid();

            var toRemove = new List<UIElement>();

            ((Grid) UIElementOut[0]).Children.Cast<UIElement>().ToList().ForEach(element =>
            {
                if (!_elementInputs.Where(input => input.IOObject[0] != null).Any(input => input.IOObject[0].Equals(element)))
                    toRemove.Add(element);
            });

            toRemove.ForEach(element => ((Grid) UIElementOut[0]).Children.Remove(element));

            for (var i = 0; i < _elementInputs.SliceCount; i++)
            {
                //if (_elementInputs[i].IOObject[0] == null && ((Grid) UIElementOut[0]).Children[i] != null)
                //    ((Grid) UIElementOut[0]).Children.RemoveAt(i);

                if (_elementInputs[i].IOObject[0] != null && !(((Grid) UIElementOut[0]).Children.Contains(_elementInputs[i].IOObject[0])))
                    ((Grid) UIElementOut[0]).Children.Add(_elementInputs[i].IOObject[0]);
            }
        }

        public void OnImportsSatisfied()
        {
            InputCountConfig.Changed += InputCountConfigOnChanged;
        }

        private void InputCountConfigOnChanged(IDiffSpread<int> sender)
        {
            if (_elementInputs.SliceCount > sender[0])
            {
                for (var i = _elementInputs.SliceCount - 1; i >= sender[0]; i--)
                {
                    if (((Grid)UIElementOut[0]).Children.Contains(_elementInputs[i].IOObject[0]))
                        ((Grid)UIElementOut[0]).Children.Remove(_elementInputs[i].IOObject[0]);

                    _elementInputs[i].Dispose();
                    _elementInputs.SliceCount = i;
                }
            }
            else if (_elementInputs.SliceCount < sender[0])
            {
                var oldCount = _elementInputs.SliceCount;

                _elementInputs.SliceCount = sender[0];

                for (var i = oldCount; i < sender[0]; i++)
                {
                    _elementInputs[i] = IOFactory.CreateIOContainer<IDiffSpread<UIElement>>(new InputAttribute("Element " + (i + 1)) { IsSingle = true, Order = i + 11 });
                }
            }
        }
    }
}
