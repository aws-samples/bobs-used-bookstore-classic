namespace Bookstore.Domain.ReferenceData
{
    public class CreateReferenceDataItemDto
    {
        public CreateReferenceDataItemDto(ReferenceDataType ReferenceDataType, string Text)
        {
            this.ReferenceDataType = ReferenceDataType;
            this.Text = Text;
        }

        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }

    public class UpdateReferenceDataItemDto
    {
        public UpdateReferenceDataItemDto(int Id, ReferenceDataType ReferenceDataType, string Text)
        {
            this.Id = Id;
            this.ReferenceDataType = ReferenceDataType;
            this.Text = Text;
        }

        public int Id { get; }
        public ReferenceDataType ReferenceDataType { get; }
        public string Text { get; }
    }
}
