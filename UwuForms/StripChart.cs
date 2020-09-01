using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Uwu.Core;

namespace UwuForms
{
    public partial class StripChart : Canvas
    {
        List<float> dots;
        float min = 0;
        float max = 1;
        private MessageDeliveryAgent messageDeliveryAgent1;
        private IContainer components;
        float dotsize = 20;

        public StripChart()
            : base()
        {
            dots = new List<float>();
            DrawCanvas += StripChart_DrawCanvas;

            Start(30);
        }

        private void StripChart_DrawCanvas(object sender, Graphics g, float dt)
        {
            float dot = (float)Math.Sin(Runtime.Milliseconds*3/1000f);
            Add(dot);

            RectangleF box = new RectangleF();
            float scale = 980 / (max - min);
            for (int i = 0; i < dots.Count; i++) {
                box.X = i*10 + dotsize/2;
                box.Y = (dots[i]-min)*scale + dotsize/2;
                box.Width = dotsize;
                box.Height = dotsize;
                g.FillEllipse(Brushes.White, box);
            }
        }

        (float, float) DotRange() {
            float mindot = float.MaxValue;
            float maxdot = float.MinValue;
            foreach (var dot in dots) {
                if (dot < mindot) mindot = dot;
                if (dot > maxdot) maxdot = dot;
            }
            return (mindot, maxdot);
        }


        public new void Scroll(int ndots)
        {
            dots.RemoveRange(0, ndots);
            (min, max) = DotRange();
        }

        public void Add(float val)
        {
            dots.Add(val);
            if (dots.Count > 100) Scroll(1);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.messageDeliveryAgent1 = new UwuForms.MessageDeliveryAgent(this.components);
            this.SuspendLayout();
            // 
            // StripChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "StripChart";
            this.ResumeLayout(false);

        }
    }
}
