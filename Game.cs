using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Lines
{
    public class Game
    {
        public static Random Rnd = new Random();
        protected List<Point> FreeCells = new List<Point>();


        public int Rows { get; set; }
        public int Cols { get; set; }

        public bool StatusGame { get; set; }

        public int Scores { get; protected set; }

        protected int[,] _field;


        public Game(int cols = 9, int rows = 9)
        {
            _field = new int[cols, rows];
            Cols = cols;
            Rows = rows;
            StatusGame = true;
            Scores = 0;


            for (int i = 0; i < Cols; ++i)
            {
                for (int j = 0; j < Rows; ++j)
                {
                    FreeCells.Add(new Point(j, i));
                }
            }

            for (int i = 0; i < FreeCells.Count; ++i)
            {
                int rnd = Game.Rnd.Next(FreeCells.Count);
                Point tmp = FreeCells[rnd];
                FreeCells[rnd] = FreeCells[i];
                FreeCells[i] = tmp;
            }

        }


    }




    public class GDI_Game : Game
    {
        List<Ball> _balls = new List<Ball>();

        public Ball choosBall = new Ball(new Point(-1, -1));

        Color lineColor = Color.White;
        RectangleF _rectField;
        RectangleF _rectCell;

        public GDI_Game()
        {

        }

        private void CreateWay(ref Point[] waypoints,ref int p , Point choosenPoint, Graphics Gr)
        {
            Point tmp = choosenPoint;

            //Gr.DrawRectangle(new Pen(Color.Red), _rectCell.Width * tmp.X, _rectCell.Height * tmp.Y,
            //                    _rectCell.Width, _rectCell.Height);

            int N = _field[choosenPoint.X, choosenPoint.Y];

            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            if (N == 2)
            {
                waypoints[0] = choosenPoint;
                return;
            }

            do
            {
                for (int k = 0; k < 4; ++k)
                {
                    if ((tmp.X + dx[k] != -1 && tmp.X + dx[k] != Cols && tmp.Y + dy[k] != -1 && tmp.Y + dy[k] != Rows &&
                        _field[tmp.X + dx[k], tmp.Y + dy[k]] == N - 1) || new Point(tmp.X + dx[k], tmp.Y + dy[k]) == choosBall._point)
                    {
                        tmp.X = tmp.X + dx[k];
                        tmp.Y = tmp.Y + dy[k];

                        waypoints[p] = tmp;

                        break;

                        //Gr.DrawRectangle(new Pen(Color.Red), _rectCell.Width * tmp.X, _rectCell.Height * tmp.Y,
                        //    _rectCell.Width-1, _rectCell.Height-1);

                    }
                }

                N--;
                p++;

            }
            while (tmp != choosBall._point);

            waypoints[p - 1] = choosBall._point;

        }

        private void DrawWay(ref Point[] waypoints, ref int p, Point choosenPoint , Graphics Gr)
        {
            GraphicsPath path = new GraphicsPath();

            for (p = waypoints.Length-1; p >= 0; --p)
            {
                if (waypoints.Length != 1 && p == waypoints.Length - 1) --p;

                path.AddEllipse(new RectangleF(new PointF(waypoints[p].X * _rectCell.Width + _rectCell.Width / 4,
                        waypoints[p].Y * _rectCell.Height + _rectCell.Height / 4),
                        new SizeF(_rectCell.Width / 2, _rectCell.Height / 2)));

                Gr.DrawPath(new Pen(Color.White), path);
                Gr.FillPath(new SolidBrush(Color.White), path);

                Thread.Sleep(20);

            }

                 Point tmp = choosBall._point;

                 for (int i = 0; i < _balls.Count; ++i)
                 {
                     if (_balls[i]._point == choosBall._point)
                     {
                         _balls[i].BallPointSet(choosenPoint);

                         _balls[i]._rect = new RectangleF((float)_balls[i]._point.X * _rectCell.Width,
                             (float)_balls[i]._point.Y * _rectCell.Height,
                             _rectCell.Width - 1, _rectCell.Height - 1);

                         for (int j = 0; j < FreeCells.Count; ++j)
                         {
                             if (FreeCells[j] == choosenPoint)
                             {
                                 FreeCells.RemoveAt(j);
                                 FreeCells.Add(tmp);
                                 break;
                             }
                         }

                         GraphicsPath path1 = new GraphicsPath();
                         path1.AddRectangle(new RectangleF((float)tmp.X * _rectCell.Width, (float)tmp.Y * _rectCell.Height,
                             _rectCell.Width - 1, _rectCell.Height - 1));

                         Gr.DrawPath(new Pen(Color.White), path1);
                         Gr.FillPath(new SolidBrush(Color.DarkGray), path1);

                         _balls[i].Draw(Gr);

                         UnlockBall(choosenPoint, Gr);

                         break;
                     }
                 }

                 for (int i = waypoints.Length - 1; i >= 0; --i)
                 {

                     GraphicsPath path1 = new GraphicsPath();
                     path1.AddRectangle(new RectangleF((float)waypoints[i].X * _rectCell.Width,
                         (float)waypoints[i].Y * _rectCell.Height,
                         _rectCell.Width - 1, _rectCell.Height - 1));

                     Gr.DrawPath(new Pen(Color.White), path1);
                     Gr.FillPath(new SolidBrush(Color.DarkGray), path1);

                     Thread.Sleep(20);
                 }

                 DrawField(Gr);
        }

        public void MoveBall(Point choosenPoint, Graphics Gr)
        {
            bool move = Wave(choosenPoint, Gr);

            if (move)
            {
                Point[] waypoints = new Point[_field[choosenPoint.X, choosenPoint.Y] - 1];
                int p = 0;

                CreateWay(ref waypoints, ref p, choosenPoint, Gr);

                DrawWay(ref waypoints, ref p, choosenPoint, Gr);

                LineBall(Gr, choosenPoint);

                GenerateBalls(Gr);

            }

            else
                UnlockBall(choosenPoint, Gr);
        }


        private void LineBall(Graphics Gr, Point choosenPoint)
        {
            Ball[,] fieldBalls = new Ball[Cols, Rows];

            List<Ball> line = new List<Ball>();

            int X=choosenPoint.X;
            int Y=choosenPoint.Y;

            int N = 1;

            Color clrLine;

            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int i = 0; i < _balls.Count; ++i)
            {
                fieldBalls[_balls[i]._point.X, _balls[i]._point.Y] = _balls[i];
            }

            line.Add(fieldBalls[X, Y]);

            clrLine = fieldBalls[X, Y].GetColor();

            for (int k = 0; k < 4; ++k)
            {
                if (X+dx[k]!=-1 && X+dx[k]!=Cols && Y+dy[k]!=-1 && Y+dy[k]!=Rows &&
                    fieldBalls[X + dx[k], Y + dy[k]] != null &&
                    fieldBalls[X + dx[k], Y + dy[k]].GetColor() == clrLine)
                {
                    N++;
                    X+=dx[k];
                    Y+=dy[k];

                    line.Add(fieldBalls[X, Y]);

                    while (X + dx[k] != -1 && X + dx[k] != Cols && Y + dy[k] != -1 && Y + dy[k] != Rows &&
                    fieldBalls[X + dx[k], Y + dy[k]] != null &&
                    fieldBalls[X + dx[k], Y + dy[k]].GetColor() == clrLine)
                    {
                        N++;
                        X += dx[k];
                        Y += dy[k];
                        line.Add(fieldBalls[X, Y]);
                    }

                    X = choosenPoint.X;
                    Y = choosenPoint.Y;

                    if (k == 2 || k == 3) break;

                    if (k == 0) k = 1;
                    else k = 2;
                }
            }

            if (N >= 5)
            {
                DeleteBalls(line, Gr);

                Scores += N;
            }

        }

        private void DeleteBalls(List<Ball> line ,Graphics Gr)
        {
            GraphicsPath path = new GraphicsPath();

            Pen pen = new Pen(Color.White);

            SolidBrush br = new SolidBrush(Color.DarkGray);

            for (int i = 0; i < line.Count; ++i)
            {
                path.AddRectangle(line[i]._rect);
            }

            Gr.FillPath(br, path);
            Gr.DrawPath(pen, path);

            for (int i = 0; i < line.Count; ++i)
            {
                for (int j = 0; j < _balls.Count; ++j)
                {
                    if (line[i] == _balls[j])
                    {
                        FreeCells.Add(_balls[j]._point);
                        _balls.RemoveAt(j);
                    }
                }
            }
        }


        private bool Wave(Point finish, Graphics Gr)
        {
            Point start = choosBall._point;
            Point tmp;

            ResetField();

            //string loh;
            //FontFamily fontfam=new FontFamily(GenericFontFamilies.Monospace);
            //Font font=new Font(fontfam, 15);

            ////////////////////

            
            int N = 1;

            int[] dx = { 1, 0, -1, 0 };
            int[] dy = { 0, 1, 0, -1 };


            for (int k = 0; k < 4; ++k)
            {
                if (start.X+dx[k]!=-1 && start.X+dx[k]!=Cols && start.Y+dy[k]!=-1 && start.Y+dy[k]!=Rows &&
                    _field[start.X + dx[k], start.Y+dy[k]] == N)
                {
                    _field[start.X+dx[k], start.Y+dy[k]] = N + 1;



                    tmp = new Point(start.X+dx[k], start.Y+dy[k]);

                    //loh = _field[tmp.X, tmp.Y].ToString();

                    //Gr.DrawString(loh, font, new SolidBrush(Color.Red), new RectangleF(_rectCell.Width * tmp.X, _rectCell.Height * tmp.Y,
                    //            _rectCell.Width, _rectCell.Height));

                    if (tmp == finish)
                        return true;
                }
            }

            N++;

            bool NextStep;

            do
            {
                NextStep = false;

                for (int i = 0; i < Cols; ++i)
                {
                    for (int j = 0; j < Rows; ++j)
                    {

                        if (_field[i, j] == N)
                        {

                            for (int k = 0; k < 4; ++k)
                            {

                                if (i + dx[k] != -1 && i + dx[k] != Cols && j + dy[k] != -1 && j + dy[k] != Rows &&
                                    _field[i+dx[k], j+dy[k]] == 1)
                                {
                                    _field[i+dx[k], j+dy[k]] = N + 1;

                                    NextStep = true;

                                    //loh = _field[i + dx[k], j + dy[k]].ToString();

                                    //Gr.DrawString(loh, font, new SolidBrush(Color.White),
                                    //    new RectangleF(_rectCell.Width * (i + dx[k]), _rectCell.Height * (j + dy[k]),
                                    //    _rectCell.Width, _rectCell.Height));
                                }
                            }
                        }
                    }
                }

                N++;
            }
            while (_field[finish.X, finish.Y]==1 && NextStep);
            
            ////////////////////
            if (_field[finish.X, finish.Y] != 1)
                return true;

            else return false;
        }

        private void ResetField()
        {
            for (int i = 0; i < FreeCells.Count; ++i)
            {
                _field[FreeCells[i].X, FreeCells[i].Y] = 1;
            }

            for (int i = 0; i < _balls.Count; ++i)
            {
                _field[_balls[i]._point.X, _balls[i]._point.Y] = 0;
            }
        }

        public void _RestartGame(Graphics Gr)
        {
            Scores = 0;

            UnlockBall(choosBall._point, Gr);

            FreeCells.Clear();

            for (int i = 0; i < Cols; ++i)
            {
                for (int j = 0; j < Rows; ++j)
                {
                    FreeCells.Add(new Point(j, i));
                }
            }

            for (int i = 0; i < FreeCells.Count; ++i)
            {
                int rnd = Game.Rnd.Next(FreeCells.Count);
                Point tmp = FreeCells[rnd];
                FreeCells[rnd] = FreeCells[i];
                FreeCells[i] = tmp;
            }

            _balls.Clear();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(_rectField);
            Gr.FillPath(new SolidBrush(Color.DarkGray), path);
            Gr.DrawPath(new Pen(Color.DarkGray), path);

            for (int i = 0; i < 3;++i)
                GetBall();

            ResetField();

            DrawField(Gr);
        }

        private void UnlockBall(Point choosenPoint, Graphics Gr)
        {
            SolidBrush br = new SolidBrush(Color.DarkGray);
            Pen p = new Pen(Color.White);

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(choosBall._rect);

            Gr.DrawPath(p, path);
            Gr.FillRectangle(br, choosBall._rect);
            DrawField(Gr);

            choosBall = new Ball(new Point(-1, -1));
        }

        private void LockBall(Point choosenPoint, Graphics Gr, int num)
        {
            choosBall = _balls[num];

            /////////

            SolidBrush br = new SolidBrush(Color.White);

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(choosBall._rect);

            Pen p = new Pen(Color.White);

            Gr.DrawPath(p, path);
            Gr.FillRectangle(br, choosBall._rect);

            choosBall.Draw(Gr);


            /////////
        }

        public bool ChoosBallRect(ref Point choosenPoint, Graphics Gr)
        {


            choosenPoint.X = (int)(choosenPoint.X / _rectCell.Width);
            choosenPoint.Y = (int)(choosenPoint.Y / _rectCell.Height);

            if (choosenPoint == choosBall._point)
            {
                UnlockBall(choosenPoint, Gr);
                return true;
            }

             ////////////

            else
            {

                for (int i = 0; i < _balls.Count; ++i)
                {
                    if (choosenPoint == _balls[i]._point && ((choosBall._point.X == -1 && choosBall._point.Y == -1) || choosBall != _balls[i]))
                    {
                        if (choosBall != _balls[i]) UnlockBall(choosBall._point, Gr);
                        LockBall(choosenPoint, Gr, i);
                        return true;
                    }
                }
            }

            return false;

        }

        public Ball GetBall()
        {
            Ball retBall = new Ball(FreeCells[0]);
            FreeCells.RemoveAt(0);

            _balls.Add(retBall);

            return retBall;
        }

        public void GenerateBalls(Graphics Gr)
        {
            Ball tmp;

            for (int i = 0; i < 3; ++i)
            {
                if (FreeCells.Count > 1)
                {
                    tmp = GetBall();
                    LineBall(Gr, tmp._point);
                }

                else
                {
                    StatusGame = false;
                }
            }

            ResetField();

            DrawField(Gr);

        }

        public void DrawField(Graphics gr)
        {
            Pen pen = new Pen(lineColor);
           
            for (int i = 1; i < Rows; ++i)
            {
                gr.DrawLine(pen, 0, i * _rectField.Height / Rows, _rectField.Right, i * _rectField.Height / Rows);
            }
            for (int i = 1; i < Cols; ++i)
            {
                gr.DrawLine(pen, i * _rectField.Height / Cols, 0, i * _rectField.Height / Cols, _rectField.Right);
            }


            foreach (var ball in _balls)
            {
                ball.Draw(gr);
            }


        }

        internal void Resize(Rectangle rect)
        {
            _rectField = rect;
            _rectCell = new RectangleF(0, 0, _rectField.Width / Cols, _rectField.Height / Rows);

            Ball._radius = _rectCell.Width > _rectCell.Height ? _rectCell.Height : _rectCell.Width;
            Ball._cellRect = _rectCell;


            foreach (var ball in _balls)
            {
                ball._rect = new RectangleF(ball._point.X * Ball._cellRect.Width, ball._point.Y * +_rectCell.Height, _rectCell.Width, _rectCell.Height);
            }

        }
    }


    public class Ball
    {
        static Color[] BallColors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.DarkBlue, Color.DarkMagenta };
        public static float _radius = 10;
        public static RectangleF _cellRect;

        public RectangleF _rect;

        private Color _color;
        public Point _point
        {
            get;
            private set;
        }

        public Color GetColor()
        {
            return _color;
        }

        public void BallPointSet(Point p)
        {
            _point = p;
        }

        public Ball(Point pt)
        {
            _point = pt;
            _color = BallColors[Game.Rnd.Next(BallColors.Length)];

            _rect = new RectangleF(pt.X * Ball._cellRect.Width, pt.Y * +_cellRect.Height, _cellRect.Width-1, _cellRect.Height-1);
        }

        public void Draw(Graphics gr)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(_rect);

            PathGradientBrush br = new PathGradientBrush(path);

            br.CenterPoint = new Point((int)(_rect.Right + _rect.Width / 2), (int)(_rect.Top + _rect.Height / 2));

            br.SurroundColors = new Color[] { _color };
            br.CenterColor = Color.White;

            gr.FillPath(br, path);


            gr.DrawEllipse(new Pen(_color), _rect);
        }
    }
}
