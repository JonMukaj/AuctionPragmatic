using Service.Contracts;

namespace Service;

public class HangfireService : IHangfireService
{

    public HangfireService()
    {
       
    }

    public Task UpdateDataMarket()
    {
        throw new NotImplementedException();
    }

    //public Task UpdateDataMarket()
    //{
    //    Task.Run(async () =>
    //        {

    //        });
    //    return Task.CompletedTask;
    //}


}