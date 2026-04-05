using Ghost.Mobile.ViewModels;
using Ghost.Mobile.Models;

namespace Ghost.Mobile.Views;

[QueryProperty("DealId", "dealId")]
public partial class ChatPage : ContentPage
{
    private readonly ChatViewModel _viewModel;

    public ChatPage(ChatViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Template для сообщений
        MessagesCollection.ItemTemplate = new DataTemplate(() =>
        {
            var layout = new StackLayout
            {
                Padding = new Thickness(10, 2),
                Spacing = 2
            };

            var bubble = new Frame
            {
                Padding = new Thickness(10, 6),
                CornerRadius = 12,
                HasShadow = false,
                MaximumWidthRequest = 280
            };
            bubble.SetBinding(VisualElement.BackgroundColorProperty,
                new Binding("IsMine", converter: new BubbleColorConverter()));
            bubble.SetBinding(View.HorizontalOptionsProperty,
                new Binding("IsMine", converter: new AlignmentConverter()));

            var textLabel = new Label { FontSize = 14, LineBreakMode = LineBreakMode.WordWrap };
            textLabel.SetBinding(Label.TextProperty, nameof(ChatMessage.Text));
            textLabel.SetBinding(Label.TextColorProperty,
                new Binding("IsMine", converter: new TextColorConverter()));

            var timeLabel = new Label { FontSize = 10, HorizontalOptions = LayoutOptions.End };
            timeLabel.SetBinding(Label.TextProperty, nameof(ChatMessage.TimeDisplay));
            timeLabel.SetBinding(Label.TextColorProperty,
                new Binding("IsMine", converter: new TimeColorConverter()));

            bubble.Content = new VerticalStackLayout { Children = { textLabel, timeLabel } };
            layout.Children.Add(bubble);

            return layout;
        });

        MessagesCollection.SetBinding(CollectionView.ItemsSourceProperty, nameof(ChatViewModel.Messages));
        StatusLabel.SetBinding(Label.TextProperty, nameof(ChatViewModel.Deal.StatusText));
        AmountLabel.SetBinding(Label.TextProperty, new Binding("Deal.Amount", stringFormat: "{0} USDT"));

        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ChatViewModel.Messages) && _viewModel.Messages.Count > 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var lastItem = _viewModel.Messages.Count - 1;
                    MessagesCollection.ScrollTo(lastItem, position: ScrollToPosition.End, animate: false);
                });
            }
        };
    }

    public int DealId
    {
        set => _viewModel.DealId = value;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopPolling();
    }
}
