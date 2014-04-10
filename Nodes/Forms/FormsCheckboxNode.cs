using System.Drawing;
using System.Windows.Forms;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a slider control", Name = "Checkbox")]
    public class FormsCheckboxNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Checked", IsToggle = true, Order = 1)]
        public ISpread<bool> CheckedOut;

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Font", Order = 1)]
        public IDiffSpread<Font> FontIn;

        [Input("Text", Order = 2)]
        public IDiffSpread<string> TextIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

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
                    CheckedOut.SliceCount = SpreadMax;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                        ControlOut[i] = new CheckBox {Tag = i, Text = TextIn[i], Font = FontIn[i], AutoSize = true};
                    CheckedOut.SliceCount = SpreadMax;
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                    ControlOut[i] = new CheckBox {Tag = i, Text = TextIn[i], Font = FontIn[i], AutoSize = true};

                if (!(ControlOut[i] is CheckBox)) continue;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (TextIn.IsChanged)
                    ControlOut[i].Text = TextIn[i];

                if (CheckedOut[i] != ((CheckBox)ControlOut[i]).Checked)
                    CheckedOut[i] = ((CheckBox)ControlOut[i]).Checked;

                if (FontIn.IsChanged)
                    ControlOut[i].Font = FontIn[i];

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                        ControlOut[i].Location = new Point((int)(translation.x * 100.0), (int)(translation.y * 100.0));
                }
            }
        }
    }
}
