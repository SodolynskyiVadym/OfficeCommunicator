namespace OfficeCommunicatorMaui.DTO
{
    public class GroupCreateDto
    {
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }

        public GroupCreateDto(string name, string uniqueIdentifier)
        {
            Name = name;
            UniqueIdentifier = uniqueIdentifier;
        }
    }
}
