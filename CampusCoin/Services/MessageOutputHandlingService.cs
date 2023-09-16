using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusCoin.Services
{
    public interface IMessageOutputHandlingService
    {
        public Task OutputValidationErrorsToUser(IEnumerable<ValidationResult> errors);

        public Task OutputSuccessToUser(string message);
    }
    internal class MessageOutputHandlingService: IMessageOutputHandlingService
    {
        /// <summary>
        /// Outputs validation errors to the user
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public async Task OutputValidationErrorsToUser(IEnumerable<ValidationResult> errors)
        {
            // convert the errors to a list and then for each error, display an alert to the user
            errors.ToList().ForEach(async error => await App.Current.MainPage.DisplayAlert("Error", error.ErrorMessage, "OK")); 
        }

        public async Task OutputSuccessToUser(string message)
        {
            await App.Current.MainPage.DisplayAlert("Success", message, "OK");
        }
    }
}
