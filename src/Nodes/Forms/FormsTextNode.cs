using System.Drawing;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a label control", Name = "Text")]
    public class FormsTextNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Color", DefaultColor = new[] {0.0, 0.0, 0.0, 1.0}, Order = 3)]
        public IDiffSpread<RGBAColor> ColorIn;

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Font", Order = 1)]
        public IDiffSpread<Font> FontIn;

        [Input("Text", DefaultString = "vvvv", Order = 2)]
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
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        ControlOut[i] = new Label
                        {
                            AutoSize = true,
                            Tag = i,
                            Text = TextIn[i],
                            Font = FontIn[i],
                            ForeColor = ColorIn[i].Color,
                            Enabled = EnabledIn[i]
                        };
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new Label
                    {
                        AutoSize = true,
                        Tag = i,
                        Text = TextIn[i],
                        Font = FontIn[i],
                        ForeColor = ColorIn[i].Color,
                        Enabled = EnabledIn[i]
                    };
                }

                if (!(ControlOut[i] is Label)) continue;

                if (TextIn.IsChanged)
                    ControlOut[i].Text = TextIn[i];

                if (ColorIn.IsChanged)
                    ControlOut[i].ForeColor = ColorIn[i].Color;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

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
