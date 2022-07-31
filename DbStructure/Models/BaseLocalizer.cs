namespace ContestSystem.DbStructure.Models
{
    public abstract class BaseLocalizer: BaseEntity
    {
        public string Culture { get; set; }
    }
}
