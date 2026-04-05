using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Views;

public partial class DealsPage : ContentPage
{
    private readonly DealsViewModel _viewModel;

    public DealsPage(DealsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Заполняем фильтры
        foreach (var f in viewModel.Filters)
            FilterPicker.Items.Add(f);
        FilterPicker.SelectedIndex = 0;

        FilterPicker.SelectedIndexChanged += (s, e) =>
        {
            viewModel.SelectedFilter = FilterPicker.Items[FilterPicker.SelectedIndex];
        };

        RefreshView.Command = viewModel.RefreshCommand;

        // Template для сделок
        DealsCollection.ItemTemplate = new DataTemplate(() =>
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
            emojiLabel.SetBinding(Label.TextProperty, nameof(Deal.StatusEmoji));

            var titleLabel = new Label
            {
                TextColor = Color.FromArgb("#ffffff"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(Deal.StatusText));

            var amountLabel = new Label
            {
                TextColor = Color.FromArgb("#4ade80"),
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.End
            };
            amountLabel.SetBinding(Label.TextProperty, nameof(Deal.Amount));

            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };
            grid.Children.Add(emojiLabel);
            grid.Children.Add(titleLabel);
            Grid.SetColumn(titleLabel, 1);
            grid.Children.Add(amountLabel);
            Grid.SetColumn(amountLabel, 2);

            frame.Content = grid;

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                if (frame.BindingContext is Deal deal)
                {
                    await Shell.Current.GoToAsync($"chat?dealId={deal.Id}");
                }
            };
            frame.GestureRecognizers.Add(tap);

            return frame;
        });

        DealsCollection.SetBinding(CollectionView.ItemsSourceProperty, nameof(DealsViewModel.Deals));
        LoadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(DealsViewModel.IsBusy));
        LoadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(DealsViewModel.IsBusy));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDealsCommand.Execute(null);
    }
}
