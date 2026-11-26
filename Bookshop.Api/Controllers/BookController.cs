using Bookshop.Application.DTOs;
using Bookshop.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookshop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Default: শুধু Admin book create/update/delete করতে পারবে
    public class BookController : ControllerBase
    {
        private readonly IBookServices _bookServices;

        public BookController(IBookServices bookServices)
        {
            _bookServices = bookServices;
        }

        [HttpGet]
        [AllowAnonymous] // বই দেখা সবার জন্য খোলা
        [ProducesResponseType(typeof(IReadOnlyList<BookResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookServices.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookServices.GetByIdAsync(id);
            if (book is null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBookDto createBook)
        {
            var book = await _bookServices.CreateAsync(createBook);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BookResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto updateBook)
        {
            var book = await _bookServices.UpdateAsync(id, updateBook);
            if (book is null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _bookServices.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
