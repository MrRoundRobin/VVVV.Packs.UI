using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Forms.Nodes
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a checklist control", Name = "Checklist")]
    public class FormsChecklistNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Font", Order = 1)]
        public IDiffSpread<Font> FontIn;

        [Input("Input", Order = 2, BinOrder = 3)]
        public IDiffSpread<ISpread<string>> ItemsIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Output("Output Value", Order = 3, BinOrder = 4)]
        public ISpread<ISpread<string>> ValueOut;
        
        [Output("Output Index", Order = 1, BinOrder = 2)]
        public ISpread<ISpread<int>> IndexOut;

// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        public void Evaluate(int SpreadMax)
        {
            if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !ItemsIn.IsChanged && !EnabledIn.IsChanged) return;

            SpreadMax = ItemsIn.SliceCount;

            if (ControlOut.SliceCount != SpreadMax)
            {
                if (ControlOut.SliceCount > SpreadMax)
                {
                    for (var i = ControlOut.SliceCount; i > SpreadMax; i--)
                        ControlOut[i].Dispose();
                    ControlOut.SliceCount = SpreadMax;
                    ValueOut.SliceCount = ItemsIn.SliceCount;
                    IndexOut.SliceCount = ItemsIn.SliceCount;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    ValueOut.SliceCount = ItemsIn.SliceCount;
                    IndexOut.SliceCount = ItemsIn.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        ControlOut[i] = new CheckedListBox { Tag = i , Font = FontIn[i]};
                        ((CheckedListBox)ControlOut[i]).ItemCheck += (sender, args) =>
                        {
                            var listBox = (CheckedListBox)sender;
                            var j = (int)((CheckedListBox)sender).Tag;
                            // Remove old values
                            ValueOut[j].Where(item => !listBox.CheckedItems.Contains(item)).ToList().ForEach(item => ValueOut[j].Remove(item));

                            // Add new values
                            listBox.CheckedItems.Cast<string>().Where(item => !ValueOut[j].Contains(item)).ToList().ForEach(item => ValueOut[j].Add(item));


                            // Remove old indices
                            IndexOut[j].Where(index => !listBox.CheckedIndices.Contains(index)).ToList().ForEach(index => IndexOut[j].Remove(index));

                            // Add new indices
                            listBox.CheckedIndices.Cast<int>().Where(index => !IndexOut[j].Contains(index)).ToList().ForEach(index => IndexOut[j].Add(index));
                        };
                        ValueOut[i].SliceCount = 0;
                        IndexOut[i].SliceCount = 0;
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new CheckedListBox { Tag = i, Font = FontIn[i] };
                    ((CheckedListBox)ControlOut[i]).ItemCheck += (object sender, ItemCheckEventArgs args) =>
                    {
                        var listBox = (CheckedListBox)sender;
                        var j = (int)((CheckedListBox)sender).Tag;

                        if (args.CurrentValue == CheckState.Checked)
                        { // Remove
                            IndexOut[j].Remove(args.Index);
                            ValueOut[j].Remove((string)listBox.Items[args.Index]);
                        }
                        else
                        {
                            IndexOut[j].Add(args.Index);
                            ValueOut[j].Add((string)listBox.Items[args.Index]);
                        }
                    };
                    ValueOut[i].SliceCount = 0;
                    IndexOut[i].SliceCount = 0;
                }

                if (!(ControlOut[i] is CheckedListBox)) continue;

                if (ItemsIn.IsChanged)
                {
                    ((CheckedListBox)ControlOut[i]).Items.Clear();
                    ItemsIn[i].ToList().ForEach(item => ((CheckedListBox)ControlOut[i]).Items.Add(item));
                    ValueOut[i].SliceCount = 0;
                }

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (FontIn.IsChanged)
                    ControlOut[i].Font = FontIn[i];

                if (TransformIn.IsChanged || ItemsIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                    {
                        ControlOut[i].Location = new Point((int)(translation.x * 100.0), (int)(translation.y * 100.0));
                        ControlOut[i].Width = (int)(scale.x * 100.0);
                        ControlOut[i].Height = (int)(scale.y * 100.0);
                    }
                }
            }
        }
    }
}
