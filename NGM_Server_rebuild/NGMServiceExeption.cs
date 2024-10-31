using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild;

public class NGMServiceExeption : Exception
{
    public IActionResult Res;
    public NGMServiceExeption(IActionResult res)
    {
        this.Res = res;
    }
}