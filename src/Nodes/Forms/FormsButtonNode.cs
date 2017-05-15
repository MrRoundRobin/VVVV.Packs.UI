using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a button control", Name = "Button")]
    public class FormsButtonNode : IPluginEvaluate
    {
        private readonly Dictionary<int, bool> _onClick = new Dictionary<int, bool>();

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Input("Background Color", DefaultColor = new[] {0.95, 0.95, 0.95, 1.0}, HasAlpha = false, Order = 5)]
        public IDiffSpread<RGBAColor> BackColorIn;

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Font", Order = 1)]
        public IDiffSpread<Font> FontIn;

        [Input("Forground Color", DefaultColor = new[] {0.0, 0.0, 0.0, 1.0}, HasAlpha = false, Order = 4)]
        public IDiffSpread<RGBAColor> ForeColorIn;

        [Output("OnClick", IsBang = true, Order = 1)]
        public ISpread<bool> OnClickOut;

        [Input("Own Style", IsToggle = true, DefaultBoolean = false, Order = 3)]
        public IDiffSpread<bool> OwnStyleIn;

        [Input("Text", DefaultString = "Button", Order = 2)]
        public IDiffSpread<string> TextIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            if (OnClickOut.Contains(true))
            {
                for (var i = 0; i < OnClickOut.SliceCount; i++)
                    OnClickOut[i] = false;
            }

            if (_onClick.ContainsValue(true))
            {
                (from item in _onClick where item.Value select item).ToList().ForEach(item =>
                {
                    OnClickOut[item.Key] = true;
                    _onClick[item.Key] = false;
                });
            }

            //if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !TextIn.IsChanged && !EnabledIn.IsChanged) return;

            if (ControlOut.SliceCount != SpreadMax)
            {
                if (ControlOut.SliceCount > SpreadMax)
                {
                    for (var i = ControlOut.SliceCount; i > SpreadMax; i--)
                    {
                        ControlOut[i].Dispose();
                        _onClick.Remove(i);
                    }
                    ControlOut.SliceCount = SpreadMax;
                    OnClickOut.SliceCount = SpreadMax;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        ControlOut[i] = new Button
                        {
                            Text = TextIn[i],
                            Tag = i,
                            Font = FontIn[i],
                            BackColor = BackColorIn[i].Color,
                            ForeColor = ForeColorIn[i].Color,
                            UseVisualStyleBackColor = !OwnStyleIn[i]
                        };
                        ControlOut[i].Click += (sender, args) => _onClick[(int)((Control)sender).Tag] = true;
                    }
                    OnClickOut.SliceCount = SpreadMax;
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new Button
                    {
                        Text = TextIn[i],
                        Tag = i,
                        Font = FontIn[i],
                        BackColor = BackColorIn[i].Color,
                        ForeColor = ForeColorIn[i].Color,
                        UseVisualStyleBackColor = !OwnStyleIn[i]
                    };
                    ControlOut[i].Click += (sender, args) => _onClick[(int)((Control)sender).Tag] = true;
                }

                if (TextIn.IsChanged)
                    ControlOut[i].Text = TextIn[i];

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (FontIn.IsChanged)
                    ControlOut[i].Font = FontIn[i];

                if (OwnStyleIn[i] && BackColorIn.IsChanged)
                    ControlOut[i].BackColor = BackColorIn[i].Color;

                if (ForeColorIn.IsChanged)
                    ControlOut[i].ForeColor = ForeColorIn[i].Color;

                if (OwnStyleIn.IsChanged)
                {
                    ((Button)ControlOut[i]).UseVisualStyleBackColor = !OwnStyleIn[i];
                    ((Button)ControlOut[i]).FlatStyle = OwnStyleIn[i] ? FlatStyle.Flat : FlatStyle.Standard;
                }

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                    {
                        //ControlOut[i].Bounds = new Rectangle((int) (translation.x*100.0), (int) (translation.y*100.0),
                        //    (int) (scale.x*75.0), (int) (scale.y*23.0));

                        ControlOut[i].Size = new Size((int)(scale.x * 75.0), (int)(scale.y * 23.0));
                    }
                }
            }
        }
    }
}
