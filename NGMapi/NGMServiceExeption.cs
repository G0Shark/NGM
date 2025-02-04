using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild;

public class NGMServiceExeption : Exception
{
    public ActionResult Res;
    public NGMServiceExeption(ActionResult res)
    {
        this.Res = res;
    }
}