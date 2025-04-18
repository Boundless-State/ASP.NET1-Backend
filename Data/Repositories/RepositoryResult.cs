namespace Data.Repositories;

public class RepositoryResult
{
    public bool Succeeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Error { get; set; }
}


public class RepositoryResult<T> : RepositoryResult
{
    public T? Result { get; set; }

}

