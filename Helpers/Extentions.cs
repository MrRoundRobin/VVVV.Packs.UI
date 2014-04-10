using System.Windows.Media;
using VVVV.Utils.VColor;

namespace VVVV.Packs.UI.Helpers
{
    public static class Extentions
    {
        public static Brush AsBrush(this RGBAColor rgbaColor)
        {
            return new SolidColorBrush(Color.FromArgb(rgbaColor.Color.A, rgbaColor.Color.R, rgbaColor.Color.G, rgbaColor.Color.B));
        }
    }
}
