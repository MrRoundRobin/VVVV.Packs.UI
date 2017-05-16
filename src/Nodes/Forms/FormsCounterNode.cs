using System;
using System.Drawing;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a numericupdown control", Name = "Counter", Tags = "numeric")]
    public class FormsCounterNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Default", AsInt = true, Order = 6)]
        public IDiffSpread<int> DefaultIn;

        [Input("Down", IsBang = true, Order = 2)]
        public IDiffSpread<bool> DownIn;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Increment", AsInt = true, DefaultValue = 1, Order = 5)]
        public IDiffSpread<int> IncrementIn;

        [Input("Maximum", AsInt = true, DefaultValue = 15, Order = 4)]
        public IDiffSpread<int> MaximumIn;

        [Input("Minimum", AsInt = true, Order = 3)]
        public IDiffSpread<int> MinimumIn;

        [Input("Reset", IsBang = true, Order = 7)]
        public IDiffSpread<bool> ResetIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Input("Up", IsBang = true, Order = 1)]
        public IDiffSpread<bool> UpIn;

        [Output("Output", Order = 1, AsInt = true)]
        public ISpread<int> ValueOut;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            //if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !EnabledIn.IsChanged && !) return;

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
                        ControlOut[i] = new NumericUpDown {Maximum = MaximumIn[i], Minimum = MinimumIn[i], Increment = IncrementIn[i], Tag = i};
                        ((NumericUpDown)ControlOut[i]).ValueChanged += (sender, args) => ValueOut[(int)((NumericUpDown)sender).Tag] = Convert.ToInt32(((NumericUpDown)sender).Value);
                        ValueOut[i] = DefaultIn[0];
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new NumericUpDown {Maximum = MaximumIn[i], Minimum = MinimumIn[i], Increment = IncrementIn[i], Tag = i};
                    ((NumericUpDown)ControlOut[i]).ValueChanged += (sender, args) => ValueOut[(int)((NumericUpDown)sender).Tag] = Convert.ToInt32(((NumericUpDown)sender).Value);
                    ValueOut[i] = DefaultIn[0];
                }

                if (!(ControlOut[i] is NumericUpDown)) continue;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (MaximumIn.IsChanged)
                    ((NumericUpDown)ControlOut[i]).Maximum = MaximumIn[i];

                if (MinimumIn.IsChanged)
                    ((NumericUpDown)ControlOut[i]).Minimum = MinimumIn[i];

                if (IncrementIn.IsChanged)
                    ((NumericUpDown)ControlOut[i]).Increment = IncrementIn[i];

                if (UpIn[i])
                    ((NumericUpDown)ControlOut[i]).UpButton();

                if (DownIn[i])
                    ((NumericUpDown)ControlOut[i]).DownButton();

                if (ResetIn[i])
                    ((NumericUpDown)ControlOut[i]).Value = DefaultIn[i];

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                    {
                        ControlOut[i].Width = (int)(scale.x * 120.0);
                        ControlOut[i].Location = new Point((int)(translation.x * 100.0), (int)(translation.y * 100.0));
                    }
                }
            }
        }
    }
}
