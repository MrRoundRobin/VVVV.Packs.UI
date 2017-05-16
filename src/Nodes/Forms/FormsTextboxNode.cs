using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a button control", Name = "Textbox")]
    public class FormsTextboxNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Default", Order = 4)]
        public ISpread<string> DefaultIn;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("MultiLine", IsToggle = true, Order = 2)]
        public IDiffSpread<bool> MultiLineIn;

        [Input("Reset", IsBang = true, Order = 3)]
        public ISpread<bool> ResetIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Output("Value", Order = 1)]
        public ISpread<string> ValueOut;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !MultiLineIn.IsChanged && !EnabledIn.IsChanged && !ResetIn.Any(reset => reset)) return;

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
                    ValueOut.SliceCount = SpreadMax;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        ControlOut[i] = new TextBox {Text = DefaultIn[i], Multiline = MultiLineIn[i], Tag = i};
                        ControlOut[i].TextChanged += (sender, args) => ValueOut[(int)((TextBox)sender).Tag] = ((TextBox)sender).Text;
                        ValueOut[i] = ControlOut[i].Text;
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new TextBox {Text = DefaultIn[i], Multiline = MultiLineIn[i], Tag = i};
                    ControlOut[i].TextChanged += (sender, args) => ValueOut[(int)(((TextBox)sender).Tag)] = ((TextBox)sender).Text;
                    ValueOut[i] = ControlOut[i].Text;
                }

                if (ControlOut[i].Parent == null || !(ControlOut[i] is TextBox)) continue;

                if (MultiLineIn.IsChanged)
                    ((TextBox)ControlOut[i]).Multiline = MultiLineIn[i];

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (ResetIn[i])
                    ControlOut[i].Text = DefaultIn[i];

                if (TransformIn.IsChanged || MultiLineIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                        ControlOut[i].Bounds = MultiLineIn[i] ? new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * 100.0), (int)(scale.y * 100.0)) : new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * 100.0), 20);
                }
            }
        }
    }
}
