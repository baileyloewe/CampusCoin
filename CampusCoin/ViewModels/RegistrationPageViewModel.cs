using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static ABI.System.Windows.Input.ICommand_Delegates;

namespace CampusCoin.ViewModels;
    
public class RegistrationPageViewModel 
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Title => "Registration Page";
    public string Description => "Create a CampusCoin Account";
    public bool isEditing;
}

