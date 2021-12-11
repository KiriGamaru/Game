using Game.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public partial class Form1 : Form
    {
        List<BaseObject> objects = new();//список
        Player player;// поле под игрока
        Marker marker;//поле под маркер
        GreenCircle green1;//поле под первый зелёный кружочек
        GreenCircle green2;//поле под второй зелёный кружочек

        int p = 0;//счётчик очков
        public Form1()
        {
            InitializeComponent();

            player = new Player(pbMain.Width / 2, pbMain.Height / 2, 0); //экзэмпляр класса игрока в центре экрана
            var rnd = new Random();
            green1 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);//первый экзэмпляр класса зелёного кружочка в рандомном месте экрана
            green2 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);//второй экзэмпляр класса зелёного кружочка в рандомном месте экрана

            //реакция на пересечение                                                             
            player.OnOverlap += (p, obj) =>
            {
                txtLog.Text = $"[{DateTime.Now:HH:mm:ss:ff}] Игрок пересекся с {obj}\n" + txtLog.Text;
            };

            //реакция на пересечение с маркером
            player.OnMarkerOverlap += (m) =>
            {
                objects.Remove(m);
                marker = null;
            };

            //реакция на пересечение с зелёным кругом
            player.OnGreenrOverlap += (m) =>
            {
                var rnd = new Random();
                objects.Remove(m);

                if (m == green1)
                {
                    green1 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);
                    objects.Add(green1);
                }
                else
                {
                    green2 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);
                    objects.Add(green2);
                }
                p++;
                txtPoints.Text = $"очки: {p}";
            };

            marker = new Marker(pbMain.Width / 2 + 50, pbMain.Height / 2 + 50, 0);
            objects.Add(marker);
            objects.Add(green1);
            objects.Add(green2);
            objects.Add(player);
            
        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics; // вытащили объект графики из события
            g.Clear(Color.Gray);// залил фон
            updatePlayer();//пересчёт игрока

            



            // пересчитываем пересечения
            foreach (var obj in objects.ToList()) 
            {
                // проверяю было ли пересечение с игроком
                if (obj != player && player.Overlaps(obj, g))
                {
                    player.Overlap(obj); // то есть игрок пересекся с объектом
                    obj.Overlap(player); // и объект пересекся с игроком
                }
            }

            // рендерим объекты
            foreach (var obj in objects)
            {
                g.Transform = obj.GetTransform();
                obj.Render(g);
            }

        }

        private void updatePlayer()
        {
            //проверка на marker не нулевой
            if (marker != null)
            {
                //расчитываем вектор между игроком и маркером
                float dx = marker.X - player.X;
                float dy = marker.Y - player.Y;

                // находим его длинну
                float length = MathF.Sqrt(dx * dx + dy * dy);
                dx /= length;//нормализуем координаты
                dy /= length;

                // пересчитываем координаты игрока
                player.vX += dx * 0.5f;
                player.vY += dy * 0.5f;

                // расчитываем угол поворота игрока 
                player.Angle = 90 - MathF.Atan2(player.vX, player.vY) * 180 / MathF.PI;
            }

            // тормозящий момент,
            // нужен чтобы, когда игрок достигнет маркера произошло постепенное замедление
            player.vX += -player.vX * 0.1f;
            player.vY += -player.vY * 0.1f;

            // пересчет позиции игрока с помощью вектора скорости
            player.X += player.vX;
            player.Y += player.vY;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pbMain.Invalidate(); //записываем обновление pbMain. "то вызывает метод pbMain_Paint по новой
        }

        private void pbMain_MouseClick(object sender, MouseEventArgs e)
        {
            //создание маркера по клику если он еще не создан
            if (marker == null)
            {
                marker = new Marker(0, 0, 0);
                objects.Add(marker); // и главное не забыть пололжить в objects
            }
            marker.X = e.X;
            marker.Y = e.Y;
        }



        private void timerGreen_Tick(object sender, EventArgs e)
        {
            green1.R--;
            green2.R--;

            var rnd = new Random();
            //реакция на нулевой радиус первого зелёного кружочка
            if (green1.R <= 0)
            {
                objects.Remove(green1);
                green1 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);
                objects.Add(green1);
            }
            //реакция на нулевой радиус второго зелёного кружочка
            if (green2.R <= 0)
            {
                objects.Remove(green2);
                green2 = new GreenCircle(rnd.Next(15, 430), rnd.Next(15, 240), 0);
                objects.Add(green2);
            }
        }
    }
}
