﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace System.Windows.Forms
{
    [Serializable]
    public class Button : Control
    {
        internal ColorF currentBackColor;
        private Color _normalColor;
        
        private bool _toggleEditor = true;

        public virtual Color HoverBorderColor { get; set; }
        public virtual Color HoverColor { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap ImageHover { get; set; }
        public Color ImageColor { get; set; }
        public virtual Color NormalBorderColor { get; set; }
        public virtual Color NormalColor
        {
            get { return _normalColor; }
            set
            {
                _normalColor = value;
                currentBackColor = value;
            }
        }
        public ContentAlignment TextAlign { get; set; }

        public Button()
        {
            BackgroundImageLayout = ImageLayout.Center;
            Font = new Drawing.Font("Arial", 12);
            ForeColor = Color.FromArgb(64, 64, 64);
            ImageColor = Color.White;
            NormalColor = Color.FromArgb(234, 234, 234);
            NormalBorderColor = Color.FromArgb(172, 172, 172);
            HoverColor = Color.FromArgb(223, 238, 252);
            HoverBorderColor = Color.FromArgb(126, 180, 234);
            TextAlign = ContentAlignment.MiddleCenter;
            Size = new Drawing.Size(75, 23);

            currentBackColor = NormalColor;
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == UnityEngine.KeyCode.Space)
                RaiseOnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, Width / 2, Height / 2, 0));
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if ((!Hovered) || !Enabled)
            {
                currentBackColor = MathHelper.ColorLerp(currentBackColor, NormalColor, 5);

                g.FillRectangle(new SolidBrush(currentBackColor), 0, 0, Width, Height);
                g.DrawRectangle(new Pen(NormalBorderColor), 0, 0, Width, Height);
            }
            else
            {
                currentBackColor = MathHelper.ColorLerp(currentBackColor, HoverColor, 5);

                g.FillRectangle(new SolidBrush(currentBackColor), 0, 0, Width, Height);
                g.DrawRectangle(new Pen(HoverBorderColor), 0, 0, Width, Height);
            }

            SolidBrush textBrush = new SolidBrush(ForeColor);
            if (!Enabled) textBrush.Color = ForeColor + Color.FromArgb(0, 128, 128, 128);
            if (Image != null && Image.uTexture != null)
            {
                switch (BackgroundImageLayout)
                {
                    default:
                    case ImageLayout.None:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, 0, 0, Image.Width, Image.Height, ImageColor);
                        break;
                    case ImageLayout.Center:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, Width / 2 - Image.Width / 2, Height / 2 - Image.Height / 2, Image.Width, Image.Height, ImageColor);
                        break;
                    case ImageLayout.Stretch:
                        g.DrawTexture(Hovered && ImageHover != null ? ImageHover : Image, 0, 0, Width, Height, ImageColor);
                        break;
                    case ImageLayout.Zoom:
                        // TODO: not working.
                        break;
                }
            }
            g.DrawString(Text, Font, textBrush,
                    Padding.Left,
                    Padding.Top,
                    Width - Padding.Left - Padding.Right,
                    Height - Padding.Top - Padding.Bottom, TextAlign);
        }
        protected override object OnPaintEditor(float width)
        {
            var control = base.OnPaintEditor(width);
            
            Editor.BeginVertical();
            Editor.NewLine(1);

            _toggleEditor = Editor.Foldout("Button", _toggleEditor);
            if (_toggleEditor)
            {
                Editor.BeginGroup(width - 24);

                Editor.ColorField("      HoverBorderColor", HoverBorderColor, (c) => { HoverBorderColor = c; });
                Editor.ColorField("      HoverColor", HoverColor, (c) => { HoverColor = c; });

                var editorImage = Editor.ObjectField("      Image", Image, typeof(UnityEngine.Texture2D));
                if (editorImage.Changed) Image = new Bitmap((UnityEngine.Texture2D)editorImage.Value);

                var editorImageHover = Editor.ObjectField("      ImageHover", ImageHover, typeof(UnityEngine.Texture2D));
                if (editorImageHover.Changed) ImageHover = new Bitmap((UnityEngine.Texture2D)editorImageHover.Value);

                Editor.ColorField("      ImageColor", ImageColor, (c) => { ImageColor = c; });
                Editor.ColorField("      NormalBorderColor", NormalBorderColor, (c) => { NormalBorderColor = c; });
                Editor.ColorField("      NormalColor", NormalColor, (c) => { NormalColor = c; });

                var editorTextAlign = Editor.EnumField("      TextAlign", TextAlign);
                if (editorTextAlign.Changed) TextAlign = (ContentAlignment)editorTextAlign.Value;

                Editor.Label("      TextMargin", Padding);

                Editor.EndGroup();
            }
            Editor.EndVertical();

            return control;
        }
    }
}
