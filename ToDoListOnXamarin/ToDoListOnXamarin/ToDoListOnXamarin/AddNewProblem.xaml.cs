using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ToDoListOnXamarin.problemsManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoListOnXamarin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewProblem : ContentPage
    {
        problemsManager mngr = new problemsManager();
        bool isEditing;
        int global_number;

        public AddNewProblem()
        {
            InitializeComponent();
            mngr.getProblems();
            NavigationPage.SetHasNavigationBar(this, false);
            ApplyBtn.Clicked += ApplyClicked;
            CancelBtn.Clicked += CancelClicked;
        }

        public AddNewProblem(problemsManager.Problem editingProblem, int number)
        {
            InitializeComponent();
            global_number = number;
            mngr.getProblems();
            isEditing = true;
            NavigationPage.SetHasNavigationBar(this, false);
            ProblemText.Text = editingProblem.problem_text;
            if (editingProblem.repeat_type != "no")
            {
                RepeatCheckBox.IsChecked = true;
            }

            ApplyBtn.Clicked += ApplyClicked;
            CancelBtn.Clicked += CancelClicked;
        }


        private async void CancelClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Вы уверены?", "Данные которые вы ввели придётся вводить повторно, продолжить?", "Да", "Нет");
            if (answer)
            {
                Navigation.PopAsync();
            }
        }

        private void ApplyClicked(object sender, EventArgs e)
        {
            string text = ProblemText.Text;

            if (text != null)
            {
                if (RepeatCheckBox.IsChecked)
                {
                    if (isEditing)
                    {
                        mngr.EditProblem(global_number, ProblemText.Text, "everyday", mngr.allProblems[global_number].is_done);
                        Navigation.PopAsync();
                    }
                    else
                    {
                        mngr.addProblem(text, "everyday");
                        Navigation.PopAsync();
                    }
                }
                else
                {
                    if (isEditing)
                    {
                        mngr.EditProblem(global_number, ProblemText.Text, "no", mngr.allProblems[global_number].is_done);
                        Navigation.PopAsync();
                    }
                    else
                    {
                        mngr.addProblem(text, "no");
                        Navigation.PopAsync();
                    }
                }
            }
            else
            {
                DisplayAlert("Warning", "Please, enter text of your task", "Ok");
            }
        }
    }
}