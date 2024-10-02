namespace Model
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Author { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
    }
}
