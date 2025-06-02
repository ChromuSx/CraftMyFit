namespace CraftMyFit.Helpers.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Restituisce l'inizio della settimana (lunedì)
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Restituisce la fine della settimana (domenica)
        /// </summary>
        public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday) => dateTime.StartOfWeek(startOfWeek).AddDays(6).Date.AddTicks(TimeSpan.TicksPerDay - 1);

        /// <summary>
        /// Restituisce l'inizio del mese
        /// </summary>
        public static DateTime StartOfMonth(this DateTime dateTime) => new(dateTime.Year, dateTime.Month, 1);

        /// <summary>
        /// Restituisce la fine del mese
        /// </summary>
        public static DateTime EndOfMonth(this DateTime dateTime) => dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);

        /// <summary>
        /// Restituisce l'inizio dell'anno
        /// </summary>
        public static DateTime StartOfYear(this DateTime dateTime) => new(dateTime.Year, 1, 1);

        /// <summary>
        /// Restituisce la fine dell'anno
        /// </summary>
        public static DateTime EndOfYear(this DateTime dateTime) => new(dateTime.Year, 12, 31, 23, 59, 59, 999);

        /// <summary>
        /// Verifica se la data è oggi
        /// </summary>
        public static bool IsToday(this DateTime dateTime) => dateTime.Date == DateTime.Today;

        /// <summary>
        /// Verifica se la data è ieri
        /// </summary>
        public static bool IsYesterday(this DateTime dateTime) => dateTime.Date == DateTime.Today.AddDays(-1);

        /// <summary>
        /// Verifica se la data è in questa settimana
        /// </summary>
        public static bool IsThisWeek(this DateTime dateTime)
        {
            DateTime startOfWeek = DateTime.Today.StartOfWeek();
            DateTime endOfWeek = DateTime.Today.EndOfWeek();
            return dateTime >= startOfWeek && dateTime <= endOfWeek;
        }

        /// <summary>
        /// Verifica se la data è in questo mese
        /// </summary>
        public static bool IsThisMonth(this DateTime dateTime) => dateTime.Year == DateTime.Today.Year && dateTime.Month == DateTime.Today.Month;

        /// <summary>
        /// Verifica se la data è in questo anno
        /// </summary>
        public static bool IsThisYear(this DateTime dateTime) => dateTime.Year == DateTime.Today.Year;

        /// <summary>
        /// Restituisce una rappresentazione "tempo fa" della data
        /// </summary>
        public static string ToTimeAgo(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now - dateTime;

            if(timeSpan.TotalDays >= 365)
            {
                int years = (int)(timeSpan.TotalDays / 365);
                return years == 1 ? "1 anno fa" : $"{years} anni fa";
            }

            if(timeSpan.TotalDays >= 30)
            {
                int months = (int)(timeSpan.TotalDays / 30);
                return months == 1 ? "1 mese fa" : $"{months} mesi fa";
            }

            if(timeSpan.TotalDays >= 7)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);
                return weeks == 1 ? "1 settimana fa" : $"{weeks} settimane fa";
            }

            if(timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;
                return days == 1 ? "1 giorno fa" : $"{days} giorni fa";
            }

            if(timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;
                return hours == 1 ? "1 ora fa" : $"{hours} ore fa";
            }

            if(timeSpan.TotalMinutes >= 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;
                return minutes == 1 ? "1 minuto fa" : $"{minutes} minuti fa";
            }

            return "Ora";
        }

        /// <summary>
        /// Formatta la data in modo user-friendly
        /// </summary>
        public static string ToFriendlyString(this DateTime dateTime)
        {
            if(dateTime.IsToday())
            {
                return "Oggi";
            }

            if(dateTime.IsYesterday())
            {
                return "Ieri";
            }

            if(dateTime.IsThisWeek())
            {
                return dateTime.ToString("dddd");
            }

            return dateTime.IsThisYear() ? dateTime.ToString("dd MMM") : dateTime.ToString("dd MMM yyyy");
        }

        /// <summary>
        /// Calcola l'età basata sulla data di nascita
        /// </summary>
        public static int CalculateAge(this DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if(birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        /// <summary>
        /// Restituisce il numero della settimana nell'anno
        /// </summary>
        public static int GetWeekOfYear(this DateTime dateTime)
        {
            System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.Calendar calendar = culture.Calendar;
            System.Globalization.CalendarWeekRule calendarWeekRule = culture.DateTimeFormat.CalendarWeekRule;
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            return calendar.GetWeekOfYear(dateTime, calendarWeekRule, firstDayOfWeek);
        }

        /// <summary>
        /// Verifica se l'anno è bisestile
        /// </summary>
        public static bool IsLeapYear(this DateTime dateTime) => DateTime.IsLeapYear(dateTime.Year);

        /// <summary>
        /// Restituisce il numero di giorni fino alla data specificata
        /// </summary>
        public static int DaysUntil(this DateTime fromDate, DateTime toDate) => (toDate.Date - fromDate.Date).Days;

        /// <summary>
        /// Restituisce il numero di giorni lavorativi tra due date
        /// </summary>
        public static int BusinessDaysUntil(this DateTime fromDate, DateTime toDate)
        {
            int businessDays = 0;
            DateTime currentDate = fromDate.Date;

            while(currentDate <= toDate.Date)
            {
                if(currentDate.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
                {
                    businessDays++;
                }

                currentDate = currentDate.AddDays(1);
            }

            return businessDays;
        }
    }
}