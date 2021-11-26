﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing; // добавил using
using System.Drawing.Drawing2D; //и ещё один

namespace Game.Objects
{
    class BaseObject
    {
        public float X;
        public float Y;
        public float Angle;

        // добавил поле делегат, к которому можно будет привязать реакцию на собыития
        public Action<BaseObject, BaseObject> OnOverlap;

        public BaseObject(float x, float y, float angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }

        public Matrix GetTransform()
        {
            // вытаскиваем матрицу преобразования Graphics
            var matrix = new Matrix();
            matrix.Translate(X, Y); // смещаем ее в пространстве
            matrix.Rotate(Angle); // меняем угол
            return matrix;
        }

        // виртуальный метод для отрисовки
        public virtual void Render(Graphics g)
        {
            // тут пусто
        }

        public virtual GraphicsPath GetGraphicsPath()
        {
            // пока возвращаем пустую форму
            return new GraphicsPath();
        }

        // так как пересечение учитывает толщину линий и матрицы трансформацией
        // то для того чтобы определить пересечение объекта с другим объектом
        // надо передать туда объект Graphics
        public virtual bool Overlaps(BaseObject obj, Graphics g)
        {
            // берем информацию о форме
            var path1 = this.GetGraphicsPath();
            var path2 = obj.GetGraphicsPath();

            // применяем к объектам матрицы трансформации
            path1.Transform(this.GetTransform());
            path2.Transform(obj.GetTransform());

            // используем класс Region, который позволяет определить пересечение объектов в данном графическом контексте
            var region = new Region(path1);
            region.Intersect(path2); // пересекаем формы
            return !region.IsEmpty(g); // если полученная форма не пуста то значит было пересечение
        }

        public virtual void Overlap(BaseObject obj)
        {
            if (this.OnOverlap != null) // если к полю есть привязанные функции
            {
                this.OnOverlap(this, obj);//то вызываем их
            }
        }
    }
}
