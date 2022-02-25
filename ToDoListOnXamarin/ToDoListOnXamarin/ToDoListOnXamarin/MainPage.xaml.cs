using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using Xamarin.Forms;

namespace ToDoListOnXamarin
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        bool editButtonAreVisible = false;
        problemsManager mngr;
        ImageSource defaultSrc;
        ImageSource doneSrc;
        int allTasksToday;
        int doneTasksToday;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.Appearing += MainPage_Appearing;
            mngr = new problemsManager();
            addNewTaskBtn.Clicked += addNote;
            showEditTaskBtn.Clicked += showEditBtn;
            showStatisticsBtn.Clicked += showStatistics;

            addNewTaskBtn.ImageSource = ImageSource.FromResource("ToDoListOnXamarin.Images.addIcon.png");
            showEditTaskBtn.ImageSource = ImageSource.FromResource("ToDoListOnXamarin.Images.edit.png");
            showStatisticsBtn.ImageSource = ImageSource.FromResource("ToDoListOnXamarin.Images.statsIcon.png");

            defaultSrc = ImageSource.FromResource("ToDoListOnXamarin.Images.no.png");
            doneSrc = ImageSource.FromResource("ToDoListOnXamarin.Images.checked.png");

            UpdatePage();
        }

        private void showStatistics(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Stats(allTasksToday, doneTasksToday));
        }

        private void showEditBtn(object sender, EventArgs e)
        {
            if (!editButtonAreVisible)
            {
                for (int i = 0; i < mngr.allProblems.Count; i++)
                {
                    StackLayout lout = (StackLayout)VerticalLayout.Children[i];
                    Button temp = (Button)lout.Children[2];
                    temp.FadeTo(1, 250, Easing.Linear);
                }
                editButtonAreVisible = true;
            }
            else
            {
                for (int i = 0; i < mngr.allProblems.Count; i++)
                {
                    StackLayout lout = (StackLayout)VerticalLayout.Children[i];
                    Button temp = (Button)lout.Children[2];
                    temp.FadeTo(0, 250, Easing.Linear);
                }
                editButtonAreVisible = false;
            }
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            UpdatePage();
        }

        private void checkIfAllTasksDone()
        {
            allTasksToday = mngr.allProblems.Count;
            int done_number = 0;
            foreach (problemsManager.Problem pr in mngr.allProblems)
            {
                if (pr.is_done)
                {
                    done_number++;
                }
            }
            doneTasksToday = done_number;
        }

        private void UpdatePage()
        {
            VerticalLayout.Children.Clear();
            mngr.getProblems();
            mngr.checkProblemsOutOfDate();

            checkIfAllTasksDone();
            for (int i = 0; i < mngr.allProblems.Count; i++)
            {
                StackLayout horizontalLine = new StackLayout();
                horizontalLine.Orientation = StackOrientation.Horizontal;
                horizontalLine.HorizontalOptions = LayoutOptions.Center;
                horizontalLine.AutomationId = i.ToString();

                Label temp_label = new Label();
                temp_label.Text = mngr.allProblems[i].problem_text;
                temp_label.HorizontalOptions = LayoutOptions.Start;
                temp_label.VerticalOptions = LayoutOptions.Center;
                temp_label.TextColor = Color.Black;
                temp_label.FontSize = 20;
                temp_label.FontFamily = "segoe ui";

                Button editTaskBtn = new Button();
                editTaskBtn.WidthRequest = 50;
                editTaskBtn.HeightRequest = 50;

                editTaskBtn.Clicked += editTaskBtnClicked;
                editTaskBtn.BackgroundColor = Color.Blue;
                editTaskBtn.ImageSource = ImageSource.FromResource("FirstXamarinApp.Images.ButtonAnimation.edit.png");
                editTaskBtn.CommandParameter = i;
                editTaskBtn.HorizontalOptions = LayoutOptions.Start;
                editTaskBtn.VerticalOptions = LayoutOptions.Center;
                if (!editButtonAreVisible)
                {
                    editTaskBtn.Opacity = 0;
                }
                ImageButton temp_click_btn = new ImageButton();
                temp_click_btn.WidthRequest = 50;
                temp_click_btn.HeightRequest = 50;
                temp_click_btn.Source = defaultSrc;
                temp_click_btn.BackgroundColor = Color.Transparent;

                temp_click_btn.Clicked += taskIsDone;
                temp_click_btn.CommandParameter = i;
                temp_click_btn.HorizontalOptions = LayoutOptions.End;
                temp_click_btn.VerticalOptions = LayoutOptions.Center;


                horizontalLine.Children.Add(temp_label);
                horizontalLine.Children.Add(temp_click_btn);
                horizontalLine.Children.Add(editTaskBtn);

                if (mngr.allProblems[i].is_done)
                {
                    temp_label.FontAttributes = FontAttributes.Bold;
                    temp_label.TextColor = Color.Green;
                    temp_click_btn.Source = doneSrc;
                }

                horizontalLine.Children.Add(temp_label);
                horizontalLine.Children.Add(temp_click_btn);
                horizontalLine.Children.Add(editTaskBtn);

                VerticalLayout.Children.Add(horizontalLine);
            }
        }

        private async void editTaskBtnClicked(object sender, EventArgs e)
        {
            if (((Button)sender).Opacity == 1)
            {
                int number = Convert.ToInt32(((Button)sender).CommandParameter);
                await Navigation.PushAsync(new AddNewProblem(mngr.allProblems[number], number));
            }
        }

        private async void taskIsDone(object sender, EventArgs e)
        {
            ImageButton senderBtn = (ImageButton)sender;
            int number = Convert.ToInt32((senderBtn).CommandParameter);
            StackLayout lout = (StackLayout)VerticalLayout.Children[number];
            Label temp = (Label)lout.Children[0];
            if (!mngr.allProblems[number].is_done)
            {
                temp.TextColor = Color.Green;
                temp.FontAttributes = FontAttributes.Bold;

                senderBtn.Source = doneSrc;
                mngr.EditProblem(number, mngr.allProblems[number].problem_text, mngr.allProblems[number].repeat_type, true);

            }
            else
            {
                bool answer = await DisplayAlert("Внимание", "Вы хотите отменить выполнение данного задания?", "Да", "Нет");
                if (answer)
                {
                    temp.TextColor = Color.Black;
                    temp.FontAttributes = FontAttributes.None;

                    senderBtn.Source = defaultSrc;
                    mngr.EditProblem(number, mngr.allProblems[number].problem_text, mngr.allProblems[number].repeat_type, false);
                }
            }

            checkIfAllTasksDone();
            UpdatePage();
        }


        private async void addNote(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddNewProblem());
        }
    }
}