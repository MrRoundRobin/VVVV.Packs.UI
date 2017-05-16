using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

namespace VVVV.Packs.UI.Nodes.WPF
{
    public class WPFGenericNode<T> where T : FrameworkElement, new()
    {
        // ReSharper disable UnassignedField.Global
        // ReSharper disable MemberCanBePrivate.Global

        [Input("Background Color", DefaultColor = new[] { 0.86, 0.86, 0.86, 1.0 }, HasAlpha = true, Order = 5)]
        public IDiffSpread<RGBAColor> BackColorIn;

        // ReSharper disable once MemberCanBeProtected.Global
        [Output("Element", Order = 0)]
        public ISpread<UIElement> UIElementOut;

        [Input("Enabled", IsToggle = true, DefaultBoolean = true, Order = 9999)]
        public IDiffSpread<bool> EnabledIn;

        [Input("Forground Color", DefaultColor = new[] { 0.0, 0.0, 0.0, 1.0 }, HasAlpha = true, Order = 4)]
        public IDiffSpread<RGBAColor> ForeColorIn;

        [Input("Border Color", DefaultColor = new[] { 0.44, 0.44, 0.44, 1.0 }, HasAlpha = true, Order = 6, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<RGBAColor> BorderColorIn;

        [Input("Transform", Order = 0)]
        public IDiffSpread<Matrix4x4> TransformIn;

        [Input("Border Thickness", DefaultValue = 1, AsInt = true, Order = 7, MinValue = 0, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<double> BorderThicknessIn;

        [Input("ToolTip", DefaultString = "", Order = 8, Visibility = PinVisibility.OnlyInspector)]
        public IDiffSpread<string> TipIn;

        [Input("Font", EnumName = "SystemFonts", Order = 11)]
        public IDiffSpread<EnumEntry> FontIn;

        [Input("Font Size", DefaultValue = 12, Order = 12)]
        public IDiffSpread<double> FontSizeIn;

        [Input("Bold", DefaultBoolean = false, IsToggle = true, Order = 13)]
        public IDiffSpread<bool> BoldIn;

        [Input("Italic", DefaultBoolean = false, IsToggle = true, Order = 14)]
        public IDiffSpread<bool> ItalicIn;
        
        // ReSharper restore UnassignedField.Global
        // ReSharper restore MemberCanBePrivate.Global

        protected void CreateElement(int i)
        {
            UIElementOut[i] = new T
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,

                Tag = i
            };

            SetProperties(i, true);
        }

        protected void SetProperties(int i, bool force = false)
        {
            var uiElement = UIElementOut[i];

            if (EnabledIn.IsChanged || force)
                uiElement.IsEnabled = EnabledIn[i];

            if (ForeColorIn.IsChanged || force)
                uiElement.SetValue(Control.ForegroundProperty, ForeColorIn[i].AsBrush());

            if (BackColorIn.IsChanged || force)
                uiElement.SetValue(Control.BackgroundProperty, BackColorIn[i].AsBrush());

            if (BorderColorIn.IsChanged || force)
                uiElement.SetValue(Control.BorderBrushProperty, BorderColorIn[i].AsBrush());

            if (BorderThicknessIn.IsChanged || force)
                uiElement.SetValue(Control.BorderThicknessProperty, new Thickness(BorderThicknessIn[i]));

            if (TipIn.IsChanged || force)
                uiElement.SetValue(FrameworkElement.ToolTipProperty, TipIn[i] == "" ? null : TipIn[i]);

            if (FontIn.IsChanged || force)
                uiElement.SetValue(Control.FontFamilyProperty, new FontFamily(FontIn[i].Name));

            if (FontSizeIn.IsChanged || force)
                uiElement.SetValue(Control.FontSizeProperty, FontSizeIn[i]);

            if (BoldIn.IsChanged || force)
                uiElement.SetValue(Control.FontWeightProperty, BoldIn[i] ? FontWeights.Bold : FontWeights.Normal);

            if (ItalicIn.IsChanged || force)
                uiElement.SetValue(Control.FontStyleProperty, ItalicIn[i] ? FontStyles.Italic : FontStyles.Normal);

            if (TransformIn.IsChanged || force)
            { //TODO: Rotation
                Vector3D scale;
                Vector3D rotation;
                Vector3D translation;

                if (TransformIn[i].Decompose(out scale, out rotation, out translation))
                {
                    TransformGroup transformGroup;
                    if (!(uiElement.RenderTransform is TransformGroup))
                    {
                        uiElement.RenderTransform = new TransformGroup();
                        transformGroup = uiElement.RenderTransform as TransformGroup;

                        transformGroup.Children.Add(new ScaleTransform());
                        //transformGroup.Children.Add(new RotateTransform());
                        transformGroup.Children.Add(new TranslateTransform());
                    }
                    else
                        transformGroup = uiElement.RenderTransform as TransformGroup;

                    var scaleTransform = transformGroup.Children.First(transform => transform is ScaleTransform) as ScaleTransform;
                    if (scaleTransform != null)
                    {
                        scaleTransform.ScaleX = scale.x;
                        scaleTransform.ScaleY = scale.y;
                    }

                    //var rotateTransform = transformGroup.Children.First(transform => transform is RotateTransform) as RotateTransform;
                    //if (rotateTransform != null)
                    //    rotateTransform.Angle = rotation.x;

                    var translateTransform = transformGroup.Children.First(transform => transform is TranslateTransform) as TranslateTransform;
                    if (translateTransform != null)
                    {
                        translateTransform.X = translation.x;
                        translateTransform.Y = translation.y;
                    }
                }
            }
        }
    }
}
