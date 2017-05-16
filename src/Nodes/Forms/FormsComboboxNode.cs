using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms

{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a combobox control", Name = "Combobox")]
    public class FormsComboboxNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        //[Input("Font", Order = 1)]
        //public IDiffSpread<Font> FontIn;

        [Input("Input", Order = 2, BinOrder = 3)]
        public IDiffSpread<ISpread<string>> ItemsIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Output("Output Value", Order = 2)]
        public ISpread<string> ValueOut;

        [Output("Output Index", AsInt = true, Order = 1)]
        public ISpread<int> IndexOut; 

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
                        ControlOut[i] = new ComboBox {AutoCompleteMode = AutoCompleteMode.SuggestAppend, Tag = i /*, Font = FontIn[i]*/};
                        ((ComboBox)ControlOut[i]).SelectedValueChanged += (sender, args) =>
                        {
                            ValueOut[(int)((ComboBox)sender).Tag] = (string)((ComboBox)sender).SelectedItem;
                            IndexOut[(int)((ComboBox)sender).Tag] = ((ComboBox)sender).SelectedIndex;
                        };
                        ValueOut[i] = (string)((ComboBox)ControlOut[i]).SelectedItem;
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    ControlOut[i] = new ComboBox {AutoCompleteMode = AutoCompleteMode.SuggestAppend, Tag = i /*, Font = FontIn[i]*/};
                    ((ComboBox)ControlOut[i]).SelectedValueChanged += (sender, args) =>
                    {
                        ValueOut[(int)((ComboBox)sender).Tag] = (string)((ComboBox)sender).SelectedItem;
                        IndexOut[(int)((ComboBox)sender).Tag] = ((ComboBox)sender).SelectedIndex;
                    };
                    ValueOut[i] = (string)((ComboBox)ControlOut[i]).SelectedItem;
                }

                if (!(ControlOut[i] is ComboBox)) continue;

                if (ItemsIn.IsChanged)
                {
                    ((ComboBox)ControlOut[i]).Items.Clear();
                    ItemsIn[i].ToList().ForEach(item => ((ComboBox)ControlOut[i]).Items.Add(item));
                    ValueOut[i] = (string)((ComboBox)ControlOut[i]).SelectedItem;
                }

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                //if (FontIn.IsChanged)
                //    ControlOut[i].Font = FontIn[i];

                if (TransformIn.IsChanged || ItemsIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                    {
                        ControlOut[i].Location = new Point((int)(translation.x * 100.0), (int)(translation.y * 100.0));
                        ControlOut[i].Width = (int)(scale.x * 100.0);
                    }
                }
            }
        }
    }
}
