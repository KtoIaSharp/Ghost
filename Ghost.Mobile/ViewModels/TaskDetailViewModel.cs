using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Models;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

[QueryProperty(nameof(TaskId), "id")]
public partial class TaskDetailViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SettingsService _settings;

    [ObservableProperty]
    private int _taskId;

    [ObservableProperty]
    private SchoolTask? _task;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isCreator;

    [ObservableProperty]
    private bool _canTake;

    [ObservableProperty]
    private bool _canComplete;

    public TaskDetailViewModel(ApiService apiService, SettingsService settings)
    {
        _apiService = apiService;
        _settings = settings;
    }

    partial void OnTaskIdChanged(int value)
    {
        _ = LoadTaskAsync();
    }

    [RelayCommand]
    private async Task LoadTaskAsync()
    {
        if (IsBusy || TaskId == 0) return;

        IsBusy = true;

        try
        {
            var task = await _apiService.GetTaskAsync(TaskId);
            Task = task;

            if (task != null)
            {
                IsCreator = task.CreatorPhraseHash == _settings.PhraseHash;
                CanTake = task.Status == "Open" && !IsCreator;
                CanComplete = task.Status == "InProgress" &&
                              task.ExecutorPhraseHash == _settings.PhraseHash;
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

    [RelayCommand]
    private async Task TakeTaskAsync()
    {
        if (Task == null || IsBusy) return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            if (Task.IsPaid)
            {
                // Для платных заданий создаём сделку
                var dealResult = await _apiService.CreateDealAsync(Task.Id);
                if (dealResult != null)
                {
                    StatusMessage = "Сделка создана! Переход к оплате...";

                    // Если есть ссылка на оплату — открываем браузер
                    if (!string.IsNullOrEmpty(dealResult.CryptoPaymentUrl))
                    {
                        await Browser.Default.OpenAsync(dealResult.CryptoPaymentUrl);
                    }

                    // Переходим к сделке
                    await Shell.Current.GoToAsync($"chat?dealId={dealResult.Id}");
                }
                else
                {
                    StatusMessage = "Не удалось создать сделку";
                }
            }
            else
            {
                // Бесплатное задание — просто берём в работу
                var result = await _apiService.TakeTaskAsync(Task.Id);
                if (result)
                {
                    StatusMessage = "Задание взято!";
                    await LoadTaskAsync();
                }
                else
                {
                    StatusMessage = "Не удалось взять задание";
                }
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

    [RelayCommand]
    private async Task CompleteTaskAsync()
    {
        if (Task == null || IsBusy) return;

        IsBusy = true;
        StatusMessage = string.Empty;

        try
        {
            var result = await _apiService.CompleteTaskAsync(Task.Id);
            if (result)
            {
                StatusMessage = "Задание выполнено!";
                await LoadTaskAsync();
            }
            else
            {
                StatusMessage = "Не удалось завершить задание";
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
