using System.Drawing;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Forms.Nodes
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a slider control", Name = "Slider")]
    public class FormsSliderNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Orientation", Order = 1)]
        public IDiffSpread<Orientation> OrientationIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Output("Value", Order = 1)]
        public ISpread<double> ValueOut;

// ReSharper restore UnassignedField.Global
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
                    ValueOut.SliceCount = SpreadMax;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                        ControlOut[i] = new TrackBar {Tag = i, Maximum = 100, TickStyle = TickStyle.Both, Orientation = OrientationIn[i]};
                    ValueOut.SliceCount = SpreadMax;
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                    ControlOut[i] = new TrackBar {Tag = i, Maximum = 100, TickStyle = TickStyle.Both, Orientation = OrientationIn[i]};

                if (!(ControlOut[i] is TrackBar)) continue;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (OrientationIn.IsChanged)
                    ((TrackBar)ControlOut[i]).Orientation = OrientationIn[i];

                if (!ValueOut[i].Equals(((TrackBar)ControlOut[i]).Value / 100.0))
                    ValueOut[i] = ((TrackBar)ControlOut[i]).Value / 100.0;

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                        ControlOut[i].Bounds = new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * 104.0), (int)(scale.y * 104.0));
                }
            }
        }
    }
}
