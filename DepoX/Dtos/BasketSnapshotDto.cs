using System;

namespace DepoX.Dtos
{
    public class BasketSnapshotDto
    {
        public string BasketId { get; set; }
        public string Status { get; set; }
        public int Version { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BasketItemSnapshotDto[] Items { get; set; }
    }
}
