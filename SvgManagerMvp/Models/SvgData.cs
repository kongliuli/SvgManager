namespace SvgManagerMvp.Models
{
    public class SvgData
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}