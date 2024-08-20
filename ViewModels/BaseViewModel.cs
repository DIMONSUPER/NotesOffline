using CommunityToolkit.Mvvm.ComponentModel;

namespace NotesOffline.ViewModels;

public class BaseViewModel : ObservableObject
{
    public virtual void OnAppearing()
    {
    }
}
