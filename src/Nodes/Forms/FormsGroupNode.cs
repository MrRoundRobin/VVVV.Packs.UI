using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Name = "Group", Category = "Forms", Author = "Robster", Help = "Groups all input controls to one output spread")]
    public class FormsGroupNode : IPluginEvaluate
    {
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control Bin Size", AsInt = true, Order = 1)]
        public ISpread<int> BinSizeOut;

        [Input("Input", IsPinGroup = true, Order = 0)]
        public IDiffSpread<ISpread<Control>> ControlIn;

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnassignedField.Global

        public void Evaluate(int SpreadMax)
        {
            if (!ControlIn.IsChanged) return;

            ControlOut.SliceCount = 0;
            BinSizeOut.SliceCount = ControlIn.SliceCount;

            for (var i = 0; i < ControlIn.SliceCount; i++)
            {
                BinSizeOut[i] = ControlIn[i].SliceCount;

                ControlIn[i].ToList().ForEach(control =>
                {
                    if (!ControlOut.Contains(control))
                        ControlOut.Add(control);
                });
            }
        }
    }
}
