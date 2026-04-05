using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ghost.Mobile.Models;
using Ghost.Mobile.Services;

namespace Ghost.Mobile.ViewModels;

[QueryProperty(nameof(DealId), "dealId")]
public partial class ChatViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly SettingsService _settings;
    private Timer? _pollingTimer;

    [ObservableProperty]
    private int _dealId;

    [ObservableProperty]
    private Deal? _deal;

    [ObservableProperty]
    private List<ChatMessage> _messages = new();

    [ObservableProperty]
    private string _messageText = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _chatPartnerNick = "Собеседник";

    public ChatViewModel(ApiService apiService, SettingsService settings)
    {
        _apiService = apiService;
        _settings = settings;
    }

    partial void OnDealIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadDealAsync();
            StartPolling();
        }
    }

    [RelayCommand]
    private async Task LoadDealAsync()
    {
        if (IsBusy || DealId == 0) return;

        IsBusy = true;

        try
        {
            var deals = await _apiService.GetMyDealsAsync();
            Deal = deals?.FirstOrDefault(d => d.Id == DealId);

            if (Deal != null)
            {
                // Определяем ник собеседника
                var isCustomer = Deal.CustomerPhraseHash == _settings.PhraseHash;
                ChatPartnerNick = isCustomer
                    ? (Deal.SchoolTask?.ExecutorNick ?? "Исполнитель")
                    : (Deal.SchoolTask?.CreatorNick ?? "Заказчик");
            }

            await LoadMessagesAsync();
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

    private async Task LoadMessagesAsync()
    {
        try
        {
            var messages = await _apiService.GetChatMessagesAsync(DealId);
            if (messages != null)
            {
                // Помечаем свои сообщения
                foreach (var msg in messages)
                {
                    msg.IsMine = msg.SenderPhraseHash == _settings.PhraseHash;
                }
                Messages = messages;
            }
        }
        catch
        {
            // Тихо игнорируем
        }
    }

    private void StartPolling()
    {
        _pollingTimer?.Dispose();
        _pollingTimer = new Timer(async _ =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadMessagesAsync();
            });
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageText) || IsBusy) return;

        var text = MessageText.Trim();
        MessageText = string.Empty;

        try
        {
            var result = await _apiService.SendChatMessageAsync(DealId, text);
            if (!result)
            {
                // Добавляем локально для UX
                Messages.Add(new ChatMessage
                {
                    DealId = DealId,
                    SenderPhraseHash = _settings.PhraseHash,
                    SenderNick = _settings.Nickname,
                    Text = text,
                    SentAt = DateTime.UtcNow,
                    IsMine = true
                });
            }
        }
        catch
        {
            // Добавляем локально
            Messages.Add(new ChatMessage
            {
                DealId = DealId,
                SenderPhraseHash = _settings.PhraseHash,
                SenderNick = _settings.Nickname,
                Text = text,
                SentAt = DateTime.UtcNow,
                IsMine = true
            });
        }
    }

    [RelayCommand]
    private async Task ConfirmDealAsync()
    {
        if (Deal == null || IsBusy) return;

        IsBusy = true;

        try
        {
            var result = await _apiService.ConfirmDealAsync(Deal.Id);
            if (result)
            {
                StatusMessage = "Сделка подтверждена!";
                Deal.Status = "completed";
            }
            else
            {
                StatusMessage = "Не удалось подтвердить сделку";
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
    private async Task DisputeDealAsync()
    {
        if (Deal == null || IsBusy) return;

        var reason = await App.Current.MainPage.DisplayPromptAsync(
            "Открыть спор", "Опишите причину спора:");

        if (string.IsNullOrWhiteSpace(reason)) return;

        IsBusy = true;

        try
        {
            var result = await _apiService.DisputeDealAsync(Deal.Id, reason);
            if (result)
            {
                StatusMessage = "Спор открыт, администратор разберётся";
                Deal.Status = "disputed";
            }
            else
            {
                StatusMessage = "Не удалось открыть спор";
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
    private async Task OpenPaymentUrlAsync()
    {
        if (Deal?.CryptoPaymentUrl != null)
        {
            await Browser.Default.OpenAsync(Deal.CryptoPaymentUrl);
        }
    }

    public void StopPolling()
    {
        _pollingTimer?.Dispose();
        _pollingTimer = null;
    }
}
