using CommunityToolkit.Mvvm.Messaging.Messages;
using NotesOffline.Models.Entities;

namespace NotesOffline.Models.Messages;

public class PendingActionMessageChanged(PendingAction Action, ActionChangedType Type)
    : ValueChangedMessage<ActionChangedModel>(new(Action, Type))
{
}

public record ActionChangedModel(PendingAction Action, ActionChangedType Type);

public enum ActionChangedType
{
    Added,
    Deleted
}