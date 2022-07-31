namespace ContestSystem.DbStructure.Models
{
    public class ContestFile: BaseFile
    {
        public long ContestId { get; set; }
        public virtual Contest Contest { get; set; }
    }
}
