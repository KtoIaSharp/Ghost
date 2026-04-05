using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Models;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private List<SchoolTask> _tasks = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Все";

    [ObservableProperty]
    private bool _onlyPaid;

    public string[] Categories { get; } = new[]
    {
        "Все", "Math", "Physics", "Chemistry", "Biology",
        "History", "Literature", "Programming", "Language",
        "Art", "Music", "Sports", "Other"
    };

    public TasksViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var category = SelectedCategory == "Все" ? null : SelectedCategory;
            var tasks = await _apiService.GetTasksAsync(category, OnlyPaid);
            Tasks = tasks ?? new List<SchoolTask>();
        }
        catch (Exception)
        {
            // Тихо игнорируем ошибки
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadTasksAsync();
    }

    [RelayCommand]
    private async Task OpenTaskAsync(SchoolTask task)
    {
        if (task == null) return;

        await Shell.Current.GoToAsync($"taskDetail?id={task.Id}");
    }

    [RelayCommand]
    private async Task NavigateToCreateAsync()
    {
        await Shell.Current.GoToAsync("createTask");
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        _ = LoadTasksAsync();
    }

    partial void OnOnlyPaidChanged(bool value)
    {
        _ = LoadTasksAsync();
    }
}
