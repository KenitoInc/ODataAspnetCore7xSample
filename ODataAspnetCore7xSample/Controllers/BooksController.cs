using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using ODataAspnetCore7xSample.Models;

namespace ODataAspnetCore7xSample.Controllers
{
    public class BooksController : ODataController
    {
        // Get ~/Books
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1, PageSize = 100, MaxExpansionDepth = 5)]
        public IQueryable<Book> Get()
        {
            return DataSource.Instance.Books.AsQueryable<Book>();
        }

        // GET ~/Books(1)
        [EnableQuery]
        public SingleResult<Book> Get(int key)
        {
            IQueryable<Book> result = DataSource.Instance.Books.AsQueryable<Book>().Where(b => b.ID == key);
            return SingleResult.Create(result);
        }

        // PUT ~/Books(1)
        [EnableQuery]
        public IActionResult Put(int key, [FromBody] Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = DataSource.Instance.Books.Where(b => b.ID == key);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        // PATCH ~/Books(1)
        [EnableQuery]
        public IActionResult Patch([FromODataUri] int key, Delta<Book> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var book = DataSource.Instance.Books.Where(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        // DELETE ~/Books(1)
        [EnableQuery]
        public IActionResult Delete(int key)
        {
            var book = DataSource.Instance.Books.Where(b => b.ID == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        #region Actions and Functions
        [EnableQuery]
        public IActionResult GetMainAuthor([FromODataUri] int key)
        {
            var mainAuthor = DataSource.Instance.Books.Where(m => m.ID == key).Select(m => m.MainAuthor).Single();
            return Ok(mainAuthor);
        }

        // GET /Books/MostRecent()
        // This is a bound function. It's bound to the entity set Books.
        public IActionResult MostRecent()
        {
            var product = DataSource.Instance.Books.Max(x => x.ID);
            return Ok(product);
        }

        // POST /Books(1)/Rate
        // Body has { Rating: 7 }
        // This is bound Action. The action is bound to the Books entity set.
        public IActionResult Rate([FromODataUri] int key, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int rating = (int)parameters["Rating"];

            if (rating < 0)
            {
                return BadRequest();
            }

            return Ok(new BookRating() { BookID = key, Rating = rating });
        }

        // GET ReturnAllForKidsBooks()
        // This is an unbound Function.
        [HttpGet("/odata/ReturnAllForKidsBooks()")]
        public IActionResult ReturnAllForKidsBooks()
        {
            var forKidsBooks = DataSource.Instance.Books.Where(m => m.ForKids == true);
            return Ok(forKidsBooks);
        }

        // GET ~/Books(1)/Authors
        // Authors is a navigation property.
        [EnableQuery]
        public IActionResult GetAuthors([FromODataUri] int key)
        {
            var result = DataSource.Instance.Books.AsQueryable<Book>().Where(b => b.ID == key).Select(b => b.Authors);
            return Ok(result);
        }
        #endregion

        #region Containment
        //Contained entities don't have their own controller; the action is defined in the containing entity set controller.

        // GET ~/Books(1)/Translators
        [EnableQuery]
        public IActionResult GetTranslators(int key)
        {
            var translators = DataSource.Instance.Books.Single(a => a.ID == key).Translators;
            return Ok(translators);
        }
        #endregion
    }
}
