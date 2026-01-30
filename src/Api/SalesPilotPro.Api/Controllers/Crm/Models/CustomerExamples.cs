public static class CustomerExamples
{
    public const string CreateRequest = """
    {
      "name": "Hospital Central",
      "code": "HC001"
    }
    """;

    public const string Response = """
    {
      "success": true,
      "data": {
        "id": "guid",
        "name": "Hospital Central",
        "code": "HC001",
        "isActive": true
      }
    }
    """;
}
