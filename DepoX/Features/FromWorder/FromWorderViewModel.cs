using DepoX.Features.Basket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.FromWorder;

public class FromWorderViewModel : INotifyPropertyChanged
{

    private readonly IFromWorderService _fromWorderService;
    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadInitialAsync()
    {
        try
        {
           

            var result = await _fromWorderService.GetBasketDataAsync();

            if (!result.Success)
            {
                //ErrorMessage = result.Message;
                return;
            }

            //Whouses.Clear();
            //foreach (var w in result.Data!.whouses)
            //    Whouses.Add(BasketMapper.ToModel(w));

            //Baskets.Clear();
            //foreach (var b in result.Data!.baskets)
            //    Baskets.Add(BasketMapper.ToModel(b));

        }
        catch (Exception ex)
        {
            
        }
        finally
        {
        }
    }
}
