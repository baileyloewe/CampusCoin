using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Models;
using CampusCoin.Services;
using CampusCoin.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;

namespace CampusCoin.ViewModels
{
    public partial class DashboardPageViewModel : ObservableValidator
    {
        private readonly LoginService loginService;
        private readonly EmailService emailService;
        private readonly IncomeService incomeService;
        private readonly PersistedLoginService persistedLoginService;
        private readonly IMessageOutputHandlingService messageOutputHandlingService;
        private readonly IDbContextFactory<CampusCoinContext> dbContextFactory;

        [ObservableProperty]
        private ObservableCollection<double> _investmentExpenses;

        [ObservableProperty]
        int height;

        [ObservableProperty]
        string incomeAmount;

        [ObservableProperty]
        private bool isIncomeVisible;

        [ObservableProperty]
        private bool isAddIncomeBtnVisible;

        public Func<double, string> XAxisLabelFormatter => value => DateTime.FromOADate(value).ToString("d");

        public DashboardPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, IMessageOutputHandlingService messageOutputHandlingService, IDbContextFactory<CampusCoinContext> dbContextFactory, IncomeService incomeService)
        {
            this.loginService = loginService;
            this.emailService = emailService;
            this.incomeService = incomeService;
            this.persistedLoginService = persistedLoginService;
            this.messageOutputHandlingService = messageOutputHandlingService;
            this.dbContextFactory = dbContextFactory;
            Initialize();
            IsAddIncomeBtnVisible = true;
            IsIncomeVisible = false;
            Height = 200;
        }

        public LabelVisual Title { get; set; } = CreateTitle();

        public List<ICartesianAxis> xAxes { get; set; } = CreateAxes();

        public IEnumerable<ISeries> Series { get; set; }

        public IEnumerable<ISeries> Series2 { get; set; }

        private void Initialize()
        {
            UpdateSeries();
            UpdateLineSeries();
        }

        private static LabelVisual CreateTitle()
        {
            return new LabelVisual
            {
                Text = "Expense Overview",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
        }
        [RelayCommand]
        public async Task RouteToExpensePage()
        {
            await Shell.Current.GoToAsync(nameof(ExpensesPage));
        }

        private static List<ICartesianAxis> CreateAxes()
        {
            return new List<ICartesianAxis>
            {
                new Axis
                {
                    LabelsRotation = 45,
                    Labeler = value => DateTime.FromOADate(value).ToString("d"),
                    Name = "Date",
                    MinLimit = DateTime.Now.AddDays(-14).Date.ToOADate(),
                    MaxLimit = DateTime.Now.Date.ToOADate(),
                }
            };
        }

        [RelayCommand]
        private void AddIncome()
        {
            IsIncomeVisible = true;
            Height = 400;
        }

        private UserIncomeData setUserIncomeValues(UserIncomeData userData)
        {
            User user = persistedLoginService.getLoggedInUser();
            DateTime date = DateTime.Now;

            userData.Category = "Income";
            userData.Amount = int.Parse(IncomeAmount);
            userData.DateEntered = date;
            userData.UserId = user.UserId;
            return userData;
        }

        [RelayCommand]
        async Task SubmitIncome()
        {
            try
            {
                Height = 200;
                var userData = new UserIncomeData();
                userData = setUserIncomeValues(userData);
                await incomeService.SubmitIncome(userData);
                await Shell.Current.DisplayAlert("Success",
                    $"Income Submitted Successfully", "OK");
                IsIncomeVisible = false;
                IncomeAmount = null;
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error!",
                    $"Your income must be an integer amount", "OK");
                IncomeAmount = null;
            }

        }

        [RelayCommand]
        async Task Back()
        {
            if (await persistedLoginService.logoutPrompt())
            {
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
        }

        private void UpdateSeries()
        {
            using var dbContext = dbContextFactory.CreateDbContext();
            var userData = dbContext.UserExpenseData.ToList();
            var groupedData = userData
                .GroupBy(x => x.Category)
                .Select(group => new
                {
                    Category = group.Key,
                    TotalAmount = group.Sum(x => x.Amount)
                }).ToList();

            var seriesCollection = new ObservableCollection<ISeries>();
            var totalExpense = userData.Sum(x => x.Amount);

            foreach (var group in groupedData)
            {
                seriesCollection.Add(CreatePieSeries(group, totalExpense));
            }

            Series = seriesCollection;
        }

        private static PieSeries<double> CreatePieSeries(dynamic group, double totalExpense)
        {
            return new PieSeries<double>
            {
                Values = new double[] { group.TotalAmount },
                Name = group.Category,
                DataLabelsPaint = new SolidColorPaint(SKColors.White)
                {
                    SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                },
                DataLabelsRotation = LiveCharts.TangentAngle,
                ToolTipLabelFormatter = point => $": {group.TotalAmount}/{totalExpense}",
                DataLabelsFormatter = point => $"${group.TotalAmount}"
            };
        }

        private void UpdateLineSeries()
        {
            using var dbContext = dbContextFactory.CreateDbContext();
            var userData = dbContext.UserExpenseData.ToList();

            var startDate = DateTime.Now.AddDays(-14).Date;
            var endDate = DateTime.Now.Date;

            var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(days => startDate.AddDays(days))
                .ToList();

            var groupedData = userData
                .Where(x => x.DateEntered.Date >= startDate && x.DateEntered.Date <= endDate)
                .GroupBy(x => x.Category)
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    DailyAmounts = catGroup
                        .GroupBy(x => x.DateEntered.Date)
                        .ToDictionary(dateGroup => dateGroup.Key, dateGroup => dateGroup.Sum(x => x.Amount))
                })
                .ToList();

            var seriesCollection = new ObservableCollection<ISeries>();

            foreach (var category in groupedData)
            {
                seriesCollection.Add(CreateLineSeries(category, dateRange));
            }

            Series2 = seriesCollection;
        }

        private static LineSeries<ObservablePoint> CreateLineSeries(dynamic category, List<DateTime> dateRange)
        {
            var points = dateRange.Select(date =>
            {
                category.DailyAmounts.TryGetValue(date, out int totalAmount);
                return new ObservablePoint(date.ToOADate(), totalAmount);
            }).ToList();

            return new LineSeries<ObservablePoint>
            {
                Values = points,
                Name = category.Category,
                LineSmoothness = 2,
                YToolTipLabelFormatter = point => $"{point.PrimaryValue}",
                DataLabelsFormatter = point => $"{point.PrimaryValue}"
            };
        }
    }
}
