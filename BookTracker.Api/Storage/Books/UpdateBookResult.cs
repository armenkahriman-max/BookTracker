namespace BookTracker.Api.Storage.Books;
//kode koje baca  
public enum UpdateBookResult
{
    Updated, // 204
    NotFound, // 404
    Conflict //409 ex email alredy exists
}