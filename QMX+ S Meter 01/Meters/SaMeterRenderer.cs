using System;
using System.Drawing;

namespace QMX__S_Meter_01.Meters
{
    internal class SaMeterRenderer
    {
        private readonly SaMeterModel model;

        public SaMeterRenderer(SaMeterModel model)
        {
            this.model = model;
        }

        public void Render(Graphics g, Rectangle rect)
        {
            g.Clear(Color.Black);

            float marginLeft = 10f;
            float marginRight = 10f;
            float innerWidth = rect.Width - marginLeft - marginRight;

            double value = model.Value;
            double norm = Math.Min(value / 30.0, 1.0);

            float barTop = rect.Height * 0.45f;
            float barBottom = rect.Height;
            float labelY = barTop - 14f;

            using (var back = new SolidBrush(Color.FromArgb(80, 80, 80)))
                g.FillRectangle(back, marginLeft, barTop, innerWidth, barBottom - barTop);

            float barWidth = (float)(norm * innerWidth);
            using (var brush = new SolidBrush(Color.LightGreen))
                g.FillRectangle(brush, marginLeft, barTop, barWidth, barBottom - barTop);

            using (var pen = new Pen(Color.Gray))
            using (var font = new Font("Consolas", 8))
            using (var b = new SolidBrush(Color.White))
            {
                int[] labels = { 0, 5, 10, 15, 20, 25, 30 };
                int count = labels.Length;

                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / (count - 1);
                    float x = marginLeft + t * innerWidth;

                    g.DrawLine(pen, x, barTop, x, barBottom);
                    g.DrawString(labels[i].ToString(), font, b, x - 6, labelY);
                }
            }

            float textX = marginLeft + innerWidth - 45;
            float textY = barTop + 5;

            using (var font = new Font("Consolas", 10))
            using (var brush = new SolidBrush(Color.White))
                g.DrawString($"{value:F1} dB", font, brush, textX, textY);
        }
    }
}