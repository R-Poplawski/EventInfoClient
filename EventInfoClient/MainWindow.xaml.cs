using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace EventInfoClient
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            EventListFrame.Content = new EventsForWeek();
            ShowEventDetails(-1);
        }

        public async void ShowEventDetails(long id)
        {
            if (id < 0)
            {
                EventDetailsGrid.Visibility = Visibility.Collapsed;
                EventDetailsRow.Height = GridLength.Auto;
                EditEventButton.IsEnabled = false;
                return;
            }
            EventInfo response = await EventInfoAPI.GetEvent(id);
            if (response == null)
            {
                EventDetailsGrid.Visibility = Visibility.Collapsed;
                EventDetailsRow.Height = GridLength.Auto;
                EditEventButton.IsEnabled = false;
                return;
            }
            else
            {
                EventDetailsGrid.Visibility = Visibility.Visible;
                EventDetailsRow.Height = new GridLength(2, GridUnitType.Star);
                EditEventButton.IsEnabled = true;
            }
            SelectedEventName = response.name;
            SelectedEventType = response.type;
            SelectedEventDate = response.date;
            SelectedEventDescription = response.description;
            SelectedEvent = response;
            NotifyPropertyChanged("SelectedEventName");
            NotifyPropertyChanged("SelectedEventType");
            NotifyPropertyChanged("SelectedEventDate");
            NotifyPropertyChanged("SelectedEventDescription");
        }

        public EventInfo SelectedEvent { get; set; }

        public string SelectedEventName { get; set; }
        public EventType SelectedEventType { get; set; }
        public DateTime SelectedEventDate { get; set; }
        public string SelectedEventDescription { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EventsForDayButton_Click(object sender, RoutedEventArgs e)
        {
            EventListFrame.Content = new EventsForDay();
            ShowEventDetails(-1);
        }

        private void EventsForWeekButton_Click(object sender, RoutedEventArgs e)
        {
            EventListFrame.Content = new EventsForWeek();
            ShowEventDetails(-1);
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditEventWindow aev = new AddEditEventWindow { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this };
            aev.ShowDialog();
            if (EventListFrame.Content is EventsForDay) (EventListFrame.Content as EventsForDay).GetEvents();
            else if (EventListFrame.Content is EventsForWeek) (EventListFrame.Content as EventsForWeek).GetEvents();
        }

        private void EditEventButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditEventWindow aev = new AddEditEventWindow(SelectedEvent) { WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this };
            aev.ShowDialog();
            if (EventListFrame.Content is EventsForDay) (EventListFrame.Content as EventsForDay).GetEvents();
            else if (EventListFrame.Content is EventsForWeek) (EventListFrame.Content as EventsForWeek).GetEvents();
        }

        private async void GetPdfButton_Click(object sender, RoutedEventArgs e)
        {
            byte[] response = null;
            if (EventListFrame.Content is EventsForDay)
            {
                var frame = EventListFrame.Content as EventsForDay;
                DateTime date = frame.Date;
                response = await EventInfoAPI.GetPdfForDate(date.Year, date.Month, date.Day);
            }
            else if (EventListFrame.Content is EventsForWeek)
            {
                var frame = EventListFrame.Content as EventsForWeek;
                response = await EventInfoAPI.GetPdfForWeek(frame.Year, frame.WeekNumber);
            }

            if (response == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "PDF|*.pdf", DefaultExt = "pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, response);
            }
                
        }
    }
}
