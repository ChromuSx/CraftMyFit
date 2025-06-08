using CraftMyFit.Models.Workout;

namespace CraftMyFit.Views
{
    public partial class WorkoutDetailPage : ContentPage
    {
        public WorkoutDetailPage()
        {
            InitializeComponent();
        }

        public WorkoutDetailPage(WorkoutPlan workout) : this()
        {
            BindingContext = workout;
        }
    }
}