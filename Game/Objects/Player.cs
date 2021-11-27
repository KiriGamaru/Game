using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Objects
{
    class Player : BaseObject
    {
        public Action<GreenCircle> OnGreenrOverlap;
        public Action<Marker> OnMarkerOverlap;
        public float vX, vY;
        public Player(float x, float y, float angle) : base(x, y, angle)
        {
        }

        public override void Render(Graphics g)
        {
            g.FillEllipse(new SolidBrush(Color.DeepPink), -15, -15, 30, 30);//кружочек с синим фоном
            g.DrawEllipse(new Pen(Color.Black, 2), -15, -15, 30, 30);//чёрная обводка кружочка
            g.DrawLine(new Pen(Color.Black, 2), 0, 0, 25, 0);//палочка, указывающая направление игрока
        }

        public override GraphicsPath GetGraphicsPath() // добавление круга, совпадающего по размерам с кругом из Render
        {
            var path = base.GetGraphicsPath();
            path.AddEllipse(-15, -15, 30, 30);
            return path;
        }

        public override void Overlap (BaseObject obj)
        {
            base.Overlap(obj);

            if (obj is Marker)
            {
                OnMarkerOverlap(obj as Marker);
            }

            if (obj is GreenCircle)
            {
                OnGreenrOverlap(obj as GreenCircle);
            }
        }
    }
}
