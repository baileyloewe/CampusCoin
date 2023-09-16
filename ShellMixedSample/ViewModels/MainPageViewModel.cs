using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellMixedSample.ViewModels
{
    public partial class MainPageViewModel : ObservableValidator
    {
        public MainPageViewModel()
        {
            Count = 0;
        }

        // the observable property is used to automatically notify the UI when the value changes
        [ObservableProperty]
        // the required attribute is used to validate the value is not null, empty or whitespace and the range attribute is used to validate the value is between 0 and int.MaxValue
        [Required (ErrorMessage = "Count is required"), Range(0,int.MaxValue, ErrorMessage = "Integer must be a positive number and less than max integer value.")]
        private int _count;

        public string CounterText
        {
            get
            {
                if (Count == 1)
                    return $"Clicked {Count} time";
                else
                    return $"Clicked {Count} times";
            }
        }

        [RelayCommand]
        public async Task IncrementCount()
        {
            Count++;
            ValidateAllProperties(); // validating all properties
            if(HasErrors) // if there are validation errors, explain to the user what they are- for example if they entered a negative number
            {
                // the first error is the one that will be displayed to the user
                var firstErrorString = GetErrors().First().ErrorMessage;
                await App.Current.MainPage.DisplayAlert("Error", firstErrorString , "OK"); // display the first error
            }
            OnPropertyChanged(nameof(CounterText)); // signaling to the UI that the CounterText property has changed
        }
    }
}
