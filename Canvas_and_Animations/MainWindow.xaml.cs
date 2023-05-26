using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Canvas_and_Animations
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        public lineControl Line;
        public int Mode = 1;
        
        public int Delay = 0;
        public int Speed = 2000;

        private readonly List<lineControl> _lines = new List<lineControl>();
        private System.Threading.Thread _animation;

        private void Animate(object sender = null, RoutedEventArgs e = null)
        {
            
            StopAnimation();
            canvas.IsEnabled = false;

            lineControl.CompleteAnimationCallback callback = delegate (lineControl l)
            {
                
                Task.Run(delegate ()
                {
                    _animation = System.Threading.Thread.CurrentThread; //берём поток нащей функии чтобы потом в любой момент убить его
                    foreach (lineControl line in _lines)
                    {
                        if (_animation == null) return;
                        System.Threading.Thread.Sleep(Delay); //задержка
                        App.Current.Dispatcher.Invoke(delegate ()
                        {
                            line.SetSpeed(new TimeSpan(0, 0, 0, 0, Speed)); //установка скорости
                            line.BeginAnimation(); //старт анимации
                        });

                    }
                });
            };

            _lines.Last().CompleteAnimationEvent += callback; //подписиваемся на событие

            foreach (lineControl line in _lines)
            {
                line.line.X1 = line.line.X2 = line.line.Y1 = line.line.Y2 = 0; //скрываем линии
            }
            callback(null);

        }

        private void StopAnimation(object sender = null, RoutedEventArgs e = null)
        {

            _animation?.Abort(); //убиваем поток если он есть
            _animation = null;

            foreach (lineControl line in _lines) //убираем функции с события и останавливаем анимации
            {
                line.StopAnimation();
                line.RemoveAllHandles_CompleteAnimationEvent();
            }
            canvas.IsEnabled = true;
        }

        private void SetLinePosition(int mode)
        {
            if (Line == null) return;
            switch (mode)
            {
                case 1: //стандарт
                    Line.X2 = Mouse.GetPosition(this).X - canvas.Margin.Left;
                    Line.Y2 = Mouse.GetPosition(this).Y - canvas.Margin.Top;
                    break;

                case 2: //по углом 45 градусов слева
                    Line.X2 = Mouse.GetPosition(this).X - canvas.Margin.Left;
                    Line.Y2 = Line.Y1 - (Line.X1 - Mouse.GetPosition(this).X + canvas.Margin.Left);
                    break;

                case 3: //по углом 45 градусов справа
                    Line.X2 = Mouse.GetPosition(this).X - canvas.Margin.Left;
                    Line.Y2 = Line.Y1 + (Line.X1 - Mouse.GetPosition(this).X + canvas.Margin.Left);
                    break;

                case 4: //по вертикале
                    Line.X2 = Line.X1;
                    Line.Y2 = Mouse.GetPosition(this).Y - canvas.Margin.Top;
                    break;

                case 5: //по горизонтале
                    Line.X2 = Mouse.GetPosition(this).X - canvas.Margin.Left;
                    Line.Y2 = Line.Y1;
                    break;
            }
            //Console.WriteLine($"X1: {l.X1}, X2: {l.X2}, Y1: {l.Y1}, Y2: {l.Y2}, V: {(l.X1 - Mouse.GetPosition(this).X)}"); //DEBUG
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            SetLinePosition(Mode); //обновляем линию
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Line = new lineControl()
            {
                X1 = (Line == null) ? Mouse.GetPosition(this).X - canvas.Margin.Left : Line.X2,
                Y1 = (Line == null) ? Mouse.GetPosition(this).Y - canvas.Margin.Top : Line.Y2,
                X2 = Mouse.GetPosition(this).X - canvas.Margin.Left,
                Y2 = Mouse.GetPosition(this).Y - canvas.Margin.Top
            };
            Line.line.StrokeThickness = 2; //шырина линии

            _lines.Add(Line);
            canvas.Children.Add(Line);
            
        }

        private void Canvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            SetLinePosition(Mode);
            Line = null;

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Source.GetType() == typeof(TextBox) || (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))) return; //проверка на Ctrl
                Mode = Convert.ToInt32(e.Key.ToString()[1].ToString());
                ModeChange_Click(Modes_grid.Children[Mode - 1]);
            }
            catch (Exception)
            {

            }
            finally
            {
                
                Canvas_MouseMove(null, null);
            }

            
        }

        

        private void TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender.GetType() != typeof(TextBox)) return;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void Delay_TextChanged(object sender, TextChangedEventArgs e)
        {
            Delay = ((sender as TextBox).Text == "") ? 0 : Convert.ToInt32((sender as TextBox).Text);
        }

        private void Speed_TextChanged(object sender, TextChangedEventArgs e)
        {
            Speed = ((sender as TextBox).Text == "") ? 0 : Convert.ToInt32((sender as TextBox).Text);
        }

        private void ModeChange_Click(object sender, RoutedEventArgs e = null)
        {
            ToggleButton s = sender as ToggleButton;
            UniformGrid parent = s.Parent as UniformGrid;

            foreach (ToggleButton button in parent.Children)
            {
                button.IsChecked = false;
            }
            s.IsChecked = true;

            Mode = Convert.ToInt32(s.Tag);
            Canvas_MouseMove(null, null);
        }
    }
}
