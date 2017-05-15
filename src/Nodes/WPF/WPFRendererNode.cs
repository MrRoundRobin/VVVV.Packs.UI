using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;

namespace VVVV.Packs.UI.Nodes.WPF
{
    [PluginInfo(Author = "Robster", Name = "Renderer", Category = "WPF", Help = "Renders WPF elements and displays the UI", AutoEvaluate = true, InitialBoxHeight = 120, InitialBoxWidth = 160, InitialComponentMode = TComponentMode.InAWindow, InitialWindowHeight = 300, InitialWindowWidth = 400)]
    public class WPFRendererNode : System.Windows.Forms.UserControl, IPluginEvaluate, IUserInputWindow
    {
        private readonly ElementHost _container = new ElementHost { Dock = System.Windows.Forms.DockStyle.Fill };

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Background Color", DefaultColor = new[] {0.94, 0.94, 0.94, 1.0}, IsSingle = true, HasAlpha = false, Order = 1)]
        public IDiffSpread<RGBAColor> ColorIn;

        [Input("Element", IsSingle = true, Order = 0)]
        public IDiffSpread<UIElement> UIElementIn;

        //[Input("Enabled", Order = 9999, DefaultValue = 1, IsSingle = true, IsToggle = true)]
        //public IDiffSpread<bool> FEnabledInput;

        [Output("Height", AsInt = true, IsSingle = true, Order = 1)]
        public ISpread<int> HeightOut;

        [Output("Width", AsInt = true, IsSingle = true, Order = 0)]
        public ISpread<int> WidthOut;

        [Output("Control", IsSingle = true, Order = 2)]
        public ISpread<System.Windows.Forms.Control> ControlOut; 

// ReSharper resture UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public WPFRendererNode()
        {
            Controls.Clear();
            _container.Child = new Grid();
            Controls.Add(_container);
        }

        // Called when data for any output pin is requested.
        public void Evaluate(int SpreadMax)
        {
            ControlOut.SliceCount = 1;

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (ControlOut[0] != _container)
                ControlOut[0] = _container;

            if (ColorIn.IsChanged)
                _container.BackColor = ColorIn[0].Color;

            WidthOut[0] = _container.Width;
            HeightOut[0] = _container.Height;

            if (UIElementIn.IsChanged)
            {
                var child = (Grid) _container.Child;

                if (!child.Children.Contains(UIElementIn[0]))
                    child.Children.Clear();

                if (UIElementIn[0] != null)
                    child.Children.Add(UIElementIn[0]);
            }
        }

        public IntPtr InputWindowHandle
        {
            get { return _container.Handle; }
        }
    }
}
