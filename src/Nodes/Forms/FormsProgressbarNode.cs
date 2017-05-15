using System.Drawing;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Forms.Nodes
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a progressbar control", Name = "Progressbar")]
    public class FormsProgressbarNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Input("Value", Order = 1)]
        public IDiffSpread<double> ValueIn;

// ReSharper resture UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global


        public void Evaluate(int SpreadMax)
        {
            //if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !EnabledIn.IsChanged) return;

            if (ControlOut.SliceCount != SpreadMax)
            {
                if (ControlOut.SliceCount > SpreadMax)
                {
                    for (var i = ControlOut.SliceCount; i > SpreadMax; i--)
                        ControlOut[i].Dispose();
                    ControlOut.SliceCount = SpreadMax;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                        ControlOut[i] = new ProgressBar {Tag = i, Maximum = 100, Value = (int)(ValueIn[i] * 100.0)};
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                    ControlOut[i] = new ProgressBar {Tag = i, Maximum = 100, Value = (int)(ValueIn[i] * 100.0)};

                if (!(ControlOut[i] is ProgressBar)) continue;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (ValueIn.IsChanged)
                    ((ProgressBar)ControlOut[i]).Value = ValueIn[i] > (ControlOut[i] as ProgressBar).Maximum ? (ControlOut[i] as ProgressBar).Maximum : (int)(ValueIn[i] * 100.0);

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                        ControlOut[i].Bounds = new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * 100.0), (int)(scale.y * 32.0));
                }
            }
        }
    }
}
