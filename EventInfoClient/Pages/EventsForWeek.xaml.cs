using System;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace EventInfoClient
{
    /// <summary>
    /// Interaction logic for EventsForWeek.xaml
    /// </summary>
    public partial class EventsForWeek : Page, INotifyPropertyChanged
    {
        public EventsForWeek(DateTime date)
        {
            WeekNumber = GetWeekNumberFromDate(date);
            Year = date.Year;
            InitializeComponent();
            EventListView.ItemsSource = Events;
            GetEvents();
        }

        public EventsForWeek() : this(DateTime.Now) { }

        private int weekNumber;
        public int WeekNumber {
            get => weekNumber;
            set
            {
                if (value < 1)
                {
                    value = 52;
                    year--;
                    NotifyPropertyChanged("Year");
                }
                else if (value > 52)
                {
                    value = 1;
                    year++;
                    NotifyPropertyChanged("Year");
                }
                weekNumber = value;
                NotifyPropertyChanged("WeekNumber");
                NotifyPropertyChanged("FirstDateOfWeek");
                NotifyPropertyChanged("LastDateOfWeek");
                GetEvents();
            }
        }

        private int year;
        public int Year {
            get => year;
            set
            {
                year = value;
                NotifyPropertyChanged("Year");
                NotifyPropertyChanged("FirstDateOfWeek");
                NotifyPropertyChanged("LastDateOfWeek");
                GetEvents();
            }
        }

        public string FirstDateOfWeek => GetFirstDateOfWeek(Year, WeekNumber, CultureInfo.CurrentCulture).ToShortDateString();

        public string LastDateOfWeek
        {
            get
            {
                DateTime date = GetFirstDateOfWeek(Year, WeekNumber, CultureInfo.CurrentCulture);
                date = date + new TimeSpan(6, 0, 0, 0);
                return date.ToShortDateString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<EventInfo> Events { get; set; } = new ObservableCollection<EventInfo>();

        private static int GetWeekNumberFromDate(DateTime dateTime)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            int weekNumber = calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekNumber;
        }

        public static DateTime GetFirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        public async void GetEvents()
        {
            Events.Clear();
            var response = await EventInfoAPI.GetEventsForWeek(Year, WeekNumber);
            Events.Clear();
            if (response == null) return;
            foreach (var r in response) Events.Add(r);
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            WeekNumber--;
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            WeekNumber++;
        }

        private void PreviousYearButton_Click(object sender, RoutedEventArgs e)
        {
            Year--;
        }

        private void NextYearButton_Click(object sender, RoutedEventArgs e)
        {
            Year++;
        }

        private void EventListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow wnd = Window.GetWindow(this) as MainWindow;
            long eventId = (e.AddedItems.Count != 0) ? (e.AddedItems[0] as EventInfo).id : -1;
            wnd.ShowEventDetails(eventId);
        }
    }
}
