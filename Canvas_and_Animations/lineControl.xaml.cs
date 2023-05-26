using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Canvas_and_Animations
{
    /// <summary>
    /// Interaction logic for lineControl.xaml
    /// </summary>
    public partial class lineControl : UserControl
    {
        public lineControl()
        {
            InitializeComponent();
        }

        static lineControl()
        {
            X1P = DependencyProperty.Register("X1", typeof(double), typeof(lineControl), new PropertyMetadata(0.1, X1_PC));
            X2P = DependencyProperty.Register("X2", typeof(double), typeof(lineControl), new PropertyMetadata(0.1, X2_PC));
            Y1P = DependencyProperty.Register("Y1", typeof(double), typeof(lineControl), new PropertyMetadata(0.1, Y1_PC));
            Y2P = DependencyProperty.Register("Y2", typeof(double), typeof(lineControl), new PropertyMetadata(0.1, Y2_PC));
        }


        #region Property

        public double X1
        {
            get { return (double)GetValue(X1P); }
            set { SetValue(X1P, value); }
        }

        public double X2
        {
            get { return (double)GetValue(X2P); }
            set { SetValue(X2P, value); }
        }

        public double Y1
        {
            get { return (double)GetValue(Y1P); }
            set { SetValue(Y1P, value); }
        }

        public double Y2
        {
            get { return (double)GetValue(Y2P); }
            set { SetValue(Y2P, value); }
        }

        #endregion

        #region DependencyProperty

        public static readonly DependencyProperty X1P;
        public static readonly DependencyProperty X2P;
        public static readonly DependencyProperty Y1P;
        public static readonly DependencyProperty Y2P;

        #endregion

        #region Change calback functions

        public static void X1_PC(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            lineControl c = obj as lineControl;
            double nv = (double)e.NewValue;
            c.X1 = c.line.X1 = nv;
            c.ChangeAnimationValue();
        }

        public static void X2_PC(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            lineControl c = obj as lineControl;
            double nv = (double)e.NewValue;
            c.X2 = c.line.X2 = nv;
            c.ChangeAnimationValue();
        }

        public static void Y1_PC(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            lineControl c = obj as lineControl;
            double nv = (double)e.NewValue;
            c.Y1 = c.line.Y1 = nv;
            c.ChangeAnimationValue();
        }

        public static void Y2_PC(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            lineControl c = obj as lineControl;
            double nv = (double)e.NewValue;
            c.Y2 = c.line.Y2 = nv;
            c.ChangeAnimationValue();
        }

        #endregion

        #region Delegates structure

        public delegate void CompleteAnimationCallback(lineControl lineControl);

        #endregion

        #region Class variables
        //спикок функций(CompleteAnimationCallback) подписанных на событие CompleteAnimationEvent 
        private readonly List<CompleteAnimationCallback> _listHandles = new List<CompleteAnimationCallback>();

        public event CompleteAnimationCallback CompleteAnimationEvent
        {
            add
            {
                _listHandles.Add(value);
            }

            remove
            {
                _listHandles.Remove(value);
            }
        }
        public TimeSpan SpeedAnimation { get; private set; }

        #endregion

        private void ChangeAnimationValue()
        {
            TimelineCollection collection = a.Storyboard.Children;//берём колекцию колекций кадров
            //устанавливаем значения
            (collection[0] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (collection[1] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (collection[0] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = X1;
            (collection[2] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (collection[3] as DoubleAnimationUsingKeyFrames).KeyFrames[0].Value = (collection[2] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = Y1;

            (collection[0] as DoubleAnimationUsingKeyFrames).KeyFrames[2].Value = X2;
            (collection[1] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = X2;
            (collection[2] as DoubleAnimationUsingKeyFrames).KeyFrames[2].Value = Y2;
            (collection[3] as DoubleAnimationUsingKeyFrames).KeyFrames[1].Value = Y2;
        }

        public void BeginAnimation()
        {
            a.Storyboard.Begin();
        }

        public void StopAnimation()
        {
            a.Storyboard.Stop();

            //возвращаем значения
            line.X1 = X1;
            line.X2 = X2;
            line.Y1 = Y1;
            line.Y2 = Y2;
        }

        public void RemoveAllHandles_CompleteAnimationEvent()
        {
            for (int i = 0; i < _listHandles.Count; i++)
            {
                CompleteAnimationEvent -= _listHandles[i];
                //_listHandles.Remove(_listHandles[i]);
            }
        }

        public void SetSpeed(TimeSpan time)
        {
            StopAnimation();

            TimelineCollection collection = a.Storyboard.Children;//берём колекцию колекций кадров
            //устанавливаем значения
            (collection[0] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = (collection[1] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = (collection[2] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = (collection[3] as DoubleAnimationUsingKeyFrames).KeyFrames[1].KeyTime = new TimeSpan(0, 0, 0, 0, (int)Math.Round(time.TotalMilliseconds / 2));
            (collection[0] as DoubleAnimationUsingKeyFrames).KeyFrames[2].KeyTime = (collection[2] as DoubleAnimationUsingKeyFrames).KeyFrames[2].KeyTime = time;
            SpeedAnimation = time;
        }

        private void CompleteAnimation(object sender, EventArgs e)
        {
            //CompleteAnimationEvent?.Invoke(this);
            foreach (CompleteAnimationCallback callback in _listHandles)
            {
                callback(this);//вызываем функции
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            a.Storyboard.Completed += CompleteAnimation; //подписываемся на событие оканчании анимации
        }
    }
}
