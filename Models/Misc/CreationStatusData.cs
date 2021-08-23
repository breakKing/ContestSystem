using ContestSystem.Models.Dictionaries;

namespace ContestSystem.Models.Misc
{
    public class CreationStatusData<TData>
    {
        public CreationStatus Status { get; set; }
        public TData Id { get; set; }
    }
}
