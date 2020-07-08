using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace EventInfoClient
{
    /// <summary>
    /// Interaction logic for EventsForDay.xaml
    /// </summary>
    public partial class EventsForDay : Page, INotifyPropertyChanged
    {
        public EventsForDay(DateTime date)
        {
            Date = date;
            InitializeComponent();
            EventListView.ItemsSource = Events;
            GetEvents();
        }

        public EventsForDay() : this(DateTime.Now) { }

        private DateTime date;
        public DateTime Date {
            get => date;
            set
            {
                date = value;
                NotifyPropertyChanged("Date");
                GetEvents();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<EventInfo> Events { get; set; } = new ObservableCollection<EventInfo>();

        public async void GetEvents()
        {
            var response = await EventInfoAPI.GetEventsForDate(Date.Year, Date.Month, Date.Day);
            Events.Clear();
            if (response == null) return;
            foreach (var r in response)
            {
                Events.Add(r);
            }
        }

        private void EventListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow wnd = Window.GetWindow(this) as MainWindow;
            long eventId = (e.AddedItems.Count != 0) ? (e.AddedItems[0] as EventInfo).id : -1;
            wnd.ShowEventDetails(eventId);
        }
    }
}
