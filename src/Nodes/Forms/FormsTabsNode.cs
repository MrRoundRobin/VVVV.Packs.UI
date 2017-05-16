using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a tab control", Name = "Tabs")]
    public class FormsTabsNode : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        private readonly Spread<IIOContainer<IDiffSpread<Control>>> _controlInputs = new Spread<IIOContainer<IDiffSpread<Control>>>();
        private readonly Spread<IIOContainer<IDiffSpread<string>>> _nameInputs = new Spread<IIOContainer<IDiffSpread<string>>>();

        [Output("Control", Order = 0, IsSingle = true)]
        public ISpread<Control> ControlOut;

        [Input("Dock", DefaultEnumEntry = "Fill", IsSingle = true, Order = 1)]
        public IDiffSpread<DockStyle> DockIn;

        [Input("Background Color", DefaultColor = new[] { 0.94, 0.94, 0.94, 1.0 }, HasAlpha = false, Order = 3)]
        public IDiffSpread<RGBAColor> BackgroundColorIn;

        [Import]
        public IIOFactory IOFactory;

        [Config("Input Count", DefaultValue = 2, MinValue = 1, Order = 0)]
        public IDiffSpread<int> InputCountConfig;

        [Input("Transform", Order = 0, IsSingle = true)]
        public IDiffSpread<Matrix4x4> TransformIn;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void OnImportsSatisfied()
        {
            InputCountConfig.Changed += HandleInputCountChanged;
        }

        public void Evaluate(int SpreadMax)
        {
            //var controlCount = (from IIOContainer<IDiffSpread<Control>> controlInput in ControlInputs.ToList() select controlInput.IOObject.SliceCount).Sum();

            ControlOut.SliceCount = 1;

            if (ControlOut[0] == null)
            {
                ControlOut[0] = new TabControl
                { //TODO: color Pins
                    Dock = DockIn[0]
                };
            }

            for (var i = ((TabControl)ControlOut[0]).TabPages.Count - 1; ((TabControl)ControlOut[0]).TabPages.Count > _nameInputs.SliceCount; i--)
                ((TabControl)ControlOut[0]).TabPages[i].Dispose();

            for (var i = 0; i < _nameInputs.SliceCount; i++)
            {
                if (((TabControl)ControlOut[0]).TabPages.Count <= i)
                    ((TabControl)ControlOut[0]).TabPages.Add(new TabPage(_nameInputs[i].IOObject[0]) { BackColor = BackgroundColorIn[i].Color });


                //Remove old controls
                (from Control c in ((TabControl)ControlOut[0]).TabPages[i].Controls where !_controlInputs[i].IOObject.Contains(c) select c).ToList().ForEach(c => ((TabControl)ControlOut[0]).TabPages[i].Controls.Remove(c));


                for (var j = 0; j < _controlInputs[i].IOObject.SliceCount; j++)
                {
                    if (((TabControl)ControlOut[0]).TabPages[i].Controls.Contains(_controlInputs[i].IOObject[j])) continue;

                    ((TabControl)ControlOut[0]).TabPages[i].Controls.Add(_controlInputs[i].IOObject[j]);
                }

                if (BackgroundColorIn.IsChanged)
                    ((TabControl)ControlOut[0]).TabPages[i].BackColor = BackgroundColorIn[i].Color;
            }

            if (DockIn.IsChanged)
                ControlOut[0].Dock = DockIn[0];

            if (TransformIn.IsChanged)
            {
                Vector3D scale;
                Vector3D rotation;
                Vector3D translation;
                if (TransformIn[0].Decompose(out scale, out rotation, out translation))
                    ControlOut[0].Bounds = new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * 100), (int)(scale.y * 100));
            }
        }

        private void HandleInputCountChanged(IDiffSpread<int> sender)
        {
            if (_controlInputs.SliceCount > sender[0])
            {
                while (_controlInputs.SliceCount > sender[0])
                {
                    _nameInputs.Last().Dispose();
                    _nameInputs.SliceCount--;
                    _controlInputs.Last().Dispose();
                    _controlInputs.SliceCount--;

                }
                //_controlInputs.SliceCount = sender[0];
                //_nameInputs.SliceCount = sender[0];
            }
            else
            {
                var i = _nameInputs.SliceCount;
                _controlInputs.SliceCount = sender[0];
                for (_nameInputs.SliceCount = sender[0]; i < sender[0]; i++)
                {
                    _nameInputs[i] = IOFactory.CreateIOContainer<IDiffSpread<string>>(new InputAttribute("Name " + (i + 1)) {IsSingle = true, DefaultString = "Tab " + (i + 1), Order = i + 10 });
                    _controlInputs[i] = IOFactory.CreateIOContainer<IDiffSpread<Control>>(new InputAttribute("Control " + (i + 1)) { Order = i + 11 });
                }
            }
        }
    }
}
