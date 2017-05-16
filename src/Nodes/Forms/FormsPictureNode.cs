using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.Forms
{
    [PluginInfo(Author = "Robster", Category = "Forms", Help = "Creates a picture control", Name = "Picture")]
    public class FormsPictureNode : IPluginEvaluate
    {
        private readonly Dictionary<int, bool> _onClick = new Dictionary<int, bool>();

// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global

        [Output("Control", Order = 0)]
        public ISpread<Control> ControlOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Output("Click", IsBang = true, Order = 1)]
        public ISpread<bool> OnClickOut;

        [Input("Path", FileMask = "Image File (*.bmp,*.gif,*.exif,*.jpg,*.jpeg,*.png,*.png)|*.bmp;*.gif;*.exif,*.jpg;*.jpeg;*.png;*.tif;*.tiff", StringType = StringType.Filename, Order = 1)]
        public IDiffSpread<string> PathIn;

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

            //if (ControlOut.SliceCount == SpreadMax && !TransformIn.IsChanged && !EnabledIn.IsChanged) return;

            if (!(from path in PathIn.ToList() where File.Exists(path) select path).Any())
            {
                ControlOut.SliceCount = 0;
                return;
            }

            if (ControlOut.SliceCount != SpreadMax)
            {
                if (ControlOut.SliceCount > SpreadMax)
                {
                    for (var i = ControlOut.SliceCount; i > SpreadMax; i--)
                    {
                        ControlOut[i].Dispose();
                        _onClick.Remove(i);
                    }
                    OnClickOut.SliceCount = SpreadMax;
                    ControlOut.SliceCount = SpreadMax;
                }
                else
                {
                    var i = ControlOut.SliceCount;
                    for (ControlOut.SliceCount = SpreadMax; i < SpreadMax; i++)
                    {
                        if (File.Exists(PathIn[i]))
                        {
                            ControlOut[i] = new PictureBox {Tag = i, Image = new Bitmap(PathIn[i]), SizeMode = PictureBoxSizeMode.Zoom};
                            ControlOut[i].Click += (sender, args) => _onClick[(int)((Control)sender).Tag] = true;
                        }
                        OnClickOut.SliceCount = SpreadMax;
                    }
                }
            }

            for (var i = 0; i < SpreadMax; i++)
            {
                if (ControlOut[i] == null)
                {
                    if (File.Exists(PathIn[i]))
                    {
                        ControlOut[i] = new PictureBox {Tag = i, Image = new Bitmap(PathIn[i]), SizeMode = PictureBoxSizeMode.Zoom};
                        ControlOut[i].Click += (sender, args) => _onClick[(int)((Control)sender).Tag] = true;
                    }
                }

                if (!(ControlOut[i] is PictureBox)) continue;

                if (EnabledIn.IsChanged)
                    ControlOut[i].Enabled = EnabledIn[i];

                if (PathIn.IsChanged)
                {
                    if (File.Exists(PathIn[i]))
                        ((PictureBox)ControlOut[i]).Image = new Bitmap(PathIn[i]);
                }

                if (TransformIn.IsChanged)
                {
                    Vector3D scale;
                    Vector3D rotation;
                    Vector3D translation;
                    if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                        ControlOut[i].Bounds = new Rectangle((int)(translation.x * 100.0), (int)(translation.y * 100.0), (int)(scale.x * ((PictureBox)ControlOut[i]).Image.Width), (int)(scale.y * ((PictureBox)ControlOut[i]).Image.Height));
                }
            }
        }
    }
}
