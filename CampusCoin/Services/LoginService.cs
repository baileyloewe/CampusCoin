//using CoreML;
using Microsoft.EntityFrameworkCore;
using ShellMixedSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ShellMixedSample.Services
{
    public class LoginService
    {
        HttpClient httpClient;
        public LoginService()
        {
            httpClient = new HttpClient();
        }

        List<Users> usersList = new();
        public async Task<List<Users>> GetUsers()
        {
            if (usersList?.Count > 0)
                return usersList;

            // TODO: update url for users data
            var url = "https://tobedetermined.com";

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                usersList = await response.Content.ReadFromJsonAsync<List<Users>>();
            }

            return usersList;
        }
    }
}
