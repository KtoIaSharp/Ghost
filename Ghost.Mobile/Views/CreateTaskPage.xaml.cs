using Ghost.Mobile.ViewModels;

namespace Ghost.Mobile.Views;

public partial class CreateTaskPage : ContentPage
{
    private readonly CreateTaskViewModel _viewModel;

    public CreateTaskPage(CreateTaskViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Заполняем категории
        foreach (var c in _viewModel.Categories)
            CategoryPicker.Items.Add(c);
        CategoryPicker.SelectedIndex = _viewModel.Categories.Length - 1;

        CategoryPicker.SelectedIndexChanged += (s, e) =>
        {
            _viewModel.SelectedCategory = CategoryPicker.Items[CategoryPicker.SelectedIndex];
        };

        IsPaidCheck.CheckedChanged += (s, e) =>
        {
            _viewModel.IsPaid = e.Value;
        };

        // Привязки
        TitleEntry.SetBinding(Entry.TextProperty, nameof(CreateTaskViewModel.Title));
        DescEditor.SetBinding(Editor.TextProperty, nameof(CreateTaskViewModel.Description));
        RewardEntry.SetBinding(Entry.TextProperty, nameof(CreateTaskViewModel.RewardAmount));
        StatusLabel.SetBinding(Label.TextProperty, nameof(CreateTaskViewModel.StatusMessage));
        StatusLabel.SetBinding(Label.IsVisibleProperty, nameof(CreateTaskViewModel.StatusMessage),
            converter: new StringNotEmptyConverter());
        CreateButton.SetBinding(Button.CommandProperty, nameof(CreateTaskViewModel.CreateTaskCommand));
        CreateButton.SetBinding(Button.IsEnabledProperty, nameof(CreateTaskViewModel.IsBusy));
        LoadingIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(CreateTaskViewModel.IsBusy));
        LoadingIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(CreateTaskViewModel.IsBusy));
    }
}
