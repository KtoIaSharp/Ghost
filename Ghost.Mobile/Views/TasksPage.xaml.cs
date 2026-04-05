using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Views;

public partial class TasksPage : ContentPage
{
    private readonly TasksViewModel _viewModel;

    public TasksPage(TasksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        // Заполняем категории
        CategoryPicker.Items.Add("Все");
        foreach (var cat in viewModel.Categories)
        {
            if (cat != "Все")
                CategoryPicker.Items.Add(cat);
        }
        CategoryPicker.SelectedIndex = 0;

        CategoryPicker.SelectedIndexChanged += (s, e) =>
        {
            viewModel.SelectedCategory = CategoryPicker.Items[CategoryPicker.SelectedIndex];
        };

        OnlyPaidCheckBox.CheckedChanged += (s, e) =>
        {
            viewModel.OnlyPaid = e.Value;
        };

        RefreshView.Command = viewModel.RefreshCommand;

        // Template для заданий
        TasksCollection.ItemTemplate = new DataTemplate(() =>
        {
            var frame = new Frame
            {
                Margin = new Thickness(10, 5),
                Padding = new Thickness(15),
                BackgroundColor = Color.FromArgb("#16213e"),
                BorderColor = Color.FromArgb("#1a1a3e"),
                CornerRadius = 12,
                HasShadow = false
            };

            var emojiLabel = new Label { FontSize = 24 };
            emojiLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.CategoryEmoji));

            var titleLabel = new Label
            {
                TextColor = Color.FromArgb("#ffffff"),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                LineBreakMode = LineBreakMode.TailTruncation,
                MaxLines = 1
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.Title));

            var nickLabel = new Label
            {
                TextColor = Color.FromArgb("#667eea"),
                FontSize = 12
            };
            nickLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.CreatorNick));

            var rewardLabel = new Label
            {
                TextColor = Color.FromArgb("#4ade80"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.End
            };
            rewardLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.RewardDisplay));

            var timeLabel = new Label
            {
                TextColor = Color.FromArgb("#666680"),
                FontSize = 11,
                HorizontalOptions = LayoutOptions.End
            };
            timeLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.TimeAgo));

            var descLabel = new Label
            {
                TextColor = Color.FromArgb("#a0a0b0"),
                FontSize = 13,
                LineBreakMode = LineBreakMode.TailTruncation,
                MaxLines = 2
            };
            descLabel.SetBinding(Label.TextProperty, nameof(SchoolTask.Description));

            var titleStack = new VerticalStackLayout { Children = { titleLabel, nickLabel } };
            var headerGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Children = { emojiLabel, titleStack, new VerticalStackLayout { Children = { rewardLabel, timeLabel } } }
            };
            Grid.SetColumn(titleStack, 1);
            Grid.SetColumn(new VerticalStackLayout { Children = { rewardLabel, timeLabel } }, 2);

            frame.Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children = { headerGrid, descLabel }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                if (frame.BindingContext is SchoolTask task)
                {
                    await Shell.Current.GoToAsync($"taskDetail?id={task.Id}");
                }
            };
            frame.GestureRecognizers.Add(tap);

            return frame;
        });

        TasksCollection.SetBinding(CollectionView.ItemsSourceProperty, nameof(TasksViewModel.Tasks));
        LoadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(TasksViewModel.IsBusy));
        LoadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(TasksViewModel.IsBusy));

        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadTasksCommand.Execute(null);
    }
}
