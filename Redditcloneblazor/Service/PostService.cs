using System.Net.Http.Json;
using System.Xml.Linq;
using Model;
using Redditcloneblazor.Pages;

namespace Service
{
    public class PostService
    {
        private readonly HttpClient _httpClient;

        public PostService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Post>>("/api/posts");
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Post>($"/api/posts/{id}");
        }

        public async Task CreatePostAsync(Post post)
        {
            await _httpClient.PostAsJsonAsync("/api/posts", post);
        }

        public async Task DeletePostAsync(int id)
        {
            await _httpClient.DeleteAsync($"/api/posts/{id}");
        }

        // New methods for upvoting/downvoting a post
        public async Task UpvotePostAsync(int id)
        {
            await _httpClient.PutAsync($"/api/posts/{id}/upvote", null);
        }

        public async Task DownvotePostAsync(int id)
        {
            await _httpClient.PutAsync($"/api/posts/{id}/downvote", null);
        }

        // Methods for adding a comment
        public async Task AddCommentAsync(int postId, Comment comment)
        {
            await _httpClient.PostAsJsonAsync($"/api/posts/{postId}/comments", comment);
        }

        // Methods for upvoting/downvoting a comment
        public async Task UpvoteCommentAsync(int postId, int commentId)
        {
            await _httpClient.PutAsync($"/api/posts/{postId}/comments/{commentId}/upvote", null);
        }

        public async Task DownvoteCommentAsync(int postId, int commentId)
        {
            await _httpClient.PutAsync($"/api/posts/{postId}/comments/{commentId}/downvote", null);
        }
    }

}
