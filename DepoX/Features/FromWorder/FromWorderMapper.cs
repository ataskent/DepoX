using DepoX.Features.Basket;
using DepoX.Services.Erp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.FromWorder
{
    public static class FromWorderMapper
    {
        public static TransferMVm ToModel(this TransferM dto)
        {
            return new TransferMVm
            {
                Code = dto.Code,
                Name = dto.Code,
                WhouseCode = dto.WhouseCode
            };
        }
        public static WhouseVm ToModel(this Whouse dto)
        {
            return new WhouseVm
            {
                Code = dto.Code,
                Name = dto.Name
            };
        }
        public static BarcodeVm ToModel(this Barcode dto)
        {
            return new BarcodeVm
            {
                Code = dto.Code,
                Quantity = dto.Quantity,
                WhouseCode = dto.WhouseCode
            };
        }
        public static ItemsVm ToModel(this Items dto)
        {
            return new ItemsVm
            {
                Code = dto.Code,
                Quantity = dto.Quantity,
                WhouseCode = dto.WhouseCode
            };
        }

        public static TransferData ToModel(this TransferDataVm vm)
        {
            return new TransferData
            {
                TransferM = new TransferM
                {
                    Code = vm.Transfer.Code,
                    Name = vm.Transfer.Name,
                    WhouseCode = vm.Transfer.WhouseCode
                },
                Barcodes = vm.Barcodes.Select(b => new Barcode
                {
                    Code = b.Code,
                    Quantity = b.Quantity,
                    WhouseCode = b.WhouseCode
                }).ToList(),
                Items = vm.Items.Select(i => new Items
                {
                    Code = i.Code,
                    Quantity = i.Quantity,
                    WhouseCode = i.WhouseCode
                }).ToList()
            };
        }
    }
}
