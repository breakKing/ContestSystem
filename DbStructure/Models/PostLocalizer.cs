namespace ContestSystem.DbStructure.Models
{
    public class PostLocalizer: BaseLocalizer
    {
        public string Name { get; set; }
        public string HtmlText { get; set; }
        public string PreviewText { get; set; }
        public long PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}
