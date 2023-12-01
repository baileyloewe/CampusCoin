using System.ComponentModel.DataAnnotations;

namespace CampusCoin.Services
{
    public interface IMessageOutputHandlingService
    {
        public Task OutputValidationErrorsToUser(IEnumerable<ValidationResult> errors);

        public Task OutputSuccessToUser(string message);
    }
    internal class MessageOutputHandlingService: IMessageOutputHandlingService
    {
        /// <summary> Outputs validation errors to the user </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task OutputValidationErrorsToUser(IEnumerable<ValidationResult> errors)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // convert the errors to a list and then  display an alert with all errors to the user
            string errorMessage = "";
            errors.ToList().ForEach(error => errorMessage += (error.ErrorMessage + "\n\n"));
            errorMessage = errorMessage.Trim(); // Remove the leading & trailing newline characters
            await App.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
        }
        /// <summary> Outputs success message to user </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task OutputSuccessToUser(string message)
        {
            await App.Current.MainPage.DisplayAlert("Success", message, "OK");
        }
    }
}
