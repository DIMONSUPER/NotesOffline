using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NotesOffline.Models.Entities;
using NotesOffline.Models.Messages;
using NotesOffline.Services;
using System.Collections.ObjectModel;

namespace NotesOffline.ViewModels;

public partial class MessagesViewModel : BaseViewModel, IRecipient<PendingActionMessageChanged>
{
    private readonly IActionService _actionService;

    public MessagesViewModel(IActionService actionService)
    {
        _actionService = actionService;

        WeakReferenceMessenger.Default.Register(this);

        Initialize();
    }

    [ObservableProperty]
    public string title = "Messages";

    [ObservableProperty]
    public ObservableCollection<PendingAction> messages;

    [RelayCommand]
    public Task BackButtonTapped()
    {
        return Shell.Current.GoToAsync("..");
    }

    public void Receive(PendingActionMessageChanged message)
    {
        switch (message.Value.Type)
        {
            case ActionChangedType.Added:
                {
                    Messages.Insert(0, message.Value.Action);
                    break;
                }
            case ActionChangedType.Deleted:
                {
                    var actionWithIndex = Messages
                        .Select((action, index) => new { Action = action, Index = index })
                        .Single(x => x.Action.Id == message.Value.Action.Id);

                    Messages.RemoveAt(actionWithIndex.Index);
                    break;
                }
        }
    }

    private async void Initialize()
    {
        var dbActions = await _actionService.GetAllActionsAsync();

        Messages = new(dbActions);
    }
}