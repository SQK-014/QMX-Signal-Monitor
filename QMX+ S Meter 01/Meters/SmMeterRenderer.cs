using System;
using System.Drawing;
using QMX__S_Meter_01.Utils;

namespace QMX__S_Meter_01.Meters
{
    internal class SmMeterRenderer
    {
        private readonly SmMeterModel model;

        public SmMeterRenderer(SmMeterModel model)
        {
            this.model = model;
        }

        public void Render(Graphics g, Rectangle rect)
        {
            g.Clear(Color.Black);

            float marginLeft = 10f;
            float marginRight = 15f;
            float innerWidth = rect.Width - marginLeft - marginRight;

            double sValue = DbToSConverter.DbmToS(model.Value);
            double sClamped = Math.Max(0.0, Math.Min(sValue, 15.0));

            float barTop = rect.Height * 0.45f;
            float barBottom = rect.Height;
            float labelY = barTop - 14f;

            using (var back = new SolidBrush(Color.FromArgb(80, 80, 80)))
                g.FillRectangle(back, marginLeft, barTop, innerWidth, barBottom - barTop);

            float s9Pos = (9.0f / 15.0f) * innerWidth;
            float smPos = (float)(sClamped / 15.0 * innerWidth);

            using (var green = new SolidBrush(Color.Lime))
            {
                float greenEnd = Math.Min(smPos, s9Pos);
                g.FillRectangle(green,
                    marginLeft,
                    barTop,
                    greenEnd,
                    barBottom - barTop);
            }

            if (smPos > s9Pos)
            {
                using (var red = new SolidBrush(Color.OrangeRed))
                    g.FillRectangle(red,
                        marginLeft + s9Pos,
                        barTop,
                        smPos - s9Pos,
                        barBottom - barTop);
            }

            string[] labels = {
                "1","2","3","4","5","6","7","8","9",
                "+10","+20","+30","+40","+50","+60"
            };

            int[] sSteps = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            using (var pen = new Pen(Color.Gray))
            using (var font = new Font("Consolas", 8))
            using (var b = new SolidBrush(Color.White))
            {
                for (int i = 0; i < sSteps.Length; i++)
                {
                    float pos = (sSteps[i] / 15f) * innerWidth;
                    float x = marginLeft + pos;

                    g.DrawLine(pen, x, barTop, x, barBottom);

                    float labelX = (i < 9) ? x - 5f : x - 8f;
                    g.DrawString(labels[i], font, b, labelX, labelY);
                }
            }

            //
            // ★ S値の表示ロジック（S9超過は S9+○dB 表記）
            //
            string sText;

            if (sValue <= 9.0)
            {
                // S9以下は通常表記
                sText = $"S {sValue:F1}";
            }
            else
            {
                // S9超過分の dB を計算（10dB/step）
                double overDb = (sValue - 9.0) * 10.0;
                sText = $"S9+{overDb:F0}dB";
            }

            //
            // ★ 描画位置（あなたの最新コードに合わせて +1文字右へ移動済み）
            //
            float textX = marginLeft + innerWidth - 38;   // -45 → -38（+7px = 1文字）
            float textY = barTop + 5;

            using (var font = new Font("Consolas", 10))
            using (var brush = new SolidBrush(Color.White))
                g.DrawString(sText, font, brush, textX, textY);
        }
    }
}
