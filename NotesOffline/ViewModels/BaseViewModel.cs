using CommunityToolkit.Mvvm.ComponentModel;

namespace NotesOffline.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    public virtual void OnAppearing()
    {
    }
}
