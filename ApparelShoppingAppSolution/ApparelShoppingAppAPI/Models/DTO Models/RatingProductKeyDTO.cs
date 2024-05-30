namespace ApparelShoppingAppAPI.Models.DTO_Models
{
    public class RatingProductKeyDTO
    {
        public int Rating { get; set; }
        public int ProductId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RatingProductKeyDTO other)
            {
                return Rating == other.Rating && ProductId == other.ProductId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rating, ProductId);
        }
    }

}
