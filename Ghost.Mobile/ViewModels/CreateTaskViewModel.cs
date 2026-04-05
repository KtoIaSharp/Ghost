using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Models;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

public partial class CreateTaskViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _rewardAmount;

    [ObservableProperty]
    private string _selectedCategory = "Other";

    [ObservableProperty]
    private bool _isPaid;

    [ObservableProperty]
    private bool _isUrgent;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public string[] Categories { get; } = new[]
    {
        "Math", "Physics", "Chemistry", "Biology",
        "History", "Literature", "Programming", "Language",
        "Art", "Music", "Sports", "Other"
    };

    public CreateTaskViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task CreateTaskAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Title))
        {
            StatusMessage = "Введите название задания";
            return;
        }

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var request = new CreateTaskRequest
            {
                Title = Title.Trim(),
                Description = Description.Trim(),
                RewardAmount = IsPaid ? RewardAmount : 0,
                Category = SelectedCategory,
                IsPaid = IsPaid,
                IsUrgent = IsUrgent
            };

            var result = await _apiService.CreateTaskAsync(request);

            if (result)
            {
                StatusMessage = "Задание создано!";
                await Task.Delay(500);
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Не удалось создать задание";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
