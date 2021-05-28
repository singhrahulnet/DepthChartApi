namespace DepthChartApi.Models
{
    public class Player
    {
        public int AtomicCounter;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string GameName { get; set; }
        public int? Depth { get; set; }
    }
}
