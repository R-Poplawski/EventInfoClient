using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace EventInfoClient
{
    public class EventTypeConverter : MarkupExtension, IValueConverter
    {
        private static EventTypeConverter _converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null) _converter = new EventTypeConverter();
            return _converter;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case EventType.Concert:
                    return "Koncert";
                case EventType.Exhibition:
                    return "Wystawa";
                case EventType.Sport:
                    return "Sport";
                case EventType.Theater:
                    return "Teatr";
                case EventType.Other:
                    return "Inne";
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "koncert":
                    return EventType.Concert;
                case "wystawa":
                    return EventType.Exhibition;
                case "sport":
                    return EventType.Sport;
                case "teatr":
                    return EventType.Theater;
                case "inne":
                    return EventType.Other;
            }
            return value;
        }
    }
}
