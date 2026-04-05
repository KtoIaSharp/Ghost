using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Views;

public partial class TaskDetailPage : ContentPage
{
    private readonly TaskDetailViewModel _viewModel;

    public TaskDetailPage(TaskDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TaskDetailViewModel.Task) && viewModel.Task != null)
            {
                UpdateUI(viewModel.Task);
            }
            if (e.PropertyName == nameof(TaskDetailViewModel.StatusMessage) && !string.IsNullOrEmpty(viewModel.StatusMessage))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Информация", viewModel.StatusMessage, "OK");
                });
            }
        };
    }

    private void UpdateUI(SchoolTask task)
    {
        ContentLayout.Children.Clear();

        ContentLayout.Children.Add(new Frame
        {
            Padding = 20,
            BackgroundColor = Color.FromArgb("#16213e"),
            BorderColor = Color.FromArgb("#1a1a3e"),
            CornerRadius = 16,
            HasShadow = false,
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    new Label { Text = $"{task.CategoryEmoji} {task.Title}", TextColor = Color.FromArgb("#ffffff"), FontSize = 22, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"Создатель: {task.CreatorNick}", TextColor = Color.FromArgb("#667eea"), FontSize = 14 },
                    new Label { Text = task.Description, TextColor = Color.FromArgb("#a0a0b0"), FontSize = 14 },
                    new Label { Text = task.RewardDisplay, TextColor = Color.FromArgb("#4ade80"), FontSize = 20, FontAttributes = FontAttributes.Bold },
                    new Label { Text = $"Статус: {task.Status}", TextColor = Color.FromArgb("#fbbf24"), FontSize = 14 },
                    new Label { Text = $"Создано: {task.TimeAgo}", TextColor = Color.FromArgb("#666680"), FontSize = 12 },
                    new Label { Text = $"Срок до: {task.ExpiresAt:dd.MM HH:mm}", TextColor = Color.FromArgb("#666680"), FontSize = 12 }
                }
            }
        });

        // Кнопка для бесплатных заданий
        if (task.Status == "Open" && !_viewModel.IsCreator)
        {
            var btn = new Button
            {
                Text = task.IsPaid ? "💰 Взять задание (сделка)" : "✋ Взять задание",
                BackgroundColor = Color.FromArgb("#667eea"),
                TextColor = Color.FromArgb("#ffffff"),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 55,
                CornerRadius = 12,
                Command = _viewModel.TakeTaskCommand
            };
            ContentLayout.Children.Add(btn);
        }

        // Кнопка завершения для исполнителя
        if (task.Status == "InProgress" && task.ExecutorPhraseHash == _viewModel.Task?.CreatorPhraseHash)
        {
            var btn = new Button
            {
                Text = "✅ Завершить задание",
                BackgroundColor = Color.FromArgb("#4ade80"),
                TextColor = Color.FromArgb("#000000"),
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                HeightRequest = 55,
                CornerRadius = 12,
                Command = _viewModel.CompleteTaskCommand
            };
            ContentLayout.Children.Add(btn);
        }

        // Кнопка создания (перейти к сделке/чату)
        if (task.Status == "InProgress" && _viewModel.IsCreator)
        {
            ContentLayout.Children.Add(new Label
            {
                Text = "⏳ Задание выполняется. Перейдите в Сделки для общения.",
                TextColor = Color.FromArgb("#fbbf24"),
                FontSize = 13,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10)
            });
        }
    }
}
