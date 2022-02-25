using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
namespace ToDoListOnXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Stats : ContentPage
    {
        statsManager stats_manager;
        public Stats(int numberOfTasksToday, int numberOfDoneTasksToday)
        {

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            returnBtn.Clicked += returnToMainPage;
            stats_manager = new statsManager(numberOfDoneTasksToday, numberOfTasksToday);

            timesDoneLabel.Text = $"В этом месяце вы выполняли все поставленные задачи за день {stats_manager.getDoneThisMonth()} дней! Так Держать!";
            
            canvasView.PaintSurface += paintOnCanvas;
            contentLayout.Children.Add(canvasView);
        }

        private void paintOnCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            double donePercent = Convert.ToDouble(stats_manager.getDoneThisMonth())/Convert.ToDouble(stats_manager.allDoneTasksInfo.Count);
            double undonePercent = 1.0 - donePercent;
            float doneAngle = Convert.ToInt32(donePercent*360.0);
            float undoneAngle = Convert.ToInt32(undonePercent * 360.0);

            doneLabel.Text = $"{Convert.ToInt16(donePercent*100)}% дней вы выполнили все задания";
            undoneLabel.Text = $"{Convert.ToInt16(undonePercent * 100)}% дней вы выполнили не все задания";

            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKPaint doneElementPaint = new SKPaint { Color = Color.LightBlue.ToSKColor(), Style = SKPaintStyle.Fill };
            SKPaint notDoneElementPaint = new SKPaint { Color = Color.Blue.ToSKColor(), Style = SKPaintStyle.Fill };
            SKPaint bgPaint = new SKPaint { Color = Color.DarkBlue.ToSKColor(), Style = SKPaintStyle.Fill };


            canvas.Clear();
            
            SKPath path = new SKPath();
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2 -40);
            float radius = Math.Min(info.Width / 2, info.Height / 2 - 40);

            SKRect rect = new SKRect(center.X-radius, center.Y-radius, center.X + radius, center.Y + radius);
            SKRect bgRect = rect;
            bgRect.Offset(0, 40);
            canvas.DrawOval(bgRect, bgPaint);
            if (doneAngle == 360)
            {
                canvas.DrawOval(rect, doneElementPaint);
            }
            else if (undoneAngle == 360)
            {
                canvas.DrawOval(rect, notDoneElementPaint);
            }
            else
            {
                path.MoveTo(center);
                path.ArcTo(rect, 0, doneAngle, false);
                canvas.DrawPath(path, doneElementPaint);
                path = new SKPath();
                path.MoveTo(center);
                path.ArcTo(rect, doneAngle, undoneAngle, false);
                path.Close();
                canvas.DrawPath(path, notDoneElementPaint);
            }
        }

        private void DrawCircleDiogramm(SKCanvas canvas)
        {
            
        }

        private void returnToMainPage(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}