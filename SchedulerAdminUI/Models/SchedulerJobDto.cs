namespace SchedulerAdminUI.Models
{
    public class SchedulerJobDto
    {
        public string Name { get; set; } = "";
        public bool Enabled { get; set; }
        public string TimeOfDay { get; set; } = "";
        public int PlantId { get; set; }
        public List<string> DaysOfWeek { get; set; } = new();
        public List<string> Recipients { get; set; } = new();
        public string ReportType { get; set; } = "";
        public int DaysBack { get; set; }


        public string DaysDisplay
        {
            get
            {
                var orderedDays = DaysOfWeek
                    .OrderBy(d => GetDayOrder(d))
                    .ToList();

                return string.Join(", ", orderedDays);
            }
        }

        private int GetDayOrder(string day)
        {
            return day switch
            {
                "Monday" => 1,
                "Tuesday" => 2,
                "Wednesday" => 3,
                "Thursday" => 4,
                "Friday" => 5,
                "Saturday" => 6,
                "Sunday" => 7,
                _ => 99
            };
        }
        public string RecipientsDisplay => string.Join(", ", Recipients);

        public int RecipientCount => Recipients?.Count ?? 0;

        public string PlantDisplay
        {
            get
            {
                return PlantId switch
                {
                    0 => "Kitchener/Gatineau",
                    1 => "Kitchener",
                    2 => "Gatineau",
                    _ => $"Plant {PlantId}"
                };
            }
        }
    }
}