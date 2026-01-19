namespace DepoX.Features.Count
{
    public class BasketDraftDto
    {
        public string ClientDraftId { get; set; }
        public string BasketId { get; set; }
        public string WorkplaceCode { get; set; }

        public BasketItemDraftDto[] Items { get; set; }
    }
}
