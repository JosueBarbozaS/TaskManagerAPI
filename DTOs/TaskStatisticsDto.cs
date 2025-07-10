namespace TaskManagerAPI.DTOs
{
    public class TaskStatisticsDto
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<string, int> TasksByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<int, int> TasksByPriority { get; set; } = new Dictionary<int, int>();
    }
}