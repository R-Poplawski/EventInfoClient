using System;
using System.Windows;

namespace EventInfoClient
{
    /// <summary>
    /// Interaction logic for AddEventWindow.xaml
    /// </summary>
    public partial class AddEditEventWindow : Window
    {
        public AddEditEventWindow(EventInfo eventInfo)
        {
            if (eventInfo != null)
            {
                EventName = eventInfo.name;
                Type = eventInfo.type;
                Date = eventInfo.date;
                Description = eventInfo.description;
                id = eventInfo.id;
            }
            InitializeComponent();
            if (id!=-1)
            {
                AddEditButton.Content = "Zapisz wydarzenie";
                Title = "Edytuj wydarzenie \"" + EventName + "\"";
            }
        }

        private long id = -1;

        public AddEditEventWindow() : this(null) { }

        public string EventName { get; set; }
        public EventType Type { get; set; } = (EventType)(-1);
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public string Description { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (EventName == null || EventName.Trim() == "")
            {
                MessageBox.Show("Podaj nazwę wydarzenia");
                return;
            }
            if ((int)Type == -1)
            {
                MessageBox.Show("Wybierz typ wydarzenia", "Niepoprawne dane");
                return;
            }
            if (Date < DateTime.Now.Date)
            {
                MessageBox.Show("Niepoprawna data wydarzenia");
                return;
            }
            if (id == -1) await EventInfoAPI.AddEvent(EventName, Description, Type, Date);
            else await EventInfoAPI.EditEvent(id, EventName, Description, Type, Date);
            Close();
        }
    }
}
