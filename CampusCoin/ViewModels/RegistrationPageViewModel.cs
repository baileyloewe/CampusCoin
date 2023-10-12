using System.ComponentModel;

namespace CampusCoin.ViewModels;
    
public class RegistrationPageViewModel 
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Title => "Registration Page";
    public string Description => "Create a CampusCoin Account";
    public bool isEditing;
}

