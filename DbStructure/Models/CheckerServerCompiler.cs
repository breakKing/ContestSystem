namespace ContestSystem.DbStructure.Models
{
    public class CheckerServerCompiler: BaseEntity
    {
        public long CheckerServerId { get; set; }
        public virtual CheckerServer CheckerServer { get; set; }
        public string CompilerGUID { get; set; }
        public string CompilerName { get; set; }
    }
}
