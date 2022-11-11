using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Entities;

namespace PostService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostServiceController : ControllerBase
    {

        private readonly ILogger<PostServiceController> _logger;
        private readonly PostServiceContext _context;

        public PostServiceController(ILogger<PostServiceController> logger, PostServiceContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost()
        {
            return await _context.Post.Include(x => x.User).ToListAsync();
        }

        [HttpGet]
        [Route("/users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            _context.Post.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }
    }
}