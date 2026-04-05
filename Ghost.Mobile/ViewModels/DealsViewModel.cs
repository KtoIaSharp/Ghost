using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Models;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

public partial class DealsViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SettingsService _settings;

    [ObservableProperty]
    private List<Deal> _deals = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _selectedFilter = "Все";

    public string[] Filters { get; } = { "Все", "pending", "paid", "completed", "disputed" };

    public DealsViewModel(ApiService apiService, SettingsService settings)
    {
        _apiService = apiService;
        _settings = settings;
    }

    [RelayCommand]
    private async Task LoadDealsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            var deals = await _apiService.GetMyDealsAsync();
            var allDeals = deals ?? new List<Deal>();

            // Фильтрация
            Deals = SelectedFilter == "Все"
                ? allDeals
                : allDeals.Where(d => d.Status == SelectedFilter).ToList();
        }
        catch (Exception)
        {
            // Тихо игнорируем
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
        await LoadDealsAsync();
    }

    [RelayCommand]
    private async Task OpenDealAsync(Deal deal)
    {
        if (deal == null) return;

        await Shell.Current.GoToAsync($"chat?dealId={deal.Id}");
    }

    partial void OnSelectedFilterChanged(string value)
    {
        _ = LoadDealsAsync();
    }
}
