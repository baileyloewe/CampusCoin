using System.Collections.ObjectModel;
using CampusCoin.Models;
using CampusCoin.Services;
using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class GraphTestPageViewModel : ObservableValidator
    {
        LoginService loginService;
        EmailService emailService;
        PersistedLoginService persistedLoginService;
        IMessageOutputHandlingService messageOutputHandlingService;
        IDbContextFactory<CampusCoinContext> dbContextFactory;

        [ObservableProperty] private ObservableCollection<double> _investmentExpenses;

        public Func<double, string> XAxisLabelFormatter => value => DateTime.FromOADate(value).ToString("d");
        public GraphTestPageViewModel(LoginService loginService, EmailService emailService,
            PersistedLoginService persistedLoginService, IMessageOutputHandlingService messageOutputHandlingService,
            IDbContextFactory<CampusCoinContext> dbContextFactory)
        {
            this.loginService = loginService;
            this.emailService = emailService;
            this.persistedLoginService = persistedLoginService;
            this.messageOutputHandlingService = messageOutputHandlingService;
            this.dbContextFactory = dbContextFactory;
            OnLoaded();


        }

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "Expense Overview",
                TextSize = 25,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
        public List<ICartesianAxis> xAxes { get; set; } = new List<ICartesianAxis>
        {
            new Axis
            {
                LabelsRotation = 45,
                Labeler = value => DateTime.FromOADate(value).ToString("d"),
                Name = "Date"
            }
        };
        public IEnumerable<ISeries> Series { get; set; }

        public IEnumerable<ISeries> Series2 { get; set; } = new ISeries[]
        {
            new PieSeries<double>
            {
                Values = new ObservableCollection<double> { 3 },
                Pushout = 10,
                DataLabelsPosition = PolarLabelsPosition.ChartCenter,

            }
        };

        private void OnLoaded()
        {
            UpdateSeries();
            UpdateLineSeries();
        }


        private void UpdateSeries()
        {

            using var dbContext = dbContextFactory.CreateDbContext();
            var userData = dbContext.UserData.ToList();
            var groupedData = userData
                .GroupBy(x => x.Category)
                .Select(group => new
                {
                    Category = group.Key,
                    TotalAmount = group.Sum(x => x.Amount)
                }).ToList();

            // Assuming Series is a collection of ISeries
            var seriesCollection = new ObservableCollection<ISeries>();
            var totalExpense = userData.Sum(x => x.Amount);
            foreach (var group in groupedData)
            {
                var pieSeries = new PieSeries<double>
                {
                    Values = new double[] { group.TotalAmount },
                    Name = group.Category, // This sets the name of the series to the category name
                    DataLabelsPaint = new SolidColorPaint(SKColors.White)
                    {
                        SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                    },
                    MaxRadialColumnWidth = 50,
                    DataLabelsRotation = LiveCharts.TangentAngle, // the tangent + 30 degrees
                    ToolTipLabelFormatter = point =>
                    {
                        // This will show the category name and total amount in the tooltip
                        return $": {group.TotalAmount}/{totalExpense}";
                    },
                    DataLabelsFormatter = point =>
                    {
                        // Format for labels on the pie slices if needed
                        return $"{group.TotalAmount}/{totalExpense}";
                    }
                };

                seriesCollection.Add(pieSeries);
            }

            // Assuming you have a property for the series collection bound to the chart
            Series = seriesCollection;


        }

        //get the expenses per day over the last 30 days and return the corresponding line series
        private void UpdateLineSeries()
        {
            using var dbContext = dbContextFactory.CreateDbContext();
            var userData = dbContext.UserData.ToList();

            var startDate = DateTime.Now.AddDays(-30).Date; // Ensure start date is at the beginning of the day
            var endDate = DateTime.Now.Date; // Similarly, ensure end date is at the beginning of the day

            // Generate a list of all dates from the last 30 days
            var uniqueDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(days => startDate.AddDays(days))
                .ToList();

            // Group by Category and then by Date
            var groupedData = userData
                .Where(x => x.DateEntered.Date >= startDate && x.DateEntered.Date <= endDate)
                .GroupBy(x => x.Category)
                .ToList();

            var seriesCollection = new ObservableCollection<ISeries>();

            foreach (var categoryGroup in groupedData)
            {
                var categoryData = categoryGroup
                    .GroupBy(x => x.DateEntered.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

                var points = uniqueDates.Select(date =>
                {
                    categoryData.TryGetValue(date, out var totalAmount);
                    return new ObservablePoint(date.ToOADate(), totalAmount);
                }).ToList();

                var lineSeries = new LineSeries<ObservablePoint>
                {
                    Values = points,
                    Name = categoryGroup.Key, // Set the name to the category
                    LineSmoothness = 2,
                    YToolTipLabelFormatter = point =>
                    {
                        return $"{point.PrimaryValue}";
                    },
                    DataLabelsFormatter = point =>
                    {
                        return $"{point.PrimaryValue}";
                    }
                };

                seriesCollection.Add(lineSeries);
            }

            Series2 = seriesCollection;
        }

        

        


    }
}
