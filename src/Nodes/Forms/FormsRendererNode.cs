using System;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Name = "Renderer", Category = "Forms", Help = "Renders Forms controls and displays the form", AutoEvaluate = true, InitialBoxHeight = 120, InitialBoxWidth = 160, InitialComponentMode = TComponentMode.InAWindow, InitialWindowHeight = 300, InitialWindowWidth = 400)]
    public class FormsRendererNode : UserControl, IPluginEvaluate, IUserInputWindow
    {
        private readonly ContainerControl _container = new ContainerControl {Dock = DockStyle.Fill, AutoScroll = true};

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Background Color", DefaultColor = new[] {0.94, 0.94, 0.94, 1.0}, IsSingle = true, HasAlpha = false, Order = 1)]
        public IDiffSpread<RGBAColor> ColorIn;

        [Input("Control", Order = 0)]
        public IDiffSpread<Control> ControlIn;

        //[Input("Enabled", Order = 9999, DefaultValue = 1, IsSingle = true, IsToggle = true)]
        //public IDiffSpread<bool> FEnabledInput;

        [Output("Control Count", AsInt = true, IsSingle = true, Visibility = PinVisibility.OnlyInspector, Order = 999)]
        public ISpread<int> ControlsOut;

        [Output("Height", AsInt = true, IsSingle = true, Order = 1)]
        public ISpread<int> HeightOut;

        [Output("Width", AsInt = true, IsSingle = true, Order = 0)]
        public ISpread<int> WidthOut;

        [Output("Handle", Visibility = PinVisibility.OnlyInspector)]
        public ISpread<int> HandleOut; 

// ReSharper resture UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public FormsRendererNode()
        {
            Controls.Clear();
            Controls.Add(_container);
        }

        // Called when data for any output pin is requested.
        public void Evaluate(int SpreadMax)
        {
            HandleOut.SliceCount = 1;
            if (HandleOut[0] != _container.Handle.ToInt32())
                HandleOut[0] = _container.Handle.ToInt32();

            if (ColorIn.IsChanged)
                _container.BackColor = ColorIn[0].Color;

            WidthOut[0] = _container.Width;
            HeightOut[0] = _container.Height;

// ReSharper disable once RedundantCheckBeforeAssignment
            if (ControlsOut[0] != _container.Controls.Count) ControlsOut[0] = _container.Controls.Count;

            if (!ControlIn.IsChanged) return;

            (from Control c in _container.Controls where !ControlIn.Contains(c) select c).ToList().ForEach(c => _container.Controls.Remove(c));

            for (var i = 0; i < ControlIn.SliceCount; i++)
            {
                //if (ControlIn[i] == null) continue;

                if (_container.Controls.Contains(ControlIn[i])) continue;

                //ControlIn[i].TabStop = false;
                _container.Controls.Add(ControlIn[i]);
            }
        }

        public IntPtr InputWindowHandle
        {
            get { return _container.Handle; }
        }
    }
}
